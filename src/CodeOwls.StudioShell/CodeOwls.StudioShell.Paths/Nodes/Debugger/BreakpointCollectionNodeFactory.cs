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
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.StudioShell.Common.Utility;
using CodeOwls.StudioShell.Paths.Items;
using EnvDTE;
using EnvDTE80;

namespace CodeOwls.StudioShell.Paths.Nodes.Debugger
{
    public class BreakpointCollectionNodeFactory : CollectionNodeFactoryBase, INewItem
    {
        private readonly Breakpoints _breakpoints;

        public BreakpointCollectionNodeFactory(Breakpoints breakpoints)
        {
            _breakpoints = breakpoints;
        }

        #region Overrides of NodeFactoryBase

        public override string Name
        {
            get { return NodeNames.Breakpoints; }
        }

        public override IEnumerable<INodeFactory>  GetNodeChildren( IContext context )
        {
            if (null != _breakpoints)
            {
                foreach (Breakpoint2 child in _breakpoints)
                {
                    yield return new BreakpointNodeFactory(child);
                }
            }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode(new ShellContainer(this), Name, true);
        }

        #endregion

        #region Implementation of INewItem

        public IEnumerable<string> NewItemTypeNames
        {
            get { return new[] {"function", "file", "data", "address"}; }
        }

        public object NewItemParameters
        {
            get { return new NewBreakpointParameters(); }
        }

        public IPathNode NewItem(IContext context, string path, string itemTypeName, object newItemValue)
        {
            var p = context.DynamicParameters as NewBreakpointParameters;
            _breakpoints.Add(p.Function, p.File, p.Line, p.Column, p.Condition, p.ConditionType, p.Language, p.Data,
                             p.DataCount, p.Address, p.HitCount, p.HitCountType);

            return new BreakpointNodeFactory(_breakpoints.Item(_breakpoints.Count) as Breakpoint2).GetNodeValue();
        }

        private class NewBreakpointParameters
        {
            [Parameter(Mandatory = true, ParameterSetName = "Address")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public string Address = "";
            
            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public int Column = 1;

            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "Function")] 
            [Parameter(ParameterSetName = "Address")] 
            [Parameter(ParameterSetName = "Data")] 
            public string Condition = "";

            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "Function")] 
            [Parameter(ParameterSetName = "Address")] 
            [Parameter(ParameterSetName = "Data")] 
            public dbgBreakpointConditionType ConditionType = dbgBreakpointConditionType.dbgBreakpointConditionTypeWhenTrue;

            [Parameter(Mandatory = true, ParameterSetName = "Data")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public string Data = "";

            [Parameter(ParameterSetName = "Data")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public int DataCount = 1;
            
            [Parameter(Mandatory = true, ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public string File = "";
            
            [Parameter(Mandatory = true, ParameterSetName = "Function")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public string Function = "";

            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "Function")] 
            [Parameter(ParameterSetName = "Address")] 
            [Parameter(ParameterSetName = "Data")] 
            public int HitCount=0;

            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "Function")] 
            [Parameter(ParameterSetName = "Address")] 
            [Parameter(ParameterSetName = "Data")] 
            public dbgHitCountType HitCountType = dbgHitCountType.dbgHitCountTypeNone;

            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "Function")] 
            [Parameter(ParameterSetName = "Address")] 
            [Parameter(ParameterSetName = "Data")] 
            public string Language = "";

            [Parameter(ParameterSetName = "File")] 
            [Parameter(ParameterSetName = "nameSet")] 
            [Parameter(ParameterSetName = "pathSet")] 
            public int Line = 1;
        }

        #endregion
    }
}