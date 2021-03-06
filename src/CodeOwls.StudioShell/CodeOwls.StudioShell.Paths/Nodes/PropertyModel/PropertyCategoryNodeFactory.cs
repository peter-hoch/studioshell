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
using CodeOwls.StudioShell.Paths.Items;
using EnvDTE80;
using Microsoft.Win32;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.PropertyModel
{
    public class PropertyCategoryNodeFactory : NodeFactoryBase
    {
        private readonly string _category;
        private readonly DTE2 _dte;
        private readonly RegistryKey _subkey;

        public PropertyCategoryNodeFactory(DTE2 dte2, RegistryKey subkey)
        {
            _dte = dte2;
            _subkey = subkey;

            _category = GetCategoryNameFromKey(_subkey.Name);
        }

        public override string Name
        {
            get { return _category; }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(new ShellContainer(this), Name, true);
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            var factories = new List<INodeFactory>();
            foreach (var subkeyName in _subkey.GetSubKeyNames())
            {
                factories.Add(new PropertyPageNodeFactory(_dte, Name, GetCategoryNameFromKey(subkeyName)));
            }
            return factories;
        }

        private string GetCategoryNameFromKey(string subkeyName)
        {
            return subkeyName.Split('/', '\\').Last();
        }
    }
}