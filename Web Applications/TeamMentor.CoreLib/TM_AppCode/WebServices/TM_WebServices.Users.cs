using System.Collections.Generic;
using System.Web.Services;

namespace TeamMentor.CoreLib
{
    
    public partial class TM_WebServices  // Users
    {
        [WebMethod(EnableSession = true)]											public int CreateUser(NewUser newUser)      				 {   return userData.createTmUser(newUser);	                        }
        [WebMethod(EnableSession = true)]											public TM_User CreateUser_Random()      					 {   return userData.tmUser(userData.newUser()).user();	            }		
        [WebMethod(EnableSession = true)]	[Admin]	                    			public TM_User GetUser_byID(int userId)        				 {   return userData.tmUser(userId).user();                         }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> GetUsers_byID(List<int> userIds)      	 {   return userIds.tmUsers().users();                              }
        [WebMethod(EnableSession = true)]   [Admin]	                    			public TM_User GetUser_byName(string name)					 {   return userData.tmUser(name).user();                           }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> GetUsers()        						 {   return userData.tmUsers().users();                             }  	

        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> CreateUsers(List<NewUser> newUsers)     {	return userData.createTmUsers(newUsers).tmUsers().users();      }        
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> BatchUserCreation(string batchUserData) {	return userData.createTmUsers(batchUserData).tmUsers().users(); }        
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool DeleteUser(int userId)	        				 {	return userData.deleteTmUser(userId);                           }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<bool> DeleteUsers(List<int> userIds)        	 {	return userData.deleteTmUsers(userIds);                         }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool UpdateUser(int userId, string userName, 
                                                                                                           string firstname, string lastname, 
                                                                                                           string title, string company, 
                                                                                                           string email, int groupId) 			 {	return userData.updateTmUser(userId, userName, firstname, lastname, title, company, email, groupId); }  			

        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool         SetUserPassword (int userId,  string password) 	{ 	return userData.setUserPassword  (userId, password);   }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public int          GetUserGroupId  (int userId)        			{	return userData.getUserGroupId   (userId);             }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public string       GetUserGroupName(int userId)        			{	return userData.getUserGroupName (userId);             }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool         SetUserGroupId  (int userId, int roleId)  		{	return userData.setUserGroupId   (userId, roleId);     }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<string> GetUserRoles    (int userId)					{	return userData.getUserRoles     (userId);             }  	

        [WebMethod(EnableSession = true)]	[Admin]	                    			public TM_User      SetUser_PostLoginView   (string userName, string postLoginView)	    {	return userName.tmUser().set_PostLoginView(postLoginView).user();        }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public TM_User      SetUser_PostLoginScript (string userName, string postLoginScript)   {	return userName.tmUser().set_PostLoginScript(postLoginScript).user();    }

    }
}
