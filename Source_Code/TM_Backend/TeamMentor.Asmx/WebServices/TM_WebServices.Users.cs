using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Services;
using FluentSharp.CoreLib.API;
using TeamMentor.UserData;

namespace TeamMentor.CoreLib
{    
    public partial class TM_WebServices  // Users
    {
        [WebMethod(EnableSession = true)]											public bool PasswordReset(string userName, Guid token, string newPassword)	 {   return userData.passwordReset(userName, token, newPassword);   }

        [WebMethod(EnableSession = true)]											public int  CreateUser(NewUser newUser)      				        {   return userData.createTmUser(newUser);	                                }
        [WebMethod(EnableSession = true)]											public List<String> CreateUser_Validate(NewUser newUser)            {   return newUser.validate().asStringList();	                                }        
        [WebMethod(EnableSession = true)]											public TM_User CreateUser_Random()      					        {   return userData.tmUser(userData.newUser()).user();	                    }
        [WebMethod(EnableSession = true)]		                    			    public bool SendPasswordReminder(string email)                      {   email.sendPasswordReminder();   return true;                            }   // always return true
        [WebMethod(EnableSession = true)]		                    			    public string GetCurrentUserPasswordExpiryUrl()                     {   return Current_User().passwordExpiredUrl();                             }   // always return true
        [WebMethod(EnableSession = true)]	[Admin]	                   			    public Guid NewPasswordResetToken(string email)                     {   admin.demand(); return email.tmUser_FromEmail().passwordResetToken_getTokenAndSetHash();}
        //[WebMethod(EnableSession = true)]	[Admin]	                    			public Guid GetLoginToken(string userName)        			        {   return userName.tmUser().current_SingleUseLoginToken(true);             }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public TM_User GetUser_byID(int userId)        				        {   admin.demand(); return userData.tmUser(userId).user();                                  }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> GetUsers_byID(List<int> userIds)      	        {   admin.demand(); return userIds.tmUsers().users();                                       }
        [WebMethod(EnableSession = true)]   [Admin]	                    			public TM_User GetUser_byName(string name)					        {   admin.demand(); return userData.tmUser(name).user();                                    }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> GetUsers()        						        {   admin.demand(); return userData.tmUsers().users();                                      }  	

        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> CreateUsers(List<NewUser> newUsers)     {	admin.demand(); return userData.createTmUsers(newUsers).tmUsers().users();      }        
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<TM_User> BatchUserCreation(string batchUserData) {	admin.demand(); return userData.createTmUsers(batchUserData).tmUsers().users(); }        
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool DeleteUser(int userId)	        				 {	admin.demand(); return userData.deleteTmUser(userId);                           }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<bool> DeleteUsers(List<int> userIds)        	 {	admin.demand(); return userData.deleteTmUsers(userIds);                         }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool UpdateTmUser(TM_User user)                       {  admin.demand(); return userData.updateTmUser(user);                             }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool UpdateUser(int userId, string userName, 
                                                                                                           string firstname, string lastname, 
                                                                                                           string title, string company, string email, string country,
                                                                                                           string state, DateTime accountExpiration, bool passwordExpired,
                                                                                                           bool userEnabled, bool accountNeverExpires, int groupId) 			    {	admin.demand(); return userData.updateTmUser(userId, userName, firstname, lastname, title, company, email,country, state, accountExpiration, passwordExpired, userEnabled,accountNeverExpires, groupId); }  			

        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool         SetUserPassword (int userId,  string password) 	{ 	admin.demand(); return userData.setUserPassword  (userId, password);   }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public int          GetUserGroupId  (int userId)        			{	admin.demand(); return userData.getUserGroupId   (userId);             }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public string       GetUserGroupName(int userId)        			{	admin.demand(); return userData.getUserGroupName (userId);             }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public bool         SetUserGroupId  (int userId, int roleId)  		{	admin.demand(); return userData.setUserGroupId   (userId, roleId);     }  	
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<string> GetUserRoles    (int userId)					{	admin.demand(); return userData.getUserRoles     (userId);             }  	

        [WebMethod(EnableSession = true)]	[Admin]	                                public bool         SendEmail               (EmailMessage_Post emailMessagePost)        {   admin.demand(); return new SendEmails().send(emailMessagePost);                     }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public List<Guid>   GetUser_AuthTokens      (int userId)                                {	admin.demand(); return  userData.getUserAuthTokens(userId);                              }
        [WebMethod(EnableSession = true)]	[Admin]	                    			public Guid         CreateUser_AuthToken    (int userId)                                {	admin.demand(); return  userData.createUserAuthToken(userId);                            }        
//        [WebMethod(EnableSession = true)]	[Admin]	                    			public TM_User      SetUser_PostLoginView   (string userName, string postLoginView)	    {	return userName.tmUser().set_PostLoginView(postLoginView).user();        }
//        [WebMethod(EnableSession = true)]	[Admin]	                    			public TM_User      SetUser_PostLoginScript (string userName, string postLoginScript)   {	return userName.tmUser().set_PostLoginScript(postLoginScript).user();    }
        
    }
}
