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


using Microsoft.VisualStudio.CommandBars;

namespace CodeOwls.StudioShell.Paths.Items.CommandBars
{
    internal class ShellCommandBarPopup
    {
        private readonly CommandBarPopup _popup;

        internal ShellCommandBarPopup(CommandBarPopup popup)
        {
            _popup = popup;
        }

        public int Creator
        {
            get { return _popup.Creator; }
        }

        public bool BeginGroup
        {
            get { return _popup.BeginGroup; }
            set { _popup.BeginGroup = value; }
        }

        public bool BuiltIn
        {
            get { return _popup.BuiltIn; }
        }

        public string Caption
        {
            get { return _popup.Caption; }
            set { _popup.Caption = value; }
        }

        public string DescriptionText
        {
            get { return _popup.DescriptionText; }
            set { _popup.DescriptionText = value; }
        }

        public bool Enabled
        {
            get { return _popup.Enabled; }
            set { _popup.Enabled = value; }
        }

        public int Height
        {
            get { return _popup.Height; }
            set { _popup.Height = value; }
        }

        public int Id
        {
            get { return _popup.Id; }
        }

        public int Index
        {
            get { return _popup.Index; }
        }

        public int InstanceId
        {
            get { return _popup.InstanceId; }
        }

        public ShellCommandBar Parent
        {
            get { return new ShellCommandBar(_popup.Parent); }
        }

        public string Parameter
        {
            get { return _popup.Parameter; }
            set { _popup.Parameter = value; }
        }

        public string Tag
        {
            get { return _popup.Tag; }
            set { _popup.Tag = value; }
        }

        public string TooltipText
        {
            get { return _popup.TooltipText; }
            set { _popup.TooltipText = value; }
        }

        public MsoControlType Type
        {
            get { return _popup.Type; }
        }

        public bool Visible
        {
            get { return _popup.Visible; }
            set { _popup.Visible = value; }
        }

        public int Width
        {
            get { return _popup.Width; }
            set { _popup.Width = value; }
        }

        public ShellCommandBar CommandBar
        {
            get { return new ShellCommandBar(_popup.CommandBar); }
        }

        public ShellCommandBarPopup Copy(object Bar, object Before)
        {
            return new ShellCommandBarPopup((CommandBarPopup) _popup.Copy(Bar, Before));
        }

        public void Delete(object Temporary)
        {
            _popup.Delete(Temporary);
        }

        public void Execute()
        {
            _popup.Execute();
        }

        public ShellCommandBarPopup Move(object Bar, object Before)
        {
            return new ShellCommandBarPopup((CommandBarPopup) _popup.Move(Bar, Before));
        }

        public void Reset()
        {
            _popup.Reset();
        }

        public void SetFocus()
        {
            _popup.SetFocus();
        }
    }
}