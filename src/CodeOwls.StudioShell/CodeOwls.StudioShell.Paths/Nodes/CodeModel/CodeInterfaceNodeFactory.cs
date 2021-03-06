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
using CodeOwls.StudioShell.Paths.Utility;
using EnvDTE;
using EnvDTE80;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.StudioShell.Paths.Nodes.CodeModel
{
    public class CodeInterfaceNodeFactory : CodeElementWithChildrenNodeFactory
    {
        private readonly CodeInterface2 _interface;

        public CodeInterfaceNodeFactory(CodeInterface2 element) : base(element as CodeElement)
        {
            _interface = element;
        }

        protected override string CodeItemTypeName
        {
            get { return CodeItemTypes.Interface; }
        }

        public override IEnumerable<string> NewItemTypeNames
        {
            get { return CodeItemTypes.InterfaceNewItemTypeNames; }
        }

        protected override object NewEvent(NewCodeElementItemParams newCodeElementItemParams, string path)
        {
            return _interface.AddEvent(
                path,
                newCodeElementItemParams.MemberType,
                false,
                Type.Missing,
                newCodeElementItemParams.Access.ToCMAccess()
                );
        }

        protected override object NewProperty(NewCodeElementItemParams newItemParams, string path)
        {
            return _interface.AddProperty(newItemParams.Get ? path : null,
                                          newItemParams.Set ? path : null,
                                          newItemParams.MemberType,
                                          newItemParams.Position,
                                          newItemParams.AccessKind,
                                          Type.Missing);
        }

        protected override object NewMethod(NewCodeElementItemParams newItemParams, string path)
        {
            return _interface.AddFunction(path,
                                          newItemParams.FunctionKind,
                                          newItemParams.MemberType,
                                          newItemParams.Position,
                                          newItemParams.AccessKind);
        }

        protected override object NewAttribute(NewCodeElementItemParams p, string path, object newItemValue)
        {
            var value = null == newItemValue ? null : newItemValue.ToString();
            return _interface.AddAttribute(path,
                                           value,
                                           p.Position.ToDTEParameter());
        }
    }
}