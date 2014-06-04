using System;
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Users
    {        


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

        public static bool          setPassword                 (this TMUser tmUser, string password)
        {		          
            if (tmUser.isNull() || password.notValid())
                return false;
            tmUser.logUserActivity("Password Change", "Direct change (by an admin)");
            return setPasswordHash(tmUser, tmUser.createPasswordHash(password));         
        }

        public static bool          setPasswordHash             (this TMUser tmUser, string passwordHash)
        {
            if (tmUser.notNull() && passwordHash.valid())
            { 
                tmUser.SecretData.PasswordHash       = passwordHash;
                tmUser.AccountStatus.PasswordExpired = false;
                tmUser.saveTmUser();                
                return true;
            }
            return false;    		
        }
        public static bool          setUserPassword      (this TM_UserData userData, TMUser tmUser, string currentPassword, string newPassword)
        {
            //var tmUser = tmAuthentication.currentUser;
            if (tmUser.notNull())
            {
                if (tmUser.SecretData.PasswordHash == tmUser.createPasswordHash(currentPassword)) // check if current password matches provided value
                {
                    var newPasswordHash =  tmUser.createPasswordHash(newPassword);
                    if (newPasswordHash != tmUser.SecretData.PasswordHash)                        // check that password are not repeated
                    {
                        tmUser.logUserActivity("User Password Change", "With previous password provided");
                        return tmUser.setPasswordHash(newPasswordHash);
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
                    tmUser.logUserActivity("Password Change", "Using Password Reset: {0}".format(token));
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
        
        public static TMUser        tmUser_by_Name_or_Id(this TM_UserData userData, string userNameOrId)
        {
            var tmUser = userData.tmUser(userNameOrId);
            if (tmUser.isNull() && userNameOrId.isInt())
                return userData.tmUser(userNameOrId.toInt());
            return tmUser;
        }
        public static TMUser        tmUser              (this TM_UserData userData, string userName)
        {        
            lock( userData.TMUsers)
            {
                return userData.TMUsers.Where((tmUser) => tmUser.UserName == userName).first() ;
            }
        }
        public static TMUser        tmUser              (this TM_UserData userData, int userId)
        {
            lock( userData.TMUsers)
            {
                return userData.TMUsers.Where((tmUser) => tmUser.UserID == userId).first() ;
            }
        }        
        public static TMUser        tmUser_FromEmail    (this TM_UserData userData, string eMail)
        {
            if (eMail.isNull())
                return null;
            lock( userData.TMUsers)
            {
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
        }
        
        public static List<TMUser>  tmUsers             (this List<int> usersId)
        {
            if (usersId.isNull())
                return new List<TMUser>();
            return usersId.Select(userId => userId.tmUser()).toList();
        }
        public static List<TMUser>  tmUsers             (this TM_UserData userData)
        {
            return TM_UserData.Current.TMUsers.toList();
        }                                        

        
        [ManageUsers]   public static List<bool>    deleteTmUsers       (this TM_UserData userData, List<int> userIds)
        {
            UserGroup.Admin.demand();
            if(userIds.isNull())
                return new List<bool>();
            return userIds.Select(userId => userData.deleteTmUser(userId)).toList();
        }
        [ManageUsers]   public static bool          deleteTmUser        (this TM_UserData userData, int userId)
        {
            UserGroup.Admin.demand();
            return userData.deleteTmUser(userId.tmUser());
        }
        [ManageUsers]   public static bool          updateTmUser        (this TM_UserData userData, TM_User user)
        {
            UserGroup.Admin.demand();
            return userData.tmUser(user.UserId).updateTmUser(user);
        }
        [ManageUsers]   public static bool          updateTmUser        (this TM_UserData userData, int userId, string userName, string firstname, string lastname, string title, string company, string email, string country, string state, DateTime accountExpiration, bool passwordExpired, bool userEnabled, bool accountNeverExpires, int groupId)
        {
            UserGroup.Admin.demand();
            return userData.tmUser(userId).updateTmUser(userName, firstname, lastname,  title, company, email,country, state, accountExpiration, passwordExpired,userEnabled, accountNeverExpires,groupId);
        }	
	    [ManageUsers]  public static List<TMUser>   users               (this TM_UserData userData)
	    {     
            UserGroup.Admin.demand();
	        return userData.tmUsers();            
	    }              
        [ManageUsers]   public static List<string>  getUserRoles        (this TM_UserData userData, int userId)
        {
            UserGroup.Admin.demand();
            var tmUser = userData.tmUser(userId);
            if (tmUser.notNull()) 
                return tmUser.userGroup().userRoles().toStringList();
            return new List<string>();
        }        
        [ManageUsers]   public static bool          setUserGroupId      (this TM_UserData userData, int userId, int groupId)
        {			
            UserGroup.Admin.demand();
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
                tmUser.SecretData.PostLoginView = postLoginView;
                tmUser.saveTmUser();                
            }
            return tmUser;
        }
        [Admin]         public static TMUser        set_PostLoginScript (this TMUser tmUser, string postLoginScript)
        {
            if (tmUser.notNull())
            {
                tmUser.SecretData.PostLoginScript = postLoginScript;
                tmUser.saveTmUser();                
            }
            return tmUser;
        }
    }
}