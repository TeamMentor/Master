using System;
using System.Security.Principal;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using TeamMentor.UserData;

namespace TeamMentor.CoreLib
{	
	public class WindowsAuthentication
	{				        				
		public Guid login_Using_WindowsAuthentication(WindowsIdentity identity)
		{			
            var userName = "";
			if (identity != null && identity.IsAuthenticated && identity.ImpersonationLevel == TokenImpersonationLevel.Impersonation)			
				userName = identity.Name;
            else
            {         
                // not sure how to test the one bellow since it needs a valid HttpContext                
                userName = HttpContextFactory.Current.field("_context")
                                             .field("_wr")
                                             .invoke("GetServerVariable", "LOGON_USER") as string;                                 
            }                  
           
			if (userName.valid())
			{
                var tmUser = userName.tmUser();
                if(tmUser.isNull())
                {
                    TM_UserData.Current.logTBotActivity("Windows Authentication", "Creating User: {0}".format(userName));
                    tmUser = userName.newUser().tmUser();
                }
                if (tmUser.GroupID != (int)calculateUserGroupBasedOnWindowsIdentity(identity))
                {
                    tmUser.GroupID = (int)calculateUserGroupBasedOnWindowsIdentity(identity);
                    tmUser.save();
                    TM_UserData.Current.logTBotActivity("Windows Authentication", "Created session for User: {0}".format(userName));
                }                
                return tmUser.login("WindowsAuth");
			}			
			return Guid.Empty;
		}

		public UserGroup calculateUserGroupBasedOnWindowsIdentity(WindowsIdentity identity)
		{			
		    if (identity != null)
		    {                
			    var windowsAuth = TMConfig.Current.WindowsAuthentication;			
		        var principal = new WindowsPrincipal(identity);
		        if (principal.IsInRole(windowsAuth.AdminGroup.trim()))
		            return UserGroup.Admin;
		        if (principal.IsInRole(windowsAuth.EditorGroup.trim()))
		            return UserGroup.Editor;
		        if (principal.IsInRole(windowsAuth.ReaderGroup.trim()))
		            return UserGroup.Reader;
		    }            
		    return UserGroup.None;
		}
	}
}