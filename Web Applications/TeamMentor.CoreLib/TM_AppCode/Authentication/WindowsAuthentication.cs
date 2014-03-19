using System;
using System.Security.Principal;
using FluentSharp.CoreLib;

//O2File:../XmlDatabase/TM_Xml_Database.Users.cs

namespace TeamMentor.CoreLib
{	
	public class WindowsAuthentication
	{
		public static bool windowsAuthentication_Enabled;
		public static string readerGroup = "";
		public static string editorGroup = "";
		public static string adminGroup  = "";
		
        public WindowsIdentity CurrentWindowsIdentity { get; set; }     				

		public WindowsAuthentication()
		{
            CurrentWindowsIdentity = WindowsIdentity.GetCurrent();
            if(readerGroup.notValid())
			    loadConfiguration();
		}

		public static void loadConfiguration()
		{
			windowsAuthentication_Enabled = TMConfig.Current.WindowsAuthentication.Enabled;
			readerGroup = TMConfig.Current.WindowsAuthentication.ReaderGroup.trim();
			editorGroup = TMConfig.Current.WindowsAuthentication.EditorGroup.trim();
			adminGroup = TMConfig.Current.WindowsAuthentication.AdminGroup.trim();
		}

		public Guid authenticateUserBaseOn_ActiveDirectory()
		{
			var identity = CurrentWindowsIdentity;

			if (identity != null && identity.IsAuthenticated && identity.ImpersonationLevel == TokenImpersonationLevel.Impersonation)
			{
				var userName = identity.Name;
				if (userName.valid())
				{
                    var tmUser = userName.tmUser();
                    if(tmUser.isNull())
                    {
                        tmUser = userName.newUser().tmUser();
                    }
                    if (tmUser.GroupID != (int)calculateUserGroupBasedOnWindowsIdentity())
                    {
                        tmUser.GroupID = (int)calculateUserGroupBasedOnWindowsIdentity();
                        tmUser.save();
                    }
                    return tmUser.login();
				}
			}
			return Guid.Empty;
		}

		public UserGroup calculateUserGroupBasedOnWindowsIdentity()
		{			
		    if (CurrentWindowsIdentity != null)
		    {
		        var principal = new WindowsPrincipal(CurrentWindowsIdentity);
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