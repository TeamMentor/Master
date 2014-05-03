using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TmConfig_ExtensionMethods
    {		
        /*    public static TMConfig  setDefaultValues    (this TMConfig tmConfig)
        {
            tmConfig.TMSetup = new TMConfig.TMSetup_Config
                {
                    TMLibraryDataVirtualPath    = "..\\..",
                    XmlLibrariesPath            = "TM_Libraries",
                    UserDataPath                = "User_Data",
                    LibrariesUploadedFiles      = "LibrariesUploadedFiles",
                    Enable302Redirects          = true,
                    EnableGZipForWebServices    = true
                };

            tmConfig.TMSecurity = new TMConfig.TMSecurity_Config
                {
                    Show_ContentToAnonymousUsers = false,
                    SSL_RedirectHttpToHttps      = true,
                    EvalAccounts_Enabled         = true,
                    EvalAccounts_Days            = 15,
                    Default_AdminUserName        = "admin",
                    Default_AdminPassword        = "!!tmadmin",
                    Default_AdminEmail           = "tm_alerts@securityinnovation.com"
                };
            

            tmConfig.WindowsAuthentication = new TMConfig.WindowsAuthentication_Config
                {
                    Enabled               = false,
                    ReaderGroup           = "TM_Reader",
                    EditorGroup           = "TM_Editor",
                    AdminGroup            = "TM_Admin"
                };            

            tmConfig.OnInstallation = new TMConfig.OnInstallation_Config
                {
                    ForceDefaultAdminPassword          = false,
                    DefaultLibraryToInstall_Name     = "",
                    DefaultLibraryToInstall_Location = ""
                };
            tmConfig.Git            = new TMConfig.Git_Config
                {
                    AutoCommit_LibraryData          = false,            // disabled by default
                    AutoCommit_UserData             = true
                };
            
            return tmConfig;	
        }*/
        public static string    virtualPathMapping  (this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.TMLibraryDataVirtualPath.valid())
                return tmConfig.TMSetup.TMLibraryDataVirtualPath;
            return TMConsts.VIRTUAL_PATH_MAPPING;
        }
        public static string    xmlDatabasePath     (this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.UseAppDataFolder)
                return TMConfig.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH);
            return tmConfig.rootDataFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
        }
        public static string    rootDataFolder      (this TMConfig tmConfig)
        {									
            //set xmlDatabasePath based on virtualPathMapping			
            var virtualPathMapping = tmConfig.virtualPathMapping();			
            var xmlDatabasePath = TMConfig.WebRoot.pathCombine(virtualPathMapping).fullPath();

            //check if we can write to xmlDatabasePath (and default to App_Data if we can't write to provided direct)
            if (xmlDatabasePath.canNotWriteToPath())
                xmlDatabasePath = TMConfig.AppData_Folder; 

            return xmlDatabasePath;
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