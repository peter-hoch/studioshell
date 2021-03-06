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
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.StudioShell.Paths.Nodes.CodeModel;
using EnvDTE;
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Nodes.DTE
{
    internal class SelectedCodeModelItemNodeFactory : NodeFactoryBase
    {
        private readonly vsCMElement _codeElementType;
        private readonly DTE2 _dte;
        private readonly string _name;

        public SelectedCodeModelItemNodeFactory(DTE2 dte, vsCMElement codeElementType, string name)
        {
            _dte = dte;
            _codeElementType = codeElementType;
            _name = name;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _name; }
        }

        public override IPathNode GetNodeValue()
        {
            INodeFactory factory = FileCodeModelNodeFactory.CreateNodeFactoryFromCurrentSelection(_dte, _codeElementType);
            if (null == factory)
            {
                return null;
            }

            factory = new RenamedNodeFactoryDecorator(factory, _name);
            return factory.GetNodeValue();
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            INodeFactory factory = FileCodeModelNodeFactory.CreateNodeFactoryFromCurrentSelection(_dte, _codeElementType);
            if (null == factory)
            {
                return null;
            }
            factory = new RenamedNodeFactoryDecorator(factory, _name);
            return factory.GetNodeChildren( context );
        }

        #endregion
    }
}