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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using CodeOwls.PowerShell.Paths.Exceptions;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.Attributes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Provider
{
    //[CmdletProvider("YourProviderName", ProviderCapabilities.ShouldProcess)]
    public abstract class Provider : NavigationCmdletProvider, 
        IPropertyCmdletProvider, ICmdletProviderSupportsHelp,
        IContentCmdletProvider
    {
        internal Drive DefaultDrive
        {
            get
            {
                var drive = PSDriveInfo as Drive;

                if (null == drive)
                {
                    drive = ProviderInfo.Drives.FirstOrDefault() as Drive;
                }

                return drive;
            }
        }

 
        internal Drive GetDriveForPath( string path )
        {
            var name = Drive.GetDriveName(path);
            return (from drive in ProviderInfo.Drives
                    where StringComparer.InvariantCultureIgnoreCase.Equals(drive.Name, name)
                    select drive).FirstOrDefault() as Drive;
        }

        protected abstract IPathNodeProcessor PathNodeProcessor { get; }

        protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
        {
            return base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);
        }

        IEnumerable<INodeFactory> ResolvePath( string path )
        {
            using (var call = new CallLog(this, "ResolvePath", path))
            {
                path = EnsurePathIsRooted(path);
                return PathNodeProcessor.ResolvePath(CreateContext(path), path);           
            }
        }

        string NormalizeWhacks( string path )
        {
            return path.Replace("/", "\\");
        }

        private string EnsurePathIsRooted(string path)
        {
            path = NormalizeWhacks(path);
            if( ! path.StartsWith("\\") )
            {
                path = "\\" + path;
            }

            return path;
        }

        IContext CreateContext( string path )
        {
            return CreateContext(path, false, true);
        }

        IContext CreateContext(string path, bool recurse )
        {
            return CreateContext(path, recurse, false );
        }

        protected abstract IContext CreateContext(string path, bool recurse, bool resolveFinalNodeFilterItems);

        #region Implementation of IPropertyCmdletProvider
        public void GetProperty(string path, Collection<string> providerSpecificPickList)
        {
            using (var call = new CallLog(this, "GetProperty", path, providerSpecificPickList))
            {
                var factories = GetNodeFactoryFromPath(path);
                factories.ToList().ForEach(f => GetProperty(path, f, providerSpecificPickList));
            }
        }

        void GetProperty(string path, INodeFactory factory, Collection<string> providerSpecificPickList)
        {           
            var node = factory.GetNodeValue();
            PSObject values = null;

            if (null == providerSpecificPickList || 0 == providerSpecificPickList.Count)
            {
                values = PSObject.AsPSObject(node.Item);
            }
            else
            {
                values = new PSObject();
                var value = node.Item;
                var propDescs = TypeDescriptor.GetProperties(value);
                var props = (from PropertyDescriptor prop in propDescs
                             where (providerSpecificPickList.Contains(prop.Name,
                                                                      StringComparer.InvariantCultureIgnoreCase))
                             select prop);

                props.ToList().ForEach(p =>
                                           {
                                               var iv = p.GetValue(value);
                                               if (null != iv)
                                               {
                                                   values.Properties.Add(new PSNoteProperty(p.Name, p.GetValue(value)));
                                               }
                                           });
            }
            WritePropertyObject(values, path);
        }

        public object GetPropertyDynamicParameters(string path, Collection<string> providerSpecificPickList)
        {
            using (var call = new CallLog(this, "GetPropertyDynamicParameters", path, providerSpecificPickList))
            {
                return null;
            }
        }
        
        public void SetProperty(string path, PSObject propertyValue)
        {
            using (var call = new CallLog(this, "SetProperty", path, propertyValue))
            {
                var factories = GetNodeFactoryFromPath(path);
                factories.ToList().ForEach(f => SetProperty(path, f, propertyValue));
            }
        }

        void SetProperty(string path, INodeFactory factory, PSObject propertyValue)
        {
            var node = factory.GetNodeValue();
            var value = node.Item;
            var propDescs = TypeDescriptor.GetProperties(value);
            var psoDesc = propertyValue.Properties;
            var props = (from PropertyDescriptor prop in propDescs
                         let psod = (from pso in psoDesc
                                     where StringComparer.InvariantCultureIgnoreCase.Equals(pso.Name, prop.Name)
                                     select pso).FirstOrDefault()
                         where null != psod
                         select new {PSProperty = psod, Property = prop});


            props.ToList().ForEach(p => p.Property.SetValue(value, p.PSProperty.Value));
        }

        public object SetPropertyDynamicParameters(string path, PSObject propertyValue)
        {
            using (var call = new CallLog(this, "SetPropertyDynamicParameters", path))
            {
                return null;
            }
        }

        public void ClearProperty(string path, Collection<string> propertyToClear)
        {
            using (var call = new CallLog(this, "ClearProperty", path, propertyToClear))
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.ClearItemProperty,
                                                   ClearItemPropertyNotsupportedErrorID);
            }
        }

        public object ClearPropertyDynamicParameters(string path, Collection<string> propertyToClear)
        {
            using (var call = new CallLog(this, "ClearPropertyDynamicParameters", path, propertyToClear))
            {
                return null;
            }
        }

        #endregion


        #region Implementation of ICmdletProviderSupportsHelp

        public string GetHelpMaml(string helpItemName, string path)
        {
            using (var call = new CallLog(this, "GetHelpMaml", helpItemName, path))
            {
                if (String.IsNullOrEmpty(helpItemName) || String.IsNullOrEmpty(path))
                {
                    return String.Empty;
                }

                var parts = helpItemName.Split(new[] {'-'});
                if (2 != parts.Length)
                {
                    return String.Empty;
                }

                var nodeFactory = GetFirstNodeFactoryFromPath(path);
                if (null == nodeFactory)
                {
                    return String.Empty;
                }

                XmlDocument document = new XmlDocument();

                string filename = GetExistingHelpDocumentFilename();

                if (String.IsNullOrEmpty(filename)
                    || !File.Exists(filename))
                {
                    return String.Empty;
                }

                try
                {
                    document.Load(filename);
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(
                                   new MamlHelpDocumentExistsButCannotBeLoadedException(filename, e),
                                   GetHelpCustomMamlErrorID,
                                   ErrorCategory.ParserError,
                                   filename
                                   ));

                    return string.Empty;
                }

                List<string> keys = GetCmdletHelpKeysForNodeFactory(nodeFactory);

                string verb = parts[0];
                string noun = parts[1];
                string maml = (from key in keys
                               let m = GetHelpMaml(document, key, verb, noun)
                               where !String.IsNullOrEmpty(m)
                               select m).FirstOrDefault();

                if (String.IsNullOrEmpty(maml))
                {
                    maml = GetHelpMaml(document, NotSupportedCmdletHelpID, verb, noun);
                }
                return maml ?? String.Empty;
            }
        }

        private List<string> GetCmdletHelpKeysForNodeFactory(INodeFactory nodeFactory)
        {
            var nodeFactoryType = nodeFactory.GetType();
            var idsFromAttributes =
                from CmdletHelpPathIDAttribute attr in
                    nodeFactoryType.GetCustomAttributes(typeof (CmdletHelpPathIDAttribute), true)
                select attr.ID;

            List<string> keys = new List<string>(idsFromAttributes);
            keys.AddRange(new[]
                              {
                                  nodeFactoryType.FullName,
                                  nodeFactoryType.Name,
                                  nodeFactoryType.Name.Replace("NodeFactory", ""),
                              });
            return keys;
        }

        private string GetExistingHelpDocumentFilename()
        {
            CultureInfo currentUICulture = Host.CurrentUICulture;
            string moduleLocation = this.ProviderInfo.Module.ModuleBase; 
            string filename = null;
            while (currentUICulture != null && currentUICulture != currentUICulture.Parent)
            {
                string helpFilePath = GetHelpPathForCultureUI(currentUICulture.Name, moduleLocation);

                if (File.Exists(helpFilePath))
                {
                    filename = helpFilePath;
                    break;
                }
                currentUICulture = currentUICulture.Parent;
            }

            if (String.IsNullOrEmpty(filename))
            {
                string helpFilePath = GetHelpPathForCultureUI("en-US", moduleLocation);

                if (File.Exists(helpFilePath))
                {
                    filename = helpFilePath;
                }
            }

            LogVerbose( "Existing help document filename: {0}", filename);
            return filename;
        }

        private string GetHelpPathForCultureUI(string cultureName, string moduleLocation)
        {
            string documentationDirectory = Path.Combine(moduleLocation, cultureName);
            var path = Path.Combine(documentationDirectory, ProviderInfo.HelpFile);

            return path;
        }

        private string GetHelpMaml(XmlDocument document, string key, string verb, string noun)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("cmd", "http://schemas.microsoft.com/maml/dev/command/2004/10");

            string xpath = String.Format(
                "/helpItems/providerHelp/CmdletHelpPaths/CmdletHelpPath[@ID='{0}']/cmd:command[ ./cmd:details[ (cmd:verb='{1}') and (cmd:noun='{2}') ] ]",
                key,
                verb,
                noun);

            XmlNode node = null;
            try
            {
                node = document.SelectSingleNode(xpath, nsmgr);
            }
            catch (XPathException)
            {
                return string.Empty;
            }

            if (node == null)
            {
                return String.Empty;
            }

            return node.OuterXml;
        }

        #endregion

        private void WriteCmdletNotSupportedAtNodeError(string path, string cmdlet, string errorId)
        {
            var exception = new NodeDoesNotSupportCmdletException(path, cmdlet);
            var error = new ErrorRecord(exception, errorId, ErrorCategory.NotImplemented, path);
            WriteError(error);
        }

        private void WriteGeneralCmdletError(Exception exception, string errorId, string path)
        {
            WriteError(
                new ErrorRecord(
                    exception,
                    errorId,
                    ErrorCategory.NotSpecified,
                    path
                    ));
        }

        protected override bool IsItemContainer(string path)
        {
            using (var call = new CallLog(this, "IsItemContainer", path))
            {
                if (IsRootPath(path))
                {
                    return true;
                }

                var node = GetFirstNodeFactoryFromPath(path);
                if (null == node)
                {
                    return false;
                }

                var value = node.GetNodeValue();

                if (null == value)
                {
                    return false;
                }

                return value.IsCollection;
            }
        }

        protected override object MoveItemDynamicParameters(string path, string destination)
        {
            using (new CallLog(this, "MoveItemDynamicParameters", path, destination))
            {
                return null;
            }
        }

        protected override void MoveItem( string path, string destination )
        {
            using (new CallLog(this, "MoveItem", path, destination))
            {
                var sourceNodes = GetNodeFactoryFromPath(path);
                sourceNodes.ToList().ForEach(n => MoveItem(path, n, destination));
            }
        }

        void MoveItem(string path, INodeFactory sourceNode, string destination)
        {
            var copy = GetCopyItem(sourceNode);
            var remove = copy as IRemoveItem;
            if (null == copy || null == remove)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.MoveItem, MoveItemNotSupportedErrorID);
                return;
            }

            if (!ShouldProcess(path, ProviderCmdlet.MoveItem ))
            {
                return;
            }

            try
            {
                DoCopyItem(path, destination, true, copy);
                DoRemoveItem(path, true, remove);
            }
            catch( Exception e )
            {
                WriteGeneralCmdletError( e, MoveItemInvokeErrorID, path);
            }
        }

        protected override string MakePath(string parent, string child)
        {
            using (new CallLog(this, "MakePath", parent, child))
            {
                var newPath = NormalizeWhacks(base.MakePath(parent, child));
                return newPath;
            }
        }

        protected override string GetParentPath(string path, string root)
        {
            using (new CallLog(this, "GetParentPath", path, root))
            {
                if (!path.Any())
                {
                    return path;
                }

                path = NormalizeWhacks(base.GetParentPath(path, root));
                return path;
            }
        }
        
        protected override string NormalizeRelativePath(string path, string basePath)
        {
            using (new CallLog(this, "NormalizeRelativePath", path, basePath))
            {
                return NormalizeWhacks(base.NormalizeRelativePath(path, basePath));
            }
        }

        protected override string GetChildName(string path)
        {
            using (new CallLog(this, "GetChildName", path))
            {
                //path = MakePath(Drive.Name + ":/" + Drive.CurrentLocation, path).TrimEnd('/');
                var rootPath = GetRootPath();
                if (!String.IsNullOrEmpty(rootPath))
                {
                    path = path.Replace(rootPath, String.Empty);
                }
                path = NormalizeWhacks(path);
                return path.Split('\\').Last();
            }
        }

        
        void GetItem( string path, INodeFactory factory )
        {
            try
            {
                WritePathNode(path, factory);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, GetItemInvokeErrorID, path);
            }
        }
        protected override void GetItem(string path)
        {
            using (new CallLog(this, "GetItem", path))
            {
                var factories = GetNodeFactoryFromPath(path);
                if (null == factories)
                {
                    return;
                }

                factories.ToList().ForEach(f => GetItem(path, f));
            }
        }

        void SetItem( string path, INodeFactory factory, object value )
        {
            var @set = factory as ISetItem;
            if (null == factory || null == @set)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.SetItem, SetItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;
            path = GetChildName(path);

            if (!ShouldProcess(fullPath, ProviderCmdlet.SetItem))
            {
                return;
            }

            try
            {
                var result = @set.SetItem(CreateContext(fullPath), path, value);
                if (null != result)
                {
                    WritePathNode(fullPath, result);
                }
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, SetItemInvokeErrorID, fullPath);
            }
        }

        protected override void SetItem(string path, object value)
        {
            using (new CallLog(this, "SetItem", path, value))
            {
                var factories = GetNodeFactoryFromPath(path);
                if (null == factories)
                {
                    return;
                }

                factories.ToList().ForEach(f => SetItem(path, f, value));
            }
        }

        protected override object SetItemDynamicParameters(string path, object value)
        {
            using (new CallLog(this, "SetItemDynamicParameters", path, value))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                var @set = factory as ISetItem;
                if (null == factory || null == @set)
                {
                    return null;
                }

                return @set.SetItemParameters;
            }
        }

        protected override object ClearItemDynamicParameters(string path)
        {
            using (new CallLog(this, "ClearItemDynamicParameters", path))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                var clear = factory as IClearItem;
                if (null == factory || null == clear)
                {
                    return null;
                }

                return clear.ClearItemDynamicParamters;
            }
        }

        protected override void ClearItem(string path)
        {
            using (new CallLog(this, "ClearItem", path))
            {
                var factories = GetNodeFactoryFromPath(path);
                if (null == factories)
                {
                    return;
                }

                factories.ToList().ForEach(f => ClearItem(path, f));
            }
        }

        void ClearItem( string path, INodeFactory factory )
        {
            var clear = factory as IClearItem;
            if (null == factory || null == clear)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.ClearItem, ClearItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;
            path = GetChildName(path);

            if (!ShouldProcess(fullPath, ProviderCmdlet.ClearItem))
            {
                return;
            }

            try
            {
                clear.ClearItem(CreateContext(fullPath), path);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, ClearItemInvokeErrorID, fullPath);
            }
        }

        private bool ForceOrShouldContinue(INodeFactory factory, string fullPath, string op)
        {
            return ForceOrShouldContinue(factory.Name, fullPath, op);
        }

        private bool ForceOrShouldContinue(string itemName, string fullPath, string op)
        {
            if (Force || !ShouldContinue(ShouldContinuePrompt, String.Format("{2} {0} ({1})", itemName, fullPath, op)))
            {
                return false;
            }
            return true;
        }

        protected override object InvokeDefaultActionDynamicParameters(string path)
        {
            using (new CallLog(this, "InvokeDefaultActionDynamicParameters", path))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                var invoke = factory as IInvokeItem;
                if (null == factory || null == invoke)
                {
                    return null;
                }

                return invoke.InvokeItemParameters;
            }
        }

        protected override void InvokeDefaultAction(string path)
        {
            using (new CallLog(this, "InvokeDefaultAction", path))
            {
                var factories = GetNodeFactoryFromPath(path);
                if (null == factories)
                {
                    return;
                }

                factories.ToList().ForEach(f => InvokeDefaultAction(path, f));
            }
        }

        void InvokeDefaultAction( string path, INodeFactory factory )
        {
            var invoke = factory as IInvokeItem;
            if (null == factory || null == invoke)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.InvokeItem, InvokeItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;

            if (!ShouldProcess(fullPath, ProviderCmdlet.InvokeItem))
            {
                return;
            }

            path = GetChildName(path);
            try
            {
                var results = invoke.InvokeItem(CreateContext(fullPath), path);
                if (null == results)
                {
                    return;
                }

                // TODO: determine what exactly to return here
                //  http://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=1&ved=0CCAQFjAA&url=http%3A%2F%2Fmsdn.microsoft.com%2Fen-us%2Flibrary%2Fsystem.management.automation.provider.itemcmdletprovider.invokedefaultaction(v%3Dvs.85).aspx&ei=28vLTpyrJ42utwfUo6WYAQ&usg=AFQjCNFto_ye_BBjxxWfzBFGfNxw3eEgTw
                //  docs tell me to return the item being invoked... but I'm not sure.
                //  is there any way for the target of the invoke to return data to the runspace??
                // results.ToList().ForEach(r => this.WriteOutput(r));
            }
            catch( Exception e )
            {
                WriteGeneralCmdletError(e, InvokeItemInvokeErrorID, fullPath);
            }
        }


        protected override bool ItemExists(string path)
        {
            using (new CallLog(this, "ItemExists", path))
            {
                if (IsRootPath(path))
                {
                    return true;
                }

                return null != GetNodeFactoryFromPath(path);
            }
        }

        protected override bool IsValidPath(string path)
        {
            using (new CallLog(this, "IsValidPath", path))
            {
                return true;
            }
        }

        protected override void GetChildItems( string path, bool recurse )
        {
            using (new CallLog(this, "GetChildItems", path, recurse))
            {
                var nodeFactory = GetNodeFactoryFromPath(path, false);
                if (null == nodeFactory)
                {
                    return;
                }

                nodeFactory.ToList().ForEach(f => GetChildItems(path, f, recurse));
            }
        }

        void GetChildItems(string path, INodeFactory nodeFactory, bool recurse)
        {
            var context = CreateContext(path, recurse );
            var children = nodeFactory.GetNodeChildren(context);
            WriteChildItem(path, recurse, children);
        }

        private void WriteChildItem(string path, bool recurse, IEnumerable<INodeFactory> children)
        {
            if (null == children)
            {
                return;
            }

            children.ToList().ForEach(child => WriteChildItem(path, recurse, child));
        }

        private void WriteChildItem(string path, bool recurse, INodeFactory child)
        {
            try
            {
                var i = child.GetNodeValue();
                if (null == i)
                {
                    return;
                }
                var childPath = MakePath(path, i.Name);
                WritePathNode(childPath, child);
                if (recurse)
                {
                    var context = CreateContext(path, recurse);
                    var kids = child.GetNodeChildren(context);
                    WriteChildItem(childPath, recurse, kids);
                }
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteDebug("An exception was raised while writing child items: " + e.ToString());
            }
        }

        private bool IsRootPath(string path)
        {
            path = Regex.Replace(path.ToLower(), @"[a-z0-9_]+:[/\\]?", "");
            return String.IsNullOrEmpty(path);
        }

        protected override object GetChildItemsDynamicParameters(string path, bool recurse)
        {
            using (new CallLog(this, "GetChildItemsDynamicParameters", path, recurse))
            {
                INodeFactory nodeFactory = GetFirstNodeFactoryFromPath(path);
                if (null == nodeFactory)
                {
                    return null;
                }

                return nodeFactory.GetNodeChildrenParameters;
            }
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            using (new CallLog(this, "GetChildNames", path, returnContainers))
            {
                var nodeFactory = GetNodeFactoryFromPath(path, false);
                if (null == nodeFactory)
                {
                    return;
                }

                nodeFactory.ToList().ForEach(f => GetChildNames(path, f, returnContainers));
            }
        }

        void GetChildNames( string path, INodeFactory nodeFactory, ReturnContainers returnContainers )
        {
            nodeFactory.GetNodeChildren(CreateContext(path)).ToList().ForEach(
                f =>
                    {
                        var i = f.GetNodeValue();
                        if (null == i)
                        {
                            return;
                        }
                        WriteItemObject(i.Name, path + "\\" + i.Name, i.IsCollection);
                    });
        }

        protected override object GetChildNamesDynamicParameters(string path)
        {
            using (new CallLog(this, "GetChildNamesDynamicParameters", path))
            {
                INodeFactory nodeFactory = GetFirstNodeFactoryFromPath(path);
                if (null == nodeFactory)
                {
                    return null;
                }

                return nodeFactory.GetNodeChildrenParameters;
            }
        }

        protected override void RenameItem(string path, string newName)
        {
            using (new CallLog(this, "RenameItem", path, newName))
            {
                var factory = GetNodeFactoryFromPath(path);
                if (null == factory)
                {
                    return;
                }

                factory.ToList().ForEach(a => RenameItem(path, newName, a));
            }
        }

        void RenameItem(string path, string newName, INodeFactory factory)
        {
            var rename = factory as IRenameItem;
            if (null == factory || null == rename)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.RenameItem, RenameItemNotsupportedErrorID);
                return;
            }

            var fullPath = path;

            if (!ShouldProcess(fullPath, ProviderCmdlet.RenameItem))
            {
                return;
            }

            var child = GetChildName(path);
            try
            {
                rename.RenameItem(CreateContext(fullPath), child, newName);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, RenameItemInvokeErrorID, fullPath);
            }            
        }

        protected override object RenameItemDynamicParameters(string path, string newName)
        {
            using (new CallLog(this, "RenameItemDynamicParameters", path, newName))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                var rename = factory as IRenameItem;
                if (null == factory || null == rename)
                {
                    return null;
                }
                return rename.RenameItemParameters;
            }
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            using (new CallLog(this, "NewItem", path, itemTypeName, newItemValue))
            {
                bool isParentOfPath;
                var factories = GetNodeFactoryFromPathOrParent(path, out isParentOfPath);
                if (null == factories)
                {
                    return;
                }

                factories.ToList().ForEach(f => NewItem(path, isParentOfPath, f, itemTypeName, newItemValue));
            }
        }

        void NewItem( string path, bool isParentPathNodeFactory, INodeFactory factory, string itemTypeName, object newItemValue )
        {
            var @new = factory as INewItem;
            if (null == factory || null == @new)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.NewItem, NewItemNotSupportedErrorID);
                return;
            }

            var fullPath = path;
            var parentPath = fullPath;
            var child = isParentPathNodeFactory ? GetChildName(path) : null;
            if( null != child )
            {
                parentPath = GetParentPath(fullPath, GetRootPath());
            }

            if (!ShouldProcess(fullPath, ProviderCmdlet.NewItem))
            {
                return;
            }
            
            try
            {
                var item = @new.NewItem(CreateContext(fullPath), child, itemTypeName, newItemValue);
                PathNode node = item as PathNode;
                
                WritePathNode(parentPath, node);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, NewItemInvokeErrorID, fullPath);
            }
        }

        protected string GetRootPath()
        {
            if (null != PSDriveInfo)
            {
                return PSDriveInfo.Root;
            }
            return String.Empty;
        }

        protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue)
        {
            using (new CallLog(this, "NewItemDynamicParameters", path, itemTypeName, newItemValue))
            {
                var factory = GetNodeFactoryFromPathOrParent(path).FirstOrDefault();
                var @new = factory as INewItem;
                if (null == factory || null == @new)
                {
                    return null;
                }

                return @new.NewItemParameters;
            }
        }

        private void WritePathNode(string nodeContainerPath, INodeFactory factory)
        {
            var value = factory.GetNodeValue();
            if (null == value)
            {
                return;
            }

            PSObject pso = PSObject.AsPSObject(value.Item);
            pso.Properties.Add(new PSNoteProperty(ItemModePropertyName, factory.ItemMode));
            WriteItemObject(pso, nodeContainerPath, value.IsCollection);
        }

        private void WritePathNode(string nodeContainerPath, IPathNode node)
        {
            if (null != node)
            {
                WriteItemObject(node.Item, MakePath( nodeContainerPath, node.Name ), node.IsCollection);
            }
        }
        
        protected override void RemoveItem(string path, bool recurse)
        {
            using (new CallLog(this, "RemoveItem", path, recurse))
            {
                var factories = GetNodeFactoryFromPath(path);
                if (null == factories)
                {
                    return;
                }
                factories.ToList().ForEach(f => RemoveItem(path, f, recurse));
            }
        }

        void RemoveItem( string path, INodeFactory factory, bool recurse)
        {
            var remove = factory as IRemoveItem;
            if (null == factory || null == remove)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.RemoveItem, RemoveItemNotSupportedErrorID);
                return;
            }

            if (!ShouldProcess(path, ProviderCmdlet.RemoveItem))
            {
                return;
            }
            
            try
            {
                DoRemoveItem(path, recurse, remove);                
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, RemoveItemInvokeErrorID, path);
            }
        }

        private void DoRemoveItem(string path, bool recurse, IRemoveItem remove)
        {
            var fullPath = path;
            path = this.GetChildName(path);
            remove.RemoveItem(CreateContext(fullPath), path, recurse);
        }

        protected override object RemoveItemDynamicParameters(string path, bool recurse)
        {
            using (new CallLog(this, "RemoveItemDynamicParameters", path, recurse))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                var remove = factory as IRemoveItem;
                if (null == factory || null == remove)
                {
                    return null;
                }

                return remove.RemoveItemParameters;
            }
        }

        private INodeFactory GetFirstNodeFactoryFromPath( string path )
        {
            var factories = GetNodeFactoryFromPath(path);
            if( null == factories )
            {
                return null;
            }

            return factories.FirstOrDefault();
        }

        IEnumerable<INodeFactory> GetNodeFactoryFromPath(string path)
        {
            return GetNodeFactoryFromPath(path, true);
        }

        private IEnumerable<INodeFactory> GetNodeFactoryFromPath(string path, bool resolveFinalFilter)
        {
            IEnumerable<INodeFactory> factories = null;
            factories = ResolvePath(path);

            if ( resolveFinalFilter && !String.IsNullOrEmpty(Filter))
            {
                factories = factories.First().Resolve(CreateContext(path), null);
            }

            return factories;
        }

        private IEnumerable<INodeFactory> GetNodeFactoryFromPathOrParent(string path)
        {
            bool r;
            return GetNodeFactoryFromPathOrParent(path, out r);
        }

        private IEnumerable<INodeFactory> GetNodeFactoryFromPathOrParent(string path, out bool isParentOfPath)
        {
            isParentOfPath = false;
            IEnumerable<INodeFactory> factory = null;
            factory = ResolvePath(path);

            if (null == factory)
            {
                path = GetParentPath(path, null);
                factory = ResolvePath(path);

                if (null == factory)
                {
                    return null;
                }

                isParentOfPath = true;
            }

            if (!String.IsNullOrEmpty(Filter))
            {
                factory = factory.First().Resolve(CreateContext(null), null);
            }

            return factory;
        }

        protected override bool HasChildItems(string path)
        {
            using (new CallLog(this, "HasChildItems", path))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                if (null == factory)
                {
                    return false;
                }
                var nodes = factory.GetNodeChildren(CreateContext(path));
                if (null == nodes)
                {
                    return false;
                }
                return nodes.Any();
            }
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            using (new CallLog(this, "CopyItem", path, copyPath, recurse))
            {
                var sourceNodes = GetNodeFactoryFromPath(path);
                if (null == sourceNodes)
                {
                    return;
                }

                sourceNodes.ToList().ForEach(n => CopyItem(path, n, copyPath, recurse));
            }
        }

        void CopyItem( string path, INodeFactory sourceNode, string copyPath, bool recurse )
        {
            ICopyItem copyItem = GetCopyItem(sourceNode);
            if (null == copyItem)
            {
                WriteCmdletNotSupportedAtNodeError(path, ProviderCmdlet.CopyItem, CopyItemNotSupportedErrorID);
                return;
            }

            if (!ShouldProcess(path, ProviderCmdlet.CopyItem ))
            {
                return;
            }
            
            try
            {
                IPathNode node = DoCopyItem(path, copyPath, recurse, copyItem);
                WritePathNode(copyPath, node);
            }
            catch (Exception e)
            {
                WriteGeneralCmdletError(e, CopyItemInvokeErrorID, path);
            }
        }

        private ICopyItem GetCopyItem(INodeFactory sourceNode)
        {
            var copyItem = sourceNode as ICopyItem;
            if (null == sourceNode || null == copyItem)
            {
                return null;
            }

            return copyItem;
        }

        private IPathNode DoCopyItem(string path, string copyPath, bool recurse, ICopyItem copyItem)
        {
            bool targetNodeIsParentNode;
            var targetNodes = GetNodeFactoryFromPathOrParent(copyPath, out targetNodeIsParentNode);
            var targetNode = targetNodes.FirstOrDefault();

            var sourceName = GetChildName(path);
            var copyName = targetNodeIsParentNode ? GetChildName(copyPath) : null;

            if (null == targetNode)
            {
                WriteError(
                    new ErrorRecord(
                        new CopyOrMoveToNonexistentContainerException(copyPath),
                        CopyItemDestinationContainerDoesNotExistErrorID,
                        ErrorCategory.WriteError,
                        copyPath
                        )
                    );
                return null;
            }

            return copyItem.CopyItem(CreateContext(path), sourceName, copyName, targetNode.GetNodeValue(), recurse);
        }

        protected override object CopyItemDynamicParameters(string path, string destination, bool recurse)
        {
            using (new CallLog(this, "CopyItemDynamicParameters", path, destination, recurse))
            {
                var factory = GetFirstNodeFactoryFromPath(path);
                var copy = factory as ICopyItem;
                if (null == factory || null == copy)
                {
                    return null;
                }

                return copy.CopyItemParameters;
            }
        }

        public IContentReader GetContentReader(string path)
        {
            var factories = GetNodeFactoryFromPath(path);
            if (null == factories)
            {
                return null;
            }

            return GetContentReader(path, factories.FirstOrDefault(a => a is IGetItemContent));
        }

        private IContentReader GetContentReader(string path, INodeFactory nodeFactory)
        {
            var getContentReader = nodeFactory as IGetItemContent;
            if (null == getContentReader)
            {
                return null;
            }

            return getContentReader.GetContentReader(CreateContext(path));
        }

        public object GetContentReaderDynamicParameters(string path)
        {
            var factories = GetFirstNodeFactoryFromPath(path);
            if (null == factories)
            {
                return null;
            }

            var getContentReader = factories as IGetItemContent;
            if (null == getContentReader)
            {
                return null;
            }

            return getContentReader.GetContentReaderDynamicParameters(CreateContext(path));
        }

        public IContentWriter GetContentWriter(string path)
        {
            var factories = GetNodeFactoryFromPath(path);
            if (null == factories)
            {
                return null;
            }

            return GetContentWriter(path, factories.FirstOrDefault(a => a is ISetItemContent));

        }

        private IContentWriter GetContentWriter(string path, INodeFactory nodeFactory)
        {
            var getContentReader = nodeFactory as ISetItemContent;
            if (null == getContentReader)
            {
                return null;
            }

            return getContentReader.GetContentWriter(CreateContext(path));

        }

        public object GetContentWriterDynamicParameters(string path)
        {
            var factories = GetFirstNodeFactoryFromPath(path);
            if (null == factories)
            {
                return null;
            }

            var getContentReader = factories as ISetItemContent;
            if (null == getContentReader)
            {
                return null;
            }

            return getContentReader.GetContentWriterDynamicParameters(CreateContext(path));
        }

        public void ClearContent(string path)
        {
            var factories = GetNodeFactoryFromPath(path);
            if (null == factories)
            {
                return;
            }

            ClearContent(path, factories.FirstOrDefault(a => a is IClearItemContent));

        }

        private void ClearContent(string path, INodeFactory nodeFactory)
        {
            var getContentReader = nodeFactory as IClearItemContent;
            if (null == getContentReader)
            {
                return;
            }

            getContentReader.ClearContent(CreateContext(path));

        }

        public object ClearContentDynamicParameters(string path)
        {
            var factories = GetFirstNodeFactoryFromPath(path);
            if (null == factories)
            {
                return null;
            }

            var getContentReader = factories as IClearItemContent;
            if (null == getContentReader)
            {
                return null;
            }

            return getContentReader.GetClearContentDynamicParameters(CreateContext(path));
        }

        protected void LogDebug(string format, params object[] args)
        {
            try
            {
                WriteDebug(String.Format(format, args));
            }
            catch
            {              
            }
        }
        protected void LogVerbose(string format, params object[] args)
        {
            try
            {
                WriteVerbose(String.Format(format, args));
            }
            catch
            {
            }
        }

        class CallLog : IDisposable
        {
            private readonly Provider _provider;
            private readonly string _name;

            public CallLog(Provider provider, string name, params  object[] args )
            {
                _provider = provider;
                _name = name;
                
                _provider.LogCallIn( name, args );
            }
            
            public object ReturnValue { get; set; }
            public void Dispose()
            {
                _provider.LogCallOut(_name, ReturnValue );
            }
        }

        private void LogCallIn(string name, params object[] args)
        {
            args = args ?? new object[] {};
            var msg = String.Format(">> {0}( [{1}] )", name,
                                       String.Join("], [",
                                                   args.ToList()
                                                       .ConvertAll(o => null == o ? "<null>" : o.ToString())
                                                       .ToArray()));
            LogDebug( msg );
        }

        private void LogCallOut(string name, object rt)
        {            
            var msg = String.Format("<< {0} [{1}]", name, rt);
            LogDebug( msg );
        }

        private const string NotSupportedCmdletHelpID = "__NotSupported__";
        private const string RenameItemNotsupportedErrorID = "RenameItem.NotSupported";
        private const string RenameItemInvokeErrorID = "RenameItem.Invoke";
        private const string NewItemNotSupportedErrorID = "NewItem.NotSupported";
        private const string NewItemInvokeErrorID = "NewItem.Invoke";
        private const string ItemModePropertyName = "SSItemMode";
        private const string RemoveItemNotSupportedErrorID = "RemoveItem.NotSupported";
        private const string RemoveItemInvokeErrorID = "RemoveItem.Invoke";
        private const string CopyItemNotSupportedErrorID = "CopyItem.NotSupported";
        private const string CopyItemInvokeErrorID = "CopyItem.Invoke";
        private const string CopyItemDestinationContainerDoesNotExistErrorID = "CopyItem.DestinationContainerDoesNotExist";
        private const string ClearItemPropertyNotsupportedErrorID = "ClearItemProperty.NotSupported";
        private const string GetHelpCustomMamlErrorID = "GetHelp.CustomMaml";
        private const string GetItemInvokeErrorID = "GetItem.Invoke";
        private const string SetItemNotSupportedErrorID = "SetItem.NotSupported";
        private const string SetItemInvokeErrorID = "SetItem.Invoke";
        private const string ClearItemNotSupportedErrorID = "ClearItem.NotSupported";
        private const string ClearItemInvokeErrorID = "ClearItem.Invoke";
        private const string InvokeItemNotSupportedErrorID = "InvokeItem.NotSupported";
        private const string InvokeItemInvokeErrorID = "InvokeItem.Invoke";
        private const string MoveItemNotSupportedErrorID = "MoveItem.NotSupported";
        private const string MoveItemInvokeErrorID = "MoveItem.Invoke";
        private const string ShouldContinuePrompt = "Are you sure?";
                
    }
}
