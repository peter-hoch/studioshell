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
using System.Security.AccessControl;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.StudioShell.Common.Utility;
using CodeOwls.StudioShell.Paths.Items;
using EnvDTE80;
using Microsoft.Win32;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.PropertyModel
{
    public class PropertyCategoriesNodeFactory : NodeFactoryBase
    {
        private readonly DTE2 _dte;
        private readonly string _settingsKeyPath;

        public PropertyCategoriesNodeFactory(DTE2 dte, string settingsKeyPath)
        {
            _dte = dte;
            _settingsKeyPath = settingsKeyPath;
        }

        public override string Name
        {
            get { return NodeNames.Properties; }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(new ShellContainer(this), Name, true);
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            var factories = new List<INodeFactory>();
            using (var settingsKey = Registry.LocalMachine.OpenSubKey(_settingsKeyPath, false))
            {
                foreach (var subkeyName in settingsKey.GetSubKeyNames())
                {
                    var subkey = settingsKey.OpenSubKey(
                        subkeyName,
                        RegistryKeyPermissionCheck.ReadSubTree,
                        RegistryRights.QueryValues | RegistryRights.ReadKey);

                    factories.Add(new PropertyCategoryNodeFactory(_dte, subkey));
                }
            }
            return factories;
        }
    }
}