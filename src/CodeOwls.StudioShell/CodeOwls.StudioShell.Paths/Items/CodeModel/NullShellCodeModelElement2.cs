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
using CodeOwls.StudioShell.Paths.Items.ProjectModel;
using EnvDTE;

namespace CodeOwls.StudioShell.Paths.Items.CodeModel
{
    public class NullShellCodeModelElement2 : IShellCodeModelElement2
    {
        public bool IsContainer
        {
            get { return false; }
        }

        public string Name
        {
            get { return null; }
            set { }
        }

        public string FullName
        {
            get { return null; }
        }

        public ShellProjectItem ProjectItem
        {
            get { return null; }
        }

        public vsCMElement Kind
        {
            get { return vsCMElement.vsCMElementImportStmt; }
        }

        public bool IsCodeType
        {
            get { return false; }
        }

        public vsCMInfoLocation InfoLocation
        {
            get { return vsCMInfoLocation.vsCMInfoLocationNone; }
        }

        public IEnumerable<IShellCodeModelElement2> Children
        {
            get { return null; }
        }

        public string Language
        {
            get { return null; }
        }

        public TextPoint StartPoint
        {
            get { return null; }
        }

        public TextPoint EndPoint
        {
            get { return null; }
        }

        public string ElementID
        {
            get { return null; }
        }

        public TextPoint GetStartPoint(vsCMPart Part)
        {
            return null;
        }

        public TextPoint GetEndPoint(vsCMPart Part)
        {
            return null;
        }

        public void RenameSymbol(string NewName)
        {
        }
    }
}