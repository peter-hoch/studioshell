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
using CodeOwls.StudioShell.Paths.Items.UI;
using EnvDTE;
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Nodes.ProjectModel
{
    class ProjectItemNodeFactory : ProjectItemNodeFactoryBase, IInvokeItem
    {
        public static ProjectItemNodeFactoryBase Create( ProjectItem item )
        {
            if( item.Kind == Constants.vsProjectItemKindPhysicalFolder )
            {
                return new ProjectFolderItemNodeFactory( item );
            }

            return new ProjectItemNodeFactory( item );
        }
        
        protected ProjectItemNodeFactory(ProjectItem item) : base(item)
        {
        }

        #region Implementation of IInvokeItem
        public object InvokeItemParameters
        {
            get { return null; }
        }

        public IEnumerable<object> InvokeItem(IContext context, string path)
        {
            var window = _item.Open(Constants.vsViewKindPrimary) as Window2;
            window.Visible = true;
            window.SetFocus();

            return new[] { new ShellWindow(window) };
        }

        #endregion

    }
}