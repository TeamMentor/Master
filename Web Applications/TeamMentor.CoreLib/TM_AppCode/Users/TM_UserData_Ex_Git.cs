using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Git
    {
        public static TM_UserData   setupGitSupport  (this TM_UserData userData)
        {
            if (userData.UsingFileStorage && userData.Path_UserData.notNull())
            {
                userData.handle_UserData_ConfigActions(); 

                if (userData.AutoGitCommit)
                { 
                    userData.handle_External_GitPull();
                    userData.handle_UserData_ConfigActions();               // run this again in case it was changed from the git pull           
                
                    if (userData.Path_UserData.isGitRepository())
                    {
                        //"[TM_UserData] [setupGitSupport] open repository: {0}".info(userData.Path_UserData);
                        "[TM_UserData] [GitOpen]".info();
                        userData.NGit = userData.Path_UserData.git_Open();                    
                    }
                    else
                    {
                        //"[TM_UserData] [setupGitSupport] initializing repository at: {0}".info(userData.Path_UserData);
                        "[TM_UserData] [GitInit]".info();
                        userData.NGit = userData.Path_UserData.git_Init();
                    }
                    userData.triggerGitCommit();        // in case there are any files that need to be commited                
                }                
            }
            return userData;
        }
        public static TMUser        triggerGitCommit (this TMUser tmUser)
        {
            TM_UserData.Current.triggerGitCommit();
            return tmUser;
        }
        public static TM_UserData   triggerGitCommit (this TM_UserData userData)
        {
            if (MiscUtils.runningOnLocalHost() && TMConfig.Current.getGitUserConfigFile().valid()) //don't commit local changes in order to prevent git merge conflicts
            {
                "[TM_UserData] [triggerGitCommit] skipping because it is a local request and getGitUserConfigFile is set".info();
                return userData;
            }
            if (userData.AutoGitCommit && userData.NGit.notNull())
                if (userData.NGit.status().valid())
                {
                    var start = DateTime.Now;
                    userData.NGit.add_and_Commit_using_Status();
                    "[TM_UserData] [GitCommit] in ".info(start.duration_To_Now());
                }
            return userData;
        }
        public static TM_UserData   pushUserRepository(this TM_UserData userData, API_NGit nGit)
        {
            if (MiscUtils.runningOnLocalHost() && TMConfig.Current.getGitUserConfigFile().valid())  //don't push local changes in order to prevent git merge conflicts            
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
        public static TM_UserData   handle_External_GitPull      (this TM_UserData userData)
        {
            try
            {                
                //var gitLocationFile = HttpContextFactory.Server.MapPath("gitUserData.config");
                var gitLocationFile = TMConfig.Current.getGitUserConfigFile();
                if (gitLocationFile.fileExists())
                {
                    "[TM_UserData] [handleExternalGitPull] found gitConfigFile: {0}".info(gitLocationFile);
                    var gitLocation = gitLocationFile.fileContents();
                    if (gitLocation.notValid())
                        return userData;
                    //if (userData.Path_UserData.dirExists() && userData.Path_UserData.files().empty())                        
                    //    userData.Path_UserData.delete_Folder();

                    //Adjust Path_UserData so that there is an unique folder per repo
                    var extraFolderName = "_Git_";
                    
                        // extra mode to switch of multiple Git_Hosting in same server
                    extraFolderName += gitLocation.replace("\\","/").split("/").last().remove(".git").safeFileName();

                    userData.Path_UserData = userData.Path_UserData_Base + extraFolderName;
                    //userData.Path_UserData.createDir();
                    "[TM_UserData] [handleExternalGitPull] userData.Path_UserData set to: {0}".debug(userData.Path_UserData);
             
                    if (MiscUtils.online().isFalse() && gitLocation.dirExists().isFalse())
                        return userData;

                    if (userData.Path_UserData.isGitRepository())
                    {                        
                        "[TM_UserData] [GitPull]".info();
                        var result = userData.Path_UserData.git_Pull();
                        if(result)
                            userData.pushUserRepository(userData.Path_UserData.git_Open());

                    }
                    else
                    {
                        userData.clone_UserDataRepo(gitLocation, userData.Path_UserData);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData]  [handleExternalGitPull]");
            }
            return userData;
        }
    
        public static TM_UserData   clone_UserDataRepo      (this TM_UserData userData, string gitLocation, string targetFolder)
        {
            var start = DateTime.Now;
            "[TM_UserData] [GitClone] Start".info();
            if (Git.CloneUsingGit(gitLocation,targetFolder).isFalse())
            {
                "[TM_UserData] [GitClone] Using NGit for the clone".info();    
                gitLocation.git_Clone(targetFolder);
            }
            "\n\n[TM_UserData] [GitClone] in  {0}\n\n".debug(start.duration_To_Now());
            return userData;
        }
    }
}