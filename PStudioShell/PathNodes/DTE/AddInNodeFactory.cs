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
using System.Linq;
using System.Text;
using EnvDTE;
using CodeOwls.StudioShell.DTE.AddIns;

namespace CodeOwls.StudioShell.PathNodes
{
    public class AddInNodeFactory : NodeFactoryBase
    {
        private readonly AddIn _addIn;

        public AddInNodeFactory(AddIn addIn)
        {
            _addIn = addIn;
        }

        #region Overrides of NodeFactoryBase

        public override IPathNode GetNodeValue()
        {
            return new PathNode( new ShellAddIn( _addIn ), Name, false );
        }

        public override string Name
        {
            get { return _addIn.Name; }
        }

        #endregion
    }
}
