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
using System.Text.RegularExpressions;

namespace CodeOwls.StudioShell.Host
{
    public static class EnvironmentConfiguration
    {
        static public void UpdateEnvironmentForHost()
        {
            var personalModulePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "WindowsPowershell\\Modules"
                );

            AddEnvironmentPath("psmodulepath", personalModulePath);

            UpdateEnvironmentForModule();
        }

        static public void UpdateEnvironmentForModule()
        {
            var rootPath = StudioShellInfo.InstallationPath;

            AddEnvironmentPath("psmodulepath", rootPath, "..\\..");
            AddEnvironmentPath("path", rootPath, "Scripts");

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            /* this is nasty, but Visual Studio uses an assembly redirect to load EnvDTE version 8.0.0.0
             * when anyone attempts to load versions 2.0.0.0-7.0.0.0.  Redirects can't be applied in powershell
             * because the dll config files are not loaded during a module import (apparantly).  so instead the
             * environment is altered to provide an explicit assembly reference when the EnvDTE assembly is 
             * requested and cannot be found.
             * 
             * this only applies when using StudioShell from the powershell console; when using it from Visual
             * Studio the binding redirects are enforced.
             * 
             * see http://msdn.microsoft.com/en-us/library/ms228768.aspx
             */
            if ( Regex.IsMatch( args.Name, @"EnvDTE" ) )
            {
                return typeof (EnvDTE.DTE).Assembly;
            }
            return null;
        }

        static private void AddEnvironmentPath(string environmentVariable, string rootPath, string subdir)
        {                       
            var newPath = Path.Combine(
                rootPath,
                subdir
                );
            AddEnvironmentPath(environmentVariable, newPath);
        }

        static private void AddEnvironmentPath(string environmentVariable, string path)
        {
            var pspaths = (Environment.GetEnvironmentVariable(environmentVariable) ?? "").Split(';').ToList();
            if (pspaths.Contains(path, StringComparer.InvariantCultureIgnoreCase))
            {
                return;
            }
            pspaths.Add(path);
            Environment.SetEnvironmentVariable(environmentVariable, String.Join(";", pspaths.ToArray()));
        }

    }
}
