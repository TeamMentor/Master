using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public static class TM_Xml_Database_ExtensionMethods_Users_Persistance
	{	
	    public static void          loadTmUserObjects(this TM_UserData userData)
	    {
	        var userDataPath = userData.Path_UserData;
            if(userDataPath.dirExists().isFalse())
            {
                "[TM_Xml_Database_ExtensionMethods_Users_Persistance] in loadTmUserObjects, provided userDataPath didn't exist: {0}".error(userDataPath);
                return;
            }
            var tmUsersXmlFile = userData.getTmUsersXmlFile();            
            if(tmUsersXmlFile.fileExists().isFalse())			
                new List<TMUser>().saveAs(tmUsersXmlFile);                        
                
            userData.TMUsers = tmUsersXmlFile.load<List<TMUser>>();	            
        }
        public static void          loadAndCheckUserDatabase(this TM_UserData userData)
        {            
            userData.loadTmUserObjects();
            //check for invalid users
            foreach (var tmUser in userData.TMUsers.toList())
                if (tmUser.UserID == 0)
                {
                    "[loadAndCheckUserDatabase] there was an account with userId=0, so removing it".error();
                    userData.TMUsers.remove(tmUser);
                }
            userData.saveTmUserObjects();
        }        
        public static TM_UserData   saveTmUserObjects(this TM_UserData userData)
        {
            if (TM_Xml_Database.Current.UsingFileStorage)
            {
                "in saveTmUserObjects".info();
                lock (userData)
                {
                    var tmUsersXmlFile = userData.getTmUsersXmlFile();                    
                    userData.TMUsers.saveAs(tmUsersXmlFile);                    
                }
            }
            return userData;
        }			
		public static string        getTmUsersXmlFile(this TM_UserData userData)
		{
		    var userDataPath = userData.Path_UserData;
			if(userDataPath.dirExists())
				return userDataPath.pathCombine("TmUsers.xml");
			"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in getTmUsersXmlFile, provided userDataPath didn't exist: {0}".error(userDataPath);
			return null;
		} 
        public static TM_UserData   saveTmUserDataToDisk(this TM_UserData userData)
		{
			return userData.saveTmUserObjects();						
		}        
	}
}