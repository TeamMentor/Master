using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Config
    {
        [Admin] public static TM_Xml_Database   setDefaultValues(this TM_Xml_Database tmXmlDatabase)         
        {            
            tmXmlDatabase.Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();
            tmXmlDatabase.GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            tmXmlDatabase.GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();
            tmXmlDatabase.GuidanceExplorers_Paths     = new Dictionary<guidanceExplorer, string>();            
            tmXmlDatabase.VirtualArticles             = new Dictionary<Guid, VirtualArticleAction>();
            tmXmlDatabase.Events                      = new TM_Database_Events();            
            tmXmlDatabase.Path_XmlDatabase            = null;
            tmXmlDatabase.Path_XmlLibraries           = null;
            tmXmlDatabase.UserData                    = null;
            tmXmlDatabase.Server                      = null;

            return tmXmlDatabase;
        }
        /// <summary>
        /// This is the function that calculates the path to the TM XML Database (i.e. local file storage of TM files)
        /// </summary>
        /// <param name="tmXmlDatabase">this</param>
        /// <returns></returns>
        [Admin] public static TM_Xml_Database   set_Path_XmlDatabase(this TM_Xml_Database tmXmlDatabase)  
        {
            tmXmlDatabase.Path_XmlDatabase = null;
            tmXmlDatabase.Path_XmlLibraries = null;

            if (tmXmlDatabase.UsingFileStorage.isFalse())
                return tmXmlDatabase;
                
            // try to find a local folder to hold the TM Database data
            
            var webRoot  = TM_Server.WebRoot;
            var tmStatus = TM_Status.Current;

                
            var usingAppData = webRoot.contains(@"TeamMentor.UnitTests\bin") ||             // when running UnitTests under NCrunch
                                webRoot.contains(@"site\wwwroot");                           // when running from Azure (or directly on IIS)
            if (usingAppData.isFalse())
            {
                //calculate location and see if we can write to it
                
                var xmlDatabasePath = TM_Server.WebRoot.pathCombine(TMConsts.VIRTUAL_PATH_MAPPING)
                                                       .pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY)   //use by default the 'Library_Data\\XmlDatabase" value due to legacy support (pre 3.3)
                                                       .fullPath();
                    
                if (xmlDatabasePath.createDir().dirExists() && xmlDatabasePath.canWriteToPath())
                {                        
                    tmXmlDatabase.Path_XmlDatabase              = xmlDatabasePath;           // if can write it then make it the Path_XmlDatabase
                    tmStatus.TM_Database_Location_Using_AppData = false;
                    "[TM_Xml_Database][set_Path_XmlDatabase] Path_XmlDatabase set to: {0}".info(xmlDatabasePath);
                    return tmXmlDatabase;
                }
                "[TM_Xml_Database][set_Path_XmlDatabase] It was not possible to write to mapped folder: {0}".error(xmlDatabasePath);
            }
                
            var appData_Path = TM_Server.WebRoot.pathCombine("App_Data")
                                                .pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH)        // inside App_Data we can use the folder value 'TeamMentor' 
                                                .fullPath();   
            if (appData_Path.createDir().dirExists() && appData_Path.canWriteToPath())
            {
                tmXmlDatabase.Path_XmlDatabase              = appData_Path;           // if can write it then make it the Path_XmlDatabase
                tmStatus.TM_Database_Location_Using_AppData = true;
                "[TM_Xml_Database][set_Path_XmlDatabase] Path_XmlDatabase set to: {0}".info(appData_Path);
                return tmXmlDatabase;       
            }   
                           
            "[TM_Xml_Database][set_Path_XmlDatabase] It was not possible to write to App_Data folder: {0}".error(appData_Path);                
            "[TM_Xml_Database][set_Path_XmlDatabase] Disabled use of UsingFileStorage".debug();
            tmXmlDatabase.UsingFileStorage = false;                 
            return tmXmlDatabase;
        }        
