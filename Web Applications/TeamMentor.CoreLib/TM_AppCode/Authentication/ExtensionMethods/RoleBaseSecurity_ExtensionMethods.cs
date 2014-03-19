using System.Security.Principal;
using System.Threading;
using System.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class RoleBaseSecurity
    {
        public static string[] getCurrentUserRoles(this HttpContextBase httpContext)
        {
            return httpContext.User.roles();
        }
        public static IPrincipal setCurrentUserRoles(this HttpContextBase httpContext, UserGroup userGroup)
        {
            return httpContext.setCurrentUserRoles(userGroup.userRoles().ToArray());
        }
		
        public static IPrincipal setCurrentUserRoles(this HttpContextBase httpContext, params UserRole[] userRoles)
        {
            var newPrincipal = userRoles.toStringArray().setThreadPrincipalWithRoles();
			httpContext.User = newPrincipal;
            return newPrincipal;			
        }				
		
        public static IPrincipal setPrivilege(this UserRole userRole)
        {
            return userRole.setThreadPrincipalWithRoles();
        }
		public static IPrincipal setThreadPrincipalWithRoles(this UserRole userRole)
		{
            var userRoles = new [] { userRole.str() };
			return  userRoles.setThreadPrincipalWithRoles();
		}
        public static IPrincipal setPrivileges(this UserGroup userGroup)
        {
            return userGroup.setThreadPrincipalWithRoles();
        }
		public static IPrincipal setThreadPrincipalWithRoles(this UserGroup userGroup)
		{
			return userGroup.userRoles().ToArray().toStringArray().setThreadPrincipalWithRoles();
		}
		
		public static IPrincipal setThreadPrincipalWithRoles(this string[] userRoles)
		{
			var newIdentity = new GenericIdentity("TM_User"); // note that this needs to be set or the SecurityAction.Demand for roles will not work
            var newPrincipal = new GenericPrincipal(newIdentity, userRoles);						
            Thread.CurrentPrincipal = newPrincipal;
			return newPrincipal;
		}
        public static string[] getThreadPrincipalWithRoles(this HttpContextBase httpContext)
        {
            return Thread.CurrentPrincipal.roles();
        }
		
		public static string[] roles(this IPrincipal principal)
		{            
            if (principal.notNull())    
            {
                var roles = (string[])principal.field("m_roles");
                if (roles.notNull())                        // check for null array
                    if(roles.size() !=1 || roles[0] != "")  // check for case where there is only one rule but it is empty
                        return (string[])principal.field("m_roles");		
            }
            return new string[] {};
		}
		
    }
}
