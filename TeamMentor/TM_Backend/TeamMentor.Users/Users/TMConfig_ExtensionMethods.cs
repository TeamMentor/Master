using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TMConfig_ExtensionMethods
    {
        public static string tmConfig_Location(this TM_UserData userData)
        {
            return (userData.notNull())
                        ? userData.Path_UserData.pathCombine(TMConsts.TM_CONFIG_FILENAME)
                        : null;
        } 
        /*public static TM_Xml_Database tmConfig_Load(this TM_Xml_Database tmXmlDatabase)
        {
            tmXmlDatabase.userData().tmConfig_Load();
            return tmXmlDatabase;
        }*/

        public static TM_UserData tmConfig_Load(this TM_UserData userData)
        {
            if (userData.isNull())
                return null;
            try
            { 
                userData.Events.Before_TM_Config_Load.raise();
                var userConfigFile = userData.tmConfig_Location();
                if (userConfigFile.fileExists())
                {
                    var newConfig = userConfigFile.load<TMConfig>();    // to check that the new TMConfig is not corrupted
                    if (newConfig.isNull())
                    {
                        "[handleUserDataConfigActions] failed to load config file from: {0}".error(userConfigFile);
                        return null;
                    }
               
                    TMConfig.Current = newConfig;
                    return userData;            
                }

                // if userConfigFile doesn't exist, create one and save it 
                TMConfig.Current = new TMConfig();
                userData.tmConfig_Save();     
                        
                return userData;
            }
            finally
            {
                userData.Events.After_TM_Config_Load.raise();
            }
        }
        public static TMConfig tmConfig_Reload(this TM_UserData userData)
        {
            TMConfig.Current = userData.tmConfig_Location().fileExists()
                                    ? userData.tmConfig_Location().load<TMConfig>()
                                    : new TMConfig();
            return TMConfig.Current;
        }
        public static bool tmConfig_Save(this TM_UserData userData)
        {
            var tmConfig = TMConfig.Current;
            var location = userData.tmConfig_Location();
            return  (tmConfig.notNull() && location.valid())
                        ? tmConfig.saveAs(location)
                        : false;
        }
        public static bool tmConfig_SetCurrent(this TM_UserData userData, TMConfig tmConfig)
        {
            if (userData.isNull() || tmConfig.isNull())
                return false;
            TMConfig.Current = tmConfig;
            return userData.tmConfig_Save();            
        }
        public static DateTime  currentExpirationDate(this TMConfig tmConfig)
        {
            return (tmConfig.TMSecurity.EvalAccounts_Enabled)
                       ? DateTime.Now.AddDays(tmConfig.TMSecurity.EvalAccounts_Days)
                       : default(DateTime);
        }
        public static bool  newAccountsEnabled(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.TMSecurity.notNull())
                return tmConfig.TMSecurity.NewAccounts_Enabled;
            return false;
        }
        public static bool  emailAdminOnNewUsers(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.TMSecurity.notNull())
                return tmConfig.TMSecurity.EmailAdmin_On_NewUsers;
            return false;
        }
        public static bool  windowsAuth(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.TMSecurity.notNull())
                return tmConfig.WindowsAuthentication.Enabled;
            return false;
        }
        
    }
}