/*TODO*/[Admin] public static TM_Xml_Database   set_Path_XmlLibraries(this TM_Xml_Database tmXmlDatabase) 
        {
            return tmXmlDatabase;
            try
            {
                var tmConfig        = TMConfig.Current;
                var xmlDatabasePath = tmXmlDatabase.Path_XmlDatabase;  // tmConfig.xmlDatabasePath();
                var libraryPath     = tmConfig.TMSetup.XmlLibrariesPath;

                "[TM_Xml_Database] [SetPaths_XmlDatabase] TM_Xml_Database.Path_XmlDatabase: {0}".debug(xmlDatabasePath);
                "[TM_Xml_Database] [SetPaths_XmlDatabase] TMConfig.Current.XmlLibrariesPath: {0}".debug(libraryPath);


                if (libraryPath.dirExists().isFalse())
                {
                    libraryPath = xmlDatabasePath.pathCombine(libraryPath);
                    libraryPath.createDir();  // make sure it exists
                }

                tmXmlDatabase.Path_XmlDatabase = xmlDatabasePath;
                tmXmlDatabase.Path_XmlLibraries = libraryPath;
                "[TM_Xml_Database] Paths configured".info();
            }
            catch (Exception ex)
            {
                "[TM_Xml_Database] [set_Path_XmlLibraries]: {0} \n\n {1}".error(ex.Message, ex.StackTrace);
            }
            return tmXmlDatabase;
        }
        [Admin] public static TM_Xml_Database   set_Path_UserData    (this TM_Xml_Database tmXmlDatabase)     
        {            
            if (tmXmlDatabase.isNull())
                return tmXmlDatabase;

            tmXmlDatabase.UserData = null;              // to ensure that a new object is created
            var userData = tmXmlDatabase.userData();

            if(tmXmlDatabase.UsingFileStorage.isFalse())
                return tmXmlDatabase;

            var userData_Config = tmXmlDatabase.Server.userData_Config();

            if (userData_Config.isNull() || userData_Config.Name.notValid())
            { 
                "[TM_Xml_Database][set_Path_UserData] userData_Config or its name was null or empty, so going to to use the default value of: {0}".error(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
                userData_Config = new TM_Server.Config()
                                    {
                                        Name = TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA
                                    };
            }

            var xmlDatabasePath = tmXmlDatabase.Path_XmlDatabase;                    // all files are relative to this path
                                
            var userDataPath    = xmlDatabasePath.pathCombine(userData_Config.Name); // use the userData_Config.Name as the name of the folder to put UserData files
                
            userDataPath.createDir();                                                // create if needed
            if (userDataPath.dirExists())
            {
                userData.Path_UserData = userDataPath.createDir();
                "[TM_Xml_Database] [set_Path_UserData] TMConfig.Current.UserDataPath: {0}".debug(userDataPath);
            }
            else
            {
                userData.Path_UserData = null;
                "[TM_Xml_Database] [set_Path_UserData] failed to create the folder: {0}".error(userDataPath);
                "[TM_Xml_Database] [set_Path_UserData] disabing UsingFileStorage".debug();
                tmXmlDatabase.UserData.UsingFileStorage = false;
            }                                                
            
            return tmXmlDatabase;
        }
/*TODO*/[Admin] public static TM_Xml_Database   load_UserData(this TM_Xml_Database tmXmlDatabase)         
        {
            tmXmlDatabase.set_Path_UserData();
            
            tmXmlDatabase.userData()                // returns a TM_UserData object
                            //.syncWithGit()        // TODO: add userData setup event to allow GIT to support for UserData repos
                            .tmConfig_Load()                            
                            .secretData_Load()
                            .loadUsers();                         
            return tmXmlDatabase;
        }
/*TODO*/[Admin] public static TM_Xml_Database   load_Libraries(this TM_Xml_Database tmXmlDatabase)        
        {
            return tmXmlDatabase;
        }
/*TODO*/[Admin] public static TM_Xml_Database   load_SiteData(this TM_Xml_Database tmXmlDatabase)         
        {
            return tmXmlDatabase;
        }
    
        public static TM_UserData           userData(this TM_Xml_Database tmDatabase)                 
        {
            if (tmDatabase.isNull())
                return null;
            if (tmDatabase.UserData.isNull())
                tmDatabase.UserData = new TM_UserData(tmDatabase.UsingFileStorage);
            return tmDatabase.UserData;
        }
        [Admin] public static TM_Xml_Database   loadData(this TM_Xml_Database tmDatabase)                 
        {
            if (TM_Status.Current.TM_Database_In_Setup_Workflow)
                throw new Exception("TM Exeption: TM_Xml_Database Setup was called twice in a row (without the first Setup sequence had ended)");

            TM_Status.Current.TM_Database_In_Setup_Workflow = true;

            try
            {
                tmDatabase.setDefaultValues()
                          .set_Path_XmlDatabase()
                          .tmServer_Load();

                tmDatabase.load_UserData()                          
                          .load_SiteData()
                          .load_Libraries();

                //.set_Path_XmlLibraries();


                /*
                
                                UserData.SetUp();
                                Logger_Firebase.createAndHook();
                                "[TM_Xml_Database] TM is Starting up".info();
                                this.logTBotActivity("TM Xml Database", "TM is (re)starting and user Data is now loaded");
                                this.userData().copy_FilesIntoWebRoot();
                                if (UsingFileStorage)
                                {
                //                    this.set_Path_XmlLibraries();
                                    this.handle_UserData_GitLibraries();
                                    loadDataIntoMemory();
                                    this.logTBotActivity("TM Xml Database", "Library Data is loaded");
                                }
                                if (Server.Users_Create_Default_Admin)
                                    UserData.createDefaultAdminUser();  // make sure the admin user exists and is configured
                                this.logTBotActivity("TM Xml Database", "TM started at: {0}".format(DateTime.Now));
                 * * */
            }
            catch (Exception ex)
            {
                "[TM_Xml_Database] Setup: {0} \n\n".error(ex.Message, ex.StackTrace);
                //if (TM_StartUp.Current.notNull())                       //will happen when TM_Xml_Database ctor is called by an user with no admin privs
                //    TM_StartUp.Current.TrackingApplication.saveLog();
            }
            TM_Status.Current.TM_Database_In_Setup_Workflow = false;
            return tmDatabase;
        }
        [Admin] public static string            reloadData(this TM_Xml_Database tmDatabase)               
        {
            "In Reload data".info();
            tmDatabase.clear_GuidanceItemsCache()                            // start by clearing the cache                                   
                      .loadData();                                                                            

            var stats = "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems"
                            .format(tmDatabase.Path_XmlLibraries.directoryName(),
                                    tmDatabase.tmLibraries().size(),
                                    tmDatabase.tmViews().size(),
                                    tmDatabase.tmGuidanceItems().size());
            return stats;                                                   // return some stats
        }
        [Admin] public static string            tmServer_Location(this TM_Xml_Database tmDatabase)          
        {
            return (tmDatabase.notNull() && tmDatabase.UsingFileStorage)
                        ? tmDatabase.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME)
                        : null;
        }
        [Admin] public static TM_Xml_Database   tmServer_Load(this TM_Xml_Database tmDatabase)    
        {
            tmDatabase.Server = new TM_Server().setDefaultData();
            if (tmDatabase.UsingFileStorage)
            {
                var tmServerFile = tmDatabase.tmServer_Location();
                if (tmServerFile.valid())
                {
                    if (tmServerFile.fileExists().isFalse())
                    {
                        "[TM_Xml_Database][load_TMServer_Config] expected TM_Server file didn't exist, so creating it: {0}".info(tmServerFile);
                        tmDatabase.Server.saveAs(tmServerFile);
                    }
                    var tmServer = tmServerFile.load<TM_Server>();
                    if (tmServer.isNull())
                        "[TM_Xml_Database][load_TMServer_Config] Failed to load tmServer file: {0}   Default values will be used".error(tmServerFile);
                    else
                        tmDatabase.Server = tmServer;
                }
            }
            return tmDatabase;
        }
        [Admin] public static bool              tmServer_Save(this TM_Xml_Database tmDatabase)     
        {            
            if (tmDatabase.UsingFileStorage)
            {
                var tmServerFile = tmDatabase.tmServer_Location();
                if (tmServerFile.valid())
                    return tmDatabase.Server.saveAs(tmServerFile);
            }                            
            return false;
        }
    }
}
