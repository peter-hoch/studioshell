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
using CodeOwls.StudioShell.DTE.Debugger;
using EnvDTE;

namespace CodeOwls.StudioShell.PathNodes
{
    public class StackFrameNodeFactory : NodeFactoryBase
    {
        private readonly int _index;
        private StackFrame _frame;

        public StackFrameNodeFactory(int index, StackFrame frame)
        {
            _index = index;
            _frame = frame;            
        }

        #region Overrides of NodeFactoryBase

        public override IEnumerable<INodeFactory> GetNodeChildren()
        {
            return new INodeFactory[]
                       {
                           new ExpressionCollectionNodeFactory(_frame.Arguments, "Arguments"),
                           new ExpressionCollectionNodeFactory(_frame.Locals, "Locals")
                       };
        }
        public override IPathNode GetNodeValue()
        {
            return new PathNode( new ShellStackFrame(_frame), Name, true );
        }

        public override string Name
        {
            get
            {
                if( 0 == _index )
                {
                    return "Current";
                }

                return "Frame" + _index;
            }
        }

        #endregion
    }
}
