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


using EnvDTE;

namespace CodeOwls.StudioShell.Paths.Items.UI
{
    internal class ShellOutputPane
    {
        private readonly OutputWindowPane _pane;

        internal ShellOutputPane(OutputWindowPane pane)
        {
            _pane = pane;
        }

        public string Name
        {
            get { return _pane.Name; }
        }

        public string Guid
        {
            get { return _pane.Guid; }
        }

        public TextDocument TextDocument
        {
            get { return _pane.TextDocument; }
        }

        public void OutputString(string Text)
        {
            _pane.OutputString(Text);
        }

        public void ForceItemsToTaskList()
        {
            _pane.ForceItemsToTaskList();
        }

        public void Activate()
        {
            _pane.Activate();
        }

        public void Clear()
        {
            _pane.Clear();
        }

        public void OutputTaskItemString(string Text, vsTaskPriority Priority, string SubCategory, vsTaskIcon Icon,
                                         string FileName, int Line, string Description, bool Force)
        {
            _pane.OutputTaskItemString(Text, Priority, SubCategory, Icon, FileName, Line, Description, Force);
        }
    }
}