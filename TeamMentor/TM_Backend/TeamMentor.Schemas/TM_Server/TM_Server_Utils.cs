using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Server_Utils
    {
        //load and save
        [Admin] public static TM_Server     set_Path_XmlDatabase(this TM_Server tmServer)  
        {
            var webRoot  = TM_Server.WebRoot;
            var tmStatus = TM_Status.Current;
            try
            { 
                if (tmServer.isNull())
                    return null;

                TM_Server.Path_XmlDatabase = null;                

                if (TM_Server.UseFileStorage.isFalse())
                    return tmServer;
                
                // try to find a local folder to hold the TM Database data
            
                

                
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
                        TM_Server.Path_XmlDatabase              = xmlDatabasePath;           // if can write it then make it the Path_XmlDatabase
                        tmStatus.TM_Database_Location_Using_AppData = false;                                                
                        return tmServer;
                    }
                    "[TM_Server][set_Path_XmlDatabase] It was not possible to write to mapped folder: {0}".error(xmlDatabasePath);
                }
                
                var appData_Path = TM_Server.WebRoot.pathCombine("App_Data")
                                                    .pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH)        // inside App_Data we can use the folder value 'TeamMentor' 
                                                    .fullPath();   
                if (appData_Path.createDir().dirExists() && appData_Path.canWriteToPath())
                {
                    TM_Server.Path_XmlDatabase                     = appData_Path;           // if can write it then make it the Path_XmlDatabase
                    tmStatus.TM_Database_Location_Using_AppData = true;                    
                    return tmServer;       
                }   
                           
                "[TM_Server][set_Path_XmlDatabase] It was not possible to write to App_Data folder: {0}".error(appData_Path);                
                "[TM_Server][set_Path_XmlDatabase] Disabled use of UsingFileStorage".debug();
                TM_Server.UseFileStorage = false;                 
                return tmServer;
            }
            finally
            {
                "[TM_Server][set_Path_XmlDatabase] Path_XmlDatabase set to: {0}".info(TM_Server.Path_XmlDatabase);
                "[TM_Server][set_Path_XmlDatabase] tmStatus.TM_Database_Location_Using_AppData:{0}".info(tmStatus.TM_Database_Location_Using_AppData);                
            }
        } 
        [Admin] public static string        tmServer_Location(this TM_Server tmServer)          
        {
            return (tmServer.notNull() && TM_Server.UseFileStorage)
                        ? TM_Server.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME)
                        : null;
        }
       /* [Admin] public static TM_Server     tmServer_Load(this TM_Server tmServer)    
        {
            
        }*/
        [Admin] public static bool          tmServer_Save(this TM_Server tmServer)     
        {                                   
            if (tmServer.notNull() && TM_Server.UseFileStorage)
            {
                var tmServerFile = tmServer.tmServer_Location();
                if (tmServerFile.valid())
                    return tmServer.saveAs(tmServerFile);
            }                            
            return false;
        }
        public static TM_Server             setDefaultData(this TM_Server tmServer)
        {
            var userData_Config = new TM_Server.Config
            {
                Name = TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA,
                Active = true,
                Use_FileSystem = false,
                Enable_Git_Support = false
            };
            var siteData_Config = new TM_Server.Config
            {
                Name = TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA,
                Active = true,
                Use_FileSystem = false,
                Enable_Git_Support = false
            };            
            tmServer.add_UserData(userData_Config);
            tmServer.add_SiteData(siteData_Config);
            return tmServer;
        }
        
        //user data

        public static TM_Server             add_UserData(this TM_Server tmServer, TM_Server.Config config)
        {
            if (tmServer.notNull() && config.notNull())
                tmServer.UserData_Configs.add_Config(config);
            return tmServer;
        }                
        public static TM_Server.Config      userData_Config(this TM_Server tmServer)
        {
            return (tmServer.notNull())
                        ? tmServer.UserData_Configs.where(config => config.Active).first()
                        : null;
        }
        public static TM_Server.Config      userData_Config(this TM_Server tmServer, string name)
        {
            return (tmServer.notNull())
                        ? tmServer.UserData_Configs.config(name)
                        : null;
        }
               
        public static TM_Server             active_UserData(this TM_Server tmServer, string name)
        {
            return tmServer.active_UserData(tmServer.userData_Config(name));
        }
        public static TM_Server             active_UserData(this TM_Server tmServer, TM_Server.Config config)
        {
            if (tmServer.isNull() || config.isNull())
                return null;
            tmServer.UserData_Configs.active_Config(config);
            return tmServer;
        }
        

        //site data
        public static TM_Server             add_SiteData(this TM_Server tmServer, TM_Server.Config config)
        {
            if (tmServer.notNull() && config.notNull())
                tmServer.SiteData_Configs.add_Config(config);
            return tmServer;
        }
        public static TM_Server.Config      siteData_Config(this TM_Server tmServer)
        {
            return (tmServer.notNull())
                        ? tmServer.SiteData_Configs.where(config => config.Active).first()
                        : null;
        }
        public static TM_Server.Config      siteData_Config(this TM_Server tmServer, string name)
        {
            return (tmServer.notNull())
                        ? tmServer.SiteData_Configs.config(name)
                        : null;
        }
        public static TM_Server             active_SiteData(this TM_Server tmServer, string name)
        {
            return tmServer.active_SiteData(tmServer.userData_Config(name));
        }
        public static TM_Server             active_SiteData(this TM_Server tmServer, TM_Server.Config config)
        {
            if (tmServer.isNull() || config.isNull())
                return null;
            tmServer.SiteData_Configs.active_Config(config);
            return tmServer;
        }

        //config
        public static TM_Server.Config       config(this List<TM_Server.Config> configs, string name)
        {
            return configs.where(config => config.Name == name).first();
        }
        public static List<TM_Server.Config> add_Config(this List<TM_Server.Config> configs, TM_Server.Config config)
        {
            if (configs.notNull() && config.notNull())
            {
                var existingConfig = configs.config(config.Name);
                if (existingConfig.notNull())                                    // if it already exists, remove it (before adding)
                    configs.remove(existingConfig);
                configs.add(config);
            }
            return configs;
        }
        public static List<TM_Server.Config> active_Config(this List<TM_Server.Config> configs, TM_Server.Config config)
        {
            if (configs.notNull() && config.notNull())
            {
                configs.ForEach(_config => _config.Active = false);
                config.Active = true;
            }
            return configs;
        }


        //Xml Database 
        public static TM_Server tmServer(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.notNull())
                return tmDatabase.Server;
            return null;
        }
    }
}