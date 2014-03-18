using System;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;
using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public static class TM_NGit_Ex
    {
        public static API_NGit setDefaultAuthor(this API_NGit nGit)
        {
            try
            {
                var userData    = TM_UserData.Current;
                var name =      userData.NGit_Author_Name.valid() ? userData.NGit_Author_Name : "tm-bot";
                var email =      userData.NGit_Author_Email.valid() ? userData.NGit_Author_Email : "tm-bot@teammentor.net";
                nGit.Author     = name.personIdent(email);
                nGit.Committer  = "tm-bot ".personIdent("tm-bot@teammentor.net");
            }
            catch(Exception ex)
            {
                ex.log();
            }
            return nGit;
        }
    }
    public static class TM_UserData_Ex_Git
    {
        public static TM_UserData   setupGitSupportAndLoadTMConfigFile(this TM_UserData userData)
        {            
            if (userData.UsingFileStorage && userData.Path_UserData.notNull())
            {
                userData.load_UserData_FromGitRepo();               // will create the custom UserData repo clone (if mapped)
                userData.load_TMConfigFile();                       // loads the user configured TMConfig.Config file (from either the default or the custom UserData folder)

                var gitEnabled = userData.tmConfig().Git.UserData_Git_Enabled;                

                if (gitEnabled)
                {                                                         
                    if (userData.Path_UserData.isGitRepository())
                    {
                        //"[TM_UserData][setupGitSupport] open repository: {0}".info(userData.Path_UserData);
                        "[TM_UserData][GitOpen]".info();
                        userData.NGit = userData.Path_UserData.git_Open();                    
                    }
                    else
                    {
                        //"[TM_UserData][setupGitSupport] initializing repository at: {0}".info(userData.Path_UserData);
                        "[TM_UserData][GitInit]".info();
                        userData.NGit = userData.Path_UserData.git_Init();
                    }
                    userData.triggerGitCommit();        // in case there are any files that need to be commited                
                }                
            }
            return userData;
        }
        public static TMUser        triggerGitCommit                  (this TMUser tmUser)
        {
            TM_UserData.Current.triggerGitCommit();
            return tmUser;
        }
        public static TM_UserData   triggerGitCommit                  (this TM_UserData userData)
        {
            if (userData.tmConfig().Git.UserData_Git_Enabled && userData.NGit.notNull())
                if (userData.NGit.status().valid())
                {
                    var start = DateTime.Now;
                    userData.NGit.setDefaultAuthor();
                    userData.NGit.add_and_Commit_using_Status();
                    "[TM_UserData][GitCommit] in ".info(start.duration_To_Now());
                }
            return userData;
        }
        public static TM_UserData   pushUserRepository                (this TM_UserData userData, API_NGit nGit)
        {
            if (userData.tmConfig().Git.UserData_Auto_Push.isFalse())           //skip if this is set
                return userData;

            if (MiscUtils.runningOnLocalHost())  //don't push local changes in order to prevent git merge conflicts            
            {
                "[triggerGitCommit] skipping because it is a local request and getGitUserConfigFile is set".info();
                return userData;
            }
            TM_UserData.GitPushThread = O2Thread.mtaThread(
                ()=>{                                            
                        var start = DateTime.Now;
                        "[TM_UserData][GitPush] Start".info();
                        nGit.push();
                        "[TM_UserData][GitPush] in ".info(start.duration_To_Now());
                    });
            return userData;
        }
        public static TM_UserData   load_UserData_FromGitRepo         (this TM_UserData userData)
        {
            try
            {
                var gitConfig = userData.tmConfig().Git;
                if (gitConfig.UserData_Git_Enabled.isFalse())
                    return userData;

                var gitLocation = TM_Xml_Database.Current.TM_Server_Config.getActive_UserData_Repo_GitPath();
                if (gitLocation.valid())
                {                       
                    //Adjust Path_UserData so that there is an unique folder per repo
                    var extraFolderName = "_Git_";
                    
                    // extra mode to switch of multiple Git_Hosting in same server
                    extraFolderName += gitLocation.replace("\\","/").split("/").last().remove(".git").safeFileName();

                    userData.Path_UserData = userData.Path_UserData_Base + extraFolderName;
                    //userData.Path_UserData.createDir();
                    "[handleExternalGitPull] userData.Path_UserData set to: {0}".debug(userData.Path_UserData);
             
                    if (MiscUtils.online().isFalse())
                        return userData;

                    if (userData.Path_UserData.isGitRepository())
                    {
                        if (gitConfig.UserData_Auto_Pull.isFalse())     //skip if this is set     
                            return userData;

                        "[TM_UserData][GitPull]".info();
                        var nGit = userData.Path_UserData.git_Open();
                        nGit.pull();
                        //var nGit = userData.Path_UserData.git_Pull();
                        userData.pushUserRepository(nGit);
                    }
                    else
                    {
                        var start = DateTime.Now;
                        "[TM_UserData][GitClone] Start".info();
                        gitLocation.git_Clone(userData.Path_UserData);
                        "[TM_UserData][GitClone] in ".info(start.duration_To_Now());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("[handleExternalGitPull]");
            }
            return userData;
        }
    }
}