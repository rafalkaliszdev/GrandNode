﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
//using System.Web;
using Grand.Core.ComponentModel;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyModel;

//Contributor: Umbraco (http://www.umbraco.com). Thanks a lot! 
//SEE THIS POST for full details of what this does - http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

namespace Grand.Core.Plugins
{
    /// <summary>
    /// Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        #region Const

        private const string InstalledPluginsFilePath = "~/App_Data/InstalledPlugins.txt";
        private const string PluginsPath = "~/Plugins";
        private const string ShadowCopyPath = "~/Plugins/bin";

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// Returns a collection of all plugin which are not compatible with the current version
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize(ApplicationPartManager applicationPartManager)
        {
            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                var pluginFolder = new DirectoryInfo(CommonHelper.MapPath(PluginsPath));
                _shadowCopyFolder = new DirectoryInfo(CommonHelper.MapPath(ShadowCopyPath));

                var referencedPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();

                //TODO move to settings
                var _clearShadowDirectoryOnStartup = true;

                try
                {
                    var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

                    Debug.WriteLine("Creating shadow copy folder and querying for dlls");
                    //ensure folders are created
                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    //get list of all files in bin
                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    if (_clearShadowDirectoryOnStartup)
                    {
                        //clear out shadow copied plugins
                        foreach (var f in binFiles)
                        {
                            Debug.WriteLine("Deleting " + f.Name);
                            try
                            {
                                //ignore index.htm
                                var fileName = Path.GetFileName(f.FullName);
                                if (fileName.Equals("index.htm", StringComparison.OrdinalIgnoreCase))
                                    continue;

                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("Error deleting file " + f.Name + ". Exception: " + exc);
                            }
                        }
                    }

                    //load description files
                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolder))
                    {
                        var descriptionFile = dfd.Key;
                        var pluginDescriptor = dfd.Value;

                        //ensure that version of plugin is valid
                        if (!pluginDescriptor.SupportedVersions.Contains(GrandVersion.CurrentVersion, StringComparer.OrdinalIgnoreCase/*OrdinalIgnoreCase*/))
                        {
                            incompatiblePlugins.Add(pluginDescriptor.SystemName);
                            continue;
                        }

                        //some validation
                        if (String.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                            throw new Exception(string.Format("A plugin '{0}' has no system name. Try assigning the plugin a unique name and recompiling.", descriptionFile.FullName));
                        if (referencedPlugins.Contains(pluginDescriptor))
                            throw new Exception(string.Format("A plugin with '{0}' system name is already defined", pluginDescriptor.SystemName));

                        //set 'Installed' property
                        pluginDescriptor.Installed = installedPluginSystemNames
                            .FirstOrDefault(x => x.Equals(pluginDescriptor.SystemName, StringComparison.OrdinalIgnoreCase)) != null;

                        try
                        {
                            if (descriptionFile.Directory == null)
                                throw new Exception(string.Format("Directory cannot be resolved for '{0}' description file", descriptionFile.Name));
                            //get list of all DLLs in plugins (not in bin!)
                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFolder(x.Directory))
                                .ToList();
                            //other plugin description info
                            var mainPluginFile = pluginFiles
                                .FirstOrDefault(x => x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.OrdinalIgnoreCase));
                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;




                            //shadow copy main plugin file
                            pluginDescriptor.ReferencedAssembly = PerformFileDeploy(mainPluginFile, applicationPartManager);




                            //load all other referenced assemblies now
                            foreach (var plugin in pluginFiles
                                .Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.OrdinalIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                                PerformFileDeploy(plugin, applicationPartManager);

                            //init plugin type (only one plugin per assembly is allowed)
                            foreach (var t in pluginDescriptor.ReferencedAssembly.GetTypes())
                                if (typeof(IPlugin).IsAssignableFrom(t))
                                    if (!t.GetTypeInfo().IsInterface)
                                        if (t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract)
                                        {
                                            pluginDescriptor.PluginType = t;
                                            break;
                                        }

                            referencedPlugins.Add(pluginDescriptor);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            //add a plugin name. this way we can easily identify a problematic plugin
                            var msg = string.Format("Plugin '{0}'. ", pluginDescriptor.FriendlyName);
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                        catch (Exception ex)
                        {
                            //add a plugin name. this way we can easily identify a problematic plugin
                            var msg = string.Format("Plugin '{0}'. {1}", pluginDescriptor.FriendlyName, ex.Message);

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    throw fail;
                }


                ReferencedPlugins = referencedPlugins;
                IncompatiblePlugins = incompatiblePlugins;

            }
        }

