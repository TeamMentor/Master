using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.UnitTests.TM_Website.WebServices;

namespace TeamMentor.UnitTests.TM_Website
{
    public static class TM_WebServices_Users_ExtensionMethods
    {        
        public static int           add_User            (this TM_WebServices webServices)                   
        {
            return webServices.add_User(10.randomLetters(), "!!123".add_RandomLetters(10));
        }
    	public static int           add_User            (this TM_WebServices webServices, string username, 	string password)
    	{
            var newUser = new NewUser
    		    {
    		        Username = username, 
                    Password = password, 
                    Email    = "{0}@{1}.com".format(10.randomLetters(), 10.randomLetters()),                    
    		    };
            
    		return webServices.add_User(newUser); 
    	}    	
    	public static int           add_User            (this TM_WebServices webServices, NewUser newUser)  
		{
            var userValidationErrors = webServices.CreateUser_Validate(newUser);
            if (userValidationErrors.notEmpty())
            {
                "[add_User] There where validations: {0}".error(userValidationErrors.asString());
                return -1;
            }
    	    var userId =  webServices.CreateUser(newUser);

            return userId;
		}
        
        public static TM_User       current_User        (this TM_WebServices webServices)                   
        {
            return webServices.Current_User();                                    
        }
        public static Guid          current_SessionId   (this TM_WebServices webServices)                   
        {
            return webServices.Current_SessionID();
        }
  
		public static bool          delete_User         (this TM_WebServices webServices, string userName)  
		{
			var tmUser = webServices.user(userName);
			return webServices.delete_User(tmUser);
			
		}
        public static bool          delete_User         (this TM_WebServices webServices, TM_User tmUser)
        {
            if(tmUser.notNull())
				return webServices.DeleteUser(tmUser.UserId);
            return false;
        }
		public static bool          delete_User         (this TM_WebServices webServices, int userId)       
		{
			return webServices.DeleteUser(userId);
		}

        public static bool          logout              (this TM_WebServices webServices)                   
        {
            return webServices.Logout() == Guid.Empty;
        }        
     
        public static bool          login_with_AuthToken(this TM_WebServices webServices, Guid authToken)   
    	{
    		try
    		{
	    		"[TeamMentor] login using AuthToken".info();
                webServices.Login_Using_AuthToken(authToken);
                return webServices.Current_User().notNull();                    
	    	}
	    	catch(Exception ex)
	    	{
	    		ex.log();
	    	}
	    	return false;
    	}
    	public static bool          login_with_Pwd      (this TM_WebServices webServices, string username, string password)
    	{
    		try
    		{
	    		"[TeamMentor] login as: {0}".info(username);
                webServices.Login(username, password);
                return webServices.Current_User().notNull();                    
	    	}
	    	catch(Exception ex)
	    	{
	    		ex.log();
	    	}
	    	return false;
    	}    	
        public static bool          loggedIn            (this TM_WebServices webServices)                   
    	{
    		return webServices.sessionId() != Guid.Empty;
    	}

    	public static Guid          sessionId           (this TM_WebServices webServices)                   
    	{
    		return webServices.Current_SessionID();
    	}
    	
        
    	public static List<TM_User> users               (this TM_WebServices webServices)                   
    	{
    		return webServices.GetUsers().toList();
    	}    	
        public static TM_User       user                (this TM_WebServices webServices, int userId)       
    	{
    		return webServices.GetUser_byID(userId);
    	}
    	public static TM_User       user                (this TM_WebServices webServices, string userName)  
    	{
    		return webServices.GetUser_byName(userName);
    	}    	
        public static Guid          user_AuthToken      (this TM_WebServices webServices, int userId)       
        {
            var existingTokens = webServices.GetUser_AuthTokens(userId);
            if (existingTokens.size()>0)
                return existingTokens.first();
            return  webServices.CreateUser_AuthToken(userId);                        
        }
    }
}
