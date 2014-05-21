using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex
    {
        public static TMConfig tmConfig(this TM_UserData userData)
        {
            return TMConfig.Current;
        }
        public static TM_UserData resetData             (this TM_UserData userData)          
        {
            userData.NGit_Author_Name    = TMConsts.NGIT_DEFAULT_AUTHOR_NAME;
            userData.NGit_Author_Email   = TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL;
            userData.FirstScriptToInvoke = TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE;
            userData.Path_WebRootFiles   = TMConsts.USERDATA_PATH_WEB_ROOT_FILES;
            userData.TMUsers             = new List<TMUser>();                        
            userData.SecretData          = new TM_SecretData();                        
            return userData;
        }
        public static TM_UserData SetUp                 (this TM_UserData userData)  
        {
            try
            {
                userData.setupGitSupportAndLoadTMConfigFile();                
                userData.firstScript_Invoke();                
                userData.SecretData = userData.secretData_Load();
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] [SetUp]");
            }            
            return userData;
        }
        public static TM_UserData ReloadData            (this TM_UserData userData)  
        {
            userData.SetUp();
            userData.loadTmUserData();
            userData.createDefaultAdminUser();  // make sure the admin user exists and is configured
            return userData;
        }
        public static string      webRootFiles          (this TM_UserData userData)  
        {
            if (userData.notNull() && 
                userData.Path_UserData.valid() && 
                userData.Path_UserData.dirExists() && 
                userData.Path_WebRootFiles.valid() &&
                userData.UsingFileStorage)
            {
                return userData.Path_UserData.pathCombine(userData.Path_WebRootFiles);
            }
            return null;
        }
        public static bool        copy_FilesIntoWebRoot (this TM_UserData userData)  
            {            
                var sourceFolder = userData.webRootFiles();
                if (sourceFolder.notValid())
                    return false;                
                if (sourceFolder.dirExists().isFalse())                                    
                    return false;
                "[TM_UserData] [copy_FilesIntoWebRoot] sourceFolder was found: {0}".debug(sourceFolder);
                var targetFolder = TM_Server.WebRoot;
                if (targetFolder.pathCombine("web.config").fileExists().isFalse())
                {
                    "[TM_UserData] [copy_FilesIntoWebRoot] failed because web.config was not found on targetFolder: {0}".error(targetFolder);
                    return false;
                }
                "[TM_UserData] [copy_FilesIntoWebRoot] target Folder with web.config was found: {0}".debug(targetFolder);    
                Files.copyFolder(sourceFolder, targetFolder,true,true,"");            
                return true;
            }
    }
}