        /// <summary>
        /// Mark plugin as installed
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsInstalled(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
            bool alreadyMarkedAsInstalled = installedPluginSystemNames
                                .FirstOrDefault(x => x.Equals(systemName, StringComparison.OrdinalIgnoreCase)) != null;
            if (!alreadyMarkedAsInstalled)
                installedPluginSystemNames.Add(systemName);
            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsUninstalled(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
            bool alreadyMarkedAsInstalled = installedPluginSystemNames
                                .FirstOrDefault(x => x.Equals(systemName, StringComparison.OrdinalIgnoreCase)) != null;
            if (alreadyMarkedAsInstalled)
                installedPluginSystemNames.Remove(systemName);
            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {
            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// Find a plugin descriptor by some type which is located into the same assembly as plugin
        /// </summary>
        /// <param name="typeInAssembly">Type</param>
        /// <returns>Plugin descriptor if exists; otherwise null</returns>
        public static PluginDescriptor FindPlugin(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException("typeInAssembly");

            if (ReferencedPlugins == null)
                return null;

            return ReferencedPlugins.FirstOrDefault(plugin => plugin.ReferencedAssembly != null
                && plugin.ReferencedAssembly.FullName.Equals(typeInAssembly.GetTypeInfo().Assembly.FullName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get description files
        /// </summary>
        /// <param name="pluginFolder">Plugin directory info</param>
        /// <returns>Original and parsed description files</returns>
        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
        {
            if (pluginFolder == null)
                throw new ArgumentNullException("pluginFolder");

            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
            //add display order and path to list
            foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
            {
                if (!IsPackagePluginFolder(descriptionFile.Directory))
                    continue;

                //parse file
                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);

                //populate list
                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
            //it's required: http://www.nopcommerce.com/boards/t/17455/load-plugins-based-on-their-displayorder-on-startup.aspx
            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        /// <summary>
        /// Indicates whether assembly file is already loaded
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <returns>Result</returns>
        private static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.OrdinalIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (string.IsNullOrEmpty(fileNameWithoutExt))
                    throw new Exception(string.Format("Cannot get file extension for {0}", fileInfo.Name));



                var qwqewqewqwq = DependencyContext.Default.CompileLibraries;

                foreach (var qwq in qwqewqewqwq)
                {
                    string assemblyName = qwq.Name/*.FullName*/.Split(new[] { ',' }).FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }



                //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                //{
                //    string assemblyName = a.FullName.Split(new[] { ',' }).FirstOrDefault();
                //    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
                //        return true;
                //}
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + exc);
            }
            return false;
        }

        /// <summary>
        /// Perform file deply
        /// </summary>
        /// <param name="plug">Plugin file info</param>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(FileInfo plug, ApplicationPartManager applicationPartManager)
        {


            if (plug.Directory == null || plug.Directory.Parent == null)
                throw new InvalidOperationException("The plugin directory for the " + plug.Name + " file exists in a folder outside of the allowed nopCommerce folder hierarchy");

            //TODO
            //now asp.net core doesn't init DynamicDirectory in AppContext, that's why we commented the following code
            //FileInfo shadowCopiedPlug;
            //if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted || string.IsNullOrEmpty(AppDomain.CurrentDomain.DynamicDirectory))
            //{
            //    //all plugins will need to be copied to ~/Plugins/bin/
            //    //this is absolutely required because all of this relies on probingPaths being set statically in the web.config

            //    //were running in med trust, so copy to custom bin folder
            //    var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            //    shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
            //}
            //else
            //{
            //    var directory = AppDomain.CurrentDomain.DynamicDirectory;
            //    Debug.WriteLine(plug.FullName + " to " + directory);
            //    //were running in full trust so copy to standard dynamic folder
            //    shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
            //}
            //but in order to avoid possible issues we still copy libraries into ~/Plugins/bin/ directory
            var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            var shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);

            //we can now register the plugin definition







            //'Plugin 'Pay In Store'. The given assembly name or codebase was invalid. (Exception from HRESULT: 0x80131047)
            //The given assembly name or codebase was invalid. (Exception from HRESULT: 0x80131047)
            var shadowCopiedAssembly0 = Assembly.Load(new AssemblyName(shadowCopiedPlug.FullName));

            //{System.IO.FileNotFoundException: Could not load file or assembly 'Grand.Plugin.Payments.PayByBitcoin.dll, Culture=neutral, PublicKeyToken=null'. Nie można odnaleźć określonego pliku.
            //File name: 'Grand.Plugin.Payments.PayByBitcoin.dll, Culture=neutral, PublicKeyToken=null'
            //var shadowCopiedAssembly2 = Assembly.Load(new AssemblyName(shadowCopiedPlug.Name));

            //{System.IO.FileLoadException: The given assembly name or codebase was invalid. (Exception from HRESULT: 0x80131047)
            //var shadowCopiedAssembly3 = Assembly.Load(new AssemblyName(shadowCopiedPlug.DirectoryName));

            //{System.IO.FileNotFoundException: Could not load file or assembly 'qweqqeqw, Culture=neutral, PublicKeyToken=null'. Nie można odnaleźć określonego pliku.


            var qwwq = shadowCopiedPlug.FullName;

            var shadowCopiedAssembly4 = Assembly.Load(new AssemblyName("qweqqeqw"));








            //var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));
            var shadowCopiedAssembly = Assembly.Load(new AssemblyName(shadowCopiedPlug.FullName));
            Debug.WriteLine("Adding to ApplicationParts: '{0}'", shadowCopiedAssembly.FullName);
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(shadowCopiedAssembly));
            return shadowCopiedAssembly;


















































            //if (plug.Directory == null || plug.Directory.Parent == null)
            //    throw new InvalidOperationException("The plugin directory for the " + plug.Name + " file exists in a folder outside of the allowed nopCommerce folder hierarchy");

            ////TODO
            ////now asp.net core doesn't init DynamicDirectory in AppContext, that's why we commented the following code
            ////FileInfo shadowCopiedPlug;
            ////if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted || string.IsNullOrEmpty(AppDomain.CurrentDomain.DynamicDirectory))
            ////{
            ////    //all plugins will need to be copied to ~/Plugins/bin/
            ////    //this is absolutely required because all of this relies on probingPaths being set statically in the web.config

            ////    //were running in med trust, so copy to custom bin folder
            ////    var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            ////    shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
            ////}
            ////else
            ////{
            ////    var directory = AppDomain.CurrentDomain.DynamicDirectory;
            ////    Debug.WriteLine(plug.FullName + " to " + directory);
            ////    //were running in full trust so copy to standard dynamic folder
            ////    shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
            ////}





            //if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            //{
            //    //all plugins will need to be copied to ~/Plugins/bin/
            //    //this is aboslutely required because all of this relies on probingPaths being set statically in the web.config

            //    //were running in med trust, so copy to custom bin folder
            //    var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            //    shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
            //}
            //else
            //{
            //    var directory = AppDomain.CurrentDomain.DynamicDirectory;
            //    Debug.WriteLine(plug.FullName + " to " + directory);
            //    //were running in full trust so copy to standard dynamic folder
            //    shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
            //}




            ////but in order to avoid possible issues we still copy libraries into ~/Plugins/bin/ directory
            //var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            //var shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);



            ////we can now register the plugin definition
            ////var shadowCopiedAssembly = Assembly.Load(new AssemblyName(shadowCopiedPlug.FullName));//tbh


            try
            {
                //var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));


                //
                var qwqe4 = new AssemblyName(shadowCopiedPlug.Name);



                var qewqew1 = Assembly.GetEntryAssembly();

                var qewqew2 = Assembly.CreateQualifiedName(qwqe4.Name, shadowCopiedPlug.DirectoryName);



                //test
                var webass = Assembly.GetEntryAssembly();
                var web1 = new AssemblyName(webass.FullName);
                var web2 = webass.GetName();
                var test04 = Assembly.Load(web1);




                var qwqe3 = new AssemblyName(shadowCopiedPlug.DirectoryName);

                var qqdwq = @"C:\DONT REMOVE\gn_migration_github\GrandNode\Presentation\Grand.Web\Plugins\bin\Grand.Plugin.Payments.PayByBitcoin.dll";
                var qwqe1 = new AssemblyName(qqdwq);
                var qwqe2 = new AssemblyName(shadowCopiedPlug.FullName);


                //qwqe.



                //var test01 = Assembly.Load(new AssemblyName(shadowCopiedPlug.Name));
                var test02 = Assembly.Load(new AssemblyName(shadowCopiedPlug.FullName));
                var test03 = Assembly.Load(new AssemblyName(shadowCopiedPlug.Name));

            }
            catch (Exception ex)
            {

            }


            //Debug.WriteLine("Adding to ApplicationParts: '{0}'", shadowCopiedAssembly.FullName);
            //applicationPartManager.ApplicationParts.Add(new AssemblyPart(shadowCopiedAssembly));

            return null;// shadowCopiedAssembly;
        }

        /// <summary>
        /// Used to initialize plugins when running in Full Trust
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }

        /// <summary>
        /// Used to initialize plugins when running in Medium Trust
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeMediumTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file

                    //More info: http://www.nopcommerce.com/boards/t/11511/access-error-nopplugindiscountrulesbillingcountrydll.aspx?p=4#60838
                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlug.FullName, oldFile);
                    }
                    catch (IOException exc)
                    {
                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
            }

            return shadowCopiedPlug;
        }

        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder == null) return false;
            if (folder.Parent == null) return false;
            if (!folder.Parent.Name.Equals("Plugins", StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        /// <summary>
        /// Gets the full path of InstalledPlugins.txt file
        /// </summary>
        /// <returns></returns>
        private static string GetInstalledPluginsFilePath()
        {
            return CommonHelper.MapPath(InstalledPluginsFilePath);
        }

        #endregion
    }
}

