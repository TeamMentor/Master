using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex
    {    
        public static TM_UserData ResetData     (this TM_UserData userData)  
        {
            userData.FirstScriptToInvoke = TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE;
            userData.Path_WebRootFiles   = TMConsts.USERDATA_PATH_WEB_ROOT_FILES;
            userData.TMUsers             = new List<TMUser>();                        
            userData.SecretData          = new TM_SecretData();
            userData.AutoGitCommit       = TMConfig.Current.Git.AutoCommit_UserData;           
            return userData;
        }
        public static TM_UserData SetUp         (this TM_UserData userData)  
        {
            try
            {
                userData.setupGitSupport();
                userData.firstScript_Invoke();                
                userData.SecretData = userData.secretData_Load();
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData][SetUp]");
            }            
            return userData;
        }
        public static TM_UserData ReloadData    (this TM_UserData userData)  
        {
            userData.SetUp();
            userData.loadTmUserData();
            userData.createDefaultAdminUser();  // make sure the admin user exists and is configured
            return userData;
        }
        public static string      webRootFiles  (this TM_UserData userData)  
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
    }
}
