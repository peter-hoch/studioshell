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
using System.IO;
using System.Linq;
using System.Reflection;
using CodeOwls.PowerShell.Host;

namespace CodeOwls.StudioShell.Host
{
    class StudioShellInfo
    {
        internal static string InstallationPath
        {
            get 
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                return Path.GetDirectoryName(asm.Location);
            }
        }

        internal static string ModulePath
        {
            get { return Path.Combine(InstallationPath, "Module\\StudioShell"); }
        }

        internal static string ShellProfilePath
        {
            get 
            { 
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return Path.Combine(appData, CompanyDirectoryName);
            }
        }

        protected static string CompanyDirectoryName
        {
            get { return "CodeOwlsLLC"; }
        }

        internal static Version Version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                return GetAssemblyFileVersion(asm);
            }
        }

        internal static Version ShellVersion
        {
            get
            {
                Assembly asm = typeof (Shell).Assembly;
                return GetAssemblyFileVersion(asm);
            }
        }

        internal static string ShellId
        {
            get { return "CodeOwlsLLC.StudioShell"; }
        }

        internal static string ShellName
        {
            get { return "StudioShell Default Console"; }
        }

        static Version GetAssemblyFileVersion( Assembly asm )
        {
            return (
                from AssemblyFileVersionAttribute attr in
                asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                select new Version(attr.Version.Replace(".*", ""))
            ).First();
        }
    }
}
