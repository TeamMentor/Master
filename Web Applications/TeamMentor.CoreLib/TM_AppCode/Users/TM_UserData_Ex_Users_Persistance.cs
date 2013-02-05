using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public static class TM_UserData_Ex_Users_Persistance
	{	
	    public static TM_UserData   loadTmUserData(this TM_UserData userData)
	    {
            userData.TMUsers = new List<TMUser>();	        
	        if (userData.Path_UserData.dirExists().isFalse())
	        {
	            "[TM_UserData_Ex_Users_Persistance] in loadTmUserObjects, provided userDataPath didn't exist: {0}"
	                .error(userData.Path_UserData);
	        }
	        else
	        {	            
	            foreach (var file in userData.Path_UserData.files("*.userData.xml"))
	            {
	                var tmUser = file.load<TMUser>();
	                if (tmUser.notNull() && tmUser.UserID > 0)
	                    userData.TMUsers.Add(tmUser);
	            }
	        }
	        return userData;
	    }               
        /*public static TM_UserData   saveTmUserData(this TM_UserData userData)
        {
            if (TM_Xml_Database.Current.UsingFileStorage)
            {
                "in saveTmUserObjects".info();
                lock (userData)
                {
                    foreach (var tmUser in userData.TMUsers)
                    {
                        var tmUserXmlFile = userData.Path_UserData.pathCombine("{0}.userData.xml".format(tmUser.ID));
                        tmUser.saveAs(tmUserXmlFile);
                    }
                }
            }
            return userData;
        }*/

	    public static string getTmUserXmlFile(this TMUser tmUser)
	    {
            return TM_UserData.Current.Path_UserData.pathCombine("{0}.userData.xml".format(tmUser.ID));
	    }

	    public static TMUser   saveTmUser(this TMUser tmUser)
        {
            if (TM_Xml_Database.Current.UsingFileStorage)
            {                
                lock (tmUser)
                {                    
                    tmUser.saveAs(tmUser.getTmUserXmlFile());                    
                }
            }
            return tmUser;
        }

        public static bool delete(this List<TMUser> tmUsers, int id)
    	{    		
    		foreach(var tmUser in tmUsers)
    			if (tmUser.UserID == id)
    			{
    				tmUsers.remove(tmUser);   
 			        lock (tmUser)
 			        {
 			            tmUser.getTmUserXmlFile().file_Delete();
 			        }
    				return true;
    			}
    		return false;
    	}
	}
}