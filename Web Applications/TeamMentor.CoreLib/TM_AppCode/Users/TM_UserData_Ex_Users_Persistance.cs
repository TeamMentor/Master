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
            if (userDataPath.isNull() || userDataPath.dirExists().isFalse())
            {
                "[TM_UserData] [setUserDataPath] provided userDataPath didn't exist: {0}".error("userDataPath");
                return false;
            }
            try
            {
                userData.Path_UserData = userDataPath;
                userData.resetData();                                
                userData.SetUp();
                userData.loadTmUserData();
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] [setUserDataPath]");
                return false;
            }            
        }

        public static TM_UserData   loadTmUserData   (this TM_UserData userData)
        {
            userData.TMUsers = new List<TMUser>();	        
            if (userData.Path_UserData.dirExists().isFalse())
            {
                "[TM_UserData_Ex_Users_Persistance] [loadTmUserData] provided userDataPath didn't exist: {0}"
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
                    else
                        "[TM_UserData_Ex_Users_Persistance] [loadTmUserData] error loading tmUser file (or UserId < 1): {0}".error(file);
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
                    //tmUser.triggerGitCommit();
                }
            }
            return tmUser;
        }
        public static bool          deleteTmUser     (this TM_UserData userData, TMUser tmUser)
        {    		
            if (tmUser.notNull())
            {
                lock(userData.TMUsers)
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
                    userData.logTBotActivity("User Delete","{0} - {1}".format(tmUser.UserName, tmUser.UserID));
                    return true;
                }
            }
            return false;
        }
        public static bool          updateTmUser     (this TMUser tmUser, string userName, string firstname, string lastname, string title, string company, 
                                                      string email, string country, string state, DateTime accountExpiration, bool passwordExpired, 
                                                      bool userEnabled, bool accountNeverExpires, int groupId)
        {
            var user = new TM_User
                {
                    UserName            = userName,
                    FirstName           = firstname,
                    LastName            = lastname,
                    Title               = title,
                    Company             = company,
                    Email               = email,
                    Country             = country,
                    State               = state,
                    ExpirationDate      = accountExpiration,
                    PasswordExpired     = passwordExpired,
                    UserEnabled         = userEnabled, 
                    AccountNeverExpires = accountNeverExpires,
                    GroupID             = groupId
                };
            return tmUser.updateTmUser(user);
        }
        public static bool          updateTmUser     (this TMUser tmUser, TM_User user)
        {                         
            if (tmUser.isNull())
                return false;
            if (tmUser.UserName == user.UserName)
            {
                tmUser.EMail        = user.Email;     //Encoder.XmlEncode(user.Email);    // these encodings should now be enfored on TBOT (and the user does not see them)
                tmUser.UserName     = user.UserName;  //Encoder.XmlEncode(user.UserName); // they were causing double encoding isues on the new TBOT editor
                tmUser.FirstName    = user.FirstName; //Encoder.XmlEncode(user.FirstName);
                tmUser.LastName     = user.LastName;  //Encoder.XmlEncode(user.LastName);
                tmUser.Title        = user.Title;     //Encoder.XmlEncode(user.Title);
                tmUser.Company      = user.Company;   //Encoder.XmlEncode(user.Company);
                tmUser.Country      = user.Country;   //Encoder.XmlEncode(user.Country);
                tmUser.State        = user.State;     //Encoder.XmlEncode(user.State);
                tmUser.UserTags     = user.UserTags;
                tmUser.GroupID      = user.GroupID > -1 ? user.GroupID : tmUser.GroupID;
                tmUser.AccountStatus.ExpirationDate      = user.ExpirationDate;
                tmUser.AccountStatus.PasswordExpired     = user.PasswordExpired;
                tmUser.AccountStatus.UserEnabled         = user.UserEnabled;
                tmUser.AccountStatus.AccountNeverExpires = user.AccountNeverExpires; 
                tmUser.saveTmUser();
                            
                tmUser.logUserActivity("User Updated",""); // so that we don't get this log entry on new user creation

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
                var newConfig = userConfigFile.load<TMConfig>();    // to check that the new TMConfig is not corrupted
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