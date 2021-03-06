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
    public class ShellCodeInterface : ShellCodeModelElement2
    {
        private readonly CodeInterface2 _codeInterface;

        internal ShellCodeInterface(CodeInterface2 codeInterface) : base(codeInterface as CodeElement2)
        {
            _codeInterface = codeInterface;
        }

        public ShellCodeNamespace Namespace
        {
            get { return new ShellCodeNamespace(_codeInterface.Namespace); }
        }

        public IEnumerable<IShellCodeModelElement2> Bases
        {
            get { return GetEnumerator(_codeInterface.Bases); }
        }

        public IEnumerable<IShellCodeModelElement2> Members
        {
            get { return GetEnumerator(_codeInterface.Members); }
        }

        public vsCMAccess Access
        {
            get { return _codeInterface.Access; }
            set { _codeInterface.Access = value; }
        }

        public IEnumerable<IShellCodeModelElement2> Attributes
        {
            get { return GetEnumerator(_codeInterface.Members); }
        }

        public string DocComment
        {
            get { return _codeInterface.DocComment; }
            set { _codeInterface.DocComment = value; }
        }

        public string Comment
        {
            get { return _codeInterface.Comment; }
            set { _codeInterface.Comment = value; }
        }

        public IEnumerable<IShellCodeModelElement2> DerivedTypes
        {
            get { return GetEnumerator(_codeInterface.DerivedTypes); }
        }

        public bool IsGeneric
        {
            get { return _codeInterface.IsGeneric; }
        }

        public vsCMDataTypeKind DataTypeKind
        {
            get { return _codeInterface.DataTypeKind; }
            set { _codeInterface.DataTypeKind = value; }
        }

        public IEnumerable<IShellCodeModelElement2> Parts
        {
            get { return GetEnumerator(_codeInterface.Parts); }
        }

        public IShellCodeModelElement2 AddBase(object Base, object Position)
        {
            return ShellObjectFactory.CreateFromCodeElement(_codeInterface.AddBase(Base, Position));
        }

        public ShellCodeAttribute AddAttribute(string Name, string Value, object Position)
        {
            return new ShellCodeAttribute(_codeInterface.AddAttribute(Name, Value, Position) as CodeAttribute2);
        }

        public void RemoveBase(object Element)
        {
            _codeInterface.RemoveBase(Element);
        }

        public void RemoveMember(object Element)
        {
            _codeInterface.RemoveMember(Element);
        }

        public ShellCodeMethod AddFunction(string Name, vsCMFunction Kind, object Type, object Position,
                                           vsCMAccess Access)
        {
            return new ShellCodeMethod(_codeInterface.AddFunction(Name, Kind, Type, Position, Access) as CodeFunction2);
        }

        public IShellCodeModelElement2 AddProperty(string GetterName, string PutterName, object Type, object Position,
                                                   vsCMAccess Access, object Location)
        {
            return
                ShellObjectFactory.CreateFromCodeElement(
                    _codeInterface.AddProperty(GetterName, PutterName, Type, Position, Access, Location) as CodeElement);
        }

        public ShellCodeEvent AddEvent(string Name, string FullDelegateName, bool CreatePropertyStyleEvent,
                                       object Position, vsCMAccess Access)
        {
            return
                new ShellCodeEvent(_codeInterface.AddEvent(Name, FullDelegateName, CreatePropertyStyleEvent, Position,
                                                           Access));
        }

        public bool IsDerivedFrom(string FullName)
        {
            return _codeInterface.get_IsDerivedFrom(FullName);
        }
    }
}