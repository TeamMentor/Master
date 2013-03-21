using System;
using System.Linq;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Network;
using urn.microsoft.guidanceexplorer;

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
        public static string                 xmlDB_DeleteGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var tmLibrary = tmDatabase.tmLibrary(libraryId);
            if (tmLibrary.notNull())			
            {
                var caption = tmLibrary.Caption;
                if (caption.isValidGuidanceExplorerName())
                {                    
                    "[xmlDB_DeleteGuidanceExplorer] deleting library with caption: {0}".info(caption);
                    var pathToLibrary = tmDatabase.xmlDB_LibraryPath(caption);								
                    
                    var pathToGuidanceItemsFolder = tmDatabase.xmlDB_LibraryPath_GuidanceItems(caption);
                                    
                    if (pathToGuidanceItemsFolder.dirExists())
                    {
                        "[xmlDB_DeleteGuidanceExplorer] deleting library guidanceItems folder: {0}".debug(pathToGuidanceItemsFolder);
                        if (Files.deleteFolder(pathToGuidanceItemsFolder,true).isFalse())
                        {
                            "[xmlDB_DeleteGuidanceExplorer] there was an error deleting the folder: {0}".error(pathToGuidanceItemsFolder);
                            return null;
                        }						
                    }
                    
                    "[xmlDB_DeleteGuidanceExplorer] deleting library guidanceItems file: {0}".debug(pathToLibrary);
                    Files.deleteFile(pathToLibrary);
                    
                    if(pathToLibrary.fileExists())
                            "[xmlDB_DeleteGuidanceExplorer] there was problem deleting the file: {0}".error(pathToLibrary);				
                    
                    //check if there is a root directory with the caption name (happens when imported from ZIP
                    pathToGuidanceItemsFolder = tmDatabase.xmlDB_LibraryPath_GuidanceItems(caption);
                    
                    if (pathToGuidanceItemsFolder.dirExists() && pathToGuidanceItemsFolder.files().size() ==0)
                        Files.deleteFolder(pathToGuidanceItemsFolder);					
                    
                    //finally reset these						
                    tmDatabase.reloadGuidanceExplorerObjects(); //reset these                                        
                }
            }
            return null;
        }
        public static guidanceExplorer       xmlDB_Save_GuidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
        /*{
            return tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase, true);
        }
        public static guidanceExplorer       xmlDB_Save_GuidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase,  bool reloadGuidanceItemsMappings)*/
        {
            return tmDatabase.xmlDB_Save_GuidanceExplorer(tmLibrary.Id);//,  reloadGuidanceItemsMappings);			
        }
        public static guidanceExplorer       xmlDB_Save_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        /*{
            return tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId, true);
        }
        public static guidanceExplorer       xmlDB_Save_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId,  bool reloadGuidanceItemsMappings)*/
        {
            var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
            return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);//, reloadGuidanceItemsMappings);	
        }		
        /*public static guidanceExplorer       xmlDB_Save_GuidanceExplorer(this guidanceExplorer guidanceExplorer, TM_Xml_Database tmDatabase)		
        {
            return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase, true);
        }*/
        public static guidanceExplorer       xmlDB_Save_GuidanceExplorer(this guidanceExplorer guidanceExplorer, TM_Xml_Database tmDatabase)//, bool reloadGuidanceItemsMappings)
        {
            if (tmDatabase.UsingFileStorage)
            { 
                var caption = guidanceExplorer.library.caption;
                var libraryPath =  tmDatabase.xmlDB_LibraryPath(caption);
                "[xmlDB_Save_GuidanceExplorer] saving GuidanceExplorer '{0}' to {1}'".debug(caption, libraryPath);			
                if (libraryPath.notNull())
                {
                    libraryPath.parentFolder().createDir(); // ensure library folder exists
                    guidanceExplorer.Save(libraryPath);
                    //if (reloadGuidanceItemsMappings)
                    //    tmDatabase.reloadGuidanceExplorerObjects();		
                    tmDatabase.triggerGitCommit();
                    return guidanceExplorer;
                }			
                "[xmlDB_Save_GuidanceExplorer] could not find libraryPath for GuidanceExplorer: {0} - {1}".error(guidanceExplorer.library.caption, guidanceExplorer.library.name);
            }
            return null;
            //TM_Xml_Database.Current.mapGuidanceItemsViews();			
        }		
        public static TM_Xml_Database        xmlDB_Save_GuidanceExplorers(this TM_Xml_Database tmDatabase)
        {
            foreach(var guidanceExplorer in tmDatabase.xmlDB_GuidanceExplorers())
                guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);
            return tmDatabase;
        }		
        public static guidanceExplorer       xmlDB_UpdateGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption, bool deleteLibrary)
        {
            //"[xmlDB_UpdateGuidanceExplorer]".info();
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId).isFalse())
            {
                "[TM_Xml_Database] in xmlDB_UpdateGuidanceExplorer, could not find library to update with id: {0}".error(libraryId);
                return null;
            }						
            if (deleteLibrary)
            {				
                tmDatabase.xmlDB_DeleteGuidanceExplorer(libraryId);
                return null;
            }
                        
            var guidanceExplorerToUpdate = TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId];
                
            // this is a rename 
            if (guidanceExplorerToUpdate.library.caption != caption)
                return tmDatabase.xmlDB_RenameGuidanceExplorer(guidanceExplorerToUpdate, caption);
            return guidanceExplorerToUpdate;			
        }		
        public static guidanceExplorer       xmlDB_RenameGuidanceExplorer(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer, string newCaption)
        {
            if (newCaption.isValidGuidanceExplorerName().isFalse())
            {
                "[TM_Xml_Database][xmlDB_RenameGuidanceExplorer] provided caption didn't pass validation regex".error();
                throw new Exception("Provided Library name didn't pass validation regex"); 				
            }
            //"[xmlDB_RenameGuidanceExplorer]".info();
            if(guidanceExplorer.notNull())
            {
                if (tmDatabase.UsingFileStorage.isFalse())
                {
                    guidanceExplorer.library.caption = newCaption;									// update in memory library name value
                    return guidanceExplorer;
                }
                var existingCaption = guidanceExplorer.library.caption;
                var existingLibraryPath = tmDatabase.xmlDB_LibraryPath(existingCaption); // TM_Xml_Database.Current.Path_XmlLibraries.pathCombine("{0}.xml".format(guidanceExplorer.library.caption));                
                if(existingLibraryPath.fileExists().isFalse())
                    "[xmlDB_RenameGuidanceExplorer] something is wrong since existingLibraryPath was not there: {0}".error(existingLibraryPath);
                else
                {	
                    var newLibraryPath = tmDatabase.xmlDB_LibraryPath(newCaption);
                    if (newLibraryPath.fileExists())
                        "[xmlDB_RenameGuidanceExplorer] there was already a library and/or file with that name, so stopping rename): {0}".error(newLibraryPath);
                    else
                    {
                    
                        var pathToGuidanceItems_Existing = tmDatabase.xmlDB_LibraryPath_GuidanceItems(existingCaption);
                        var pathToGuidanceItems_New = tmDatabase.xmlDB_LibraryPath_GuidanceItems(newCaption);
                        if(pathToGuidanceItems_Existing.dirExists())	
                        {
                            "xmlDB_RenameGuidanceExplorer {0}-> {1}".info(pathToGuidanceItems_Existing, pathToGuidanceItems_New);
                            //Renaming workflow:
                            Files.deleteFile(existingLibraryPath);                                          // delete original xml library file                            
                            Files.renameFolder(pathToGuidanceItems_Existing,  pathToGuidanceItems_New);     // rename folders
                            guidanceExplorer.library.caption = newCaption;									// update in memory library name value
                            guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);                       // save into new path
                            tmDatabase.updateGuidanceItems_FileMappings_withNewPath(pathToGuidanceItems_Existing,pathToGuidanceItems_New);  //update article's cache mappings
                            return guidanceExplorer;
                        }
                        "[xmlDB_RenameGuidanceExplorer] could find dir with library to rename".error(
                            pathToGuidanceItems_Existing);
                    }                    
                }
            }
            return null;			
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
        public static string                 xmlDB_LibraryPath(this TM_Xml_Database tmDatabase, string caption)
        {
            if (tmDatabase.UsingFileStorage)
            { 
                return TM_Xml_Database.Current.Path_XmlLibraries.pathCombine("{0}\\{0}.xml".format(caption));
                //Removing Legacy suport for library folders in the root of the Xml Libraries folder
                //var libraryPath = TM_Xml_Database.Current.Path_XmlLibraries.pathCombine("{0}.xml".format(caption)); // legacy support for Xml Files on Root of Library
                //if (libraryPath.fileExists())
                //    return libraryPath;
                //Librarypath = TM_Xml_Database.Current.Path_XmlLibraries.pathCombine("{0}\\{0}.xml".format(caption));                
                //if (libraryPath.fileExists())
                //return libraryPath;
                //"[xmlDB_LibraryPath] could not find library path for library called '{0}'".info(caption);
            }
            return null;
        }				
        public static string                 xmlDB_LibraryPath_GuidanceItems(this TM_Xml_Database tmDatabase, string caption)
        {
            var libraryPath = tmDatabase.xmlDB_LibraryPath(caption);
            if (libraryPath.notNull())
            {
                return libraryPath.directoryName(); // from 3.3 the library path is the parent folder of the Library's Xml file

                /*var parentFolder = libraryPath.parentFolder();							//check if the xml file is in a folder with the same name as the library name
                if (parentFolder.folderName() == caption)			
                    return parentFolder;
                return libraryPath.directoryName().pathCombine("{0}".format(caption));  // if it is not , return the parent folder
                 * */
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
                    }   
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

