using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;
using FluentSharp.Git.APIs;
using urn.microsoft.guidanceexplorer;
using System.Threading;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Git
    {
        public static TM_Xml_Database setupGitSupport(this TM_Xml_Database tmDatabase)
        {
            var gitConfig = tmDatabase.tmConfig().Git;

            if (gitConfig.LibraryData_Git_Enabled)
            {
                foreach (var library in tmDatabase.tmLibraries())
                {
                    var libraryPath = tmDatabase.xmlDB_Path_Library_XmlFile(library).parentFolder();
                    if (libraryPath.isGitRepository())
                    {
                        var nGit = libraryPath.git_Open();
                        try
                        {
                            nGit.git_Pull_Library();
                            nGit.git_Push_Library();
                        }
                        catch (Exception ex)
                        {
                            ex.log();
                        }
                        
                        tmDatabase.NGits.Add(nGit);
                    }
                }
                tmDatabase.triggerGitCommit();                
            }
            return tmDatabase;        
        }
        public static TM_Xml_Database   triggerGitCommit (this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.tmConfig().Git.LibraryData_Git_Enabled)
            {
                foreach(var nGit in tmDatabase.NGits)
                    if (nGit.status().valid())
                        nGit.gitCommit_SeparateThread();
            }
            return tmDatabase;
        }

        public static API_NGit git_Push_Library(this API_NGit nGit)
        {
            if (TMConfig.Current.Git.LibraryData_Auto_Push)
                try
                {
                    nGit.push();
                }
                catch (Exception ex)
                {
                    ex.log("git_Push_Library");
                }
            return nGit;
        }

        public static API_NGit git_Pull_Library(this API_NGit nGit)
        {
            if (TMConfig.Current.Git.LibraryData_Auto_Pull)
                try
                {
                    nGit.pull();
                }
                catch (Exception ex)
                {
                    ex.log("git_Pull_Library");
                }
            return nGit;
        }

        public static API_NGit gitCommit_SeparateThread(this API_NGit nGit)
        {
            O2Thread.mtaThread(
                ()=>{                        
                        lock (nGit)
                        {
                            nGit.setDefaultAuthor();
                            nGit.add_and_Commit_using_Status();
                            nGit.git_Push_Library();
                        }
                });
            return nGit;
        }


         public static TM_Xml_Database handle_UserData_GitLibraries(this TM_Xml_Database tmDatabase)
        {
            try
            {
                foreach (var gitLibrary in tmDatabase.UserData.SecretData.Libraries_Git_Repositories)
                {
                    if (gitLibrary.regEx("Lib_.*.git"))
                    {
                        var libraryName = gitLibrary.split("Lib_").last().remove(".git").replace("_" , " ");
                        var targetFolder = tmDatabase.Path_XmlLibraries.pathCombine(libraryName);
                        if (targetFolder.dirExists().isFalse())
                        {
                            gitLibrary.git_Clone(targetFolder);
                        }
                        else 
                            "[handle_UserData_GitLibraries] skipping git clone since there was already a library called: {0}".info(libraryName);

                    }
                    else
                        "[handle_UserData_GitLibraries] provided git library didn't fit expected format (it should be called Lib_{LibName}.git, and it was: {0}".error(gitLibrary);
                }
            }
            catch (Exception ex)
            {
                ex.log("handle_UserData_GitLibraries");
                
            }            
            return tmDatabase;
        }
    }

    // this is a (bit) time consumining (less 1s for 8000 files), so it should only be done once (this is another good cache target)
    public static class TM_Xml_Database_Load_And_FileCache_Utils
    {		
        public static void populateGuidanceItemsFileMappings(this TM_Xml_Database tmXmlDatabase)
        {
            tmXmlDatabase.GuidanceItems_FileMappings.Clear();
            var o2Timer = new O2Timer("populateGuidanceExplorersFileMappings").start();
            foreach (var filePath in tmXmlDatabase.Path_XmlLibraries.files(true, "*.xml"))
            {
                var fileId = filePath.fileName().remove(".xml");
                if (fileId.isGuid())
                {
                    //"[populateGuidanceItemsFileMappings] loading GuidanceItem ID {0}".info(fileId);
                    var guid = fileId.guid();
                    if (tmXmlDatabase.GuidanceItems_FileMappings.hasKey(guid))
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

    public static class TM_Xml_Database_ExtensionMethods_Load_And_FileCache
    {
        public static Thread thread_Save_GuidanceItemsCache;

        public static TM_Xml_Database                    loadLibraryDataFromDisk        (this TM_Xml_Database tmXmlDatabase)
        {
            tmXmlDatabase.GuidanceExplorers_XmlFormat = tmXmlDatabase.Path_XmlLibraries.getGuidanceExplorerObjects();
            tmXmlDatabase.load_GuidanceItemsFromCache();
            
            //removed since this need to be handled on the GUI
            /*if (tmXmlDatabase.tmLibraries().empty())     //ensure that there is at least one library                          
                tmXmlDatabase.new_TmLibrary();*/
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
            tmDatabase.GuidanceExplorers_Paths.Clear();
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
                    TM_Xml_Database.Current.GuidanceExplorers_Paths.add(newGuidanceExplorer, xmlFile); // add this mapping so that we can find it base on name
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
                tmDatabase.populateGuidanceItemsFileMappings();
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

            TM_Xml_Database.Current.GuidanceItems_FileMappings.add(guidanceItemId, xmlPath); //add it to the file_mappings dictionary so that we know it for next time
            return xmlPath;
            
        }
    }
    
}