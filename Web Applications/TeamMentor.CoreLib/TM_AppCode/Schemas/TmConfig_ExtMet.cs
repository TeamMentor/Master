using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TmConfig_ExtMet
    {		       
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
            return tmConfig.librarDataRootFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
        }
        public static string    librarDataRootFolder      (this TMConfig tmConfig)
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

        
    }
}