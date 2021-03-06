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


using CodeOwls.StudioShell.Paths.Items.CodeModel;
using CodeOwls.StudioShell.Paths.Items.CommandBars;
using CodeOwls.StudioShell.Paths.Items.ProjectModel;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using VSLangProj80;

namespace CodeOwls.StudioShell.Paths.Items
{
    public class ShellObjectFactory
    {
        public static object CreateFromProject(Project project)
        {
            if (Constants.vsProjectItemKindSolutionItems == project.Kind)
            {
                return new ShellSolutionFolder(project);
            }
            if (project.Object is VSProject2)
            {
                return new ShellVSProject2(project.Object as VSProject2);
            }

            return new ShellProject(project);
        }

        public static object CreateFromProjectItem(ProjectItem item)
        {
            return new ShellProjectItem(item);
        }

        public static IShellCodeModelElement2 CreateFromCodeElement(CodeElement element)
        {
            if (element is CodeNamespace)
            {
                return new ShellCodeNamespace(element as CodeNamespace);
            }
            if (element is CodeImport)
            {
                return new ShellCodeImport(element as CodeImport);
            }
            if (element is CodeClass2)
            {
                return new ShellCodeClass(element as CodeClass2);
            }
            if (element is CodeInterface2)
            {
                return new ShellCodeInterface(element as CodeInterface2);
            }
            if (element is CodeProperty2)
            {
                return new ShellCodeProperty2(element as CodeProperty2);
            }
            if (element is CodeProperty)
            {
                return new ShellCodeProperty(element as CodeProperty);
            }
            if (element is CodeFunction2)
            {
                return new ShellCodeMethod(element as CodeFunction2);
            }
            if (element is CodeEvent)
            {
                return new ShellCodeEvent(element as CodeEvent);
            }
            if (element is CodeVariable2)
            {
                return new ShellCodeVariable(element as CodeVariable2);
            }
            if (element is CodeEnum)
            {
                return new ShellCodeEnum(element as CodeEnum);
            }
            if (element is CodeAttribute2)
            {
                return new ShellCodeAttribute(element as CodeAttribute2);
            }
            if (element is CodeDelegate2)
            {
                return new ShellCodeDelegate(element as CodeDelegate2);
            }
            if (element is CodeParameter2)
            {
                return new ShellCodeParameter(element as CodeParameter2);
            }
            if (element is CodeAttributeArgument)
            {
                return new ShellCodeAttributeArgument(element as CodeAttributeArgument);
            }
            if (element is CodeStruct2)
            {
                return new ShellCodeStruct(element as CodeStruct2);
            }
            if (element is CodeType)
            {
                return new ShellCodeType(element as CodeType);
            }
            if (element is CodeTypeRef2)
            {
                return new ShellCodeTypeReference(element as CodeTypeRef2);
            }

            if (element is CodeElement2)
            {
                var kind = element.Kind;
                var prp = element as CodeProperty;
                IShellCodeModelElement2 item = new ShellCodeModelElement2(element as CodeElement2);

                return item;
            }

            return null;
        }

        public static object CreateFromCommandBarControl(object control)
        {
            if (control is CommandBar)
            {
                return new ShellCommandBar(control as CommandBar);
            }
            if (control is CommandBarButton)
            {
                return new ShellCommandBarButton(control as CommandBarButton);
            }
            if (control is CommandBarPopup)
            {
                return new ShellCommandBarPopup(control as CommandBarPopup);
            }
            if (control is CommandBarComboBox)
            {
                return new ShellCommandBarComboBox(control as CommandBarComboBox);
            }

            return null;
        }
    }
}