//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
/////*using System.Web;*/
////using System.Web.Compilation;
//using Grand.Core.ComponentModel;
//using Grand.Core.Plugins;

//using Microsoft.Extensions.DependencyModel;


////using Microsoft.Web.Infrastructure.DynamicModuleHelper;

////Contributor: Umbraco (http://www.umbraco.com). Thanks a lot! 
////SEE THIS POST for full details of what this does - http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

////[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
//namespace Grand.Core.Plugins
//{
//    /// <summary>
//    /// Sets the application up for the plugin referencing
//    /// </summary>
//    public class PluginManager
//    {
//        #region Const

//        private const string InstalledPluginsFilePath = "~/App_Data/InstalledPlugins.txt";
//        private const string PluginsPath = "~/Plugins";
//        private const string ShadowCopyPath = "~/Plugins/bin";

//        #endregion

//        #region Fields

//        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
//        private static DirectoryInfo _shadowCopyFolder;
//        private static bool _clearShadowDirectoryOnStartup;

//        #endregion

//        #region Methods

//        /// <summary>
//        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
//        /// </summary>
//        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

//        /// <summary>
//        /// Returns a collection of all plugin which are not compatible with the current version
//        /// </summary>
//        public static IEnumerable<string> IncompatiblePlugins { get; set; }

