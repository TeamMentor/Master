    using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_ActiveSessions
    {   
        public static int               FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION = 500;

        public static Guid              login (this TM_UserData userData, string username, string password)
        {
            try
            {
                userData.sleep(FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION, false);      // to slow down brute force attacks
                if (username.valid() && password.valid())
                {
                    var tmUser = userData.TMUsers.user(username);
                
                    if (TMConfig.Current.Eval_Accounts.Enabled)
                        if (tmUser.notNull() && 
                            tmUser.AccountStatus.ExpirationDate < DateTime.Now && 
                            tmUser.AccountStatus.ExpirationDate != default(DateTime))
                        {
                            tmUser.logUserActivity("Account Expired",tmUser.UserName);
                            return Guid.Empty;
                        }

                    if (tmUser.notNull() && 
                        tmUser.PasswordHash == tmUser.createPasswordHash(password))
                    {
                        return tmUser.login(Guid.NewGuid());
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
                if (tmUser.notNull() && sessionId != Guid.Empty)
                {
                    tmUser.Stats.LastLogin = DateTime.Now;
                    tmUser.Stats.LoginOk++;
                    tmUser.logUserActivity("User Login", tmUser.UserName);
                    userData.ActiveSessions.add(sessionId, tmUser);
                    return sessionId;
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData][login]");
            }
            return Guid.Empty;
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
                    userData.ActiveSessions.Remove(sessionId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_UserData] invalidateSession");
            }
            return false;
        }
             
        public static bool              validSession         (this Guid sessionId)
        {
            try
            {
                return TM_UserData.Current.ActiveSessions.hasKey(sessionId);
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
                if(sessionId.validSession())
                    return TM_UserData.Current.ActiveSessions[sessionId];
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

    }
}