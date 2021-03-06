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


namespace CodeOwls.StudioShell.DataPaneControls
{
    partial class DataPaneControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPaneControl));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.buttonClosePane = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.comboBox);
            this.splitContainer.Panel1.Controls.Add(this.buttonClosePane);
            this.splitContainer.Size = new System.Drawing.Size(545, 256);
            this.splitContainer.SplitterDistance = 28;
            this.splitContainer.TabIndex = 3;
            // 
            // comboBox
            // 
            this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox.DisplayMember = "Name";
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(8, 5);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(505, 21);
            this.comboBox.TabIndex = 4;
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.OnSelectedPaneChanged);
            // 
            // buttonClosePane
            // 
            this.buttonClosePane.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClosePane.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.buttonClosePane.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.HotTrack;
            this.buttonClosePane.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClosePane.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonClosePane.ImageIndex = 0;
            this.buttonClosePane.ImageList = this.imageList;
            this.buttonClosePane.Location = new System.Drawing.Point(519, 0);
            this.buttonClosePane.Name = "buttonClosePane";
            this.buttonClosePane.Size = new System.Drawing.Size(23, 25);
            this.buttonClosePane.TabIndex = 3;
            this.buttonClosePane.UseVisualStyleBackColor = true;
            this.buttonClosePane.Click += new System.EventHandler(this.OnClickClosePane);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(254)))), ((int)(((byte)(0)))));
            this.imageList.Images.SetKeyName(0, "PSIconStop.bmp");
            // 
            // DataPaneControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "DataPaneControl";
            this.Size = new System.Drawing.Size(545, 256);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button buttonClosePane;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ComboBox comboBox;
    }
}
