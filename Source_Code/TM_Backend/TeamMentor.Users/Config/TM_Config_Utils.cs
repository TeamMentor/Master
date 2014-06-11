using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Config_Utils
    {
        
        public static bool tmConfig_SetCurrent(this TM_UserData userData, TMConfig tmConfig)
        {
            if (userData.isNull() || tmConfig.isNull())
                return false;
            TMConfig.Current = tmConfig;
            return userData.event_TM_Config_Changed();   //return userData.tmConfig_Save();            
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
        public static bool  windowsAuthentication_Enabled(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.WindowsAuthentication.notNull())
                return tmConfig.WindowsAuthentication.Enabled;                               
            return false;
        }

        public static bool  show_ContentToAnonymousUsers(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.TMSecurity.notNull())
                return tmConfig.TMSecurity.Show_ContentToAnonymousUsers;                               
            return false;
        }
        public static bool  show_LibraryToAnonymousUsers(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.TMSecurity.notNull())
                return tmConfig.TMSecurity.Show_LibraryToAnonymousUsers;                               
            return false;
        }
        public static bool  enableGZipForWebServices(this TMConfig tmConfig)
        {
            if (tmConfig.notNull() && tmConfig.TMSetup.notNull())
                return tmConfig.TMSetup.EnableGZipForWebServices;                               
            return false;
        }        
        //
        
    }
}