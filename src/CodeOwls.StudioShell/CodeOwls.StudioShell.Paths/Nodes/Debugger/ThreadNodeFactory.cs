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
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.StudioShell.Paths.Items.Debugger;
using EnvDTE;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.Debugger
{
    public class ThreadNodeFactory : NodeFactoryBase
    {
        private readonly Thread _thread;

        public ThreadNodeFactory(Thread thread)
        {
            _thread = thread;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return IsUsableThreadName(_thread.Name) ? _thread.Name : _thread.ID.ToString(); }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            int index = 0;
            foreach (StackFrame frame in _thread.StackFrames)
            {
                yield return new StackFrameNodeFactory(index++, frame);
            }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(new ShellThread(_thread), Name, true);
        }

        private bool IsUsableThreadName(string name)
        {
            return (!String.IsNullOrEmpty(_thread.Name) &&
                    !_thread.Name.Contains("No Name"));
        }

        #endregion
    }
}