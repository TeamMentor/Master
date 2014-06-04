using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using TeamMentor.Database;
using TeamMentor.UserData;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Config
    {
        [Admin] public static TM_Xml_Database   set_Default_Values(this TM_Xml_Database tmXmlDatabase)
        {
            return tmXmlDatabase.set_Default_Values(tmXmlDatabase.Server);
        }
        [Admin] public static TM_Xml_Database   set_Default_Values(this TM_Xml_Database tmXmlDatabase, TM_Server tmServer)
        {                        
            tmXmlDatabase.Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();
            tmXmlDatabase.GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            tmXmlDatabase.GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();
            tmXmlDatabase.GuidanceExplorers_Paths     = new Dictionary<guidanceExplorer, string>();            
            tmXmlDatabase.VirtualArticles             = new Dictionary<Guid, VirtualArticleAction>();
            //tmXmlDatabase.Events                      = new Events_TM_Xml_Database(tmXmlDatabase);                        
            tmXmlDatabase.Path_XmlLibraries           = null;
            tmXmlDatabase.UserData                    = null;
            tmXmlDatabase.Server                      = tmServer ??  TM_Server.Load();
;

            tmXmlDatabase.Events.After_Set_Default_Values.raise();

            return tmXmlDatabase;
        }
        /// <summary>
        /// This is the function that calculates the path to the TM XML Database (i.e. local file storage of TM files)
        /// </summary>
        /// <param name="tmXmlDatabase">this</param>
        /// <returns></returns>
               
[Admin] public static TM_Xml_Database   set_Path_XmlLibraries(this TM_Xml_Database tmXmlDatabase) 
        {
            try
            {
                var tmConfig        = TMConfig.Current;
                var xmlDatabasePath = tmXmlDatabase.path_XmlDatabase();  // tmConfig.xmlDatabasePath();
                var libraryPath     = tmConfig.TMSetup.XmlLibrariesPath;

                "[TM_Xml_Database] [SetPaths_XmlDatabase] TM_Xml_Database.path_XmlDatabase(): {0}".debug(xmlDatabasePath);
                "[TM_Xml_Database] [SetPaths_XmlDatabase] TMConfig.Current.XmlLibrariesPath: {0}".debug(libraryPath);


                if (libraryPath.dirExists().isFalse())
                {
                    libraryPath = xmlDatabasePath.pathCombine(libraryPath);
                    libraryPath.createDir();  // make sure it exists
                }
                
                tmXmlDatabase.Path_XmlLibraries = libraryPath;
                "[TM_Xml_Database] [SetPaths_XmlDatabase] tmXmlDatabase.Path_XmlLibraries = {0}".info(libraryPath);
                "[TM_Xml_Database] Paths configured".info();
            }
            catch (Exception ex)
            {
                "[TM_Xml_Database] [set_Path_XmlLibraries]: {0} \n\n {1}".error(ex.Message, ex.StackTrace);
            }
            tmXmlDatabase.Events.After_Set_Path_XmlLibraries.raise();   
            return tmXmlDatabase;
        }
        [Admin] public static TM_Xml_Database   set_Path_UserData    (this TM_Xml_Database tmXmlDatabase)     
        {            
            if (tmXmlDatabase.isNull())
                return tmXmlDatabase;

            tmXmlDatabase.UserData = null;              // to ensure that a new object is created
            var userData = tmXmlDatabase.userData();

            if(tmXmlDatabase.usingFileStorage().isFalse())
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

            var xmlDatabasePath = tmXmlDatabase.path_XmlDatabase();                    // all files are relative to this path
                                
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
                tmXmlDatabase.UserData.useFileStorage(false);
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
                            .users_Load()
                            .createDefaultAdminUser();  // make sure the admin user exists and is configured                        ;       
            
            tmXmlDatabase.Events.After_Load_UserData.raise();                      
            return tmXmlDatabase;
        }
/*TODO*/[Admin] public static TM_Xml_Database   load_Libraries(this TM_Xml_Database tmXmlDatabase)        
        {
            tmXmlDatabase.set_Path_XmlLibraries();
            tmXmlDatabase.loadDataIntoMemory();

            tmXmlDatabase.Events.After_Load_Libraries.raise();
            return tmXmlDatabase;
        }
/*TODO*/[Admin] public static TM_Xml_Database   load_SiteData(this TM_Xml_Database tmXmlDatabase)         
        {
            
            tmXmlDatabase.Events.After_Load_SiteData.raise();
            return tmXmlDatabase;
        }
    
        public static TM_UserData           userData(this TM_Xml_Database tmDatabase)                 
        {
            if (tmDatabase.isNull())
                return null;
            if (tmDatabase.UserData.isNull())
            { 
                tmDatabase.UserData = new TM_UserData(tmDatabase.tmServer());                
                tmDatabase.Events.After_UserData_Ctor.raise();
            }
            return tmDatabase.UserData;
        }
        [Admin] public static TM_Xml_Database   setup(this TM_Xml_Database tmDatabase)                 
        {
            UserRole.Admin.demand();
            if (TM_Status.Current.TM_Database_In_Setup_Workflow)
                throw new Exception("TM Exeption: TM_Xml_Database Setup was called twice in a row (without the first Setup sequence had ended)");

            TM_Status.Current.TM_Database_In_Setup_Workflow = true;
            

            try
            {
                tmDatabase.Events.Before_Setup.raise();

                tmDatabase.set_Default_Values();
                          //.set_Path_XmlDatabase()
                          //.tmServer_Load();

                tmDatabase.load_UserData()                          
                          .load_SiteData()
                          .load_Libraries();


               tmDatabase.Events.After_Setup.raise(); 
                
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
            UserGroup.Admin.demand();
            "In Reload data".info();
            tmDatabase.clear_GuidanceItemsCache()                            // start by clearing the cache                                   
                      .setup();                                                                            

            var stats = "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems"
                            .format(tmDatabase.Path_XmlLibraries.directoryName(),
                                    tmDatabase.tmLibraries().size(),
                                    tmDatabase.tmViews().size(),
                                    tmDatabase.tmGuidanceItems().size());
            return stats;                                                   // return some stats
        }

        [Admin] public static string            path_XmlDatabase(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.usingFileStorage())
                return TM_Server.Path_XmlDatabase;
            return null;
        }
       
    }
}
