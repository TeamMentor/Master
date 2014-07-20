using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Web35.API;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage.XmlDatabase;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.FileStorage
{
    public static class FileStorage_Libraries
    {        
        [Admin] public static TM_FileStorage                hook_Events_TM_Xml_Database(this TM_FileStorage tmFileStorage)
        {
            UserRole.Admin.demand();
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            tmXmlDatabase.Events.Articles_Cache_Updated .add((tmArticle)=> tmFileStorage.queue_Save_GuidanceItemsCache());
            tmXmlDatabase.Events.Article_Saved          .add((tmDatabase, tmArticle) => tmFileStorage.article_Save(tmArticle));           
            tmXmlDatabase.Events.Library_Deleted        .add((tmDatabase, tmLibrary) => tmFileStorage.library_Deleted(tmLibrary));                       
            tmXmlDatabase.Events.GuidanceExplorer_Save  .add((tmDatabase, guidanceExplorer) => guidanceExplorer.guidanceExplorer_Save(tmFileStorage));           
            return tmFileStorage;
        }


        public static bool library_Deleted(this TM_FileStorage tmFileStorage, TM_Library tmLibrary)
        {                        
            "[xmlDB_DeleteGuidanceExplorer] deleting library with caption: {0}".info(tmLibrary.Caption);
            var pathToLibraryFolder = tmFileStorage.xmlDB_Path_Library_RootFolder(tmLibrary);
                // this is also the Library Root
            if (pathToLibraryFolder.notValid() || pathToLibraryFolder == tmFileStorage.path_XmlDatabase() ||
                pathToLibraryFolder == tmFileStorage.Path_XmlLibraries)
            {
                "[xmlDB_DeleteGuidanceExplorer] [Stopping delete] Something is wrong with the pathToLibrary to delete : {0}"
                    .error(pathToLibraryFolder);
                return false;
            }
            if (pathToLibraryFolder.contains(tmFileStorage.Path_XmlLibraries).isFalse())
            {
                "[xmlDB_DeleteGuidanceExplorer] [Stopping delete] the  pathToLibrary should contain tmDatabase.Path_XmlLibraries : {0}"
                    .error(pathToLibraryFolder);
                return false;
            }
            // the checks above are important since the line below is a recursive folder delete (which can delete a LOT of content is pointed to the wrong folder)
            if (Files.deleteFolder(pathToLibraryFolder, true).isFalse())
            {
                "[xmlDB_DeleteGuidanceExplorer] there was an error deleting the folder: {0}".error(
                    pathToLibraryFolder);
                return false;
            }

            "[xmlDB_DeleteGuidanceExplorer] Library folder deleted OK: {0}".info(pathToLibraryFolder);
            tmFileStorage.reloadGuidanceExplorerObjects(); //reset these

            return true;    
        }

        public static bool                                  guidanceExplorer_Save(this guidanceExplorer guidanceExplorer, TM_FileStorage tmFileStorage)
        {            
            var guidanceExplorersPaths= tmFileStorage.GuidanceExplorers_Paths;

            var libraryName = guidanceExplorer.library.caption;
            if (guidanceExplorersPaths.hasKey(guidanceExplorer).isFalse())
            {                    
                var libraryFolder = tmFileStorage.Path_XmlLibraries.pathCombine(libraryName).createDir();
                var libraryXmlFile = libraryFolder.pathCombine("{0}.xml".format(libraryName));
                guidanceExplorersPaths.add(guidanceExplorer, libraryXmlFile);
            }
                
            var libraryPath = guidanceExplorersPaths.value(guidanceExplorer);
            if (libraryPath.notNull())
            {
                "[xmlDB_Save_GuidanceExplorer] saving GuidanceExplorer '{0}' to {1}'".debug(libraryName, libraryPath);
                    
                libraryPath.parentFolder().createDir();         // ensure library folder exists
                guidanceExplorer.SaveLibraryTo(libraryPath);
                //tmDatabase.triggerGitCommit();                //TODO:add saveToLibraryEvent (to allow TeamMentor.Git to trigger the git commit)
            }
            else
                return false;                            
            return true;            
        }	
        public static Dictionary<guidanceExplorer, string>  guidanceExplorers_Paths(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.GuidanceExplorers_Paths;
        }
        public static string                                xmlDB_Path_Library_XmlFile(this TM_FileStorage tmFileStorage, guidanceExplorer guidanceExplorer)
        {
            if (tmFileStorage.notNull())
            {
                return tmFileStorage.guidanceExplorers_Paths().value(guidanceExplorer);                
            }
            return null;
        }
        public static string xmlDB_Path_Library_XmlFile(this TM_FileStorage tmFileStorage, TM_Library library)
        {
            if (library.isNull())
                return null;
            return tmFileStorage.xmlDB_Path_Library_XmlFile(library.Id);
        }
        public static string xmlDB_Path_Library_XmlFile(this TM_FileStorage tmFileStorage, Guid libraryId)
        {
            var guidanceExplorer = tmFileStorage.tmXmlDatabase().xmlDB_GuidanceExplorer(libraryId);
            return tmFileStorage.xmlDB_Path_Library_XmlFile(guidanceExplorer);
        }
        public static TM_FileStorage        updateGuidanceItems_FileMappings_withNewPath(this TM_FileStorage tmFileStorage, string oldPath, string newPath)
        {            
            foreach(var key in tmFileStorage.GuidanceItems_FileMappings.Keys.toList())
            {
                var value = tmFileStorage.GuidanceItems_FileMappings[key];
                if(value.contains(oldPath))
                    tmFileStorage.GuidanceItems_FileMappings[key] = value.replace(oldPath, newPath);
            }
            return tmFileStorage;
        }
        public static string xmlDB_Path_Library_RootFolder(this TM_FileStorage tmFileStorage, TM_Library tmLibrary)
        {
            var guidanceExplorer = tmLibrary.guidanceExplorer(tmFileStorage.tmXmlDatabase());
            return tmFileStorage.xmlDB_Path_Library_RootFolder(guidanceExplorer);
        }
        public static string xmlDB_Path_Library_RootFolder(this TM_FileStorage tmFileStorage, guidanceExplorer guidanceExplorer)
        {
            var libraryPath = tmFileStorage.xmlDB_Path_Library_XmlFile(guidanceExplorer);
            if (libraryPath.notNull())
            {
                return libraryPath.directoryName(); // from 3.3 the library path is the parent folder of the Library's Xml file
            }
            return null;
        }						



        [Admin]	                    
        public static bool xmlDB_Libraries_ImportFromZip(this TM_FileStorage tmFileStorage, string zipFileToImport, string unzipPassword)
        {
            UserRole.Admin.demand();

            var result = false;
            try
            {    
                var currentLibraryPath = tmFileStorage.Path_XmlLibraries;
                if (currentLibraryPath.isNull())
                    return false;
                if (zipFileToImport.isUri())
                {
                    "[xmlDB_Libraries_ImportFromZip] provided value was an URL so, downloading it: {0}".info(zipFileToImport);
                    zipFileToImport = new Web().downloadBinaryFile(zipFileToImport);                    	
                }
                "[xmlDB_Libraries_ImportFromZip] importing library from: {0}".info(zipFileToImport);
                if (zipFileToImport.fileExists().isFalse())
                    "[xmlDB_Libraries_ImportFromZip] could not find file to import".error(zipFileToImport);
                else
                {                    
                    // handle the zips we get from GitHub

                    var tempDir = @"..\_".add_RandomLetters(3).tempDir(false).fullPath(); //trying to make the unzip path as small as possible
                    var fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip {Password = unzipPassword ?? ""};
                    
                    fastZip.ExtractZip(zipFileToImport, tempDir, "");

                    Files.copyFolder(tempDir, currentLibraryPath, true, true,"");          // just copy all files into Library path
                    Files.deleteFolder(tempDir,true);                                      // delete tmp folder created
                    result = true;
                    
                }
            }
            catch (Exception ex)
            { 
                ex.log("[xmlDB_Libraries_ImportFromZip]");
            }

            if (result)
                tmFileStorage.reloadGuidanceExplorerObjects();                

            return result;
        }				
   
    }
}
