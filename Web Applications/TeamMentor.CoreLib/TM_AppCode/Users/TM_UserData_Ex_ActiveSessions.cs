using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_ActiveSessions
    {   
        /*public static Guid              login_Using_LoginToken (this TM_UserData userData, string username, Guid loginToken)
        {
            try
            {
                if (username.valid() && loginToken != Guid.Empty)
                {
                    var tmUser = userData.tmUser(username);
                    if (tmUser.notNull())
                    {
                        if (tmUser.SecretData.SingleUseLoginToken == loginToken)
                        {
                            tmUser.SecretData.SingleUseLoginToken = Guid.Empty;
                            tmUser.logUserActivity("SingleUseLoginToken used", loginToken.str());
                            return tmUser.login();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_Xml_Database] login_Using_LoginToken"); 
            }
            return Guid.Empty;    			
        }*/
        public static Guid              expiredLogin() {
            var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes("EXPIRED!"));
            return new Guid(hash);
        }

        public static Guid              login (this TM_UserData userData, string username, string password)
        {
            try
            {                
                if (username.valid() && password.valid())
                {
                    var tmUser = userData.tmUser(username);

                    if (tmUser.notNull())
                    {
                        tmUser.SecretData.SessionID = Guid.Empty;          // reset the user SessionID
                        if (tmUser.account_Expired())
                        {
                            tmUser.logUserActivity("Account Expired", "Expiry date: {0}".format(tmUser.AccountStatus.ExpirationDate));
                            return TM_UserData_Ex_ActiveSessions.expiredLogin();
                        }
                        var pwdOk = tmUser.SecretData.PasswordHash == tmUser.createPasswordHash(password);
                        if (pwdOk)
                        {
                            if(tmUser.account_Enabled())
                                return tmUser.login(Guid.NewGuid());                    // call login with a new SessionID            
                            
                            tmUser.logUserActivity("Login Fail",  "pwd ok, but account disabled");
                        }
                        else
                        {
                            tmUser.Stats.LoginFail++;
                            tmUser.logUserActivity("Login Fail", "bad pwd");                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_Xml_Database] login");                
            }
            return Guid.Empty;    			
        }
        public static Guid              login (this TMUser tmUser)                                         
        {
            return tmUser.login(Guid.NewGuid());
        }        
        public static Guid              login (this TMUser tmUser, Guid sessionId)                         
        {
            return TM_UserData.Current.login(tmUser, sessionId);
        }
        public static Guid              login (this TM_UserData userData,TMUser tmUser, Guid sessionId)    
        {
            try
            {
                if (tmUser.notNull())               // there is a valid user
                {
                    if (sessionId != Guid.Empty)    // there was a valid session set
                    {                        
                        tmUser.Stats.LastLogin = DateTime.Now;
                        tmUser.Stats.LoginOk++;
                        tmUser.SecretData.SessionID = sessionId;
                        tmUser.logUserActivity("User Login", tmUser.UserName);          // will save the user                                              
                        SendEmails.SendEmailAboutUserToTM("Logged In", tmUser);
                        return sessionId;
                    }                    
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData][login]");
            }
            return Guid.Empty;
        }
        public static bool              logout(this TMUser tmUser)
        {
            return tmUser.logout(tmUser.session_sessionId());
        }
        public static bool              logout(this Guid sessionId)                                        
        {
            return sessionId.session_TmUser()
                            .logout(sessionId);
        }
        public static bool              logout(this TMUser tmUser, Guid sessionId)                         
        {
            return TM_UserData.Current.logout(tmUser, sessionId);
        }
        public static bool              logout(this TM_UserData userData, TMUser tmUser, Guid sessionId)   
        {
            try
            {
                if (tmUser.notNull() && sessionId.validSession())
                {
                    tmUser.logUserActivity("User Logout", tmUser.UserName);
                    tmUser.SecretData.SessionID = Guid.Empty;
                    //userData.ActiveSessions.Remove(sessionId);
                    SendEmails.SendEmailAboutUserToTM("Logged Out", tmUser);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] invalidateSession");
            }
            return false;
        }

        public static List<Guid> validSessions(this TM_UserData userData)
        {
            return (from tmUser in userData.TMUsers
                    where tmUser.SecretData.SessionID != Guid.Empty
                    select tmUser.SecretData.SessionID).toList();
        }

        public static bool              validSession         (this Guid sessionId)
        {
            try
            {
                var validSessions = TM_UserData.Current.validSessions();
                return validSessions.contains(sessionId);
                //return TM_UserData.Current.ActiveSessions.hasKey(sessionId);
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] validSession");                
            }             
            return false;
        }
        public static TMUser            session_TmUser       (this Guid sessionId)
        {
            try
            {
                if (sessionId == Guid.Empty)
                    return null;
                var tmUsers = TM_UserData.Current.TMUsers;
                var tmUserInSession = (from tmUser in tmUsers
                                       where tmUser.SecretData.SessionID == sessionId
                                       select tmUser).first();

                if (tmUserInSession.notNull())
                {                    
                    if (tmUserInSession.AccountStatus.UserEnabled)
                        return tmUserInSession;
                    tmUserInSession.logUserActivity("User Disabled", "User had an active session, but his account is disabled");
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] session_TmUser");
            }
            return null;	
        }
        public static string            session_UserName     (this Guid sessionId)
        {			
            if(sessionId.validSession())
                return sessionId.session_TmUser().UserName;
            return null;
        }
        public static int               session_GroupID      (this Guid sessionId)
        { 
            var tmUser = sessionId.session_TmUser();
            if (tmUser != null)
                return tmUser.GroupID;
            return -1;            
        }        
        public static UserGroup         session_UserGroup    (this Guid sessionId)
        {
            return (UserGroup)sessionId.session_GroupID();              
        }        
        public static List<UserRole>    session_UserRoles    (this Guid sessionId)
        {
            try
            { 
                var userGroup = sessionId.session_UserGroup();
                if (UserRolesMappings.Mappings.hasKey(userGroup))
                    return UserRolesMappings.Mappings[userGroup];
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] session_UserRoles");
            }            
            return new List<UserRole>();
        }        
        public static bool              session_isAdmin      (this Guid sessionId)
        {
            return UserGroup.Admin == sessionId.session_UserGroup();
        }  
        public static Guid              session_sessionId    (this TMUser tmUser)
        {
            try
            {
                return tmUser.SecretData.SessionID;
                //foreach(var item in TM_UserData.Current.ActiveSessions)
                //    if (item.Value == tmUser)
                //        return item.Key;                
            }
            catch (Exception ex)
            {
                ex.log();                
            }
            return Guid.Empty;
        }        
    }
}