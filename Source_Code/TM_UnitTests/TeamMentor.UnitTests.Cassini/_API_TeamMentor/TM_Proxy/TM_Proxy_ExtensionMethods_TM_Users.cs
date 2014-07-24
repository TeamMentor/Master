using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_TM_Users
    {           
        public static TMUser  user               (this int userId, TM_Proxy tmProxy)
        {
            return tmProxy.user(userId);
        }     
        public static TMUser  user               (this string userName, TM_Proxy tmProxy) 
        {
            return tmProxy.user(userName);
        }     
        public static TMUser  user               (this TM_Proxy tmProxy, int userId)                                  
        {
            return tmProxy.invoke_Static<TMUser>(typeof(TM_UserData_Ex_Users),"tmUser", userId);
        }     
        public static TMUser  user               (this TM_Proxy tmProxy, string userName)                                  
        {
            return tmProxy.invoke_Static<TMUser>(typeof(TM_UserData_Ex_Users),"tmUser", userName);
        }
        public static List<TMUser>  users               (this TM_Proxy tmProxy)                                  
        {
            if(tmProxy.TmUserData.notNull())
                return tmProxy.TmUserData.TMUsers;
            return new List<TMUser>();
        }     
        public static List<TMUser>  user_Admins         (this TM_Proxy tmProxy)                                  
        {
            return tmProxy.users().where(user=>user.GroupID==(int)UserGroup.Admin);
        }
        public static List<TMUser>  user_Readers        (this TM_Proxy tmProxy)                                  
        {
            return tmProxy.users().where(user=>user.GroupID==(int)UserGroup.Reader);
        }
        public static AuthToken     user_AuthToken_Add  (this TM_Proxy tmProxy, TMUser tmUser)                   
        {
            //var authToken = tmProxy.invoke_Static(typeof(TokenAuthentication_ExtensionMethods), "add_AuthToken", tmUser);

            return tmProxy.invoke_Static<AuthToken>(typeof(TokenAuthentication_ExtensionMethods), "add_AuthToken", tmUser);
        }
        public static Guid          user_AuthToken_Valid(this TM_Proxy tmProxy, TMUser tmUser)                   
        {
            if(tmUser.AuthTokens.empty())
                return tmProxy.user_AuthToken_Add(tmUser).Token;
            return tmUser.AuthTokens.first().Token;
        }
        public static TM_Proxy      admin_Assert        (this TM_Proxy tmProxy)                                  
        {
            UserGroup.Admin.assert();
            return tmProxy;
        }
        public static TM_Proxy      editor_Assert       (this TM_Proxy tmProxy)                                  
        {
            UserGroup.Editor.assert();
            return tmProxy;
        }
        public static TMUser        user_New            (this TM_Proxy tmProxy)
        {
            return tmProxy.user_New("test_user".add_5_RandomLetters());
        }
        public static TMUser        user_New            (this TM_Proxy tmProxy, string userName)                 
	    {
		    return tmProxy.invoke_Static<TMUser>(typeof(Users_Creation),"createUser", userName);
	    }
        public static TMUser        user_New            (this TM_Proxy tmProxy, string userName, string password)
	    {
		    var userId =  tmProxy.invoke_Static<int>(typeof(Users_Creation),"newUser",tmProxy.TmUserData, userName, password);
            return tmProxy.user(userId);
	    }
   
        public static bool          user_Logout         (this TM_Proxy tmProxy, TMUser tmUser)
        {
            return tmProxy.invoke_Static<bool>(typeof(TM_UserData_Ex_ActiveSessions),"logout", tmUser);            
        }
        public static TMUser        user_Make_Admin     (this TM_Proxy tmProxy, TMUser tmUser)
        {
            return tmProxy.invoke_Static<TMUser>(typeof(TMUser_ExtensionMethods),"make_Admin", tmUser);            
        }
        
    }
}