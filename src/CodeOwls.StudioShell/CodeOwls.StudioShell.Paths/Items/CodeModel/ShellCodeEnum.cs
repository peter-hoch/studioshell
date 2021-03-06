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
    public class ShellCodeEnum : ShellCodeModelElement2
    {
        private readonly CodeEnum _enum;

        internal ShellCodeEnum(CodeEnum @enum) : base(@enum as CodeElement)
        {
            _enum = @enum;
        }

        public object Parent
        {
            get { return _enum.Parent; }
        }

        public ShellCodeNamespace Namespace
        {
            get { return new ShellCodeNamespace(_enum.Namespace); }
        }

        public IEnumerable<IShellCodeModelElement2> Bases
        {
            get { return GetEnumerator(_enum.Bases); }
        }

        public IEnumerable<IShellCodeModelElement2> Members
        {
            get { return GetEnumerator(_enum.Members); }
        }

        public vsCMAccess Access
        {
            get { return _enum.Access; }
            set { _enum.Access = value; }
        }

        public IEnumerable<IShellCodeModelElement2> Attributes
        {
            get { return GetEnumerator(_enum.Attributes); }
        }

        public string DocComment
        {
            get { return _enum.DocComment; }
            set { _enum.DocComment = value; }
        }

        public string Comment
        {
            get { return _enum.Comment; }
            set { _enum.Comment = value; }
        }

        public IEnumerable<IShellCodeModelElement2> DerivedTypes
        {
            get { return GetEnumerator(_enum.DerivedTypes); }
        }

        public IShellCodeModelElement2 AddBase(object Base, object Position)
        {
            return ShellObjectFactory.CreateFromCodeElement(_enum.AddBase(Base, Position));
        }

        public ShellCodeAttribute AddAttribute(string Name, string Value, object Position)
        {
            return new ShellCodeAttribute(_enum.AddAttribute(Name, Value, Position) as CodeAttribute2);
        }

        public void RemoveBase(object Element)
        {
            _enum.RemoveBase(Element);
        }

        public void RemoveMember(object Element)
        {
            _enum.RemoveMember(Element);
        }

        public ShellCodeVariable AddMember(string Name, object Value, object Position)
        {
            return new ShellCodeVariable(_enum.AddMember(Name, Value, Position) as CodeVariable2);
        }

        public bool get_IsDerivedFrom(string FullName)
        {
            return _enum.get_IsDerivedFrom(FullName);
        }
    }
}