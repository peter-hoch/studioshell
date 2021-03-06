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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.StudioShell.Paths.Items.Templates;

namespace CodeOwls.StudioShell.Paths.Nodes.ProjectModel
{
    public class NamedItemCollectionNodeFactory : CollectionNodeFactoryBase
    {
        private readonly IEnumerable<INodeFactory> _items;
        private readonly string _name;

        public NamedItemCollectionNodeFactory(string name, IEnumerable<INodeFactory> items)
        {
            _name = name;
            _items = items;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _name; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            return _items;
        }

        #endregion
    }

    public class NamedItemNodeFactory : NodeFactoryBase
    {
        private readonly string _name;
        private readonly object _value;

        public NamedItemNodeFactory(string name, object value)
        {
            _name = name;
            _value = value;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _name; }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(_value, Name, false);
        }

        #endregion
    }

    public class TemplateCollectionNodeFactory : NamedItemCollectionNodeFactory
    {
        public TemplateCollectionNodeFactory(DirectoryInfo projectItemTemplateRoot, DirectoryInfo projectTemplateRoot)
            : base("Templates", new[]
                                    {
                                        new ProjectItemLanguageTemplateCollectionNodeFactory(projectItemTemplateRoot,
                                                                                             "ProjectItems"),
                                        new ProjectItemLanguageTemplateCollectionNodeFactory(projectTemplateRoot,
                                                                                             "Projects"),
                                    })
        {
        }
    }

    public class ProjectItemLanguageTemplateCollectionNodeFactory : CollectionNodeFactoryBase
    {
        private readonly string _name;
        private readonly DirectoryInfo _templateRoot;

        public ProjectItemLanguageTemplateCollectionNodeFactory(DirectoryInfo templateRoot, string name)
        {
            _templateRoot = templateRoot;
            _name = name;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _name; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            return (from dir in _templateRoot.GetDirectories()
                    select new ProjectItemTemplateCollectionNodeFactory(dir, dir.Name)).Cast<INodeFactory>();
        }

        #endregion
    }

    public class ProjectItemTemplateCollectionNodeFactory : CollectionNodeFactoryBase
    {
        private readonly string _name;
        private readonly DirectoryInfo _templateRoot;

        public ProjectItemTemplateCollectionNodeFactory(DirectoryInfo templateRoot, string name)
        {
            _templateRoot = templateRoot;
            _name = name;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return _name; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            Regex re = new Regex(@"_.*$");
            HashSet<INodeFactory> nodes = new HashSet<INodeFactory>( new NodeFactoryNameComparer() );
            //List<INodeFactory> nodes = new List<INodeFactory>();
            //string languageName = Path.GetDirectoryName(_templateRoot.FullName);
                    (from file in _templateRoot.GetFiles("*.zip", SearchOption.AllDirectories)
                        let name = re.Replace( file.Name, String.Empty )
                     let t = new ShellTemplate(name, _name)
                    select new NamedItemNodeFactory(name, t)).Cast<INodeFactory>().ToList().ForEach( n=>nodes.Add(n) );
            (from file in _templateRoot.GetFiles( "*.vstemplate", SearchOption.AllDirectories)
                    let name = file.Directory.Name
                    let t = new ShellTemplate( name, _name )
             select new NamedItemNodeFactory(name, t)).Cast<INodeFactory>().ToList().ForEach(n => nodes.Add(n));
            
            nodes.ToList().Sort(NodeFactoryNameComparer.Compare);
            
            return nodes;
        }

        #endregion
    }

    public class NodeFactoryNameComparer : IEqualityComparer<INodeFactory>
    {
        public static int Compare(INodeFactory a, INodeFactory b)
        {
            if (null == a || null == b)
            {
                return 0;
            }
            return StringComparer.InvariantCultureIgnoreCase.Compare(a.Name, b.Name);
        }
        public bool Equals(INodeFactory x, INodeFactory y)
        {
            if (x == null)
            {
                return y == null;
            }
            return StringComparer.InvariantCultureIgnoreCase.Equals(x.Name, y.Name);
        }

        public int GetHashCode(INodeFactory obj)
        {            
            return obj.Name.GetHashCode();
        }
    }
}