//        /// <summary>
//        /// Initialize
//        /// </summary>
//        public static void Initialize()
//        {
//            using (new WriteLockDisposable(Locker))
//            {
//                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
//                // prevent app from starting altogether
//                var pluginFolder = new DirectoryInfo(CommonHelper.MapPath(PluginsPath));
//                _shadowCopyFolder = new DirectoryInfo(CommonHelper.MapPath(ShadowCopyPath));

//                var referencedPlugins = new List<PluginDescriptor>();
//                var incompatiblePlugins = new List<string>();

//                _clearShadowDirectoryOnStartup = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]) &&
//                   Convert.ToBoolean(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]);

//                try
//                {
//                    var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

//                    Debug.WriteLine("Creating shadow copy folder and querying for dlls");
//                    //ensure folders are created
//                    Directory.CreateDirectory(pluginFolder.FullName);
//                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

//                    //get list of all files in bin
//                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
//                    if (_clearShadowDirectoryOnStartup)
//                    {
//                        //clear out shadow copied plugins
//                        foreach (var f in binFiles)
//                        {
//                            Debug.WriteLine("Deleting " + f.Name);
//                            try
//                            {
//                                File.Delete(f.FullName);
//                            }
//                            catch (Exception exc)
//                            {
//                                Debug.WriteLine("Error deleting file " + f.Name + ". Exception: " + exc);
//                            }
//                        }
//                    }

