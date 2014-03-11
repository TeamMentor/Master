using System;
using System.Threading;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class UserRoleBaseSecurity
    {
        public UserGroup MapRolesBasedOnSessionGuid(string sessionId)
        {
            if (sessionId.isGuid())
                return MapRolesBasedOnSessionGuid(new Guid(sessionId));
            return UserGroup.None;
        }

        public UserGroup MapRolesBasedOnSessionGuid(Guid sessionIdGuid)
        {
			if (sessionIdGuid != Guid.Empty)
			{
				var userGroup = sessionIdGuid.session_UserGroup();                
				HttpContextFactory.Current.setCurrentUserRoles(userGroup);            
                return userGroup;
			}
            return UserGroup.None;
        }        
		
		public string currentIdentity_Name()
		{
			return Thread.CurrentPrincipal.Identity.Name;
		}
		
		public bool currentIdentity_IsAuthenticated()
		{
			return Thread.CurrentPrincipal.Identity.IsAuthenticated;
		}
		
		public string[] currentPrincipal_Roles()
		{
			return Thread.CurrentPrincipal.roles();
		}		
    }			

/*    public static class UserRoleBaseSecurity_ExtensionMethods
    {        
        public static UserGroup mapRolesBasedOnSessionId(this Guid sessionID)
        {
            return new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(sessionID);
        }        
    }*/
}
