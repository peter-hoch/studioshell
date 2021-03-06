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
using System.Linq;
using CodeOwls.StudioShell.Paths.Items.CommandBars;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace CodeOwls.StudioShell.Paths.Items.UI
{
    public class ShellWindow
    {
        private readonly Window2 _window;

        public ShellWindow(Window2 window)
        {
            _window = window;
        }

        public bool Visible
        {
            get { return _window.Visible; }
            set { _window.Visible = value; }
        }

        public int Left
        {
            get { return _window.Left; }
            set { _window.Left = value; }
        }

        public int Top
        {
            get { return _window.Top; }
            set { _window.Top = value; }
        }

        public int Width
        {
            get { return _window.Width; }
            set { _window.Width = value; }
        }

        public int Height
        {
            get { return _window.Height; }
            set { _window.Height = value; }
        }

        public vsWindowState WindowState
        {
            get
            {
                if (IsFloating)
                {
                    return _window.WindowState;
                }

                return vsWindowState.vsWindowStateNormal;
            }
            set
            {
                if (! IsFloating)
                {
                    return;
                }

                _window.WindowState = value;
            }
        }

        public vsWindowType Type
        {
            get { return _window.Type; }
        }

        public LinkedWindows LinkedWindows
        {
            get { return _window.LinkedWindows; }
        }

        public ShellWindow LinkedWindowFrame
        {
            get
            {
                if (null == _window.LinkedWindowFrame)
                {
                    return null;
                }
                return new ShellWindow(_window.LinkedWindowFrame as Window2);
            }
        }

        public int HWnd
        {
            get { return _window.HWnd; }
        }

        public string Kind
        {
            get { return _window.Kind; }
        }

        public string ObjectKind
        {
            get { return _window.ObjectKind; }
        }

        public object ProjectItem
        {
            get
            {
                if (null == _window.ProjectItem)
                {
                    return null;
                }

                return ShellObjectFactory.CreateFromProjectItem(_window.ProjectItem);
            }
        }

        public object Project
        {
            get
            {
                if (null == _window.Project)
                {
                    return null;
                }
                return ShellObjectFactory.CreateFromProject(_window.Project);
            }
        }

        public Document Document
        {
            get { return _window.Document; }
        }

        public object Selection
        {
            get { return _window.Selection; }
        }

        public bool Linkable
        {
            get { return _window.Linkable; }
            set { _window.Linkable = value; }
        }

        public string Caption
        {
            get { return _window.Caption; }
            set { _window.Caption = value; }
        }

        public bool IsFloating
        {
            get { return _window.IsFloating; }
            set { _window.IsFloating = value; }
        }

        public bool AutoHides
        {
            get { return _window.AutoHides; }
            set { _window.AutoHides = value; }
        }

        public IEnumerable<ShellCommandBar> CommandBars
        {
            get
            {
                if (null == _window.CommandBars)
                {
                    return null;
                }
                var commandBars = (Microsoft.VisualStudio.CommandBars.CommandBars) _window.CommandBars;
                return from CommandBar commandBar in commandBars
                       select new ShellCommandBar(commandBar);
            }
        }

        public void SetFocus()
        {
            _window.SetFocus();
        }

        public void SetKind(vsWindowType eKind)
        {
            _window.SetKind(eKind);
        }

        public void Detach()
        {
            _window.Detach();
        }

        public void Attach(int lWindowHandle)
        {
            _window.Attach(lWindowHandle);
        }

        public void Activate()
        {
            _window.Activate();
        }

        public void Close(vsSaveChanges SaveChanges)
        {
            _window.Close(SaveChanges);
        }

        public void SetSelectionContainer(ref object[] Objects)
        {
            _window.SetSelectionContainer(ref Objects);
        }

        public void SetTabPicture(object Picture)
        {
            _window.SetTabPicture(Picture);
        }

        public object get_DocumentData(string bstrWhichData)
        {
            return _window.get_DocumentData(bstrWhichData);
        }
    }
}