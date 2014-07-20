using System;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;
using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;
using FluentSharp.Web;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Git_ExtensionMethods
    {        
        public static TM_UserData_Git   setup_UserData_Git_Support(this TM_FileStorage tmFileStorage)             
        {
            if(tmFileStorage.userData().notNull())
            {
                var userDataGit =  new TM_UserData_Git(tmFileStorage);
                userDataGit.syncWithGit();
                return userDataGit;
            }
            return null;
        }

        public static TM_UserData       userData                 (this TM_UserData_Git userDataGit)              
        {
            if (userDataGit.notNull())
                return userDataGit.UserData;
            return null;
        }
        /*public static TMUser            triggerGitCommit         (this TMUser tmUser)                            
        {
            TM_UserData_Git.Current.triggerGitCommit();
            return tmUser;
        }*/
        public static TM_UserData_Git   triggerGitCommit         (this TM_UserData_Git userData)                 
        {
            var tmFileStorage = userData.FileStorage;
            var tmServer = tmFileStorage.Server;
            if (tmServer.notNull())
                if (tmServer.Git.UserData_Git_Enabled && userData.NGit.notNull())
                    if (userData.NGit.status().valid())
                    {
                        var start = DateTime.Now;
                        userData.NGit.setDefaultAuthor();
                        userData.NGit.add_and_Commit_using_Status();
                        "[TM_UserData] [GitCommit] in ".info(start.duration_To_Now());
                    }
            return userData;
        }
        public static TM_UserData_Git   pushUserRepository       (this TM_UserData_Git userData, API_NGit nGit)  
        {
            var tmServer = userData.FileStorage.tmServer();

            if (tmServer.isNull())
                return userData;

            if (tmServer.Git.UserData_Auto_Push.isFalse())           //skip if this is set
                return userData;

            if (WebUtils.runningOnLocalHost())  //don't push local changes in order to prevent git merge conflicts            
            {
                "[TM_UserData] [triggerGitCommit] skipping because it is a local request and getGitUserConfigFile is set".info();
                return userData;
            }
            TM_UserData.GitPushThread = O2Thread.mtaThread(
                ()=>{                                            
                        var start = DateTime.Now;
                        "[TM_UserData] [GitPush] Start".info();
                        nGit.push();
                        "[TM_UserData] [GitPush] in ".info(start.duration_To_Now());
                    });
            return userData;
        }
        public static TM_UserData_Git   syncWithGit              (this TM_UserData_Git userDataGit)                 
        {
            try
            {
                var tmFileStorage = userDataGit.FileStorage;

                if (userDataGit.userData().isNull())
                    return userDataGit;

                //var userData = userDataGit.userData();
             //   var gitConfig = userData.tmConfig().Git;
               // if (gitConfig.UserData_Git_Enabled.isFalse())
               //     return userData;

                var userData_Config = tmFileStorage.Server.userData_Config();

                var gitLocation = userData_Config.Remote_GitPath;
                if (gitLocation.valid())
                {             
                    
                    //Adjust Path_UserData so that there is an unique folder per repo
                    var extraFolderName = "_Git_";
                    
                    // extra mode to switch of multiple Git_Hosting in same server
                    extraFolderName += gitLocation.replace("\\","/").split("/").last().remove(".git").safeFileName();

                    //userData.Path_UserData = userData.Path_UserData_Base + extraFolderName;
                    //userData.Path_UserData.createDir();
                    "[TM_UserData] [handleExternalGitPull] userData.Path_UserData set to: {0}".debug(tmFileStorage.Path_UserData);
             
                    if (WebUtils.online().isFalse() && gitLocation.dirExists().isFalse())
                        return userDataGit;

                    if (tmFileStorage.Path_UserData.isGitRepository())
                    {
                     //   if (gitConfig.UserData_Auto_Pull.isFalse())     //skip if this is set     
                     //       return userData;

                        "[TM_UserData] [GitPull]".info();
                        var nGit = tmFileStorage.Path_UserData.git_Open();
                        nGit.pull();
                        //var nGit = userData.Path_UserData.git_Pull();
                        userDataGit.pushUserRepository(nGit);
                    }
                    else
                    {
                        userDataGit.clone_UserDataRepo(gitLocation, tmFileStorage.Path_UserData);
                    }
                }
                if (tmFileStorage.Path_UserData.notNull())
                {                    

                    //var gitEnabled = userData.tmConfig().Git.UserData_Git_Enabled;                

                    //                if (gitEnabled)
                    //               {                                                         
                    if (tmFileStorage.Path_UserData.isGitRepository())
                    {
                        //"[TM_UserData] [setupGitSupport] open repository: {0}".info(userData.Path_UserData);
                        "[TM_UserData] [GitOpen]".info();
                        userDataGit.NGit = tmFileStorage.Path_UserData.git_Open();
                    }
                    else
                    {
                        //"[TM_UserData] [setupGitSupport] initializing repository at: {0}".info(userData.Path_UserData);
                        "[TM_UserData] [GitInit]".info();
                        userDataGit.NGit = tmFileStorage.Path_UserData.git_Init();
                    }
                    userDataGit.triggerGitCommit();        // in case there are any files that need to be commited                
                    //                }                
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData]  [handleExternalGitPull]");
            }
            return userDataGit;
        }    
        public static TM_UserData_Git   clone_UserDataRepo       (this TM_UserData_Git userData, string gitLocation, string targetFolder)
        {
            var start = DateTime.Now;
            "[TM_UserData] [GitClone] Start".info();
            if (GitExe.CloneUsingGitExe(gitLocation,targetFolder).isFalse())
            {
                "[TM_UserData] [GitClone] Using NGit for the clone".info();    
                gitLocation.git_Clone(targetFolder);
            }
            "\n\n[TM_UserData] [GitClone] in  {0}\n\n".debug(start.duration_To_Now());
            return userData;
        }
    }
}