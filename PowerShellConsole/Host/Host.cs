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
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace CodeOwls.PowerShell.WinForms.Host
{
    class Host : PSHost, IDisposable, IHostSupportsInteractiveSession
    {
        private readonly PSHostUserInterface _hostUi;
        private Guid _instance = Guid.NewGuid();
        private readonly string _name;
        private readonly Version _version;
        private PSHostUserInterface _hostUI;
        private readonly RunspaceConfiguration _runspaceConfiguration;
        private ManualResetEvent _exitEvent;
        private int _exitCode;
        private Stack<Runspace> _pushedRunspaces;
        private Runspace _runspace;

        public Host(string name, Version version, PSHostUserInterface hostUI, RunspaceConfiguration runspaceConfiguration)
        {
            _pushedRunspaces = new Stack<Runspace>();
            _name = name;
            _version = version;
            _hostUI = hostUI;
            _runspaceConfiguration = runspaceConfiguration;
            _exitEvent = new ManualResetEvent( false );
        }

        public int ExitCode
        {
            get { return _exitCode; }
        }

        public WaitHandle ExitWaitHandle
        {
            get { return _exitEvent; }
        }

        public override void SetShouldExit(int exitCode)
        {
            _exitCode = exitCode;
            _exitEvent.Set();
            
        }

        public override void EnterNestedPrompt()
        {
            
        }

        public override void ExitNestedPrompt()
        {
            
        }

        public override void NotifyBeginApplication()
        {
            
        }

        public override void NotifyEndApplication()
        {
            
        }

        public override string Name
        {
            get { return _name; }
        }

        public override Version Version
        {
            get { return _version; }
        }

        public override Guid InstanceId
        {
            get { return _instance; }
        }

        public override PSHostUserInterface UI
        {
            get { return _hostUI; }
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public void Dispose()
        {
            _exitEvent.Close();
        }

        #region Implementation of IHostSupportsInteractiveSession

        public void PushRunspace(Runspace runspace)
        {
            _pushedRunspaces.Push( runspace );
        }

        public void PopRunspace()
        {
            _pushedRunspaces.Pop();
        }

        public bool IsRunspacePushed
        {
            get { return 0 < _pushedRunspaces.Count; }
        }

        public Runspace Runspace
        {
            get
            {
                if (null == _runspace)
                {
                    _runspace = RunspaceFactory.CreateRunspace(this, _runspaceConfiguration);
                }

                var stack = _pushedRunspaces;
                if( 0 == stack.Count )
                {
                    return _runspace;
                }

                return stack.Peek();
            }
        }

        #endregion
    }
}
