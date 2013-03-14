using System;
using System.Collections.Generic;
using Microsoft.Security.Application;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Users_Persistance
    {
        public static bool setUserDataPath(this TM_UserData userData, string userDataPath)
        {
            if (userDataPath.dirExists().isFalse())
            {
                "[TM_UserData][setUserDataPath] provided userDataPath didn't exist: {0}".error("userDataPath");
                return false;
            }
            try
            {
                userData.Path_UserData = userDataPath;
                userData.ResetData();                
                userData.loadTmUserData();
                userData.SetUp();
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData][setUserDataPath]");
                return false;
            }            
        }

        public static TM_UserData   loadTmUserData   (this TM_UserData userData)
        {
            userData.TMUsers = new List<TMUser>();	        
            if (userData.Path_UserData.dirExists().isFalse())
            {
                "[TM_UserData_Ex_Users_Persistance] in loadTmUserObjects, provided userDataPath didn't exist: {0}"
                    .error(userData.Path_UserData);
            }
            else
            {	            
                foreach (var file in userData.Path_UserData.files("*.userData.xml"))
                {
                    var tmUser = file.load<TMUser>();
                    if (tmUser.notNull() && tmUser.UserID > 0)
                        userData.TMUsers.Add(tmUser);
                }
            }
            return userData;
        }                       
        public static string        getTmUserXmlFile (this TMUser tmUser)
        {
            return TM_UserData.Current.Path_UserData.pathCombine("{0}.userData.xml".format(tmUser.ID));
        }
        public static TMUser        saveTmUser       (this TMUser tmUser)
        {
            if (TM_UserData.Current.UsingFileStorage)
            {                
                lock (tmUser)
                {                    
                    tmUser.saveAs(tmUser.getTmUserXmlFile());
                    tmUser.triggerGitCommit();
                }
            }
            return tmUser;
        }
        public static bool          deleteTmUser     (this TM_UserData userData, TMUser tmUser)
        {    		
            if (tmUser.notNull())
            {
                userData.TMUsers.remove(tmUser);
                if (userData.UsingFileStorage)
                {
                    lock (tmUser)
                    {
                        tmUser.getTmUserXmlFile().file_Delete();
                        userData.triggerGitCommit();
                    }
                }
                return true;
            }
            return false;
        }
        public static bool          updateTmUser     (this TMUser tmUser, string userName, string firstname, string lastname, string title, string company, string email, string country, string state, bool passwordExpired, bool userEnabled, int groupId)
        {                         
            if (tmUser.isNull())
                return false;
            if (tmUser.UserName == userName)
            {
                tmUser.EMail = Encoder.XmlEncode(email);
                tmUser.UserName = Encoder.XmlEncode(userName);
                tmUser.FirstName = Encoder.XmlEncode(firstname);
                tmUser.LastName = Encoder.XmlEncode(lastname);
                tmUser.Title = Encoder.XmlEncode(title);
                tmUser.Company = Encoder.XmlEncode(company);
                tmUser.Country = Encoder.XmlEncode(country);
                tmUser.State = Encoder.XmlEncode(state);
                tmUser.GroupID = groupId > -1 ? groupId : tmUser.GroupID;
                tmUser.AccountStatus.PasswordExpired = passwordExpired;
                tmUser.AccountStatus.UserEnabled = userEnabled;
                tmUser.saveTmUser();
                return true;
            }
            
            "[updateTmUser] provided username didn't match provided tmUser".error();
            return false;
        }

        public static TM_UserData handleExternalGitPull      (this TM_UserData userData)
        {
            try
            {
                "[TM_UserData][handleExternalGitPull]".info();
                //var gitLocationFile = HttpContextFactory.Server.MapPath("gitUserData.config");
                var gitLocationFile = TMConfig.BaseFolder.pathCombine("gitUserData.config");
                if (gitLocationFile.fileExists())
                {
                    if (userData.Path_UserData.dirExists() && userData.Path_UserData.files().empty())                        
                        userData.Path_UserData.delete_Folder();
                    if (userData.Path_UserData.ends("_Git").isFalse())
                    {
                        userData.Path_UserData += "_Git";
                        userData.Path_UserData.createDir();
                    }
                    var gitLocation = gitLocationFile.fileContents();
                    if (userData.Path_UserData.isGitRepository())
                    {                        
                        "[TM_UserData][GitPull]".info();
                        var nGit = userData.Path_UserData.git_Pull();
                        O2Thread.mtaThread(
                            ()=>{
                                    "[TM_UserData][GitPush] Start".info();
                                    nGit.push();
                                    "[TM_UserData][GitPush] End".info();
                                });


                    }
                    else
                    {                        
                        gitLocation.git_Clone(userData.Path_UserData);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("[handleExternalGitPull]");
            }
            return userData;
        }

        public static TM_UserData handleUserDataConfigActions(this TM_UserData userData)
        {
            var userConfigFile = userData.Path_UserData.pathCombine("TMConfig.config");
            if (userConfigFile.fileExists())
                TMConfig.Current = userConfigFile.load<TMConfig>();
            return userData;
        }

        public static TM_UserData   setupGitSupport  (this TM_UserData userData)
        {
            if (userData.UsingFileStorage && userData.AutoGitCommit && userData.Path_UserData.notNull())
            {
                userData.handleExternalGitPull();
                userData.handleUserDataConfigActions();
                
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
            if (userData.AutoGitCommit)
                if(userData.NGit.status().valid())
                    userData.NGit.add_and_Commit_using_Status();
            return userData;
        }
    }
}