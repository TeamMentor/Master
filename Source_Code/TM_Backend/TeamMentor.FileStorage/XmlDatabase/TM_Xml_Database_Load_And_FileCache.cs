using System;
using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.FileStorage;
using TeamMentor.FileStorage.XmlDatabase;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Load_And_FileCache
    {
        public static Thread thread_Save_GuidanceItemsCache;

        public static TM_Xml_Database           xmlDB_Load_GuidanceItems_and_Create_CacheFile(this TM_Xml_Database tmDatabase)
        {
            var tmFileStorage = TM_FileStorage.Current;
            
            var pathXmlLibraries = tmFileStorage.Path_XmlLibraries;            
            if (pathXmlLibraries.notNull() && pathXmlLibraries.notNull())
                lock (pathXmlLibraries)
                {
                    //if (tmDatabase.getCacheLocation().fileExists().isFalse())
                    //{
                    "[TM_Xml_Database] in xmlDB_Load_GuidanceItems, creating cache file".debug();
                    var o2Timer = new O2Timer("loaded GuidanceItems from disk").start();
                    //Load GuidanceItem from the disk				
                    foreach (var item in tmFileStorage.GuidanceExplorers_Paths)
                    {
                        var guidanceExplorer = item.Key;
                        var pathToLibraryGuidanceItems = item.Value.parentFolder();
                        var libraryId = guidanceExplorer.library.name.guid();                                
                        "libraryId: {0} : {1}".info(libraryId, pathToLibraryGuidanceItems);                                
                        var filesToLoad = pathToLibraryGuidanceItems.files(true, "*.xml");
                        tmDatabase.xmlDB_Load_GuidanceItemsV3(libraryId, filesToLoad);
                    }

                    //save it to the local cache file (reduces load time from 8s to 0.5s)
                    tmDatabase.save_GuidanceItemsToCache();
                            

                    tmDatabase.ensureFoldersAndViewsIdsAreUnique();
                    tmDatabase.removeMissingGuidanceItemsIdsFromViews();
                    o2Timer.stop();
                    //}
                }            
            return tmDatabase;
        } 

        public static bool                               loadDataIntoMemory(this TM_FileStorage tmFileStorage)
        {
            if (tmFileStorage.path_XmlDatabase().dirExists().isFalse())
            {
                "[TM_Xml_Database] in loadDataIntoMemory, provided pathXmlDatabase didn't exist: {0}".error(tmFileStorage.path_XmlDatabase());
                return false;
            }
            tmFileStorage.loadLibraryDataFromDisk();
            //tmXmlDatabase.setupGitSupport();
            //tmXmlDatabase.UserData.loadTmUserData();
            return true;
        }
        public static TM_Xml_Database                    loadLibraryDataFromDisk        (this TM_FileStorage tmFileStorage)
        {
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;
            tmXmlDatabase.GuidanceExplorers_XmlFormat = tmFileStorage.path_XmlLibraries().getGuidanceExplorerObjects();
            tmFileStorage.load_GuidanceItemsFromCache();                        
            return tmXmlDatabase;
        }
        public static TM_Xml_Database                    reloadGuidanceExplorerObjects     (this TM_Xml_Database tmDatabase)
        {
            tmDatabase.setGuidanceExplorerObjects();
            tmDatabase.reCreate_GuidanceItemsCache();                  
            return tmDatabase;
        }		
        public static TM_Xml_Database                    setGuidanceExplorerObjects     (this TM_Xml_Database tmDatabase)
        {			
            var tmFileStorage = TM_FileStorage.Current;
            //"in setGuidanceExplorerObjects".info();
            var pathXmlLibraries = tmFileStorage.Path_XmlLibraries;
            tmFileStorage.GuidanceExplorers_Paths.Clear();
            TM_Xml_Database.Current.GuidanceExplorers_XmlFormat = pathXmlLibraries.getGuidanceExplorerObjects();				
            return tmDatabase;
        }		
        public static guidanceExplorer                   getGuidanceExplorerObject      (this string xmlFile)
        {			
            try
            {
                var fileContents = xmlFile.fileContents();
                if(fileContents.contains("<guidanceExplorer xmlns=\"urn:microsoft:guidanceexplorer\">")) // handle legacy guidanceExplorer files
                    fileContents = fileContents.replace("<guidanceExplorer xmlns=\"urn:microsoft:guidanceexplorer\">", "<guidanceExplorer>");
                //fileContents.info();
                return fileContents.deserialize<guidanceExplorer>(false);
                //"loading :{0}".debug(xmlFile);
                //return guidanceExplorer.Load(xmlFile); 									
            }
            catch(Exception ex)
            {
                "[getGuidanceExplorerObject]: {0}".error(ex.Message);
            }
            return null;
        }		
        public static Dictionary<Guid, guidanceExplorer> addGuidanceExplorerObject      (this Dictionary<Guid, guidanceExplorer> guidanceExplorers, string xmlFile)
        {
            var guidanceExplorer = xmlFile.getGuidanceExplorerObject();
            return guidanceExplorers.addGuidanceExplorerObject(guidanceExplorer, xmlFile);			
        }		
        public static Dictionary<Guid, guidanceExplorer> addGuidanceExplorerObject      (this Dictionary<Guid, guidanceExplorer> guidanceExplorers, guidanceExplorer newGuidanceExplorer, string xmlFile)
        {
            if (newGuidanceExplorer.notNull())
            {			
                try
                {
                    var libraryGuid = newGuidanceExplorer.library.name.guid();
                    
                    //check if the name is already there
                    foreach(guidanceExplorer guidanceExplorer in guidanceExplorers.Values)
                    {						
                        if (guidanceExplorer.library.caption == newGuidanceExplorer.library.caption)
                        {
                            "[addGuidanceExplorerObject]: Skipping load due to duplicate Library name '{0}' was in both library {1} and {2}".error(guidanceExplorer.library.caption, guidanceExplorer.library.name,  newGuidanceExplorer.library.name);
                            return guidanceExplorers;
                        }
                    }
                    //check if the guid is already there
                    if (guidanceExplorers.hasKey(libraryGuid))
                    {
                        "[addGuidanceExplorerObject]: for {0} , duplicate LibraryID detected, assiging a new Library Id: {0}".error(newGuidanceExplorer.library.caption, libraryGuid);
                        libraryGuid = Guid.NewGuid();
                        newGuidanceExplorer.library.name = libraryGuid.str();
                        "[addGuidanceExplorerObject]: new ID for library {0} is now {1}".error(newGuidanceExplorer.library.caption, libraryGuid);
                        newGuidanceExplorer.xmlDB_Save_GuidanceExplorer(TM_Xml_Database.Current);//, false);
                    }
                    TM_FileStorage.Current.GuidanceExplorers_Paths.add(newGuidanceExplorer, xmlFile); // add this mapping so that we can find it base on name
                    guidanceExplorers.Add(libraryGuid, newGuidanceExplorer);

                }
                catch//(Exception ex)
                {
                    "[addGuidanceExplorerObject] error importing guidanceExplorer: {0}".error(newGuidanceExplorer);
                }
            }
            return guidanceExplorers;
        }
        public static bool                               isGuidanceExplorerFile(this string file)
        {
            if (file.fileExists().isFalse())
                return false;
            var fileContents = file.fileContents().fix_CRLF();
            var secondLine = fileContents.lines().second();
            return secondLine.starts("<guidanceExplorer");
        }
        public static List<string>                       getGuidanceExplorerFilesInPath (this string pathXmlLibraries)
        {            
                        
            var guidanceExplorerXmlFiles = pathXmlLibraries.folders()
                .files("*.xml")                                                             
                .where(isGuidanceExplorerFile);
            return guidanceExplorerXmlFiles;
        }
        public static Dictionary<Guid, guidanceExplorer> getGuidanceExplorerObjects     (this string pathXmlLibraries)
        {			            
            var guidanceExplorers           = new Dictionary<Guid,guidanceExplorer>();
            var guidanceExplorersXmlFiles   = pathXmlLibraries.getGuidanceExplorerFilesInPath();

            foreach (var xmlFile in guidanceExplorersXmlFiles)            
                guidanceExplorers.addGuidanceExplorerObject(xmlFile);            

            return guidanceExplorers;			
        }		
        public static string                             getCacheLocation               (this TM_Xml_Database tmDatabase) //, TM_Library library)
        {
            var pathXmlDatabase = tmDatabase.path_XmlDatabase();
            return pathXmlDatabase.pathCombine("Cache_guidanceItems.cacheXml");//.format(library.Caption));
        }		
        public static TM_FileStorage                    load_GuidanceItemsFromCache     (this TM_FileStorage tmFileStorage)
        {
            if (tmFileStorage.isNull())
                return tmFileStorage;
            var tmDatabase = tmFileStorage.TMXmlDatabase;
                 
            var chacheFile = tmDatabase.getCacheLocation();
            if (chacheFile.fileExists().isFalse())
            {
                "[TM_Xml_Database] [load_GuidanceItemsFromCache] cached file not found: {0}".error(chacheFile);
                tmDatabase.xmlDB_Load_GuidanceItems_and_Create_CacheFile();
            }
            else
            {
                var o2Timer = new O2Timer("[TM_Xml_Database] [loadGuidanceItemsFromCache] loaded cache ").start();
                var loadedGuidanceItems = chacheFile.load<List<TeamMentor_Article>>();
                o2Timer.stop();
                if (loadedGuidanceItems.isNull()) //if we couldn't load it , delete it
                    Files.deleteFile(chacheFile);
                else
                {
                    o2Timer = new O2Timer("[TM_Xml_Database] [loadGuidanceItemsFromCache] loading files ").start();
                    foreach (var loadedGuidanceItem in loadedGuidanceItems)
                        if (loadedGuidanceItem.notNull())
                            tmDatabase.Cached_GuidanceItems.add(loadedGuidanceItem.Metadata.Id,
                                loadedGuidanceItem);
                    o2Timer.stop();
                }
                tmDatabase.populateGuidanceItemsFileMappings();
            }
            return tmFileStorage;
        }		
        public static TM_Xml_Database                    save_GuidanceItemsToCache       (this TM_Xml_Database tmDatabase)
        {            
            var cacheFile = tmDatabase.getCacheLocation();
            if (cacheFile.notNull())
            {
                var o2Timer = new O2Timer("saveGuidanceItemsToCache").start();
                lock (tmDatabase.Cached_GuidanceItems)
                {
                    tmDatabase.Cached_GuidanceItems.Values.toList().saveAs(cacheFile);
                    //tmDatabase.triggerGitCommit();          // TODO: add save_GuidanceItemsToCache event to allow GIT to support for saving XML data
                }
                o2Timer.stop();
            }            
            return tmDatabase;
        }					        	
        public static TM_Xml_Database                    queue_Save_GuidanceItemsCache  (this TM_Xml_Database tmDatabase)
        {
            
            // do this on a separate thread so that we don't hang the current request			
            if (thread_Save_GuidanceItemsCache.isNull())
            { 
                thread_Save_GuidanceItemsCache = O2Thread.mtaThread(
                    ()=>{
                            tmDatabase.sleep(1000,false);
                            tmDatabase.save_GuidanceItemsToCache();
                            thread_Save_GuidanceItemsCache = null;
                    });
            
            }			            
            return tmDatabase;
        }       
        public static TM_Xml_Database                    clear_GuidanceItemsCache       (this TM_Xml_Database tmDatabase)
        {
            "[TM_Xml_Database] clear_GuidanceItemsCache".info();
            
            var cacheFile = tmDatabase.getCacheLocation();
            if (cacheFile.notNull() && cacheFile.fileExists())
            {
                Files.deleteFile(cacheFile);
                "cache file deleted OK:{0}".info(cacheFile.fileExists().isFalse());
            }
            
            tmDatabase.Cached_GuidanceItems.Clear();
            return tmDatabase;
        }
        public static TM_Xml_Database                    reCreate_GuidanceItemsCache    (this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.clear_GuidanceItemsCache()
                             .xmlDB_Load_GuidanceItems_and_Create_CacheFile();            
        }		
        	
        public static string                             removeGuidanceItemFileMapping  (this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            var fileMappings = tmDatabase.guidanceItems_FileMappings();

            if (fileMappings.hasKey(guidanceItemId))
            {
                var xmlPath = fileMappings[guidanceItemId];
                fileMappings.Remove(guidanceItemId);
                return xmlPath;
            }
            return null;
        }		
        public static string                             getXmlFilePathForGuidanceId    (this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            return tmDatabase.getXmlFilePathForGuidanceId(guidanceItemId, Guid.Empty);
        }		
        public static string                             getXmlFilePathForGuidanceId    (this TM_Xml_Database tmDatabase, Guid guidanceItemId, Guid libraryId)	
        {		
            var tmFileStorate = TM_FileStorage.Current;
            //first see if we already have this mapping
            if (tmFileStorate.GuidanceItems_FileMappings.hasKey(guidanceItemId))
            {
                //"in getXmlFilePathForGuidanceId, found id in current mappings: {0}".info( guidanceItemId);
                return tmFileStorate.GuidanceItems_FileMappings[guidanceItemId];
            }
            //then update the GuidanceItems_FileMappings dictionary            
            
            if (libraryId == Guid.Empty)
            {
                "[getXmlFilePathForGuidanceId] When creating a new GuidanceItem a libraryId must be provided".error();
                return null;
            }
            var tmLibrary = tmDatabase.tmLibrary(libraryId);
            if (tmLibrary == null)
            {
                "[getXmlFilePathForGuidanceId] When creating a new GuidanceItem could not find library for libraryId: {0}".error(libraryId);
                return null;
            }
            var libraryPath = tmDatabase.xmlDB_Path_Library_RootFolder(tmLibrary);
            var newArticleFolder = libraryPath.pathCombine(TMConsts.DEFAULT_ARTICLE_FOLDER_NAME);
            var xmlPath = newArticleFolder.createDir()                                         
                .pathCombine("{0}.xml".format(guidanceItemId));

            "in getXmlFilePathForGuidanceId, no previous mapping found so adding new GuidanceItems_FileMappings for :{0}".info(xmlPath);

            tmFileStorate.GuidanceItems_FileMappings.add(guidanceItemId, xmlPath); //add it to the file_mappings dictionary so that we know it for next time
            return xmlPath;
            
        }

          // this is a (bit) time consumining (less 1s for 8000 files), so it should only be done once (this is another good cache target)
        public static void populateGuidanceItemsFileMappings(this TM_Xml_Database tmXmlDatabase)
        {
            var tmFileStorate = TM_FileStorage.Current;
            tmFileStorate.GuidanceItems_FileMappings.Clear();
            var o2Timer = new O2Timer("[TM_Xml_Database] populateGuidanceExplorersFileMappings").start();
            foreach (var filePath in tmFileStorate.Path_XmlLibraries.files(true, "*.xml"))
            {
                var fileId = filePath.fileName().remove(".xml");
                if (fileId.isGuid())
                {
                    //"[populateGuidanceItemsFileMappings] loading GuidanceItem ID {0}".info(fileId);
                    var guid = fileId.guid();
                    if (tmFileStorate.GuidanceItems_FileMappings.hasKey(guid))
                    {
                        "[TM_Xml_Database] [populateGuidanceItemsFileMappings] duplicate GuidanceItem ID found {0}".error(guid);
                    }
                    else
                        tmFileStorate.GuidanceItems_FileMappings.Add(guid, filePath);				
                }
            }
            o2Timer.stop();
            "[TM_Xml_Database] [populateGuidanceItemsFileMappings] There are {0} files mapped in GuidanceItems_FileMappings".info(tmFileStorate.GuidanceItems_FileMappings.size());			
        }
    }
}