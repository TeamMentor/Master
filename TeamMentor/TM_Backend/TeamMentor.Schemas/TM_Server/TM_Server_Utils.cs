using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Server_Utils
    {        
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
        
    }
}