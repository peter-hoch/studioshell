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
using VSLangProj;
using VSLangProj80;

namespace CodeOwls.StudioShell.Paths.Items.ProjectModel
{
    public class ShellVSProject2 : ShellProject
    {
        private readonly VSProject2 _project;

        internal ShellVSProject2(VSProject2 project) : base(project.Project)
        {
            _project = project;
        }

        public IEnumerable<ShellReference> References
        {
            get
            {
                return from Reference reference in _project.References
                       select new ShellReference(reference);
            }
        }

        public BuildManager BuildManager
        {
            get { return _project.BuildManager; }
        }

        public ShellProject Project
        {
            get { return new ShellProject(_project.Project); }
        }

        public ShellProjectItem WebReferencesFolder
        {
            get { return new ShellProjectItem(_project.WebReferencesFolder); }
        }

        public string TemplatePath
        {
            get { return _project.TemplatePath; }
        }

        public bool WorkOffline
        {
            get { return _project.WorkOffline; }
            set { _project.WorkOffline = value; }
        }

        public IEnumerable<string> Imports
        {
            get
            {
                return from string import in _project.Imports
                       select import;
            }
        }

        public VSProjectEvents Events
        {
            get { return _project.Events; }
        }

        public object PublishManager
        {
            get { return _project.PublishManager; }
        }

        public VSProjectEvents2 Events2
        {
            get { return _project.Events2; }
        }

        public ShellProjectItem CreateWebReferencesFolder()
        {
            return new ShellProjectItem(_project.CreateWebReferencesFolder());
        }

        public ShellProjectItem AddWebReference(string bstrUrl)
        {
            return new ShellProjectItem(_project.AddWebReference(bstrUrl));
        }

        public void Refresh()
        {
            _project.Refresh();
        }

        public void CopyProject(string bstrDestFolder, string bstrDestUNCPath, prjCopyProjectOption copyProjectOption,
                                string bstrUsername, string bstrPassword)
        {
            _project.CopyProject(bstrDestFolder, bstrDestUNCPath, copyProjectOption, bstrUsername, bstrPassword);
        }

        public void Exec(prjExecCommand command, int bSuppressUI, object varIn, out object pVarOut)
        {
            _project.Exec(command, bSuppressUI, varIn, out pVarOut);
        }

        public void GenerateKeyPairFiles(string strPublicPrivateFile, string strPublicOnlyFile)
        {
            _project.GenerateKeyPairFiles(strPublicPrivateFile, strPublicOnlyFile);
        }

        public string GetUniqueFilename(object pDispatch, string bstrRoot, string bstrDesiredExt)
        {
            return _project.GetUniqueFilename(pDispatch, bstrRoot, bstrDesiredExt);
        }
    }
}