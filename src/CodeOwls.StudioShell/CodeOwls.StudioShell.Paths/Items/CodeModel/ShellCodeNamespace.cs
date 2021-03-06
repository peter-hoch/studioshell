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
    public class ShellCodeNamespace : ShellCodeModelElement2
    {
        private readonly CodeNamespace _namespace;

        internal ShellCodeNamespace(CodeNamespace @namespace) : base(@namespace as CodeElement)
        {
            _namespace = @namespace;
        }

        public IEnumerable<IShellCodeModelElement2> Members
        {
            get { return GetEnumerator(_namespace.Members); }
        }

        public string DocComment
        {
            get { return _namespace.DocComment; }
            set { _namespace.DocComment = value; }
        }

        public ShellCodeNamespace AddNamespace(string Name, object Position)
        {
            return new ShellCodeNamespace(_namespace.AddNamespace(Name, Position));
        }

        public ShellCodeClass AddClass(string Name, object Position, object Bases, object ImplementedInterfaces,
                                       vsCMAccess Access)
        {
            return
                new ShellCodeClass(
                    _namespace.AddClass(Name, Position, Bases, ImplementedInterfaces, Access) as CodeClass2);
        }

        public ShellCodeInterface AddInterface(string Name, object Position, object Bases, vsCMAccess Access)
        {
            return new ShellCodeInterface(_namespace.AddInterface(Name, Position, Bases, Access) as CodeInterface2);
        }

        public ShellCodeStruct AddStruct(string Name, object Position, object Bases, object ImplementedInterfaces,
                                         vsCMAccess Access)
        {
            return
                new ShellCodeStruct(
                    _namespace.AddStruct(Name, Position, Bases, ImplementedInterfaces, Access) as CodeStruct2);
        }

        public ShellCodeEnum AddEnum(string Name, object Position, object Bases, vsCMAccess Access)
        {
            return new ShellCodeEnum(_namespace.AddEnum(Name, Position, Bases, Access));
        }

        public ShellCodeDelegate AddDelegate(string Name, object Type, object Position, vsCMAccess Access)
        {
            return new ShellCodeDelegate(_namespace.AddDelegate(Name, Type, Position, Access) as CodeDelegate2);
        }

        public void Remove(object Element)
        {
            _namespace.Remove(Element);
        }
    }
}