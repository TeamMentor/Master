using System.Security.Permissions;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public static class TM_Xml_Database_ExtensionMethods_Users_Persistance
	{		
		public static string getTmUsersXmlFile(this string userDataPath)
		{
			if(userDataPath.dirExists())
				return userDataPath.pathCombine("TmUsers.xml");
			"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in getTmUsersXmlFile, provided userDataPath didn't exist: {0}".error(userDataPath);
			return null;
		}
		
		public static string getTmUsersXmlFile(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.Current.Path_UserData.getTmUsersXmlFile();
		}
		
		[ManageUsers] 
		public static TM_Xml_Database clearUserDatabase(this TM_Xml_Database tmDb)
		{
			tmDb.TMUsers.clear();
			return tmDb;
		}

		public static TM_Xml_Database saveTmUserDataToDisk(this TM_Xml_Database tmDb)
		{
			TM_Xml_Database.saveTmUserObjects(TM_Xml_Database.Current.Path_UserData);			
			return tmDb;
		}
		
	}
}