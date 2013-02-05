using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_ActiveSessions
    {
        public static Guid registerUserSession(this string userName, Guid userGuid)
        {
            var tmUser = userName.tmUser();			
            return tmUser.registerUserSession(userGuid);
        }

        public static Guid registerUserSession(this string userName, Guid userGuid, int groupId)
        {
            var tmUser = userName.tmUser();
            tmUser.GroupID = groupId;
            return tmUser.registerUserSession(userGuid);
        }

        public static Guid registerUserSession(this TMUser tmUser, Guid userGuid)
        {
            try
            {
                if (tmUser.notNull())
                {
                    TM_Xml_Database.Current.ActiveSessions.add(userGuid, tmUser);
                    return userGuid;
                }
            }
            catch (Exception ex)
            {
                ex.log();
            }
            return Guid.Empty;
        }
                        
        public static Guid registerUserSession(this TM_Xml_Database tmDb, TMUser tmUser, Guid userGuid)
        {
            if (tmUser.isNull())
            {
                "In registerUserSession tmUser object was null".error();
                return Guid.Empty;
            }
            //"[Security Event] user logged in: {0}".info(tmUser.UserName);

            "User Login".logActivity(tmUser.UserName);
            TM_Xml_Database.Current.ActiveSessions.add(userGuid, tmUser);
            return userGuid; 
        }
        
        public static Dictionary<Guid, TMUser> activeSessions(this TM_Xml_Database tmDb)
        {
            return TM_Xml_Database.Current.ActiveSessions;
        }		
        
        public static bool validSession(this Guid sessionID)
        {
            return TM_Xml_Database.Current.ActiveSessions.hasKey(sessionID);
        }

        public static bool invalidateSession(this Guid sessionID)
        {
            if (sessionID.validSession())
            {
                "User Logout".logActivity(sessionID.session_UserName());
                TM_Xml_Database.Current.ActiveSessions.Remove(sessionID);
                return true;
            }
            return false;
        }
        
        public static TMUser session_TmUser(this Guid sessionID)
        {
            if(sessionID.validSession())
                return TM_Xml_Database.Current.ActiveSessions[sessionID];
            return null;	
        }
        public static string session_UserName(this Guid sessionID)
        {			
            if(sessionID.validSession())
                return sessionID.session_TmUser().UserName;
            return null;
        }

        public static int session_GroupID(this Guid sessionID)
        { 
            var tmUser = sessionID.session_TmUser();
            if (tmUser != null)
                return tmUser.GroupID;
            return -1;            
        }
        
        public static UserGroup session_UserGroup(this Guid sessionID)
        {
            return (UserGroup)sessionID.session_GroupID();              
        }
        
        public static List<UserRole> session_UserRoles(this Guid sessionID)
        {
            var userGroup = sessionID.session_UserGroup();
            if (UserRolesMappings.Mappings.hasKey(userGroup))
                return UserRolesMappings.Mappings[userGroup];
            return new List<UserRole>();
        }
        
        public static bool session_isAdmin(this Guid sessionID)
        {
            return UserGroup.Admin == sessionID.session_UserGroup();
        }  

    }
}