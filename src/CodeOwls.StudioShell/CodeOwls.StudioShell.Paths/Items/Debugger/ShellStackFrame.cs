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
using EnvDTE;

namespace CodeOwls.StudioShell.Paths.Items.Debugger
{
    public class ShellStackFrame
    {
        private readonly StackFrame _frame;

        public ShellStackFrame(StackFrame frame)
        {
            _frame = frame;
        }

        public string Language
        {
            get { return _frame.Language; }
        }

        public string FunctionName
        {
            get { return _frame.FunctionName; }
        }

        public string ReturnType
        {
            get { return _frame.ReturnType; }
        }

        public IEnumerable<ShellExpression> Locals
        {
            get
            {
                if (null != _frame.Locals)
                {
                    foreach (Expression x in _frame.Locals)
                    {
                        yield return new ShellExpression(x);
                    }
                }
            }
        }

        public IEnumerable<ShellExpression> Arguments
        {
            get
            {
                if (null != _frame.Arguments)
                {
                    foreach (Expression x in _frame.Arguments)
                    {
                        yield return new ShellExpression(x);
                    }
                }
            }
        }

        public string Module
        {
            get { return _frame.Module; }
        }

        public ShellThread Parent
        {
            get
            {
                if (null == _frame.Parent)
                {
                    return null;
                }
                return new ShellThread(_frame.Parent);
            }
        }

        internal StackFrame AsStackFrame()
        {
            return _frame;
        }
    }
}