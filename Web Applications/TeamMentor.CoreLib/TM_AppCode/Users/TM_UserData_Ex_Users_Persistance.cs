using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Security.Application;
using FluentSharp.CoreLib;

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
        public static bool          updateTmUser     (this TMUser tmUser, TM_User userViewModel)
        {
            if (tmUser.isNull() || userViewModel.validation_Failed())
                return false;
            
            if (tmUser.UserName == userViewModel.UserName)
            {
                tmUser.EMail = Encoder.XmlEncode(userViewModel.Email);
                tmUser.UserName = Encoder.XmlEncode(userViewModel.UserName);
                tmUser.FirstName = Encoder.XmlEncode(userViewModel.FirstName);
                tmUser.LastName = Encoder.XmlEncode(userViewModel.LastName);
                tmUser.Title = Encoder.XmlEncode(userViewModel.Title);
                tmUser.Company = Encoder.XmlEncode(userViewModel.Company);
                tmUser.Country = Encoder.XmlEncode(userViewModel.Country);
                tmUser.State = Encoder.XmlEncode(userViewModel.State);
                tmUser.GroupID = userViewModel.GroupID > -1 ? userViewModel.GroupID : tmUser.GroupID;
                tmUser.AccountStatus.ExpirationDate = userViewModel.ExpirationDate;
                tmUser.AccountStatus.PasswordExpired = userViewModel.PasswordExpired;
                tmUser.AccountStatus.UserEnabled = userViewModel.UserEnabled;

                tmUser.saveTmUser();

                return true;
            }
            
            "[updateTmUser] provided username didn't match provided tmUser or validation failed".error();
            return false;
        }


        public static TM_UserData load_TMConfigFile(this TM_UserData userData)
        {            
            TMConfig.Location = userData.Path_UserData.pathCombine(TMConsts.TM_CONFIG_FILENAME);
            var userConfigFile = TMConfig.Location; 
            if (userConfigFile.fileExists())
            {
                var newConfig = userConfigFile.load<TMConfig>();
                if (newConfig.isNull())
                    "[handleUserDataConfigActions] failed to load config file from: {0}".error(userConfigFile);
                else
                {
                    TMConfig.Current = newConfig;                    
                    return userData;
                }
            }
            TMConfig.Current.SaveTMConfig(); // if the TMConfig.config doesn't exist or failed to load, save it with the current TMConfig.Current
            return userData;
        }
    }
}