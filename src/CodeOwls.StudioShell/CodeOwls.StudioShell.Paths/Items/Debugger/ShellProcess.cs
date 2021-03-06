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
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Items.Debugger
{
    public class ShellProcess
    {
        private readonly Process _process;

        public ShellProcess(Process process)
        {
            _process = process;
        }

        public string Name
        {
            get { return _process.Name; }
        }

        public int ProcessID
        {
            get { return _process.ProcessID; }
        }

        public ShellDebugger Parent
        {
            get { return new ShellDebugger(_process.Parent as Debugger2); }
        }

        public void Attach()
        {
            _process.Attach();
        }

        public void Detach(bool WaitForBreakOrEnd)
        {
            _process.Detach(WaitForBreakOrEnd);
        }

        public void Break(bool WaitForBreakMode)
        {
            _process.Break(WaitForBreakMode);
        }

        public void Terminate(bool WaitForBreakOrEnd)
        {
            _process.Terminate(WaitForBreakOrEnd);
        }

        internal Process AsProcess()
        {
            return _process;
        }
    }
}