//                    //load description files
//                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolder))
//                    {
//                        var descriptionFile = dfd.Key;
//                        var pluginDescriptor = dfd.Value;

//                        //ensure that version of plugin is valid
//                        if (!pluginDescriptor.SupportedVersions.Contains(GrandVersion.CurrentVersion, StringComparer.OrdinalIgnoreCase))
//                        {
//                            incompatiblePlugins.Add(pluginDescriptor.SystemName);
//                            continue;
//                        }

//                        //some validation
//                        if (String.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
//                            throw new Exception(string.Format("A plugin '{0}' has no system name. Try assigning the plugin a unique name and recompiling.", descriptionFile.FullName));
//                        if (referencedPlugins.Contains(pluginDescriptor))
//                            throw new Exception(string.Format("A plugin with '{0}' system name is already defined", pluginDescriptor.SystemName));

//                        //set 'Installed' property
//                        pluginDescriptor.Installed = installedPluginSystemNames
//                            .FirstOrDefault(x => x.Equals(pluginDescriptor.SystemName, StringComparison.OrdinalIgnoreCase)) != null;

//                        try
//                        {
//                            if (descriptionFile.Directory == null)
//                                throw new Exception(string.Format("Directory cannot be resolved for '{0}' description file", descriptionFile.Name));
//                            //get list of all DLLs in plugins (not in bin!)
//                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
//                                //just make sure we're not registering shadow copied plugins
//                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
//                                .Where(x => IsPackagePluginFolder(x.Directory))
//                                .ToList();

//                            //other plugin description info
//                            var mainPluginFile = pluginFiles
//                                .FirstOrDefault(x => x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.OrdinalIgnoreCase));
//                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

//                            //shadow copy main plugin file
//                            pluginDescriptor.ReferencedAssembly = PerformFileDeploy(mainPluginFile);

//                            //load all other referenced assemblies now
//                            foreach (var plugin in pluginFiles
//                                .Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.OrdinalIgnoreCase))
//                                .Where(x => !IsAlreadyLoaded(x)))
//                                PerformFileDeploy(plugin);

//                            //init plugin type (only one plugin per assembly is allowed)
//                            foreach (var t in pluginDescriptor.ReferencedAssembly.GetTypes())
//                                if (typeof(IPlugin).IsAssignableFrom(t))
//                                    if (!t.GetTypeInfo().IsInterface)
//                                        if (t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract)
//                                        {
//                                            pluginDescriptor.PluginType = t;
//                                            break;
//                                        }

//                            referencedPlugins.Add(pluginDescriptor);
//                        }
//                        catch (ReflectionTypeLoadException ex)
//                        {
//                            //add a plugin name. this way we can easily identify a problematic plugin
//                            var msg = string.Format("Plugin '{0}'. ", pluginDescriptor.FriendlyName);
//                            foreach (var e in ex.LoaderExceptions)
//                                msg += e.Message + Environment.NewLine;

//                            var fail = new Exception(msg, ex);
//                            throw fail;
//                        }
//                        catch (Exception ex)
//                        {
//                            //add a plugin name. this way we can easily identify a problematic plugin
//                            var msg = string.Format("Plugin '{0}'. {1}", pluginDescriptor.FriendlyName, ex.Message);

