using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            tmXmlDatabase.Events.Articles_Cache_Updated .add((tmArticle)=> tmXmlDatabase.queue_Save_GuidanceItemsCache());
            tmXmlDatabase.Events.Article_Saved          .add((tmDatabase, tmArticle) => tmXmlDatabase.article_Save(tmArticle));           
            tmXmlDatabase.Events.Library_Deleted        .add((tmDatabase, tmLibrary) => tmFileStorage.library_Deleted(tmLibrary));                       
            tmXmlDatabase.Events.GuidanceExplorer_Save  .add((tmDatabase, guidanceExplorer) => guidanceExplorer.guidanceExplorer_Save(tmDatabase));           
            return tmFileStorage;
        }


        public static bool library_Deleted(this TM_FileStorage tmFileStorage, TM_Library tmLibrary)
        {            
            var tmDatabase = tmFileStorage.TMXmlDatabase;

            "[xmlDB_DeleteGuidanceExplorer] deleting library with caption: {0}".info(tmLibrary.Caption);
            var pathToLibraryFolder = tmDatabase.xmlDB_Path_Library_RootFolder(tmLibrary);
                // this is also the Library Root
            if (pathToLibraryFolder.notValid() || pathToLibraryFolder == tmDatabase.path_XmlDatabase() ||
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
            tmDatabase.reloadGuidanceExplorerObjects(); //reset these

            return true;    
        }

        public static bool                                  guidanceExplorer_Save(this guidanceExplorer guidanceExplorer, TM_Xml_Database tmDatabase)
        {
            var tmFileStorage = TM_FileStorage.Current;
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
        public static Dictionary<guidanceExplorer, string>  guidanceExplorers_Paths(this TM_Xml_Database tmDatabase)
        {
            return TM_FileStorage.Current.GuidanceExplorers_Paths;
        }
        public static string                                xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer)
        {
            if (tmDatabase.notNull())
            {
                return tmDatabase.guidanceExplorers_Paths().value(guidanceExplorer);                
            }
            return null;
        }
        public static string xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, TM_Library library)
        {
            if (library.isNull())
                return null;
            return tmDatabase.xmlDB_Path_Library_XmlFile(library.Id);
        }
        public static string xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
            return tmDatabase.xmlDB_Path_Library_XmlFile(guidanceExplorer);
        }
        public static TM_Xml_Database        updateGuidanceItems_FileMappings_withNewPath(this TM_Xml_Database tmDatabase, string oldPath, string newPath)
        {
            var tmFileStorage = TM_FileStorage.Current;
            foreach(var key in tmFileStorage.GuidanceItems_FileMappings.Keys.toList())
            {
                var value = tmFileStorage.GuidanceItems_FileMappings[key];
                if(value.contains(oldPath))
                    tmFileStorage.GuidanceItems_FileMappings[key] = value.replace(oldPath, newPath);
            }
            return tmDatabase;
        }
        public static string xmlDB_Path_Library_RootFolder(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
        {
            var guidanceExplorer = tmLibrary.guidanceExplorer(tmDatabase);
            return tmDatabase.xmlDB_Path_Library_RootFolder(guidanceExplorer);
        }
        public static string xmlDB_Path_Library_RootFolder(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer)
        {
            var libraryPath = tmDatabase.xmlDB_Path_Library_XmlFile(guidanceExplorer);
            if (libraryPath.notNull())
            {
                return libraryPath.directoryName(); // from 3.3 the library path is the parent folder of the Library's Xml file
            }
            return null;
        }						



             [Admin]	                    
        public static bool xmlDB_Libraries_ImportFromZip(this TM_Xml_Database tmDatabase, string zipFileToImport, string unzipPassword)
        {
            UserGroup.Admin.demand();
            var tmFileStorage = TM_FileStorage.Current;

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
                tmDatabase.reloadGuidanceExplorerObjects();                

            return result;
        }				
   
    }
}
