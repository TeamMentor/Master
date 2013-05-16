using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Security.Application;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Users
    {        
        public static int           createDefaultAdminUser      (this TM_UserData userData)
        {  
            var tmConfig = TMConfig.Current;
            lock (tmConfig)
            {                
                var defaultAdminUser_Name  = tmConfig.TMSecurity.Default_AdminUserName;
                var defaultAdminUser_Pwd   = tmConfig.TMSecurity.Default_AdminPassword;                
                var defaultAdminUser_Email = tmConfig.TMSecurity.Default_AdminEmail;                
                var adminUser            = userData.tmUser(defaultAdminUser_Name);
                
                if (adminUser.notNull())
                {
                    if (adminUser.SecretData.PasswordHash.notValid() || tmConfig.OnInstallation.ForceAdminPasswordReset)
                    {
                        "[createDefaultAdminUser] reseting password since passwordHash was not valid and ForceAdminPasswordReset was set".error();
                        adminUser.SecretData.PasswordHash = adminUser.createPasswordHash(defaultAdminUser_Pwd);                        
                        //adminUser.AccountStatus.ExpirationDate = default(DateTime);    
                        adminUser.saveTmUser();
                    }
                    if (adminUser.GroupID != (int) UserGroup.Admin)
                    {
                        "[createDefaultAdminUser] admin user was not admin (changed to admin)".error();
                        adminUser.GroupID = (int) UserGroup.Admin;
                        adminUser.saveTmUser();
                    }
                    if (adminUser.notNull())
                        return adminUser.UserID;
                }				
                "[createDefaultAdminUser] admin user didn't exist (creating it)".debug();
                var userId = userData.newUser(defaultAdminUser_Name, defaultAdminUser_Pwd,defaultAdminUser_Email,1);                
                userId.tmUser().AccountStatus.ExpirationDate = default(DateTime);               // so that the admin user doesn't expire by default
                return userId;
            }            
        }        

        public static TMUser        tmUser                      (this int userId)
        {
            return TM_UserData.Current.tmUser(userId);
        }                
        public static TMUser        tmUser                      (this string name)
        {
            return TM_UserData.Current.tmUser(name);
        }                
        public static TMUser        tmUser_FromEmail            (this string email)
        {
            return TM_UserData.Current.tmUser_FromEmail(email);    
        }
        public static bool          deleteTmUser                (this TMUser tmUser)
        {
            return TM_UserData.Current.deleteTmUser(tmUser);
        }
        public static int           newUser                     (this TM_UserData userData)
        {
            return userData.newUser("test_user_{0}".format(5.randomLetters()));
        }        
        public static int           newUser                     (this TM_UserData userData, string  username)
        {
            return userData.newUser(username, 5.randomLetters());
        }                
        public static int           newUser                     (this TM_UserData userData, string  username, string password)
        {
            var randomEmail = "{0}@{1}.{2}".format(7.randomLetters(), 5.randomLetters(), 2.randomLetters()).lower();
            return userData.newUser(username,password, randomEmail,0);
        }        
        public static int           newUser                     (this TM_UserData userData, string  username, string password, string email)
        {
            return userData.newUser(username,password, email,0);
        }        
        public static int           newUser                     (this TM_UserData userData, string  username, string password, string email, int groupId)
        {
            return userData.newUser(username, password, email,"FName","LName","A Note", "El Title","The Company","The Country","The State",groupId);
        }        
        public static int           newUser                     (this TM_UserData userData, string  username, string password, string email, string firstname, string lastname, string note , string title, string company, string country, string state, int groupId)
        {			
            var userId = Guid.NewGuid().hash();  //10000000.random();//10.randomNumbers().toInt();
            if (userId < 0)						// find a .net that does this (maybe called 'invert')
                userId = -userId;
            "Creating new user: {0} with id {1}".debug(username, userId);
            
            if (groupId <1)						//set default user type						
                groupId = 2;					//by default new users are of type 2 (i.e. Reader)
            else
                UserRole.ManageUsers.demand();	// only users with UserRole.ManageUsers should be able to create non-default users
                
            var tmUser = new TMUser {
                UserID 		 = userId,
                UserName 	 = Encoder.XmlEncode(username),
                FirstName 	 = Encoder.XmlEncode(firstname),
                LastName 	 = Encoder.XmlEncode(lastname),
                Company 	 = Encoder.XmlEncode(company),
                Country 	 = Encoder.XmlEncode(country),
                State 	     = Encoder.XmlEncode(state),
                GroupID 	 = groupId,
                Title 		 = Encoder.XmlEncode(title), 										
                EMail 		 = Encoder.XmlEncode(email) ?? "",                     
            };
            tmUser.SecretData.PasswordHash = tmUser.createPasswordHash(password);            
            userData.TMUsers.Add(tmUser);            
        
            //save it
            SendEmails.SendNewUserEmails("New user created: {0}".format(tmUser.UserName), tmUser);
            tmUser.saveTmUser();            
                    
            return userId;    		
        }				                
        public static int           createTmUser                (this TM_UserData userData, NewUser newUser)
        {
            if (newUser.isNull())
                return 0;
            
            // ensure the email is lowercase (will fail validation otherwise)
            newUser.Email = newUser.Email.lower();              

            //validate user against the DataContract specificed in the NewUser class
            if (newUser.validation_Failed())        
                return 0;

            // if there is a groupId provided we must check if the user has the manageUsers Priviledge							
            if (newUser.GroupId !=0)		        
                UserRole.ManageUsers.demand();		

            // Check if there is already a user with the provided username or email
            if (newUser.Username.tmUser().notNull() || newUser.Email.tmUser_FromEmail().notNull())
                return 0;  

            // Create user             
            return userData.newUser(newUser.Username, newUser.Password, newUser.Email, newUser.Firstname, newUser.Lastname, newUser.Note, newUser.Title, newUser.Company, newUser.Country, newUser.State,newUser.GroupId);						
        }
        public static List<int>     createTmUsers               (this TM_UserData userData, string batchUserData) 
        {						
            var newUsers = new List<NewUser>();
            foreach(var line in batchUserData.fixCRLF().split_onLines())
            {
                var newUser = new NewUser();
                //return _newUser;
                var items = line.split(",");
                
                newUser.Username    = items.size()>0 ? items[0].trim() : "";	 
                newUser.Password    = items.size()>1 ? items[1].trim() : "";	 
                newUser.Firstname   = items.size()>2 ? items[2].trim() : "";	 
                newUser.Lastname    = items.size()>3 ? items[3].trim() : "";	 
                newUser.GroupId     = items.size()>4 ? items[4].trim().toInt() : 0;

                //default values 
                newUser.Company = "...";
                newUser.Country = "...";
                newUser.Email   = "{0}@randomm.xyz".format(10.randomLetters());
                newUser.Note    = "(Batch user created)";
                newUser.State   = "...";
                newUser.Title   = "...";
                if (newUser.validation_Failed())
                {
                    "[createTmUsers] failed validation for user data:{0}".error(newUser.toXml());
                    newUsers.Add(null);
                }
                else
                    newUsers.Add(newUser);
            } 
            return userData.createTmUsers(newUsers);
        }
        
        public static bool          setUserPassword             (this TM_UserData userData, int userId, string password)
        {
            return userData.tmUser(userId)
                           .setPassword(password);
        }        
        public static bool          setUserPassword             (this TM_UserData userData, string username, string password)
        {
            return userData.tmUser(username)
                           .setPassword(password);
        }
        public static bool          setCurrentUserPassword      (this TM_UserData userData, TM_Authentication tmAuthentication, string currentPassword, string newPassword)
        {
            var tmUser = tmAuthentication.currentUser;
            if (tmUser.notNull())
            {
                if (tmUser.SecretData.PasswordHash == tmUser.createPasswordHash(currentPassword))
                {
                    var newPasswordHash =  tmUser.createPasswordHash(newPassword);
                    if (newPasswordHash != tmUser.SecretData.PasswordHash)
                    {
                        tmUser.SecretData.PasswordHash = tmUser.createPasswordHash(newPassword);
                        tmUser.saveTmUser();
                        return true;
                    }
                }
            }
            return false;
        }                	
        public static bool          passwordReset               (this TM_UserData userData, string userName, Guid token, string newPassword)
        {
            var tmUser = userName.tmUser();
            if (tmUser.notNull() && token!= Guid.Empty)
            {
                if (tmUser.passwordResetToken_isValid(token))
                {
                    tmUser.SecretData.PasswordHash       = tmUser.createPasswordHash(newPassword);
                    tmUser.AccountStatus.PasswordExpired = false;
                    tmUser.SecretData.PasswordResetToken = null;
                    tmUser.saveTmUser();                        
                    return true;
                }            
            }
            return false;
        }
        public static List<int>     userIds                     (this List<TM_User> tmUsers)
        {
            return (from tmUser in tmUsers
                    where tmUser.notNull()
                    select tmUser.UserId).toList();
        }                   
        public static string        getUserGroupName            (this TM_UserData userData, int userId)
        {
            var tmUser = userData.tmUser(userId);
            if (tmUser.notNull())
                return tmUser.userGroup().str();
            return null;
        }        
        public static int           getUserGroupId              (this TM_UserData userData, int userId)
        {			
            var tmUser = userData.tmUser(userId);
            if (tmUser.notNull())
                return tmUser.GroupID;
            return -1;
        }                
        public static TMUser        tmUser              (this TM_UserData userData, string userName)
        {
            //userName = userName.urlDecode();
            return userData.TMUsers.Where((tmUser) => tmUser.UserName == userName).first() ;
        }
        public static TMUser        tmUser              (this TM_UserData userData, int userId)
        {
            return userData.TMUsers.Where((tmUser) => tmUser.UserID == userId).first() ;
        }        
        public static TMUser        tmUser_FromEmail    (this TM_UserData userData, string eMail)
        {
            if (eMail.isNull())
                return null;
            var tmUsers = userData.TMUsers.where((tmUser) => tmUser.EMail == eMail);
            switch (tmUsers.size())
            {
                case 0:
                    //"Could not find TM User with email'{0}'".error(eMail);
                    return null;
                case 1:
                    return tmUsers.first();
                default:
                    "There were multiple users resolved to the email '{0}', so returning null".error(eMail);
                    return null;
            }            
        }
        
        public static List<TMUser>  tmUsers             (this List<int> usersId)
        {
            return usersId.Select(userId => userId.tmUser()).toList();
        }
        public static List<TMUser>  tmUsers             (this TM_UserData userData)
        {
            return TM_UserData.Current.TMUsers.toList();
        }                
        public static bool          setPassword         (this TMUser tmUser, string password)
        {		            
            if (tmUser.notNull())
            {                
                tmUser.SecretData.PasswordHash       = tmUser.createPasswordHash(password);
                tmUser.AccountStatus.PasswordExpired = false;
                tmUser.saveTmUser();
                tmUser.logUserActivity("Password Change", tmUser.UserName);
                return true;
            }
            return false;    		
        }                

        [ManageUsers]   public static List<int>     createTmUsers       (this TM_UserData userData, List<NewUser> newUsers)
        {
            return newUsers.Select(newUser => userData.createTmUser(newUser)).toList();
        }
        [ManageUsers]   public static List<bool>    deleteTmUsers       (this TM_UserData userData, List<int> userIds)
        {
            return userIds.Select(userId => userData.deleteTmUser(userId)).toList();
        }
        [ManageUsers]   public static bool          deleteTmUser        (this TM_UserData userData, int userId)
        {
            return userData.deleteTmUser(userId.tmUser());
        }        
        [ManageUsers]   public static bool          updateTmUser        (this TM_UserData userData, int userId, string userName, string firstname, string lastname, string title, string company, string email, string country, string state, DateTime accountExpiration, bool passwordExpired, bool userEnabled, int groupId)
        {
            return userData.tmUser(userId).updateTmUser(userName, firstname, lastname,  title, company, email,country, state, accountExpiration, passwordExpired,userEnabled,groupId);
        }		                
        [ManageUsers]   public static List<string>  getUserRoles        (this TM_UserData userData, int userId)
        {
            var tmUser = userData.tmUser(userId);
            if (tmUser.notNull()) 
                return tmUser.userGroup().userRoles().toStringList();
            return new List<string>();
        }        
        [ManageUsers]   public static bool          setUserGroupId      (this TM_UserData userData, int userId, int groupId)
        {			
            var tmUser = userData.tmUser(userId);
            if (tmUser.notNull()) 
            {
                tmUser.GroupID = groupId;
                tmUser.saveTmUser();                
                return true;
            }
            return false;
        }        									        

        [Admin]         public static TMUser        set_PostLoginView   (this TMUser tmUser, string postLoginView)
        {
            if (tmUser.notNull())
            {
                tmUser.PostLoginView = postLoginView;
                tmUser.saveTmUser();                
            }
            return tmUser;
        }
        [Admin]         public static TMUser        set_PostLoginScript (this TMUser tmUser, string postLoginScript)
        {
            if (tmUser.notNull())
            {
                tmUser.PostLoginScript = postLoginScript;
                tmUser.saveTmUser();                
            }
            return tmUser;
        }        
    }
}