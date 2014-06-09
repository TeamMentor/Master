using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage.XmlDatabase;

namespace TeamMentor.FileStorage
{
    public static class TM_Xml_Database_FileStorage
    {
        public static string                    path_XmlDatabase(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.notNull() ? tmFileStorage.Path_XmlDatabase : null;
        }        
        [Admin] public static string            path_XmlDatabase(this TM_Xml_Database tmDatabase)
        {
            return TM_FileStorage.Current.path_XmlDatabase();            
        }
        [Admin] public static string            path_XmlLibraries(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.notNull() ? tmFileStorage.Path_XmlLibraries : null;
        }
        public static string                             path_XmlLibraries(this TM_Xml_Database tmXmlDatabase)
        {
            return TM_FileStorage.Current.notNull() ?  TM_FileStorage.Current.Path_XmlLibraries : null;
        }
        
        [Admin] public static TM_Xml_Database   tmXmlDatabase(this TM_FileStorage tmFileStorage)
        {
            return (tmFileStorage.notNull())
                        ? tmFileStorage.TMXmlDatabase
                        : null;
        }
        [Admin] public static TM_FileStorage    set_Path_XmlLibraries(this TM_FileStorage tmFileStorage)
        {
            var tmXmlDatabase = tmFileStorage.tmXmlDatabase();
            if (tmXmlDatabase.isNull())
                return tmFileStorage;
            try
            {
                var tmConfig        = TMConfig.Current;
                var xmlDatabasePath = tmFileStorage.path_XmlDatabase();  // tmConfig.xmlDatabasePath();
                var libraryPath     = tmConfig.TMSetup.XmlLibrariesPath;

                "[TM_Xml_Database] [SetPaths_XmlDatabase] TM_Xml_Database.path_XmlDatabase(): {0}".debug(xmlDatabasePath);
                "[TM_Xml_Database] [SetPaths_XmlDatabase] TMConfig.Current.XmlLibrariesPath: {0}".debug(libraryPath);


                if (libraryPath.dirExists().isFalse())
                {
                    libraryPath = xmlDatabasePath.pathCombine(libraryPath);
                    libraryPath.createDir();  // make sure it exists
                }
                
                tmFileStorage.Path_XmlLibraries = libraryPath;
                "[TM_Xml_Database] [SetPaths_XmlDatabase] tmXmlDatabase.Path_XmlLibraries = {0}".info(libraryPath);
                "[TM_Xml_Database] Paths configured".info();
            }
            catch (Exception ex)
            {
                "[TM_Xml_Database] [set_Path_XmlLibraries]: {0} \n\n {1}".error(ex.Message, ex.StackTrace);
            }
            tmXmlDatabase.Events.After_Set_Path_XmlLibraries.raise();   
            return tmFileStorage;
        }

        [Admin] public static TM_FileStorage    load_Libraries(this TM_FileStorage tmFileStorage)        
        {
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            tmFileStorage.loadDataIntoMemory();

            tmXmlDatabase.Events.After_Load_Libraries.raise();

            return tmFileStorage;
        }
   
        [Admin] public static string            stats(this TM_Xml_Database tmDatabase)
        {
            var tmFileStorage = TM_FileStorage.Current;
            var stats = "In the Folder '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems"
                            .format(tmFileStorage.Path_XmlLibraries.directoryName(),
                                    tmDatabase.tmLibraries().size(),
                                    tmDatabase.tmViews().size(),
                                    tmDatabase.tmGuidanceItems().size());
            return stats;                                                
        }
        [Admin] public static string            reloadData(this TM_Xml_Database tmDatabase)               
        {
            UserGroup.Admin.demand();
            "In Reload data".info();
            tmDatabase.clear_GuidanceItemsCache()                            // start by clearing the cache                                   
                      .setup();                                                                            
            return tmDatabase.stats();
               // return some stats
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

                /*tmDatabase.load_UserData()                          
                          .load_SiteData()
                          .load_Libraries();
                */

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
    
        public static bool   usingFileStorage   (this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.notNull() && TM_FileStorage.Current.notNull();
        }
    }
}
