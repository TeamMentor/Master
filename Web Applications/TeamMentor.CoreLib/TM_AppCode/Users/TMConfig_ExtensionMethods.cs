using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TmConfig_ExtensionMethods
    {		       
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