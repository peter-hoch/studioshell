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
    public class ShellCodeDelegate : ShellCodeModelElement2
    {
        private readonly CodeDelegate2 _delegate;

        internal ShellCodeDelegate(CodeDelegate2 @delegate) : base(@delegate as CodeElement2)
        {
            _delegate = @delegate;
        }

        public object Parent
        {
            get { return _delegate.Parent; }
        }

        public ShellCodeNamespace Namespace
        {
            get { return new ShellCodeNamespace(_delegate.Namespace); }
        }

        public IEnumerable<IShellCodeModelElement2> Bases
        {
            get { return GetEnumerator(_delegate.Bases); }
        }

        public IEnumerable<IShellCodeModelElement2> Members
        {
            get { return GetEnumerator(_delegate.Members); }
        }

        public vsCMAccess Access
        {
            get { return _delegate.Access; }
            set { _delegate.Access = value; }
        }

        public IEnumerable<IShellCodeModelElement2> Attributes
        {
            get { return GetEnumerator(_delegate.Attributes); }
        }

        public string DocComment
        {
            get { return _delegate.DocComment; }
            set { _delegate.DocComment = value; }
        }

        public string Comment
        {
            get { return _delegate.Comment; }
            set { _delegate.Comment = value; }
        }

        public IEnumerable<IShellCodeModelElement2> DerivedTypes
        {
            get { return GetEnumerator(_delegate.DerivedTypes); }
        }

        public ShellCodeClass BaseClass
        {
            get { return new ShellCodeClass(_delegate.BaseClass as CodeClass2); }
        }

        public ShellCodeTypeReference Type
        {
            get { return new ShellCodeTypeReference(_delegate.Type as CodeTypeRef2); }
            set { _delegate.Type = value.AsCodeTypeRef(); }
        }

        public IEnumerable<IShellCodeModelElement2> Parameters
        {
            get { return GetEnumerator(_delegate.Parameters); }
        }

        public bool IsGeneric
        {
            get { return _delegate.IsGeneric; }
        }

        public IShellCodeModelElement2 AddBase(object Base, object Position)
        {
            return ShellObjectFactory.CreateFromCodeElement(_delegate.AddBase(Base, Position));
        }

        public ShellCodeAttribute AddAttribute(string Name, string Value, object Position)
        {
            return new ShellCodeAttribute(_delegate.AddAttribute(Name, Value, Position) as CodeAttribute2);
        }

        public void RemoveBase(object Element)
        {
            _delegate.RemoveBase(Element);
        }

        public void RemoveMember(object Element)
        {
            _delegate.RemoveMember(Element);
        }

        public ShellCodeParameter AddParameter(string Name, object Type, object Position)
        {
            return new ShellCodeParameter(_delegate.AddParameter(Name, Type, Position) as CodeParameter2);
        }

        public void RemoveParameter(object Element)
        {
            _delegate.RemoveParameter(Element);
        }

        public bool get_IsDerivedFrom(string FullName)
        {
            return _delegate.get_IsDerivedFrom(FullName);
        }

        public string get_Prototype(int Flags)
        {
            return _delegate.get_Prototype(Flags);
        }
    }
}