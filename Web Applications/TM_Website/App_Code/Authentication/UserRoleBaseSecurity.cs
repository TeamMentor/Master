using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Security.Permissions;	
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using SecurityInnovation.TeamMentor.Authentication.WebServices;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
//O2File:UserRoles.cs 
//O2File:ExtensionMethods\RoleBaseSecurity_ExtensionMethods.cs
//O2File:ExtensionMethods\TeamMentorUserManagement_ExtensionMethods.cs
//O2File:..\UtilMethods.cs
//O2File:..\XmlDatabase\TM_Xml_Database.Users.cs


namespace SecurityInnovation.TeamMentor.Authentication.AuthorizationRules
{
    public class UserRoleBaseSecurity
    {
        public void MapRolesBasedOnRequestData()
        {
//            if (ObjectFactory.DisableRBAC)
//          {
//                HttpContextFactory.Current.SetCurrentUserRoles(UserType.Admin);                
//                return;
//            }
            var httpContext = HttpContextFactory.Current;
/*            if (httpContext.RequestHasSoapAction())
            {                
                var adminSessionID = httpContext.GetPostDataElementValue("AdminSessionID");
                if (adminSessionID != null && adminSessionID != Guid.Empty.ToString())
                    MapRolesBasedOnSessionGuid(adminSessionID);
            }
*/			
        }

        public void MapRolesBasedOnSessionGuid(string sessionId)
        {
            if (sessionId.isGuid())
                MapRolesBasedOnSessionGuid(new Guid(sessionId));
        }

        public void MapRolesBasedOnSessionGuid(Guid sessionIdGuid)
        {
			if (sessionIdGuid != Guid.Empty)
			{
				var userGroup = sessionIdGuid.session_UserGroup();			
				HttpContextFactory.Current.SetCurrentUserRoles(userGroup);            
			}
        }        
		
		public string currentIdentity_Name()
		{
			return System.Threading.Thread.CurrentPrincipal.Identity.Name;
		}
		
		public bool currentIdentity_IsAuthenticated()
		{
			return System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated;
		}
		
		public string[] currentPrincipal_Roles()
		{
			return Thread.CurrentPrincipal.roles();
		}		
    }			
}
