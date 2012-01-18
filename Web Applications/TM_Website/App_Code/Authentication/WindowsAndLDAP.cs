using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using SecurityInnovation.TeamMentor.WebClient.WebServices;

//O2File:../XmlDatabase/TM_Xml_Database.Users.cs
//O2Ref:System.DirectoryServices.AccountManagement.dll

namespace SecurityInnovation.TeamMentor.Authentication
{
	public class WindowsAndLDAP
	{
		public static Guid loginOnLocalMachine(string username, string password)
		{
			if(authenticateOnLocalMachiche(username, password))
				return username.registerUserSession(Guid.NewGuid());
			else
				return Guid.Empty;
		}
		public static bool authenticateOnLocalMachiche(string username, string password)
		{
			bool valid = false;
			using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
			{
				valid = context.ValidateCredentials( username, password );
			}
			return valid;
		}
		
		//modify the ContextType.* to authenticate against an ActiveDirectory Domain (or LDAP)
		/*public static bool authenticateOnLocalMachiche(string username, string password)
		{
			bool valid = false;
			using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
			{
				valid = context.ValidateCredentials( username, password );
			}
			return valid;
		}*/
	}
}