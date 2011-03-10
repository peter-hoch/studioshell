/*
   Copyright (c) 2011 Code Owls LLC, All Rights Reserved.

   Licensed under the Microsoft Reciprocal License (Ms-RL) (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.opensource.org/licenses/ms-rl

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using CodeOwls.PowerShell.Host;
using CodeOwls.PowerShell.WinForms.AutoComplete;
using CodeOwls.PowerShell.WinForms.Configuration;
using CodeOwls.PowerShell.WinForms.Executors;
using CodeOwls.PowerShell.WinForms.History;
using CodeOwls.PowerShell.WinForms.Host;
using CodeOwls.PowerShell.WinForms.Utility;

namespace CodeOwls.PowerShell.WinForms
{
    public class Shell : AsyncCommandExecutorBase, IRunnableCommandExecutor
    {
        class StartupState
        {
            readonly ManualResetEvent _complete;
            Exception _exception;

            public StartupState()
            {
                _complete = new ManualResetEvent(false);
            }

            public Exception WaitForStartup()
            {
                _complete.WaitOne( 1000 );
                return _exception;
            }

            public void SetComplete( Exception e )
            {
                _exception = e;
                _complete.Set();
            }
        }

               
        private readonly PSTextBox _consoleWindow;
        private readonly ShellConfiguration _shellConfiguration;
        private HostRawUI _rawUi;
        private HostUI _hostUi;
        private WinForms.Host.Host _host;
        private Runspace _runspace;
        private Executor _commandExecutor;
        
        private Thread _thread;
        private HistoryStackWalker _historyStackWalker;
        private AutoCompleteWalker _autoCompleteWalker;

        public Shell(PSTextBox consoleWindow, ShellConfiguration shellConfiguration)
        {
            _consoleWindow = consoleWindow;
            _shellConfiguration = shellConfiguration;
        }

        public override bool CancelCurrentExecution(int timeoutInMilliseconds)
        {
            return _commandExecutor.CancelCurrentPipeline( timeoutInMilliseconds);
        }

        public void Stop()
        {
            Stop( false );
        }

        public void Stop( bool force )
        {
            var thread = _thread;
            _host.SetShouldExit(0);

            if (!force)
            {
                return;
            }

            if (null != thread)
            {
                if (!thread.Join(2500))
                {
                    thread.Abort();
                    thread.Join(5000);
                }
            }
        }

        public void Run()
        {
            var thread = new Thread(_Run);
            
            var existing = Interlocked.CompareExchange(ref _thread, thread, null);
            if( null != existing )
            {
                return;
            }

            StartupState state = new StartupState();
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start(state);
            state.WaitForStartup();
        }

        public override CommandExecutorState CurrentState
        {
            get
            {
                return _runspace.RunspaceAvailability == RunspaceAvailability.Available ?
                    CommandExecutorState.Available :
                    CommandExecutorState.Unavailable;
            }
        }

        public event EventHandler<ExitEventArgs> ShouldExit;
        public event EventHandler<ProgressRecordEventArgs> Progress;
        public event EventHandler<EventArgs<bool>> CommandExecutionStateChange;

        private void InvokeShouldExit( int exitCode )
        {
            EventHandler<ExitEventArgs> handler = ShouldExit;
            if (handler != null)
            {
                handler(this, new ExitEventArgs(exitCode));
            }
        }

        private void _Run(object o)
        {
            try
            {
                StartupState state = (StartupState) o;
                Exception e = null;
                try
                {
                    InitializeRunspaceAndHost();
                }
                catch (Exception ex)
                {
                    state.SetComplete(ex);
                    return;
                }

                state.SetComplete(null);

                RunProfileScripts();
                RunInitializationScripts();

                ExecuteRunLoop();
            }
            catch( ThreadAbortException )
            {                
            }
            finally
            {
                Interlocked.Exchange(ref _thread, null);
            }
        }

        private void ExecuteRunLoop()
        {
            var autoCompleteProviders = new List<IAutoCompleteProvider>
                                {
                                    new AutoCompleteProviderChain(
                                        new PowerShellTabExansionAutoCompleteProvider( _commandExecutor ),
                                        new CompositeAutoCompleteProvider(
                                            new ProviderPathAutoCompleteProvider(_commandExecutor),
                                            new CommandListAutoCompleteProvider(_commandExecutor)
                                        )
                                    )
                                };

            _autoCompleteWalker = new AutoCompleteWalker(autoCompleteProviders);
            _historyStackWalker = new HistoryStackWalker(_commandExecutor);

            _consoleWindow.AutoCompleteWalker = _autoCompleteWalker;
            _consoleWindow.HistoryStackWalker = _historyStackWalker;

            WritePrompt();

            while (true)
            {
                WaitHandle[] handles = new WaitHandle[]
                                           {
                                               _host.ExitWaitHandle,
                                               _consoleWindow.CommandEnteredEvent,
                                               Queue.WaitHandle
                                           };
             
                int index = WaitHandle.WaitAny(handles);
                switch (index)
                {
                    case (0):
                        InvokeShouldExit(_host.ExitCode);
                        return;
                    case (1):
                        ExecuteConsoleCommand();
                        break;
                    case (2):
                        ExecuteQueuedCommand();
                        break;
                    default:
                        break;
                }
            }
        }

        private void ExecuteQueuedCommand()
        {
            var asynResult = Queue.Dequeue();
            Collection<PSObject> results = null;
            try
            {
                results = ExecuteCommand(asynResult.Command, asynResult.Parameters, asynResult.ExecutionOptions );
                asynResult.SetComplete(results, false, null );
            }
            catch( Exception e )
            {
                asynResult.SetComplete(results, false, e );
            }          
        }

        private void ExecuteConsoleCommand()
        {
            _historyStackWalker.Reset();
            _autoCompleteWalker.Reset();

            var input = _consoleWindow.ReadLine();

            ExecuteCommand(input, ExecutionOptions.AddToHistory | ExecutionOptions.AddOutputter);            

            WritePrompt();
        }

        private void WritePrompt()
        {
            Exception error;
            var prompt = _commandExecutor.ExecuteAndGetStringResult("prompt", out error);
            WritePrompt( prompt );
        }
        private void WritePrompt( string prompt )
        {
            Exception error;
            prompt = prompt.Trim();
            _consoleWindow.WritePrompt(prompt);
        }
        private Collection<PSObject> ExecuteCommand(string input)
        {
            return ExecuteCommand( input, ExecutionOptions.None);
        }

        private Collection<PSObject> ExecuteCommand(string input, ExecutionOptions executionOptions)
        {
            Exception error;
            var onx = CommandExecutionStateChange;
            if (null != onx)
            {
                onx(this, new EventArgs<bool>(true));
            }

            var results = _commandExecutor.ExecuteCommand(
                input,
                out error,
                executionOptions
                );

            if (null != onx)
            {
                onx(this, new EventArgs<bool>(false));
            }
            return results;
        }


        private Collection<PSObject> ExecuteCommand(string command, Dictionary<string, object> arguments, ExecutionOptions options)
        {
            Exception error;
            var onx = CommandExecutionStateChange;
            if (null != onx)
            {
                onx(this, new EventArgs<bool>(true));
            }

            var results = _commandExecutor.ExecuteCommand(
                command,
                arguments,
                out error,
                options
                );

            if (null != onx)
            {
                onx(this, new EventArgs<bool>(false));
            }
            return results;
        }

        private void InitializeRunspaceAndHost()
        {
            if (null == _shellConfiguration.RunspaceConfiguration)
            {
                _shellConfiguration.RunspaceConfiguration = RunspaceConfiguration.Create();
            }
            
            _shellConfiguration.Cmdlets.ToList().ForEach(
                cce => _shellConfiguration.RunspaceConfiguration.Cmdlets.Append(cce)
            );

            _rawUi = new HostRawUI(_consoleWindow, _shellConfiguration.ShellName );
            _hostUi = new HostUI(_consoleWindow, _shellConfiguration.UISettings, _rawUi);
            _host = new WinForms.Host.Host( _shellConfiguration.ShellName, _shellConfiguration.ShellVersion, _hostUi, _shellConfiguration.RunspaceConfiguration );

            _hostUi.Progress += NotifyProgress;

            _runspace = _host.Runspace;
            _runspace.Open();
            
            _commandExecutor = new Executor(_runspace);
            _commandExecutor.PipelineException += OnPipelineException;

            
            _shellConfiguration.InitialVariables.ToList().ForEach(pair =>
                                                                  _runspace.SessionStateProxy.PSVariable.Set( pair )

                ); 
        }

        private void OnPipelineException(object sender, EventArgs<Exception> e)
        {
            _host.UI.WriteErrorLine( e.Data.ToString() );
        }

        private void NotifyProgress(object sender, ProgressRecordEventArgs e)
        {
            var ev = Progress;
            if( null == ev )
            {
                return;
            }

            ev(sender, e);
        }

        private void RunProfileScripts()
        {
            if( null == _shellConfiguration.ProfileScripts )
            {
                return;
            }

            foreach (var entry in _shellConfiguration.ProfileScripts.InRunOrder )
            {
                var fileInfo = new FileInfo(entry);
                if( ! fileInfo.Exists )
                {
                    continue;
                }

                Exception error;
                _commandExecutor.ExecuteCommand(fileInfo.ToDotSource(), out error, ExecutionOptions.AddOutputter);
            }
        }

        private void RunInitializationScripts()
        {
            if (null == _shellConfiguration.RunspaceConfiguration.InitializationScripts)
            {
                return;
            }

            foreach (ScriptConfigurationEntry entry in _shellConfiguration.RunspaceConfiguration.InitializationScripts)
            {
                Exception error;
                _commandExecutor.ExecuteCommand(entry.Definition, out error, ExecutionOptions.AddOutputter);
            }
        }
    }
}
