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


using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Items.CodeModel
{
    public class ShellCodeImport : ShellCodeModelElement2
    {
        private readonly CodeImport _import;

        internal ShellCodeImport(CodeImport import) : base(import as CodeElement2)
        {
            _import = import;
        }

        public override string Name
        {
            get { return FullName; }
            set { }
        }

        public override string FullName
        {
            get { return "Import " + _import.Namespace; }
        }

        public string Namespace
        {
            get { return _import.Namespace; }
            set { _import.Namespace = value; }
        }

        public string Alias
        {
            get { return _import.Alias; }
            set { _import.Alias = value; }
        }

        public object Parent
        {
            get { return _import.Parent; }
        }
    }
}