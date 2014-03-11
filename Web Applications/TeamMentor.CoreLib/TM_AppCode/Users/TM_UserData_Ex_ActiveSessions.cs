using System;
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;

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
        public static Guid              login (this TM_UserData userData, string username, string password)
        {
            try
            {                
                if (username.valid() && password.valid())
                {
                    var tmUser = userData.tmUser(username);

                    if (tmUser.notNull())
                    {
                       // tmUser.SecretData.SessionID = Guid.Empty;          // reset the user SessionID
                        if (tmUser.account_Expired())
                        {
                            tmUser.logUserActivity("Account Expired", "Expiry date: {0}".format(tmUser.AccountStatus.ExpirationDate));
                            return Guid.Empty;
                        }
                        var pwdOk = tmUser.SecretData.PasswordHash == tmUser.createPasswordHash(password);
                        if (pwdOk)
                        {
                            if(tmUser.account_Enabled())
                                return tmUser.login();                    // call login with a new SessionID            
                            
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
            return TM_UserData.Current.login(tmUser);
        }
        public static Guid              login (this TM_UserData userData,TMUser tmUser)    
        {
            try
            {
                if (tmUser.notNull())                                                   // there is a valid user
                {
                    tmUser.Stats.LastLogin = DateTime.Now;
                    tmUser.Stats.LoginOk++;
                    var userSession = tmUser.add_NewSession();                          // create new session
                    if (userSession.notNull())
                    {
                        tmUser.logUserActivity("User Login", tmUser.UserName);          // will save the user                                              
                        //SendEmails.SendEmailAboutUserToTM("Logged In", tmUser);
                        return userSession.SessionID;
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
            var sessionIDs = tmUser.session_sessionIds();
            if (sessionIDs.empty())
                return false;            
            var allOk = true;
            foreach (var sessionId in sessionIDs)
            {
                var result = tmUser.logout(sessionId);
                allOk = allOk && result;
            }
            return allOk;
            // this could probably be done better with a List<bool> used to capture the results
            // the return value would come from an ExtMet allTrue(this List<bool> ...)
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
                    tmUser.remove_Session(sessionId);
                    //tmUser.SecretData.SessionID = Guid.Empty;
                    //userData.ActiveSessions.Remove(sessionId);
                    //SendEmails.SendEmailAboutUserToTM("Logged Out", tmUser);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] invalidateSession");
            }
            return false;
        }

        public static UserSession       add_NewSession(this TMUser tmUser)
        {

            var ipAddress = HttpContextFactory.Context.ipAddress();            
            var userSession = new UserSession
            {
                SessionID = Guid.NewGuid(),
                IpAddress = ipAddress,
                CreationDate = DateTime.Now
            };
            tmUser.Sessions.add(userSession);
            return userSession;            
        }
        public static bool              remove_Session(this TMUser tmUser, UserSession userSession)
        {
            if (userSession.isNull())
                return false;
            return tmUser.remove_Session(userSession.SessionID);
        }
        public static bool              remove_Session(this TMUser tmUser, Guid sessionID)
        {
            if (tmUser.isNull())
                return false;
            var currentSessions = tmUser.currentSessions();
            if (currentSessions.hasKey(sessionID))
            {
                var sessionToRemove = currentSessions[sessionID];
                tmUser.Sessions.Remove(sessionToRemove);
                return true;
            }
            "[remove_Session] was not able to find session object {0} for user {1}".error(sessionID, tmUser.UserName);
            return false;
        }

        public static Dictionary<Guid, UserSession> currentSessions(this TMUser tmUser)
        {
            return tmUser.Sessions.ToDictionary(session => session.SessionID);
        }

        public static List<Guid>        validSessions(this TM_UserData userData)
        {
            return (from tmUser in userData.TMUsers
                    from session in tmUser.Sessions
                    where session.SessionID != Guid.Empty
                    select session.SessionID).toList();
        }

        public static TM_UserData       resetAllSessions(this TM_UserData userData)
        {
            var sessionIDs = userData.validSessions();
            foreach (var sessionID in sessionIDs)
            {
                var tmUser = sessionID.session_TmUser();                
                tmUser.remove_Session(sessionID);                
            }
            return userData;
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
                                       from session in tmUser.Sessions
                                       where session.SessionID == sessionId
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
        public static List<Guid>        session_sessionIds   (this TMUser tmUser)
        {
            try
            {
                return (from session in tmUser.Sessions
                        select session.SessionID).toList();                
            }
            catch (Exception ex)
            {
                ex.log();                
            }
            return new List<Guid>();
        }        

        public static string            csrfToken            (this Guid guid)
        {
            return guid.str().hash().str();	  	// interrestingly guid.hash().str() produces a different value
        }
            
    }
}