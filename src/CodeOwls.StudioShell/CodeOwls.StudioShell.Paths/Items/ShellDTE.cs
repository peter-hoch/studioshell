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
using System.Linq;
using CodeOwls.StudioShell.Paths.Items.CommandBars;
using CodeOwls.StudioShell.Paths.Items.Commands;
using CodeOwls.StudioShell.Paths.Items.Debugger;
using CodeOwls.StudioShell.Paths.Items.PropertyModel;
using CodeOwls.StudioShell.Paths.Items.UI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace CodeOwls.StudioShell.Paths.Items
{
    public class ShellDTE
    {
        private readonly DTE2 _dte;

        public ShellDTE(DTE2 dte)
        {
            _dte = dte;
        }

        public string Name
        {
            get { return _dte.Name; }
        }

        public string FileName
        {
            get { return _dte.FileName; }
        }

        public string Version
        {
            get { return _dte.Version; }
        }

        public IEnumerable<ShellCommandBar> CommandBars
        {
            get
            {
                return from CommandBar bar in (Microsoft.VisualStudio.CommandBars.CommandBars) _dte.CommandBars
                       select new ShellCommandBar(bar);
            }
        }

        public IEnumerable<ShellWindow> Windows
        {
            get
            {
                return from Window2 window in _dte.Windows
                       select new ShellWindow(window);
            }
        }

        public EnvDTE.Events Events
        {
            get { return _dte.Events; }
        }

        public object AddIns
        {
            get { return _dte.AddIns; }
        }

        public ShellWindow MainWindow
        {
            get { return new ShellWindow(_dte.MainWindow as Window2); }
        }

        public ShellWindow ActiveWindow
        {
            get { return new ShellWindow(_dte.ActiveWindow as Window2); }
        }

        public vsDisplay DisplayMode
        {
            get { return _dte.DisplayMode; }
            set { _dte.DisplayMode = value; }
        }

        public Solution Solution
        {
            get { return _dte.Solution; }
        }

        public IEnumerable<ShellCommand> Commands
        {
            get
            {
                return from Command cmd in _dte.Commands
                       select new ShellCommand(cmd);
            }
        }

        public SelectedItems SelectedItems
        {
            get { return _dte.SelectedItems; }
        }

        public string CommandLineArguments
        {
            get { return _dte.CommandLineArguments; }
        }

        public int LocaleID
        {
            get { return _dte.LocaleID; }
        }

        public IEnumerable<ShellWindowConfiguration> WindowConfigurations
        {
            get
            {
                return from WindowConfiguration config in _dte.WindowConfigurations
                       select new ShellWindowConfiguration(config);
            }
        }

        public EnvDTE.Documents Documents
        {
            get { return _dte.Documents; }
        }

        public Document ActiveDocument
        {
            get { return _dte.ActiveDocument; }
        }

        public Globals Globals
        {
            get { return _dte.Globals; }
        }

        public StatusBar StatusBar
        {
            get { return _dte.StatusBar; }
        }

        public string FullName
        {
            get { return _dte.FullName; }
        }

        public bool UserControl
        {
            get { return _dte.UserControl; }
            set { _dte.UserControl = value; }
        }

        public Find Find
        {
            get { return _dte.Find; }
        }

        public vsIDEMode Mode
        {
            get { return _dte.Mode; }
        }

        public ItemOperations ItemOperations
        {
            get { return _dte.ItemOperations; }
        }

        public UndoContext UndoContext
        {
            get { return _dte.UndoContext; }
        }

        public Macros Macros
        {
            get { return _dte.Macros; }
        }

        public object ActiveSolutionProjects
        {
            get { return _dte.ActiveSolutionProjects; }
        }

        public EnvDTE.DTE MacrosIDE
        {
            get { return _dte.MacrosIDE; }
        }

        public string RegistryRoot
        {
            get { return _dte.RegistryRoot; }
        }

        public ContextAttributes ContextAttributes
        {
            get { return _dte.ContextAttributes; }
        }

        public SourceControl SourceControl
        {
            get { return _dte.SourceControl; }
        }

        public bool SuppressUI
        {
            get { return _dte.SuppressUI; }
            set { _dte.SuppressUI = value; }
        }

        public ShellDebugger Debugger
        {
            get { return new ShellDebugger(_dte.Debugger as Debugger2); }
        }

        public string Edition
        {
            get { return _dte.Edition; }
        }

        public ToolWindows ToolWindows
        {
            get { return _dte.ToolWindows; }
        }

        public void Quit()
        {
            _dte.Quit();
        }

        public object GetObject(string Name)
        {
            return _dte.GetObject(Name);
        }

        public ShellWindow OpenFile(string ViewKind, string FileName)
        {
            return new ShellWindow(_dte.OpenFile(ViewKind, FileName) as Window2);
        }

        public void ExecuteCommand(string CommandName, string CommandArgs)
        {
            _dte.ExecuteCommand(CommandName, CommandArgs);
        }

        public wizardResult LaunchWizard(string VSZFile, ref object[] ContextParams)
        {
            return _dte.LaunchWizard(VSZFile, ref ContextParams);
        }

        public string SatelliteDllPath(string Path, string Name)
        {
            return _dte.SatelliteDllPath(Path, Name);
        }

        public uint GetThemeColor(vsThemeColors Element)
        {
            return _dte.GetThemeColor(Element);
        }

        public IEnumerable<ShellProperty> get_Properties(string Category, string Page)
        {
            return from Property prop in _dte.get_Properties(Category, Page)
                   select new ShellProperty(prop);
        }

        public bool get_IsOpenFile(string ViewKind, string FileName)
        {
            return _dte.get_IsOpenFile(ViewKind, FileName);
        }
    }
}