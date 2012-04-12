using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Linq;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:../XmlDatabase/TM_Xml_Database.Users.cs

namespace SecurityInnovation.TeamMentor.Authentication
{
	
	public class WindowsAuthentication
	{
		public static bool windowsAuthentication_Enabled;

		public static string readerGroup = "";
		public static string editorGroup = "";
		public static string adminGroup  = "";
		
		//
		static WindowsAuthentication()
		{
			loadConfiguration();
		}

		public WindowsAuthentication()
		{
			loadConfiguration();
		}

		public static void loadConfiguration()
		{
			windowsAuthentication_Enabled = TMConfig.Current.WindowsAuthentication.Enabled;
			readerGroup = TMConfig.Current.WindowsAuthentication.ReaderGroup.trim();
			editorGroup = TMConfig.Current.WindowsAuthentication.EditorGroup.trim();
			adminGroup = TMConfig.Current.WindowsAuthentication.AdminGroup.trim();
		}
		public Guid authenticateUserBaseOnActiveDirectory()
		{
			var identity = WindowsIdentity.GetCurrent();

			if (identity.notNull() && identity.IsAuthenticated && 
                identity.ImpersonationLevel == TokenImpersonationLevel.Impersonation)
			{
				var username = identity.Name;
				if (username != null)
				{
					var tmUser = new TMUser()	
							{ 
								UserName = username, 
								FirstName = "",
								LastName = "",
								GroupID = (int)calculateUserGroupBasedOnWindowsIdentity()
							};
					return tmUser.registerUserSession(Guid.NewGuid());
				}
			}
			return Guid.Empty;
		}

		public UserGroup calculateUserGroupBasedOnWindowsIdentity()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			if (principal.IsInRole(adminGroup))
				return UserGroup.Admin;
			if (principal.IsInRole(editorGroup))
				return UserGroup.Editor;
			if (principal.IsInRole(readerGroup))
				return UserGroup.Reader;
			return UserGroup.None;
		}
	}
}