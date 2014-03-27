using System;
using System.Security.Principal;
using System.Web;
using FluentSharp.CoreLib;

//O2File:../XmlDatabase/TM_Xml_Database.Users.cs

namespace TeamMentor.CoreLib
{	
	public class WindowsAuthentication
	{
		//public static bool windowsAuthentication_Enabled;
		public static string readerGroup = "";
		public static string editorGroup = "";
		public static string adminGroup  = "";
		        		
		public WindowsAuthentication()
		{            
            if(readerGroup.notValid())
			    loadConfiguration();
		}

		public static void loadConfiguration()
		{
			//windowsAuthentication_Enabled = TMConfig.Current.WindowsAuthentication.Enabled;
			readerGroup = TMConfig.Current.WindowsAuthentication.ReaderGroup.trim();
			editorGroup = TMConfig.Current.WindowsAuthentication.EditorGroup.trim();
			adminGroup = TMConfig.Current.WindowsAuthentication.AdminGroup.trim();
		}

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
                    tmUser = userName.newUser().tmUser();
                }
                if (tmUser.GroupID != (int)calculateUserGroupBasedOnWindowsIdentity(identity))
                {
                    tmUser.GroupID = (int)calculateUserGroupBasedOnWindowsIdentity(identity);
                    tmUser.save();
                }
                return tmUser.login("WindowsAuth");
			}			
			return Guid.Empty;
		}

		public UserGroup calculateUserGroupBasedOnWindowsIdentity(WindowsIdentity identity)
		{			
		    if (identity != null)
		    {
		        var principal = new WindowsPrincipal(identity);
		        if (principal.IsInRole(adminGroup))
		            return UserGroup.Admin;
		        if (principal.IsInRole(editorGroup))
		            return UserGroup.Editor;
		        if (principal.IsInRole(readerGroup))
		            return UserGroup.Reader;
		    }            
		    return UserGroup.None;
		}
	}
}