//                            var fail = new Exception(msg, ex);
//                            throw fail;
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    var msg = string.Empty;
//                    for (var e = ex; e != null; e = e.InnerException)
//                        msg += e.Message + Environment.NewLine;

//                    var fail = new Exception(msg, ex);
//                    throw fail;
//                }


//                ReferencedPlugins = referencedPlugins;
//                IncompatiblePlugins = incompatiblePlugins;

//            }
//        }

//        /// <summary>
//        /// Mark plugin as installed
//        /// </summary>
//        /// <param name="systemName">Plugin system name</param>
//        public static void MarkPluginAsInstalled(string systemName)
//        {
//            if (String.IsNullOrEmpty(systemName))
//                throw new ArgumentNullException("systemName");

//            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
//            if (!File.Exists(filePath))
//                using (File.Create(filePath))
//                {
//                    //we use 'using' to close the file after it's created
//                }


//            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
//            bool alreadyMarkedAsInstalled = installedPluginSystemNames
//                                .FirstOrDefault(x => x.Equals(systemName, StringComparison.OrdinalIgnoreCase)) != null;
//            if (!alreadyMarkedAsInstalled)
//                installedPluginSystemNames.Add(systemName);
//            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
//        }

//        /// <summary>
//        /// Mark plugin as uninstalled
//        /// </summary>
//        /// <param name="systemName">Plugin system name</param>
//        public static void MarkPluginAsUninstalled(string systemName)
//        {
//            if (String.IsNullOrEmpty(systemName))
//                throw new ArgumentNullException("systemName");

//            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
//            if (!File.Exists(filePath))
//                using (File.Create(filePath))
//                {
//                    //we use 'using' to close the file after it's created
//                }


//            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
//            bool alreadyMarkedAsInstalled = installedPluginSystemNames
//                                .FirstOrDefault(x => x.Equals(systemName, StringComparison.OrdinalIgnoreCase)) != null;
//            if (alreadyMarkedAsInstalled)
//                installedPluginSystemNames.Remove(systemName);
//            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
//        }

//        /// <summary>
//        /// Mark plugin as uninstalled
//        /// </summary>
//        public static void MarkAllPluginsAsUninstalled()
//        {
//            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
//            if (File.Exists(filePath))
//                File.Delete(filePath);
//        }

//        #endregion

//        #region Utilities

//        /// <summary>
//        /// Get description files
//        /// </summary>
//        /// <param name="pluginFolder">Plugin direcotry info</param>
//        /// <returns>Original and parsed description files</returns>
//        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
//        {
//            if (pluginFolder == null)
//                throw new ArgumentNullException("pluginFolder");

//            //create list (<file info, parsed plugin descritor>)
//            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
//            //add display order and path to list
//            foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
//            {
//                if (!IsPackagePluginFolder(descriptionFile.Directory))
//                    continue;

//                //parse file
//                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);

//                //populate list
//                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(descriptionFile, pluginDescriptor));
//            }

//            //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
//            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
//            return result;
//        }

//        /// <summary>
//        /// Indicates whether assembly file is already loaded
//        /// </summary>
//        /// <param name="fileInfo">File info</param>
//        /// <returns>Result</returns>
//        private static bool IsAlreadyLoaded(FileInfo fileInfo)
//        {
//            try
//            {
//                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
//                if (fileNameWithoutExt == null)
//                    throw new Exception(string.Format("Cannot get file extnension for {0}", fileInfo.Name));


//                //tbh znow kurw AppDomain
//                var japierdole = DependencyContext.Default.RuntimeLibraries.Select(x => Assembly.Load(new AssemblyName(x.Name)));



//                foreach (var a in japierdole) // AppDomain.CurrentDomain.GetAssemblies())
//                {
//                    string assemblyName = a.FullName.Split(new[] { ',' }).FirstOrDefault();
//                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
//                        return true;
//                }
//            }
//            catch (Exception exc)
//            {
//                Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + exc);
//            }
//            return false;
//        }

