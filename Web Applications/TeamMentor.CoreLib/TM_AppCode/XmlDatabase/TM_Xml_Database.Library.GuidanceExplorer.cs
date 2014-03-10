using System;
using System.Linq;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using urn.microsoft.guidanceexplorer;
using Items = urn.microsoft.guidanceexplorer.Items;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceExplorer
    {		
        public static string REGEX_SAFE_FILE_NAME = @"^[a-zA-Z0-9\-_\s+.']{1,50}$";		

        public static bool                   isValidGuidanceExplorerName(this string name)
        {
            if (name.regEx(REGEX_SAFE_FILE_NAME))
                return true;
            "[isValidGuidanceExplorerName] failed validation for: {0}".info(name);
            return false;
        }		
        public static guidanceExplorer       xmlDB_NewGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption)
        {			
            if (caption.isValidGuidanceExplorerName().isFalse())
            {
                "[TM_Xml_Database][xmlDB_NewGuidanceExplorer] provided caption didn't pass validation regex".error();
                throw new Exception("Provided Library name didn't pass validation regex"); 				
            }
            
            if (tmDatabase.tmLibrary(caption).notNull())
            {
                "[TM_Xml_Database] in xmlDB_NewGuidanceExplorer, a library with that name already existed: {0}".error(caption);
                return null;
            }
            if (libraryId == Guid.Empty)
                libraryId = Guid.NewGuid();
            var newGuidanceExplorer = new guidanceExplorer
                {
                    library = new urn.microsoft.guidanceexplorer.Library
                                {
                                    items = new Items(),
                                    libraryStructure = new LibraryStructure(),
                                    name = libraryId.str(),
                                    caption = caption
                                }
                };
            
            TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.add(libraryId, newGuidanceExplorer);    //add to in memory database
            newGuidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);                             
            "[TM_Xml_Database][xmlDB_NewGuidanceExplorer] Created new Library with id {0} and caption {1}".info(libraryId, caption);
            return newGuidanceExplorer;
        }		
        public static bool                   xmlDB_DeleteGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var tmLibrary = tmDatabase.tmLibrary(libraryId);
            if (tmLibrary.isNull())
                return false;
            if (tmDatabase.UsingFileStorage)
            {
                "[xmlDB_DeleteGuidanceExplorer] deleting library with caption: {0}".info(tmLibrary.Caption);
                var pathToLibraryFolder = tmDatabase.xmlDB_Path_Library_RootFolder(tmLibrary);
                    // this is also the Library Root
                if (pathToLibraryFolder.notValid() || pathToLibraryFolder == tmDatabase.Path_XmlDatabase ||
                    pathToLibraryFolder == tmDatabase.Path_XmlLibraries)
                {
                    "[xmlDB_DeleteGuidanceExplorer][Stopping delete] Something is wrong with the pathToLibrary to delete : {0}"
                        .error(pathToLibraryFolder);
                    return false;
                }
                if (pathToLibraryFolder.contains(tmDatabase.Path_XmlLibraries).isFalse())
                {
                    "[xmlDB_DeleteGuidanceExplorer][Stopping delete] the  pathToLibrary should contain tmDatabase.Path_XmlLibraries : {0}"
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
            }
            else
            {
                TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.remove(tmLibrary.Id);
            }
            
            return true;    
        }
        public static bool       xmlDB_Save_GuidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)        
        {
            return tmDatabase.xmlDB_Save_GuidanceExplorer(tmLibrary.Id);
        }
        public static bool       xmlDB_Save_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
            return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);//, reloadGuidanceItemsMappings);	
        }		        
        public static bool       xmlDB_Save_GuidanceExplorer(this guidanceExplorer guidanceExplorer, TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.UsingFileStorage)
            {
                var libraryName = guidanceExplorer.library.caption;
                if (tmDatabase.GuidanceExplorers_Paths.hasKey(guidanceExplorer).isFalse())
                {                    
                    var libraryFolder = tmDatabase.Path_XmlLibraries.pathCombine(libraryName).createDir();
                    var libraryXmlFile = libraryFolder.pathCombine("{0}.xml".format(libraryName));
                    tmDatabase.GuidanceExplorers_Paths.add(guidanceExplorer, libraryXmlFile);
                }
                
                var libraryPath = tmDatabase.GuidanceExplorers_Paths.value(guidanceExplorer);
                if (libraryPath.notNull())
                {
                    "[xmlDB_Save_GuidanceExplorer] saving GuidanceExplorer '{0}' to {1}'".debug(libraryName, libraryPath);
                    
                    libraryPath.parentFolder().createDir();         // ensure library folder exists
                    guidanceExplorer.SaveLibraryTo(libraryPath);
                    tmDatabase.triggerGitCommit();
                }
                else
                    return false;                
            }
            return true;            
        }		
        public static TM_Xml_Database        xmlDB_Save_GuidanceExplorers(this TM_Xml_Database tmDatabase)
        {
            foreach(var guidanceExplorer in tmDatabase.xmlDB_GuidanceExplorers())
                guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);
            return tmDatabase;
        }		
        public static bool       xmlDB_UpdateGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption, bool deleteLibrary)
        {
            //"[xmlDB_UpdateGuidanceExplorer]".info();
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId).isFalse())
            {
                "[TM_Xml_Database] in xmlDB_UpdateGuidanceExplorer, could not find library to update with id: {0}".error(libraryId);
                return false;
            }						
            if (deleteLibrary)
            {				
                return tmDatabase.xmlDB_DeleteGuidanceExplorer(libraryId);                
            }
                        
            var guidanceExplorerToUpdate = TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId];
                
            // this is a rename 
            if (guidanceExplorerToUpdate.library.caption != caption)
                return tmDatabase.xmlDB_RenameGuidanceExplorer(guidanceExplorerToUpdate, caption);
            return false;			
        }		
        public static bool       xmlDB_RenameGuidanceExplorer(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer, string newCaption)
        {
            if (newCaption.isValidGuidanceExplorerName().isFalse())
            {
                "[TM_Xml_Database][xmlDB_RenameGuidanceExplorer] provided caption didn't pass validation regex".error();
                //throw new Exception("Provided Library name didn't pass validation regex"); 				                
            }
            else if(guidanceExplorer.notNull())
            {                
                guidanceExplorer.library.caption = newCaption;  // update in memory library name value

                return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);                // save it 
               

                /*if (tmDatabase.UsingFileStorage)                // soft try to rename the library (disabled for now)
                {
                    try
                    {
                        var current_LibraryRootFolder = tmDatabase.xmlDB_Path_Library_RootFolder(guidanceExplorer);
                        var new_LibraryRootFolder = tmDatabase.Path_XmlLibraries.pathCombine(newCaption);
                        Files.renameFolder(current_LibraryRootFolder, new_LibraryRootFolder);
                        if (new_LibraryRootFolder.dirExists())
                            tmDatabase.GuidanceExplorers_Paths.add(guidanceExplorer, new_LibraryRootFolder);
                    }
                    catch (Exception ex)
                    {
                        ex.log("[xmlDB_RenameGuidanceExplorer] in trying to rename the library folder");                            
                    }
                    
                }*/                       
            }
            return false;			
        }		
        public static TM_Xml_Database        updateGuidanceItems_FileMappings_withNewPath(this TM_Xml_Database tmDatabase, string oldPath, string newPath)
        {
            foreach(var key in TM_Xml_Database.Current.GuidanceItems_FileMappings.Keys.toList())
            {
                var value = TM_Xml_Database.Current.GuidanceItems_FileMappings[key];
                if(value.contains(oldPath))
                    TM_Xml_Database.Current.GuidanceItems_FileMappings[key] = value.replace(oldPath, newPath);
            }
            return tmDatabase;
        }		
        //public static string                 xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, string caption)
        

        public static string xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, TM_Library library)
        {
            return tmDatabase.xmlDB_Path_Library_XmlFile(library.Id);
        }
        public static string xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
            return tmDatabase.xmlDB_Path_Library_XmlFile(guidanceExplorer);
        }
        public static string xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer)
        {
            if (tmDatabase.UsingFileStorage)
            {
                return tmDatabase.GuidanceExplorers_Paths.value(guidanceExplorer);
                /*if (guidanceExplorer.notNull())
                    if (tmDatabase.GuidanceExplorers_Paths.hasKey(guidanceExplorer))
                        return tmDatabase.GuidanceExplorers_Paths[guidanceExplorer];*/
            }
            return null;
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
        public static guidanceExplorer       xmlDB_GuidanceExplorer(this TM_Xml_Database tmDatabase, string caption)
        {
            foreach(var guidanceExplorer in TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.Values)
                if (guidanceExplorer.library.caption == caption || guidanceExplorer.library.name == caption)
                    return guidanceExplorer;
            "[xmlDB_GuidanceExplorer] Could not find is library with caption: {0}".error(caption);		
            return null;
        }		
        public static guidanceExplorer       xmlDB_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.notNull())		
                if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId))
                    return TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId];
            "[xmlDB_GuidanceExplorer] Could not find is library with id: {0}".error(libraryId);
            return null;
        }		
        public static List<guidanceExplorer> xmlDB_GuidanceExplorers(this TM_Xml_Database tmDatabase)
        {			
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.notNull())				
                return TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.Values.toList();
            "[xmlDB_GuidanceExplorers] GuidanceExplorers_XmlFormat is null".error();
            return new List<guidanceExplorer>();
        }				
        public static guidanceExplorer       guidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
        {
            return tmDatabase.xmlDB_GuidanceExplorer(tmLibrary.Id);
        }
        public static List<Library_V3>       librariesV3(this List<TM_Library> libraries)
        {
            return (from library in libraries select library.libraryV3()).toList();
        }
        public static Library_V3             libraryV3(this TM_Library library)
        {
            if (library.isNull())
                return null;
            return new Library_V3
                            {
                                libraryId = library.Id,
                                name = library.Caption
                            };
        }
        public static Library_V3             libraryV3(this Library library)		
        {
            if (library.isNull())
                return null;
            return new Library_V3
                            {
                                libraryId = library.id.guid(),
                                name = library.caption
                            };			
        }		
        public static Library_V3             libraryV3(this guidanceExplorer guidanceExplorer)		
        {
            if (guidanceExplorer.isNull())
                return null;
            return new Library_V3
                            {
                                libraryId = guidanceExplorer.library.name.guid(), 
                                name = guidanceExplorer.library.caption
                            };			
        }

        [Admin]	                    
        public static bool xmlDB_Libraries_ImportFromZip(this TM_Xml_Database tmDatabase, string zipFileToImport, string unzipPassword)
        {
            var result = false;
            try
            {    
                var currentLibraryPath = tmDatabase.Path_XmlLibraries;
                if (currentLibraryPath.isNull())
                    return false;
                if (zipFileToImport.isUri())
                {
                    "[xmlDB_Libraries_ImportFromZip] provided value was an URL so, downloading it: {0}".info(zipFileToImport);
                    zipFileToImport = new Web().downloadBinaryFile(zipFileToImport);
                    //zipFileToImport =  zipFileToImport.uri().download(); 		
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
                    result = true;
                    /*
                    var gitZipFolderName = tempDir.folders().first().folderName();				// the first folder should be the one created by gitHub's zip
                    var xmlFile_Location1 = tempDir.pathCombine(gitZipFolderName + ".xml");
                    var xmlFile_Location2 = tempDir.pathCombine(gitZipFolderName).pathCombine(gitZipFolderName + ".xml");
                    if (xmlFile_Location1.fileExists() || xmlFile_Location2.fileExists())
                        // if these exists here, just copy the unziped files directly
                    {
                        Files.copyFolder(tempDir, currentLibraryPath, true, true, ".git");
                        if (xmlFile_Location1.fileExists())
                            Files.copy(xmlFile_Location1, currentLibraryPath.pathCombine(gitZipFolderName));
                        result = true;
                    }
                    else
                    {
                        //if (zipFileToImport.extension() == ".master")					
                        var gitZipDir = tempDir.pathCombine(gitZipFolderName);
                        foreach (var libraryFolder in gitZipDir.folders())
                        {
                            var libraryName = libraryFolder.folderName();
                            var targetFolder = currentLibraryPath.pathCombine(libraryName);

                            //default behaviour is to override the existing libraries
                            Files.copyFolder(libraryFolder, currentLibraryPath);

                            //handle the case where the xml file is located outside the library folder
                            var libraryXmlFile = gitZipDir.pathCombine("{0}.xml".format(libraryName));
                            if (libraryXmlFile.fileExists())
                                Files.copy(libraryXmlFile, targetFolder);
                                    // put it in the Library folder which is where it really should be															
                        }
                        var virtualMappings = gitZipDir.pathCombine("Virtual_Articles.xml");
                        if (virtualMappings.fileExists())
                        {
                            Files.copy(virtualMappings, currentLibraryPath); // copy virtual mappings if it exists
                            tmDatabase.mapVirtualArticles();
                        }
                        result = true;
                    } */
                }
            }
            catch (Exception ex)
            { 
                ex.log("[xmlDB_Libraries_ImportFromZip]");
            }

            if (result)
                tmDatabase.reloadGuidanceExplorerObjects();
                //tmDatabase.loadLibraryDataFromDisk();                

            return result;
        }				
    }    
}

