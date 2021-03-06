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


using System.Management.Automation;
using System.Windows.Forms;
using CodeOwls.StudioShell.DataPaneControls;

namespace CodeOwls.StudioShell.Cmdlets
{
    [Cmdlet(VerbsData.Out, "grid")]
    public class OutGridCmdlet : OutSubsetDataPaneCmdletBase
    {

        protected override Control GetPaneControl()
        {
            var gridControl = new DataGridControl();
            gridControl.SetDataSource(_data);
            gridControl.Name = Name;
            return gridControl;
        }
    }
}
