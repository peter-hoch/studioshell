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


using EnvDTE;

namespace CodeOwls.StudioShell.Paths.Items.UI
{
    internal class ShellTaskItem
    {
        private readonly TaskItem _item;

        internal ShellTaskItem(TaskItem item)
        {
            _item = item;
        }

        public string Category
        {
            get { return _item.Category; }
        }

        public string SubCategory
        {
            get { return _item.SubCategory; }
        }

        public vsTaskPriority Priority
        {
            get { return _item.Priority; }
            set { _item.Priority = value; }
        }

        public string Description
        {
            get { return _item.Description; }
            set { _item.Description = value; }
        }

        public string FileName
        {
            get { return _item.FileName; }
            set { _item.FileName = value; }
        }

        public int Line
        {
            get { return _item.Line; }
            set { _item.Line = value; }
        }

        public bool Displayed
        {
            get { return _item.Displayed; }
        }

        public bool Checked
        {
            get { return _item.Checked; }
            set { _item.Checked = value; }
        }

        public void Navigate()
        {
            _item.Navigate();
        }

        public void Delete()
        {
            _item.Delete();
        }

        public void Select()
        {
            _item.Select();
        }

        public bool get_IsSettable(vsTaskListColumn Column)
        {
            return _item.get_IsSettable(Column);
        }
    }
}