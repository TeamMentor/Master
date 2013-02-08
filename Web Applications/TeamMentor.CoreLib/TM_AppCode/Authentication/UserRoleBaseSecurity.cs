using System;
using System.Threading;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class UserRoleBaseSecurity
    {
        public void MapRolesBasedOnSessionGuid(string sessionId)
        {
            if (sessionId.isGuid())
                MapRolesBasedOnSessionGuid(new Guid(sessionId));
        }

        public UserGroup MapRolesBasedOnSessionGuid(Guid sessionIdGuid)
        {
			if (sessionIdGuid != Guid.Empty)
			{
				var userGroup = sessionIdGuid.session_UserGroup();
                //"[MapRolesBasedOnSessionGuid] userGroup = {0}".info(userGroup);
				HttpContextFactory.Current.SetCurrentUserRoles(userGroup);            
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
}
