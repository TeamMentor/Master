using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using O2.Kernel.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
//O2File:..\UserRoles.cs

namespace SecurityInnovation.TeamMentor.Authentication.ExtensionMethods
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
			return (string[])principal.field("m_roles");
		}
		
    }
}
