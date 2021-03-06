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
using EnvDTE;
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Items.CodeModel
{
    public class ShellCodeProperty : ShellCodeModelElement2
    {
        private readonly CodeProperty _property;

        internal ShellCodeProperty(CodeProperty property)
            : base(property as CodeElement)
        {
            _property = property;
        }

        public ShellCodeTypeReference Type
        {
            get { return new ShellCodeTypeReference(_property.Type as CodeTypeRef2); }
            set { _property.Type = value.AsCodeTypeRef(); }
        }

        public ShellCodeMethod Getter
        {
            get { return new ShellCodeMethod(_property.Getter as CodeFunction2); }
            set { _property.Getter = value.AsCodeFunction(); }
        }

        public ShellCodeMethod Setter
        {
            get { return new ShellCodeMethod(_property.Setter as CodeFunction2); }
            set { _property.Setter = value.AsCodeFunction(); }
        }

        public vsCMAccess Access
        {
            get { return _property.Access; }
            set { _property.Access = value; }
        }

        public IEnumerable<IShellCodeModelElement2> Attributes
        {
            get { return GetEnumerator(_property.Attributes); }
        }

        public string DocComment
        {
            get { return _property.DocComment; }
            set { _property.DocComment = value; }
        }

        public string Comment
        {
            get { return _property.Comment; }
            set { _property.Comment = value; }
        }

        public ShellCodeAttribute AddAttribute(string Name, string Value, object Position)
        {
            return new ShellCodeAttribute(_property.AddAttribute(Name, Value, Position) as CodeAttribute2);
        }

        public string get_Prototype(int Flags)
        {
            return _property.get_Prototype(Flags);
        }

        public object Parent
        {
            get { return _property.Parent; }
        }
    }
}