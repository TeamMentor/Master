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
                userData.SetUp();
                userData.loadTmUserData();
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
                var usersFolder = userData.getTmUsersFolder();
                foreach (var file in usersFolder.files("*.userData.xml"))
                {
                    var tmUser = file.load<TMUser>();
                    if (tmUser.notNull() && tmUser.UserID > 0)
                        userData.TMUsers.Add(tmUser);
                }
            }            
            return userData;
        }                
        public static string        getTmUsersFolder(this TM_UserData userData)
        {
            return userData.Path_UserData.pathCombine("Users").createDir();
        }
        public static string        getTmUserXmlFile (this TMUser tmUser)
        {
            var userNameSubstring = tmUser.UserName.subString(0, 10).safeFileName();
            return TM_UserData.Current.getTmUsersFolder()
                                     .pathCombine("{0}_{1}.userData.xml".format(userNameSubstring, tmUser.ID));
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
        public static bool          updateTmUser     (this TMUser tmUser, string userName, string firstname, string lastname, string title, string company, string email, string country, string state, DateTime accountExpiration, bool passwordExpired, bool userEnabled, int groupId)
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
                tmUser.AccountStatus.ExpirationDate = accountExpiration;
                tmUser.AccountStatus.PasswordExpired = passwordExpired;
                tmUser.AccountStatus.UserEnabled = userEnabled;
                tmUser.saveTmUser();
                return true;
            }
            
            "[updateTmUser] provided username didn't match provided tmUser".error();
            return false;
        }
        

        public static TM_UserData   handle_UserData_ConfigActions(this TM_UserData userData)
        {
            var userConfigFile = userData.Path_UserData.pathCombine("TMConfig.config");
            if (userConfigFile.fileExists())
            {
                var newConfig = userConfigFile.load<TMConfig>();
                if (newConfig.isNull())
                    "[handleUserDataConfigActions] failed to load config file from: {0}".error(userConfigFile);
                else
                {
                    TMConfig.Current = newConfig;
                    userData.AutoGitCommit = newConfig.Git.AutoCommit_UserData;     // in case this changed
                }
            }
            return userData;
        }

    }
}