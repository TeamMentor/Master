using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public static class TM_UserData_FileStorage
    {   
        public static TM_UserData       userData(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.notNull()
                        ? tmFileStorage.UserData
                        : null;
        }
        public static string            path_UserData(this TM_FileStorage tmFileStorage)
        {
            if (tmFileStorage.notNull())
                return tmFileStorage.Path_UserData;
            return null;
        }
        public static string            path_SiteData(this TM_FileStorage tmFileStorage)
        {
            if (tmFileStorage.notNull())
                return tmFileStorage.Path_SiteData;
            return null;
        }
        public static TM_FileStorage    users_Load   (this TM_FileStorage tmFileStorage)
        {            
            var userData = tmFileStorage.UserData;
            if (userData.notNull())
            { 
                userData.TMUsers = new List<TMUser>();

                var usersFolder = tmFileStorage.users_XmlFile_Location();
                if (usersFolder.isNull())
                    "[TM_FileStorage] [users_Load] could not load users because users_XmlFile_Location() returned null".error();                        
                else             
                    foreach (var file in usersFolder.files("*.userData.xml"))
                    {
                        var tmUser = file.load<TMUser>();
                        if (tmUser.notNull() && tmUser.UserID > 0)
                            userData.TMUsers.Add(tmUser);
                        else
                            "[TM_UserData_Ex_Users_Persistance] [users_Load] error loading tmUser file (or UserId < 1): {0}".error(file);
                    }                                                    
            }  
            return tmFileStorage;
        }                
        public static string            users_XmlFile_Location(this TM_FileStorage tmFileStorage)   
        {            
            if (tmFileStorage.path_UserData().notNull())
                return tmFileStorage.path_UserData()
                                    .pathCombine(TMConsts.USERDATA_PATH_USER_XML_FILES)
                                    .createDir();
            return null;
        }        

        [Admin] public static TM_FileStorage   set_Path_UserData    (this TM_FileStorage tmFileStorage)
        {           
            admin.demand();
            var tmXmlDatabase = tmFileStorage.tmXmlDatabase();
            //var userData = tmFileStorage.userData();

            if (tmXmlDatabase.isNull())
                return tmFileStorage;
          
            var userData_Config = tmFileStorage.tmServer().userData_Config();

            if (userData_Config.isNull() || userData_Config.Name.notValid())
            { 
                "[TM_Xml_Database][set_Path_UserData] userData_Config or its name was null or empty, so going to to use the default value of: {0}".error(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
                userData_Config = new TM_Server.Config()
                                    {
                                        Name = TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA
                                    };
            }

            var xmlDatabasePath = tmFileStorage.path_XmlDatabase();                    // all files are relative to this path
                                
            var userDataPath    = xmlDatabasePath.pathCombine(userData_Config.Name); // use the userData_Config.Name as the name of the folder to put UserData files
            
            tmFileStorage.Path_UserData = userDataPath;                             // needed by Git Clone
            "TeamMentor.Git".assembly()
                            .type("TM_UserData_Git_ExtensionMethods")
                            .invokeStatic("setup_UserData_Git_Support", tmFileStorage);


            userDataPath.createDir();                                                // create if needed
            if (userDataPath.dirExists())
            {
                tmFileStorage.Path_UserData = userDataPath.createDir();
                "[TM_Xml_Database] [set_Path_UserData] TMConfig.Current.UserDataPath: {0}".debug(userDataPath);
            }
            else
            {
                tmFileStorage.Path_UserData = null;
                "[TM_Xml_Database] [set_Path_UserData] failed to create the folder: {0}".error(userDataPath);                
            }                                                
            
            return tmFileStorage;
        }
        /// <summary>        
        /// Forces tmFileStorage.Path_UserData to be set to a specific folder 
        /// 
        /// Note: If <code>path_UserData</code> folder doesn't exist, the value is not changed
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <param name="path_UserData"></param>
        /// <returns></returns>
        [Admin] public static TM_FileStorage   set_Path_UserData    (this TM_FileStorage tmFileStorage, string path_UserData)
        {           
            admin.demand();
            if(tmFileStorage.notNull() && path_UserData.folderExists())
            { 
                tmFileStorage.Path_UserData = path_UserData;
                
                "TeamMentor.Git".assembly()
                                .type("TM_UserData_Git_ExtensionMethods")
                                .invokeStatic("setup_UserData_Git_Support", tmFileStorage);
            }
            return tmFileStorage;
        }
        [Admin] public static TM_FileStorage   set_Path_SiteData    (this TM_FileStorage tmFileStorage)
        {           
            UserRole.Admin.demand();
            var siteData_Config = tmFileStorage.tmServer().siteData_Config();

            if (siteData_Config.isNull() || siteData_Config.Name.notValid())
            { 
                "[TM_FileStorage][set_Path_SiteData] set_Path_SiteData or its name was null or empty, so going to to use the default value of: {0}".error(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
                siteData_Config = new TM_Server.Config()
                                    {
                                        Name = TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA
                                    };
            }

            var xmlDatabasePath = tmFileStorage.path_XmlDatabase();                    // all files are relative to this path
                                
            var siteDataPath    = xmlDatabasePath.pathCombine(siteData_Config.Name); // use the userData_Config.Name as the name of the folder to put UserData files
                
            siteDataPath.createDir();                                                // create if needed
            if (siteDataPath.dirExists())
            {
                tmFileStorage.Path_SiteData = siteDataPath.createDir();
                "[TM_FileStorage] [set_Path_SiteData] TMConfig.Current.UserDataPath: {0}".debug(siteDataPath);
            }
            else
            {
                tmFileStorage.Path_SiteData = null;
                "[TM_FileStorage] [set_Path_SiteData] failed to create the folder: {0}".error(siteDataPath);                
            }                                                
            
            return tmFileStorage;
        }
        /// <summary>        
        /// Forces tmFileStorage.Path_UserData to be set to a specific folder 
        /// 
        /// Note: If <code>path_SiteData</code> folder doesn't exist, the value is not changed
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <param name="path_SiteData"></param>
        /// <returns></returns>
        [Admin] public static TM_FileStorage   set_Path_SiteData    (this TM_FileStorage tmFileStorage, string path_SiteData)
        {           
            admin.demand();
            if(tmFileStorage.notNull() && path_SiteData.folderExists())
                tmFileStorage.Path_SiteData = path_SiteData;
            return tmFileStorage;
        }
       
        [Admin] public static TM_FileStorage   load_UserData       (this TM_FileStorage tmFileStorage)         
        {
            UserRole.Admin.demand();                    

         //   "TeamMentor.Git".assembly()
         //                   .type("TM_UserData_Git_ExtensionMethods")
         //                   .invokeStatic("setup_UserData_Git_Support", tmFileStorage);

            tmFileStorage//.tmConfig_Load()                            
                         .secretData_Load()
                         .users_Load();                         
                        
            return tmFileStorage;
        }
    
        //TM_UserData helpers
        /*public static string path_UserData(this TM_UserData userData)
        {
            return userData.usingFileStorage() 
                    ? TM_FileStorage.Current.path_UserData()
                    : null;
        }
        public static string tmConfig_Location(this TM_UserData userData)
        {
            return userData.usingFileStorage() 
                    ? TM_FileStorage.Current.tmConfig_Location()
                    : null;
        }
        public static string secretData_Location(this TM_UserData userData)
        {
            return userData.usingFileStorage() 
                    ? TM_FileStorage.Current.secretData_Location() 
                    : null;
        }
        public static bool   usingFileStorage   (this TM_UserData userData)
        {
            return userData.notNull() && TM_FileStorage.Current.notNull();
        }*/
    }
}
