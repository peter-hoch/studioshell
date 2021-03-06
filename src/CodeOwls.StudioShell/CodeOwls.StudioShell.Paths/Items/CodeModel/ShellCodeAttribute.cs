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
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Items.CodeModel
{
    public class ShellCodeAttribute : ShellCodeModelElement2
    {
        private readonly CodeAttribute2 _attribute;

        internal ShellCodeAttribute(CodeAttribute2 codeAttribute2) : base(codeAttribute2 as CodeElement2)
        {
            _attribute = codeAttribute2;
        }

        public object Parent
        {
            get { return _attribute.Parent; }
        }

        public string Value
        {
            get { return _attribute.Value; }
            set { _attribute.Value = value; }
        }

        public string Target
        {
            get { return _attribute.Target; }
            set { _attribute.Target = value; }
        }

        public IEnumerable<ShellCodeAttributeArgument> Arguments
        {
            get
            {
                foreach (CodeAttributeArgument arg in _attribute.Arguments)
                {
                    yield return new ShellCodeAttributeArgument(arg);
                }
            }
        }

        public void Delete()
        {
            _attribute.Delete();
        }

        public ShellCodeAttributeArgument AddArgument(string Value, object Name, object Position)
        {
            return new ShellCodeAttributeArgument(_attribute.AddArgument(Value, Name, Position));
        }
    }
}