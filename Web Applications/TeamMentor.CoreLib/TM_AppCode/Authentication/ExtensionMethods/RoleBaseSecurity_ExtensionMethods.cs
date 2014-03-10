using System.Security.Principal;
using System.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class RoleBaseSecurity
    {
        public static IPrincipal SetCurrentUserRoles(this HttpContextBase httpContext, UserGroup userGroup)
        {
            return httpContext.SetCurrentUserRoles(userGroup.userRoles().ToArray());
        }
		
        public static IPrincipal SetCurrentUserRoles(this HttpContextBase httpContext, params UserRole[] userRoles)
        {
            var newPrincipal = userRoles.toStringArray().setThreadPrincipalWithRoles();
			httpContext.User = newPrincipal;
            return newPrincipal;			
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
            System.Threading.Thread.CurrentPrincipal = newPrincipal;
			return newPrincipal;
		}
		
		public static string[] roles(this IPrincipal principal)
		{
            if (principal.notNull())
			    return (string[])principal.field("m_roles");
            return new string[] {};
		}
		
    }
}
