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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EnvDTE80;

namespace CodeOwls.StudioShell
{
    class StatusBarAnimationState
    {
        private readonly DTE2 _applicationObject;
        private bool _isPSIconEnabled;

        public StatusBarAnimationState( DTE2 applicationObject )
        {
            _applicationObject = applicationObject;
        }

        public bool IsPSIconEnabled
        {
            get { return _isPSIconEnabled; }
            set
            {
                if( value == _isPSIconEnabled)
                {
                    return;
                }

                _isPSIconEnabled = value;
                TogglePSIconAnimation();
            }
        }

        void TogglePSIconAnimation()
        {           
            //todo: refactor to utility class
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "resources\\psiconanimated.bmp");
            if( ! File.Exists( path ))
            {
                return;
            }

            _applicationObject.StatusBar.Animate( _isPSIconEnabled, path);
        }
    }
}
