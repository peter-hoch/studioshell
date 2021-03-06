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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CodeOwls.StudioShell.DataPaneControls
{
    [Guid("a059a005-6915-418e-a5a2-9601b577ce7c")]
    [ComVisible(true)]    
    public partial class DataPaneControl : UserControl
    {
        class Item
        {
            public Item( Control control )
            {
                Name = control.Name;
                Control = control;
            }

            public Control Control{ get; set; }
            public string Name { get; set;  }
        }
        public const string Guid = "{a059a005-6915-418e-a5a2-9601b577ce7c}";

        public DataPaneControl()
        {
            InitializeComponent();
            DataPaneControls = new BindingList<Item>();

            comboBox.DataSource = DataPaneControls;

            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "Control";
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            var paneControls = DataPaneControls;
            DataPaneControls = null;
            if (disposing && (null != paneControls))
            {
                paneControls.Where( c=>null != c && null != c.Control).ToList().ForEach( c=>c.Control.Dispose() );                
                paneControls.Clear();
            }


            base.Dispose(disposing);
        }

        BindingList<Item> DataPaneControls { get; set; }
        
        public void AddControl( Control control )
        {
            if( InvokeRequired )
            {
                MethodInvoker mi = () => AddControl(control);
                Invoke(mi);
                return;
            }

            var existing = (from item in DataPaneControls
                            where StringComparer.InvariantCultureIgnoreCase.Equals(item.Name, control.Name)
                            select item).FirstOrDefault();
            if (null != existing)
            {
                existing.Control = control;
            }
            else
            {
                existing = new Item(control);
                DataPaneControls.Add( existing );
            }
            comboBox.SelectedItem = existing;            
            ShowCurrentPane();
        }

        public void RemoveControl(Control control)
        {
            if (InvokeRequired)
            {
                MethodInvoker mi = () => RemoveControl(control);
                Invoke(mi);
                return;
            }

            var existing = (from item in DataPaneControls
                            where StringComparer.InvariantCultureIgnoreCase.Equals(item.Name, control.Name)
                            select item).FirstOrDefault();
            RemoveItem(existing);
        }

        private void OnClickClosePane(object sender, EventArgs e)
        {
            var item = comboBox.SelectedItem as Item;
            if( null == item)
            {
                return;
            }
            RemoveItem(item);
        }

        private void RemoveItem(Item item)
        {
            if (null != item)
            {
                DataPaneControls.Remove(item);
                item.Control.Dispose();
                comboBox.SelectedItem = DataPaneControls.LastOrDefault();
                ShowCurrentPane();
            }
        }

        private void OnSelectedPaneChanged(object sender, EventArgs e)
        {            
            ShowCurrentPane();            
        }

        private void ShowCurrentPane()
        {
            if( InvokeRequired )
            {
                Invoke((MethodInvoker)(ShowCurrentPane));
                return;
            }
            //panel.SuspendLayout();
            var pane = comboBox.SelectedItem as Item;
            if (null == pane)
            {
                return;
            }

            splitContainer.Panel2.Controls.Clear();
            pane.Control.Dock = DockStyle.Fill;
            try
            {
                splitContainer.Panel2.Controls.Add(pane.Control);
            }
            catch
            {
                
            }
            //panel.ResumeLayout();
        }
    }
}
