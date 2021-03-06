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
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.StudioShell.Paths.Items.Debugger;
using EnvDTE80;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.Debugger
{
    public class BreakpointNodeFactory : NodeFactoryBase, IRemoveItem, IClearItem, IInvokeItem
    {
        private readonly Breakpoint2 _breakpoint;

        public BreakpointNodeFactory(Breakpoint2 breakpoint)
        {
            _breakpoint = breakpoint;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _breakpoint.Name; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            List<INodeFactory> factories = new List<INodeFactory>();
            if (null != _breakpoint.Children)
            {
                foreach (Breakpoint2 child in _breakpoint.Children)
                {
                    factories.Add(new BreakpointNodeFactory(child));
                }
            }
            return factories;
        }

        public override IPathNode GetNodeValue()
        {
            var hasChildren = null != _breakpoint.Children && 0 < _breakpoint.Children.Count;
            return new PathNode(new ShellBreakpoint(_breakpoint), Name, hasChildren );
        }

        #endregion

        #region Implementation of IRemoveItem

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IContext context, string path, bool recurse)
        {
            _breakpoint.Delete();
        }

        #endregion

        #region Implementation of IClearItem

        public object ClearItemDynamicParamters
        {
            get { return null; }
        }

        public void ClearItem(IContext context, string path)
        {
            _breakpoint.Enabled = false;
        }

        #endregion

        #region Implementation of IInvokeItem

        public object InvokeItemParameters
        {
            get { return null; }
        }

        public IEnumerable<object> InvokeItem(IContext context, string path)
        {
            _breakpoint.Enabled = true;
            return null;
        }

        #endregion
    }
}