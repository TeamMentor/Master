using System;
using System.Collections.Generic;
using System.Linq;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.FluentSharp;
using urn.microsoft.guidanceexplorer;
using System.Threading;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Git
    {
        public static TM_Xml_Database setupGitSupport(this TM_Xml_Database tmDatabase)
        {            
            if (tmDatabase.AutoGitCommit)
            {
                tmDatabase.NGit = tmDatabase.Path_XmlLibraries.isGitRepository() 
                                        ? tmDatabase.Path_XmlLibraries.git_Open() 
                                        : tmDatabase.Path_XmlLibraries.git_Init();
                tmDatabase.triggerGitCommit();
            }
            return tmDatabase;        
        }
        public static TM_Xml_Database   triggerGitCommit (this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.AutoGitCommit)
                if (tmDatabase.NGit.status().valid())
                    tmDatabase.gitCommit_SeparateThread();                            
            return tmDatabase;
        }

        public static TM_Xml_Database gitCommit_SeparateThread(this TM_Xml_Database tmDatabase)
        {
            O2Thread.mtaThread(
                ()=>{                        
                        lock (tmDatabase.NGit)
                        {
                            tmDatabase.NGit.add_and_Commit_using_Status();
                        }
                });
            return tmDatabase;

        }
    }

    // this is a (bit) time consumining (less 1s for 8000 files), so it should only be done once (this is another good cache target)
    public static class TM_Xml_Database_Load_and_FileCache_Utils
    {		
        public static void populateGuidanceItemsFileMappings()
        {			
            var o2Timer = new O2Timer("populateGuidanceExplorersFileMappings").start();
            foreach (var filePath in TM_Xml_Database.Current.Path_XmlLibraries.files(true, "*.xml"))
            {
                var fileId = filePath.fileName().remove(".xml");
                if (fileId.isGuid())
                {
                    //"[populateGuidanceItemsFileMappings] loading GuidanceItem ID {0}".info(fileId);
                    var guid = fileId.guid();
                    if (TM_Xml_Database.Current.GuidanceItems_FileMappings.hasKey(guid))
                    {
                        "[populateGuidanceItemsFileMappings] duplicate GuidanceItem ID found {0}".error(guid);
                    }
                    else
                        TM_Xml_Database.Current.GuidanceItems_FileMappings.Add(guid, filePath);				
                }
            }
            o2Timer.stop();
            "There are {0} files mapped in GuidanceItems_FileMappings".info(TM_Xml_Database.Current.GuidanceItems_FileMappings.size());			
        }
    }

    public static class TM_Xml_Database_ExtensionMethods_Load_and_FileCache
    {
        public static Thread thread_Save_GuidanceItemsCache;

        public static TM_Xml_Database                    loadLibraryDataFromDisk        (this TM_Xml_Database tmXmlDatabase)
        {
            tmXmlDatabase.GuidanceExplorers_XmlFormat = tmXmlDatabase.Path_XmlLibraries.getGuidanceExplorerObjects();
            tmXmlDatabase.load_GuidanceItemsFromCache();

            //ensure that there is at least one library 
            if (tmXmlDatabase.tmLibraries().empty())                           
                tmXmlDatabase.new_TmLibrary();
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
            //"in setGuidanceExplorerObjects".info();
            var pathXmlLibraries = TM_Xml_Database.Current.Path_XmlLibraries;
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
            return guidanceExplorers.addGuidanceExplorerObject(guidanceExplorer);			
        }		
        public static Dictionary<Guid, guidanceExplorer> addGuidanceExplorerObject      (this Dictionary<Guid, guidanceExplorer> guidanceExplorers, guidanceExplorer newGuidanceExplorer)
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
                    guidanceExplorers.Add(libraryGuid, newGuidanceExplorer);

                }
                catch//(Exception ex)
                {
                    "[addGuidanceExplorerObject] error importing guidanceExplorer: {0}".error(newGuidanceExplorer);
                }
            }
            return guidanceExplorers;
        }		
        public static List<string>                       getGuidanceExplorerFilesInPath (this string pathXmlLibraries)
        {
            Func<string, bool> isGuidanceExplorerFile = 
                (file)=>{
                            var fileContents = file.fileContents().fixCRLF();
                            var secondLine  = fileContents.lines().second();
                            return secondLine.starts("<guidanceExplorer");
                        };

            //try first to load the library by finding it on the library root (original mode)
            var guidanceExplorerXmlFiles = pathXmlLibraries.files("*.xml")
                                                           .where(isGuidanceExplorerFile);
            
            //then try to find the guidanceItems xml file by looking for an xml file with the same name as the folder
            //this has to be done like this or the save and rename of libraries will not work.toList();
            guidanceExplorerXmlFiles.AddRange(pathXmlLibraries.folders()
                                                              .Select(folder => "{0}\\{1}.xml".format(folder, folder.fileName()))
                                                              .Where(xmlFile => xmlFile.fileExists() && 
                                                                                isGuidanceExplorerFile(xmlFile)));
            return guidanceExplorerXmlFiles;
        }
        public static Dictionary<Guid, guidanceExplorer> getGuidanceExplorerObjects     (this string pathXmlLibraries)
        {			            
            var guidanceExplorers           = new Dictionary<Guid,guidanceExplorer>();
            var guidanceExplorersXmlFiles   = pathXmlLibraries.getGuidanceExplorerFilesInPath();

            foreach(var xmlFile in guidanceExplorersXmlFiles)
                guidanceExplorers.addGuidanceExplorerObject(xmlFile);
                        
            return guidanceExplorers;			
        }		
        public static string                             getCacheLocation               (this TM_Xml_Database tmDatabase) //, TM_Library library)
        {
            var pathXmlDatabase = tmDatabase.Path_XmlDatabase;
            return pathXmlDatabase.pathCombine("Cache_guidanceItems.cacheXml");//.format(library.Caption));
        }		
        public static TM_Xml_Database                    load_GuidanceItemsFromCache     (this TM_Xml_Database tmDatabase)
        {
            //"Loading items from cache".info();            
            var chacheFile = tmDatabase.getCacheLocation();
            if (chacheFile.fileExists().isFalse())
            {
                "[TM_Xml_Database] in loadGuidanceItemsFromCache, cached file not found: {0}".error(chacheFile);
                tmDatabase.xmlDB_Load_GuidanceItems_and_Create_CacheFile();
            }
            else
            {
                var o2Timer = new O2Timer("loadGuidanceItemsFromCache").start();
                var loadedGuidanceItems = chacheFile.load<List<TeamMentor_Article>>();
                o2Timer.stop();
                if (loadedGuidanceItems.isNull()) //if we couldn't load it , delete it
                    Files.deleteFile(chacheFile);
                else
                {
                    o2Timer = new O2Timer("mapping to memory loadGuidanceItemsFromCache").start();
                    foreach (var loadedGuidanceItem in loadedGuidanceItems)
                        if (loadedGuidanceItem.notNull())
                            TM_Xml_Database.Current.Cached_GuidanceItems.add(loadedGuidanceItem.Metadata.Id,
                                                                             loadedGuidanceItem);
                    o2Timer.stop();
                }
            }
            return tmDatabase;
        }		
        public static TM_Xml_Database                    save_GuidanceItemsToCache       (this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.UsingFileStorage)
            {
                var cacheFile = tmDatabase.getCacheLocation();
                if (cacheFile.notNull())
                {
                    var o2Timer = new O2Timer("saveGuidanceItemsToCache").start();
                    lock (TM_Xml_Database.Current.Cached_GuidanceItems)
                    {
                        TM_Xml_Database.Current.Cached_GuidanceItems.Values.toList().saveAs(cacheFile);
                        tmDatabase.triggerGitCommit();
                    }
                    o2Timer.stop();
                }
            }
            return tmDatabase;
        }					        	
        public static TM_Xml_Database                    queue_Save_GuidanceItemsCache  (this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.UsingFileStorage)
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
            }
            return tmDatabase;
        }       
        public static TM_Xml_Database                    clear_GuidanceItemsCache       (this TM_Xml_Database tmDatabase)
        {
            "[TM_Xml_Database] clear_GuidanceItemsCache".info();
            if (tmDatabase.UsingFileStorage)
            {
                var cacheFile = tmDatabase.getCacheLocation();
                if (cacheFile.notNull() && cacheFile.fileExists())
                {
                    Files.deleteFile(cacheFile);
                    "cache file deleted OK:{0}".info(cacheFile.fileExists().isFalse());
                }
            }
            tmDatabase.Cached_GuidanceItems.Clear();
            return tmDatabase;
        }
        public static TM_Xml_Database                    reCreate_GuidanceItemsCache    (this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.clear_GuidanceItemsCache()
                             .xmlDB_Load_GuidanceItems_and_Create_CacheFile();            
        }		
        public static TeamMentor_Article                 update_Cache_GuidanceItems     (this TeamMentor_Article guidanceItem,  TM_Xml_Database tmDatabase)
        {
            guidanceItem.htmlEncode(); // ensure MetaData is encoded

            var guidanceItemGuid = guidanceItem.Metadata.Id;
            if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(guidanceItemGuid))
                TM_Xml_Database.Current.Cached_GuidanceItems[guidanceItemGuid] = guidanceItem;
            else
                TM_Xml_Database.Current.Cached_GuidanceItems.Add(guidanceItem.Metadata.Id, guidanceItem);						

            tmDatabase.queue_Save_GuidanceItemsCache();
            
            return guidanceItem;
        }		
        public static Dictionary<Guid, string>           guidanceItemsFileMappings      (this TM_Xml_Database tmDatabase)
        {
            return TM_Xml_Database.Current.GuidanceItems_FileMappings;
        }		
        public static string                             removeGuidanceItemFileMapping  (this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            if (TM_Xml_Database.Current.GuidanceItems_FileMappings.hasKey(guidanceItemId))
            {
                var xmlPath = TM_Xml_Database.Current.GuidanceItems_FileMappings[guidanceItemId];
                TM_Xml_Database.Current.GuidanceItems_FileMappings.Remove(guidanceItemId);
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
            //first see if we already have this mapping
            if (TM_Xml_Database.Current.GuidanceItems_FileMappings.hasKey(guidanceItemId))
            {
                //"in getXmlFilePathForGuidanceId, found id in current mappings: {0}".info( guidanceItemId);
                return TM_Xml_Database.Current.GuidanceItems_FileMappings[guidanceItemId];
            }
            //then update the GuidanceItems_FileMappings dictionary

            //tmDatabase.populateGuidanceItemsFileMappings();

            if (TM_Xml_Database.Current.GuidanceItems_FileMappings.hasKey(guidanceItemId))
            {
                "[getXmlFilePathForGuidanceId] found id after reindex: {0}".info( guidanceItemId);
                return TM_Xml_Database.Current.GuidanceItems_FileMappings[guidanceItemId];
            }
            
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
            var newGuidanceVirtualFolder = "{0}\\Articles".format(tmLibrary.Caption);
            // if not store it on a _GuidanceItems folder
            var xmlPath = TM_Xml_Database.Current.Path_XmlLibraries
                                         .pathCombine(newGuidanceVirtualFolder)
                                         .createDir()
                                         .pathCombine("{0}.xml".format(guidanceItemId));
            "in getXmlFilePathForGuidanceId, no previous mapping found so guidanceitem to :{0}".info(xmlPath);

            TM_Xml_Database.Current.GuidanceItems_FileMappings.add(guidanceItemId, xmlPath); //add it to the file_mappings dictionary so that we know it for next time
            return xmlPath;
            
        }
    }
    
}