using System.Security.Permissions;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public static class TM_Xml_Database_ExtensionMethods_Users_Persistance
	{		
		public static string getTmUsersXmlFile(this string xmlDatabasePath)
		{
			if(xmlDatabasePath.dirExists())
				return xmlDatabasePath.pathCombine("TmUsers.xml");
			"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in getTmUsersXmlFile, provided xmlDatabasePath didn't exist: {0}".error(xmlDatabasePath);
			return null;
		}
		
		public static string getTmUsersXmlFile(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.Current.Path_XmlDatabase.getTmUsersXmlFile();
		}
		
		/*public static string getTmUsersPasswordsXmlFile(this string xmlDatabasePath)
		{
			if(xmlDatabasePath.dirExists())
				return xmlDatabasePath.pathCombine("TmUsers_Passwords.xml");
			"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in getTmUsersPasswordsXmlFile, provided xmlDatabasePath didn't exist: {0}".error(xmlDatabasePath);
			return null;
		}
		
		public static string getTmUsersPasswordsXmlFile(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.Current.Path_XmlDatabase.getTmUsersPasswordsXmlFile();
		}*/

		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static TM_Xml_Database clearUserDatabase(this TM_Xml_Database tmDb)
		{
			tmDb.TMUsers.clear();
			return tmDb;
		}

		public static TM_Xml_Database saveTmUserDataToDisk(this TM_Xml_Database tmDb)
		{
			TM_Xml_Database.saveTmUserObjects(TM_Xml_Database.Current.Path_XmlDatabase);			
			return tmDb;
		}
		
	}
}