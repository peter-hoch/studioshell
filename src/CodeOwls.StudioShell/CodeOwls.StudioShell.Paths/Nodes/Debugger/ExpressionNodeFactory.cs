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
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.StudioShell.Paths.Items.Debugger;
using EnvDTE;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.Debugger
{
    public class ExpressionNodeFactory : NodeFactoryBase
    {
        private readonly Expression _expression;

        public ExpressionNodeFactory(Expression expression)
        {
            _expression = expression;
        }

        #region Overrides of NodeFactoryBase

        private bool IsContainer
        {
            get { return (null != _expression.DataMembers && 0 != _expression.DataMembers.Count); }
        }

        public override string Name
        {
            get { return _expression.Name; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            foreach (Expression x in _expression.DataMembers)
            {
                yield return new ExpressionNodeFactory(x);
            }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(new ShellExpression(_expression), Name, IsContainer);
        }

        #endregion
    }
}