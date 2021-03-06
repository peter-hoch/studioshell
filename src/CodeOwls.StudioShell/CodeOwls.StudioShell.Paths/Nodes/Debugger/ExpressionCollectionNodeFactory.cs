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
using EnvDTE;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.Debugger
{
    public class ExpressionCollectionNodeFactory : CollectionNodeFactoryBase
    {
        private readonly Expressions _expressions;
        private readonly string _name;

        public ExpressionCollectionNodeFactory(Expressions expressions, string name)
        {
            _expressions = expressions;
            _name = name;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _name; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            foreach (Expression x in _expressions)
            {
                yield return new ExpressionNodeFactory(x);
            }
        }

        #endregion
    }
}