//        /// <summary>
//        /// Perform file deply
//        /// </summary>
//        /// <param name="plug">Plugin file info</param>
//        /// <returns>Assembly</returns>
//        private static Assembly PerformFileDeploy(FileInfo plug)
//        {
//            if (plug.Directory.Parent == null)
//                throw new InvalidOperationException("The plugin directory for the " + plug.Name +
//                                                    " file exists in a folder outside of the allowed grandnode folder hierarchy");

//            FileInfo shadowCopiedPlug;


//            //tbh
//            if (false)//CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
//            {
//                //all plugins will need to be copied to ~/Plugins/bin/
//                //this is aboslutely required because all of this relies on probingPaths being set statically in the web.config

//                //were running in med trust, so copy to custom bin folder
//                var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
//                shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
//            }
//            else
//            {
//                //tbh
//                var directory = "";// AppDomain.CurrentDomain.DynamicDirectory;
//                Debug.WriteLine(plug.FullName + " to " + directory);
//                //were running in full trust so copy to standard dynamic folder
//                shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
//            }

//            //we can now register the plugin definition
//            var shadowCopiedAssembly = Assembly.Load(new AssemblyName("")); //Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));

//            //add the reference to the build manager
//            Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);


//            //tbh
//            //BuildManager.AddReferencedAssembly(shadowCopiedAssembly);

//            return shadowCopiedAssembly;
//        }

//        /// <summary>
//        /// Used to initialize plugins when running in Full Trust
//        /// </summary>
//        /// <param name="plug"></param>
//        /// <param name="shadowCopyPlugFolder"></param>
//        /// <returns></returns>
//        private static FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
//        {
//            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
//            try
//            {
//                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
//            }
//            catch (IOException)
//            {
//                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
//                //this occurs when the files are locked,
//                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
//                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
//                try
//                {
//                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
//                    File.Move(shadowCopiedPlug.FullName, oldFile);
//                }
//                catch (IOException exc)
//                {
//                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
//                }
//                //ok, we've made it this far, now retry the shadow copy
//                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
//            }
//            return shadowCopiedPlug;
//        }

//        /// <summary>
//        /// Used to initialize plugins when running in Medium Trust
//        /// </summary>
//        /// <param name="plug"></param>
//        /// <param name="shadowCopyPlugFolder"></param>
//        /// <returns></returns>
//        private static FileInfo InitializeMediumTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
//        {
//            var shouldCopy = true;
//            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

//            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
//            if (shadowCopiedPlug.Exists)
//            {
//                //it's better to use LastWriteTimeUTC, but not all file systems have this property
//                //maybe it is better to compare file hash?
//                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
//                if (areFilesIdentical)
//                {
//                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
//                    shouldCopy = false;
//                }
//                else
//                {
//                    //delete an existing file
//                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);
//                    File.Delete(shadowCopiedPlug.FullName);
//                }
//            }

//            if (shouldCopy)
//            {
//                try
//                {
//                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
//                }
//                catch (IOException)
//                {
//                    Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
//                    //this occurs when the files are locked,
//                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
//                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
//                    try
//                    {
//                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
//                        File.Move(shadowCopiedPlug.FullName, oldFile);
//                    }
//                    catch (IOException exc)
//                    {
//                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
//                    }
//                    //ok, we've made it this far, now retry the shadow copy
//                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
//                }
//            }

//            return shadowCopiedPlug;
//        }

//        /// <summary>
//        /// Determines if the folder is a bin plugin folder for a package
//        /// </summary>
//        /// <param name="folder"></param>
//        /// <returns></returns>
//        private static bool IsPackagePluginFolder(DirectoryInfo folder)
//        {
//            if (folder == null) return false;
//            if (folder.Parent == null) return false;
//            if (!folder.Parent.Name.Equals("Plugins", StringComparison.OrdinalIgnoreCase)) return false;
//            return true;
//        }

//        /// <summary>
//        /// Gets the full path of InstalledPlugins.txt file
//        /// </summary>
//        /// <returns></returns>
//        private static string GetInstalledPluginsFilePath()
//        {
//            return CommonHelper.MapPath(InstalledPluginsFilePath);
//        }

//        #endregion
//    }
//}
