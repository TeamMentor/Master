using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Users_Persistance
    {        
      /*  public static bool setUserDataPath(this TM_UserData userData, string userDataPath)
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
                userData.setUp();
                userData.loadTmUserData();
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] [setUserDataPath]");
                return false;
            }            
        }*/

        public static TM_UserData   users_Load   (this TM_UserData userData)             
        {
            try
            { 
                userData.TMUsers = new List<TMUser>();
                if (userData.usingFileStorage())
                {
                    var usersFolder = userData.users_XmlFile_Location();
                    if (usersFolder.isNull())
                    {
                        "[TM_UserData] [users_Load] could not load users because users_XmlFile_Location() returned null".error();
                        return userData;
                    }
                                  
                    foreach (var file in usersFolder.files("*.userData.xml"))
                    {
                        var tmUser = file.load<TMUser>();
                        if (tmUser.notNull() && tmUser.UserID > 0)
                            userData.TMUsers.Add(tmUser);
                        else
                            "[TM_UserData_Ex_Users_Persistance] [users_Load] error loading tmUser file (or UserId < 1): {0}".error(file);
                    }                
                }            
                return userData;
            }
            finally
            {
                userData.Events.After_Users_Load.raise();
            }
        }                
        public static string        users_XmlFile_Location(this TM_UserData userData)   
        {
            if (userData.usingFileStorage())
                if (userData.Path_UserData.notNull())
                    return userData.Path_UserData
                                   .pathCombine(TMConsts.USERDATA_PATH_USER_XML_FILES)
                                   .createDir();
            return null;
        }
        public static string        user_XmlFile_Location (this TMUser tmUser)          
        {
            var fileName =  tmUser.user_XmlFile_Name();

            return fileName.valid()
                        ? TM_UserData.Current.users_XmlFile_Location().pathCombine(fileName)
                        : null;
        }
        public static string        user_XmlFile_Name     (this TMUser tmUser)          
        {
            if (tmUser.notNull() && tmUser.UserName.valid() && tmUser.ID != Guid.Empty)
            {
                var userNameSubstring = tmUser.UserName.subString(0, 10).safeFileName();
                var fileName = TMConsts.USERDATA_FORMAT_USER_XML_FILE.format(userNameSubstring, tmUser.ID);
                return fileName;
            }
            return null;
        }
        
        public static bool          saveTmUser       (this TMUser tmUser)               
        {
            if (TM_UserData.Current.usingFileStorage())
            {                
                lock (tmUser)
                {                    
                    return tmUser.saveAs(tmUser.user_XmlFile_Location());
                }
            }
            return false;
        }
        public static bool          deleteTmUser     (this TM_UserData userData, TMUser tmUser)
        {    		
            if (tmUser.notNull())
            {
                lock(userData.TMUsers)
                {
                    userData.TMUsers.remove(tmUser);
                    if (userData.usingFileStorage())
                    {
                        lock (tmUser)
                        {
                            tmUser.user_XmlFile_Location().file_Delete();
                            //userData.triggerGitCommit();
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


        
    }
}