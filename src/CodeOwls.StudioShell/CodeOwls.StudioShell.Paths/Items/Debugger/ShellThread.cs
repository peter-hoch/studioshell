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


using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Items.Debugger
{
    public class ShellThread
    {
        private readonly Thread _thread;

        public ShellThread(Thread thread)
        {
            _thread = thread;
        }

        public string Name
        {
            get { return _thread.Name; }
        }

        public int SuspendCount
        {
            get { return _thread.SuspendCount; }
        }

        public int ID
        {
            get { return _thread.ID; }
        }

        public IEnumerable<ShellStackFrame> StackFrames
        {
            get
            {
                if (null != _thread.StackFrames)
                {
                    foreach (StackFrame sf in _thread.StackFrames)
                    {
                        yield return new ShellStackFrame(sf);
                    }
                }
            }
        }

        public bool IsAlive
        {
            get { return _thread.IsAlive; }
        }

        public string Priority
        {
            get { return _thread.Priority; }
        }

        public string Location
        {
            get { return _thread.Location; }
        }

        public bool IsFrozen
        {
            get { return _thread.IsFrozen; }
        }

        public ShellDebugger Parent
        {
            get { return new ShellDebugger(_thread.Parent as Debugger2); }
        }

        public void Freeze()
        {
            _thread.Freeze();
        }

        public void Thaw()
        {
            _thread.Thaw();
        }

        internal Thread AsThread()
        {
            return _thread;
        }
    }
}