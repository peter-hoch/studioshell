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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeOwls.StudioShell.Common.Utility;
using stdole;

namespace CodeOwls.StudioShell.Utility
{
    static class ImageAdapter
    {
        class IAHost : AxHost
        {            
            public IAHost() : base( "{63109182-966B-4e3c-A8B2-8BC4A88D221C}" )
            {
            }

            public IPicture Adapt( Image image )
            {
                return ( IPicture )GetIPictureDispFromPicture(image);
            }

            public IPicture Adapt2(Image image)
            {                
                return (IPicture)GetIPictureFromPicture(image);
            }
        }

        public static IPicture ToIPictureDisp( Image image )
        {
            return Singleton<IAHost>.Instance.Adapt(image);
        }

        public static IPicture ToIPicture(Image image)
        {
            return Singleton<IAHost>.Instance.Adapt2(image);
        }
    }
}
