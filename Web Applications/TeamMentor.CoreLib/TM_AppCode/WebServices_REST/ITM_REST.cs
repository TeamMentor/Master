using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace TeamMentor.CoreLib
{
	[ServiceContract]
	public interface ITM_REST
	{
		[OperationContract][WebGet(UriTemplate = "/version"		)]	string Version();				

		//Libraries
		[OperationContract][WebGet(UriTemplate = "/libraries"				)]	List<Library_V3>	Libraries();
		[OperationContract][WebGet(UriTemplate = "/library/{nameOrId}"		)]	Library_V3			Library(string nameOrId);
		[OperationContract][WebGet(UriTemplate = "/folders/{libraryId}"		)]	List<Folder_V3>		Folders(string libraryId);		
		[OperationContract][WebGet(UriTemplate = "/view/{viewId}"			)]	View_V3				View(string viewId);
		[OperationContract][WebGet(UriTemplate = "/article/{articleId}"		)]	string				Article(string articleId);
		[OperationContract][WebGet(UriTemplate = "/article/html/{articleId}")]	string				Article_Html(string articleId);

		//User Session
		[OperationContract][WebGet	 (UriTemplate = "/sessionId"					     )]	string		SessionId();
		[OperationContract][WebGet	 (UriTemplate = "/logout"						     )]	Guid		Logout();
		[OperationContract][WebGet   (UriTemplate = "/login/{username}/{password}"	     )]	Guid		Login(string username, string password);        
        [OperationContract][WebGet   (UriTemplate = "/passwordResetToken/{username}"	 )]	Guid		GetPasswordResetToken(string username);                                            
        [OperationContract][WebGet   (UriTemplate = "/loginToken/{username}"	         )]	Guid		GetLoginToken(string username);                
        [OperationContract][WebGet   (UriTemplate = "/loginToken/{username}/{loginToken}")]	Guid		Login_Using_LoginToken(string username, string loginToken);
        [OperationContract][WebGet   (UriTemplate = "/sendLoginToken/{username}")]	        bool		SendLoginTokenForUser(string username);
		[OperationContract][WebInvoke(UriTemplate = "/login/", Method = "POST",ResponseFormat = WebMessageFormat.Json)]	Guid		Login_using_Credentials(TM_Credentials credentials);

        
        
        
        //Active Sessions
//        [OperationContract][WebGet	 (UriTemplate = "/sessions/all"					)]	List<string>    ActiveSessions();
//        [OperationContract][WebGet	 (UriTemplate = "/sessions/{sessionId}"		    )]	TMUser          ActiveSession(string sessionId);
 
		//User and RBAC
        [OperationContract][WebGet(UriTemplate = "/user/loggedIn"				)]	bool		    User_LoggedIn();
		[OperationContract][WebGet(UriTemplate = "/user/name"					)]	string			RBAC_CurrentIdentity_Name();
		[OperationContract][WebGet(UriTemplate = "/user/isAuthenticated"	    )]	bool			RBAC_CurrentIdentity_IsAuthenticated();
		[OperationContract][WebGet(UriTemplate = "/user/roles"					)]	List<string>	RBAC_CurrentPrincipal_Roles();
		[OperationContract][WebGet(UriTemplate = "/user/hasRole/{role}"			)]	bool			RBAC_HasRole(string role);
		[OperationContract][WebGet(UriTemplate = "/user/isAdmin"				)]	bool			RBAC_IsAdmin();		
	 

		//Admin: User Management				
						
		[OperationContract] [WebGet	  (UriTemplate = "/user/{nameOrId}"			)]					TM_User		user(string nameOrId);
		//[OperationContract] [WebGet	  (UriTemplate = "/users/{usersIds}"	)]					List<TM_User>	users(string usersIds);
		[OperationContract] [WebGet	  (UriTemplate = "/users"				)]						List<TM_User>	users();

//these need CRSF protection
//		[OperationContract] [WebInvoke(UriTemplate = "/user/update"			,	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	bool		user_Update(TM_User user);
//		[OperationContract] [WebInvoke(UriTemplate = "/user/new"			,	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	int			user_New(TM_User user);
//		[OperationContract] [WebInvoke(UriTemplate = "/user/delete/{userId}",	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	bool		DeleteUser(string userId);
		

		//Admin
		[OperationContract] [WebGet(UriTemplate = "/admin/reloadCache")]		string		Admin_ReloadCache();
        [OperationContract] [WebGet(UriTemplate = "/admin/restart")]		    string		Admin_Restart();

        [OperationContract] [WebGet(UriTemplate = "/admin/script/{name}")]	    string		Admin_InvokeScript(string name);
		//Views 
		//[OperationContract] [WebGet(UriTemplate = "/users/.html"			)]  Stream		users_html();

		//UserActivities 
		//[OperationContract] [WebGet(UriTemplate = "/users/activites"		)]  Stream		users_Activities();

        //TBot
        [OperationContract] [WebGet(UriTemplate = "/tbot"	        )]      Stream		TBot_Show();
        [OperationContract] [WebGet(UriTemplate = "/tbot/list"      )]	    Stream		TBot_List();
        [OperationContract] [WebGet(UriTemplate = "/tbot/run/{what}")]	    Stream		TBot_Run (string what);
        
        //html page redirects
        [OperationContract][WebGet   (UriTemplate = "/redirect/passwordReset/{username}" )]	void		Redirect_ToPasswordReset(string username);                                    
        [OperationContract][WebGet   (UriTemplate = "/redirect/login/{referer}" )]	        void		Redirect_Login(string referer);
	}	
}