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
using CodeOwls.StudioShell.Paths.Items.UI;
using EnvDTE;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.UI
{
    internal class OutputPaneNodeFactory : NodeFactoryBase, IInvokeItem, IClearItem
    {
        private readonly OutputWindowPane _pane;

        public OutputPaneNodeFactory(OutputWindowPane pane)
        {
            _pane = pane;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _pane.Name; }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(new ShellOutputPane(_pane), Name, false);
        }

        #endregion

        #region Implementation of IInvokeItem

        public object InvokeItemParameters
        {
            get { return null; }
        }

        public IEnumerable<object> InvokeItem(IContext context, string path)
        {
            _pane.Activate();
            return null;
        }

        #endregion

        #region Implementation of IClearItem

        public object ClearItemDynamicParamters
        {
            get { return null; }
        }

        public void ClearItem(IContext context, string path)
        {
            _pane.Clear();
        }

        #endregion
    }
}