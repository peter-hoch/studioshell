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
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.Attributes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.StudioShell.Common.Exceptions;
using CodeOwls.StudioShell.Common.IoC;
using CodeOwls.StudioShell.Paths.Items;
using CodeOwls.StudioShell.Paths.Nodes.CodeModel;
using CodeOwls.StudioShell.Paths.Nodes.PropertyModel;
using EnvDTE;
using CodeOwls.PowerShell.Provider.PathNodes;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Constants = EnvDTE.Constants;

namespace CodeOwls.StudioShell.Paths.Nodes.ProjectModel
{
    [CmdletHelpPathID("Project")]
    public class ProjectNodeFactory : NodeFactoryBase, INewItem, IRenameItem, IRemoveItem
    {
        protected readonly Project _project;

        public ProjectNodeFactory(Project project)
        {
            _project = project;
        }

        public override string Name
        {
            get { return _project.Name; }
        }

        internal Project Project
        {
            get { return _project; }
        }

        public override IPathNode GetNodeValue()
        {
            object value = ShellObjectFactory.CreateFromProject(_project);
            return new PathNode(value, _project.Name, true);
        }

        internal static INodeFactory Create(object item)
        {
            if (item is Project)
            {
                return new ProjectNodeFactory(item as Project);
            }
            var projectItem = item as ProjectItem;

            System.Diagnostics.Debug.Assert(null != projectItem);

            if (null != projectItem.SubProject)
            {
                return new ProjectNodeFactory(projectItem.SubProject);
            }

            return ProjectItemNodeFactory.Create(projectItem);
        }

        public override IEnumerable<INodeFactory> GetNodeChildren(IContext context)
        {
            foreach (ProjectItem item in _project.ProjectItems)
            {
                yield return ProjectNodeFactory.Create(item);
            }

            var vsproj = _project.Object as VSLangProj.VSProject;
            if (null != vsproj)
            {
                yield return new ReferenceCollectionNodeFactory(vsproj.References);
            }

            if (null != _project.Properties)
            {
                yield return new PropertyCollectionNodeFactory("ProjectProperties", _project.Properties);
            }

            //if (null != _project.CodeModel)
            //{
            //    yield return new ProjectCodeModelNodeFactory(_project.CodeModel);
            //}

            if (null != _project.ConfigurationManager)
            {
                yield return new ConfigurationsCollectionNodeFactory( _project );
            }
        }

        #region Implementation of INewItem

        public IEnumerable<string> NewItemTypeNames
        {
            get { return NewProjectItemManager.NewItemTypeNames; }
        }

        public object NewItemParameters
        {
            get { return NewProjectItemManager.NewItemParameters; }
        }

        public IPathNode NewItem(IContext context, string path, string itemTypeName, object newItemValue)
        {

             return NewProjectItemManager.NewItem(
                _project, 
                _project.ProjectItems, 
                context, 
                path, 
                itemTypeName,
                newItemValue);
        }

        #endregion

        #region Implementation of IRenameItem

        public object RenameItemParameters
        {
            get { return null; }
        }

        public void RenameItem(IContext context, string path, string newName)
        {
            _project.Name = newName;
        }

        #endregion

        #region Implementation of IRemoveItem

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IContext context, string path, bool recurse)
        {
            if (_project.Kind == Constants.vsProjectKindMisc)
            {
                context.WriteWarning(
                    String.Format(
                        "Project {0} cannot be removed since it's of the 'Miscellaneous Files' type",
                        Name
                        ));
                return;
            }

            if (context.Force)
            {
                try
                {
                    _project.Delete();
                    return;
                }
                catch
                {
                }
                try
                {
                    
                    
                    var projPath = _project.FullName; 
                    _project.DTE.Solution.Remove(_project);
                    
                    if( ! String.IsNullOrEmpty( projPath) && 
                        ( Directory.Exists( projPath ) || File.Exists(projPath)))
                    {
                        if (! (_project is SolutionFolder) )
                        {
                            projPath = Path.GetDirectoryName(projPath);
                        }
                        Directory.Delete(projPath, true);
                    }

                }
                catch( Exception e )
                {
                    context.WriteWarning(
                        String.Format(
                            "An exception was raised while deleting project {0}.\r\n{1}",
                            Name, e.ToString()
                        ));
                }

                return;               
            }

            var solutionService = Locator.GetService<SVsSolution>() as IVsSolution;
            if (null == solutionService)
            {
                context.WriteError(
                    new ErrorRecord(
                        new ServiceUnavailableException(typeof(IVsSolution)),
                        "StudioShell.ServiceUnavailable",
                        ErrorCategory.ResourceUnavailable,
                        GetNodeValue()
                        )
                    );
                return;
            }

            IVsHierarchy heirarchy;
            solutionService.GetProjectOfUniqueName(_project.UniqueName, out heirarchy);
            if (null == heirarchy)
            {
                context.WriteWarning("The project " + _project.Name +
                                     " could not be removed: the IVsSolution service failed to locate the project by its unique name. ");
                return;
            }

            solutionService.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, heirarchy, 0);
        }

        #endregion
    }
}