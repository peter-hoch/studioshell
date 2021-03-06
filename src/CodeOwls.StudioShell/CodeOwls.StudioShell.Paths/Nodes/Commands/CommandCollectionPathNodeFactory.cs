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
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using CodeOwls.PowerShell.Provider.Attributes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.StudioShell.Common.Exceptions;
using CodeOwls.StudioShell.Common.IoC;
using CodeOwls.StudioShell.Common.Utility;
using CodeOwls.StudioShell.Paths.Utility;
using EnvDTE;
using EnvDTE80;
using Command = EnvDTE.Command;

namespace CodeOwls.StudioShell.Paths.Nodes.Commands
{
    [CmdletHelpPathID("CommandCollection")]
    public class CommandCollectionPathNodeFactory : CollectionNodeFactoryBase, INewItem
    {
        #region CommandUIType enum

        public enum CommandUIType
        {
            Picture,
            Text,
            BothPictureAndText
        }

        #endregion

        private readonly Commands2 _commands;

        public CommandCollectionPathNodeFactory(Commands2 commands)
        {
            _commands = commands;
        }

        public override string Name
        {
            get { return NodeNames.Commands; }
        }

        #region Implementation of INewItem

        public IEnumerable<string> NewItemTypeNames
        {
            get { return null; }
        }

        public object NewItemParameters
        {
            get { return new NewItemDynamicParameters(); }
        }

        public IPathNode NewItem(IContext context, string path, string itemTypeName, object value)
        {
            AddIn addIn = Locator.Get<AddIn>();
            if (null == addIn)
            {
                throw new ServiceUnavailableException(
                    typeof(AddIn), 
                    "Note that the new-item cmdlet is not available for Visual Studio Commands when using the StudioShell standalone provider."
                );
            }

            var validValueTypes = new[] { typeof(ScriptBlock), typeof(string) };
            if (!validValueTypes.Contains(value.GetType()))
            {
                var validNames = String.Join(", ", validValueTypes.ToList().ConvertAll(t => t.FullName).ToArray());
                throw new ArgumentException("new item values must be one of the following types: " + validNames);
            }

            var p = context.DynamicParameters as NewItemDynamicParameters ?? new NewItemDynamicParameters();
            object[] contextGuids = new object[] {};

            string functionName = FunctionUtilities.GetFunctionNameFromPath(path);
            
            var cmd = _commands.AddNamedCommand2(
                addIn,
                path,
                p.Label ?? path,
                p.Tooltip ?? String.Empty,
                true,
                null,
                ref contextGuids,
                (int) vsCommandStatus.vsCommandStatusSupported + (int) vsCommandStatus.vsCommandStatusEnabled,
                (int) vsCommandStyle.vsCommandStylePictAndText,
                vsCommandControlType.vsCommandControlTypeButton
                );

            if (null != p.Bindings)
            {
                cmd.Bindings = p.Bindings;
            }

            string command = "new-item -path function:global:" + functionName + " -value {" + value + "} ";

            context.InvokeCommand.InvokeScript(command, false, PipelineResultTypes.None, null, null);
            
            var factory = new CommandNodeFactory(cmd);
            return factory.GetNodeValue();
        }

        #endregion

        public override IEnumerable<INodeFactory> Resolve(IContext context, string nodeName)
        {
            Command command = null;
            try
            {
                command = _commands.Item(nodeName);
            }
            catch
            {
            }

            if (null != command)
            {
                yield return new CommandNodeFactory(command);
            }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            foreach (Command command in _commands)
            {
                yield return new CommandNodeFactory(command);
            }
        }

        #region Nested type: NewItemDynamicParameters

        public class NewItemDynamicParameters
        {
            [Parameter(
                HelpMessage = "The name of the command, as shown in UI",
                Mandatory = false
                )]
            public string Label { get; set; }

            [Parameter(
                HelpMessage = "The tooltip for the command",
                Mandatory = false
                )]
            public string Tooltip { get; set; }

            [Parameter(
                HelpMessage = "The key bindings for the command",
                Mandatory = false
                )]
            public object[] Bindings { get; set; }

            [Parameter(
                HelpMessage = "If specified, the command is currently enabled"
                )]
            public SwitchParameter Enabled { get; set; }

            [Parameter(
                HelpMessage = "If specified, the command is currently invisible"
                )]
            public SwitchParameter Invisible { get; set; }

            [Parameter(
                HelpMessage = "If specified, the command is currently supported"
                )]
            public SwitchParameter Supported { get; set; }

            [Parameter(
                HelpMessage = "If specified, the command is currently unsupported"
                )]
            public SwitchParameter Unsupported { get; set; }

            [Parameter(
                HelpMessage = "The UI type for this command; can be Text, Picture, or BothPictureAndText"
                )]
            public CommandUIType UI { get; set; }

            public SwitchParameter ComboBox { get; set; }
            public SwitchParameter Button { get; set; }
            public SwitchParameter MRUComboBox { get; set; }
            public SwitchParameter MRUButton { get; set; }
        }

        #endregion
    }
}