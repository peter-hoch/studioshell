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
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using CodeOwls.StudioShell.PathNodes;

namespace CodeOwls.StudioShell
{
    public class Drive : PSDriveInfo
    {
        private readonly DTE2 _applicationObject;
    	private readonly IPathNodeProcessor _pathProcessor;

    	public Drive(PSDriveInfo driveInfo, DTE2 applicationObject, IPathNodeProcessor pathProcessor)
            : base(driveInfo)
        {
        	_applicationObject = applicationObject;
        	_pathProcessor = pathProcessor;
		}

		public INodeFactory GetNodeFromPath( string path )
		{
		    if( ! path.StartsWith( "dte:", StringComparison.InvariantCultureIgnoreCase ))
            {
                path = MakePathDTERooted(path);
            }

		    return _pathProcessor.ResolvePath(path);
		}

        private string MakePathDTERooted(string path)
        {
            var driveName = GetDriveName(path);
            var psdrive = (from psd in Provider.Drives
                           where StringComparer.InvariantCultureIgnoreCase.Equals(psd.Name, driveName)
                           select psd).FirstOrDefault();
            if( null == psdrive )
            {
                return path;
            }

            return path.Replace(driveName + ":", psdrive.Root);
        }

        private string GetDriveName(string path)
        {
            Regex re = new Regex( @"^([^:]+):");
            var match = re.Match(path);
            if( ! match.Success)
            {
                return String.Empty;
            }

            return match.Groups[1].Value;
        }

        public IPathNodeProcessor PathProcessor
    	{
    		get { return _pathProcessor; }
    	}

    	protected DTE2 Application
        {
            get
            {
                return _applicationObject;
            }
        }
    }
}
