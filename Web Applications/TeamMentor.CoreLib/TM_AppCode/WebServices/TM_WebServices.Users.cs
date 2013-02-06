using System.Collections.Generic;
using System.Web.Services;

namespace TeamMentor.CoreLib
{
    
    public partial class TM_WebServices  // Users
    {
        [WebMethod(EnableSession = true)]											public int CreateUser(NewUser newUser)      				{   return tmXmlDatabase.createTmUser(newUser); 	        }
        [WebMethod(EnableSession = true)]											public TMUser CreateUser_Random()      						{   return tmXmlDatabase.tmUser(tmXmlDatabase.newUser());	}		
        [WebMethod(EnableSession = true)]	[Admin]	                    			public TMUser GetUser_byID(int userId)        				{   return tmXmlDatabase.tmUser(userId);                    }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TMUser> GetUsers_byID(List<int> userIds)      	{   return userIds.tmUsers();                               }
        [WebMethod(EnableSession = true)]   [Admin]	                    			public TMUser GetUser_byName(string name)					{   return tmXmlDatabase.tmUser(name);                      }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TMUser> GetUsers()        						{   return tmXmlDatabase.tmUsers();                         }  	

        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TMUser> CreateUsers(List<NewUser> newUsers)    	{	return tmXmlDatabase.createTmUsers(newUsers).tmUsers();         }        
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TMUser> BatchUserCreation(string batchUserData) {	return tmXmlDatabase.createTmUsers(batchUserData).tmUsers();    }        
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool DeleteUser(int userId)	        				{	return tmXmlDatabase.deleteTmUser(userId);                      }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<bool> DeleteUsers(List<int> userIds)        	{	return tmXmlDatabase.deleteTmUsers(userIds);                    }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool UpdateUser(int userId, string userName, 
                                                                                                           string firstname, string lastname, 
                                                                                                           string title, string company, 
                                                                                                           string email, int groupId) 			{	return tmXmlDatabase.updateTmUser(userId, userName, firstname, lastname, title, company, email, groupId); }  			

        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool         SetUserPassword (int userId,  string password) 	{ 	return tmXmlDatabase.setUserPassword  (userId, password);   }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public int          GetUserGroupId  (int userId)        			{	return tmXmlDatabase.getUserGroupId   (userId);             }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public string       GetUserGroupName(int userId)        			{	return tmXmlDatabase.getUserGroupName (userId);             }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool         SetUserGroupId  (int userId, int roleId)  		{	return tmXmlDatabase.setUserGroupId   (userId, roleId);     }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<string> GetUserRoles    (int userId)					{	return tmXmlDatabase.getUserRoles     (userId);             }  	

        [WebMethod(EnableSession = true)]	[Admin]	                    			public TMUser       SetUser_PostLoginView   (string userName, string postLoginView)	    {	return userName.tmUser().set_PostLoginView(postLoginView);        }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public TMUser       SetUser_PostLoginScript (string userName, string postLoginScript)   {	return userName.tmUser().set_PostLoginScript(postLoginScript);    }

    }
}
