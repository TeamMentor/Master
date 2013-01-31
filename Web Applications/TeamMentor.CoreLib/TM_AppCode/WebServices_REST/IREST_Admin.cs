using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace TeamMentor.CoreLib
{
	[ServiceContract]
	public interface IREST
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
		[OperationContract][WebGet	 (UriTemplate = "/sessionId"					)]	string		SessionId();
		[OperationContract][WebGet	 (UriTemplate = "/logout"						)]	Guid		Logout();
		[OperationContract][WebGet   (UriTemplate = "/login/{username}/{password}"	)]	Guid		Login(string username, string password);
        
		[OperationContract][WebInvoke(UriTemplate = "/login/", Method = "POST",ResponseFormat = WebMessageFormat.Json)]	Guid		Login_using_Credentials(TM_Credentials credentials);

		//RBAC
		[OperationContract][WebGet(UriTemplate = "/user/name"					)]	string			RBAC_CurrentIdentity_Name();
		[OperationContract][WebGet(UriTemplate = "/user/isAuthenticated"		)]	bool			RBAC_CurrentIdentity_IsAuthenticated();
		[OperationContract][WebGet(UriTemplate = "/user/roles"					)]	List<string>	RBAC_CurrentPrincipal_Roles();
		[OperationContract][WebGet(UriTemplate = "/user/hasRole/{role}"			)]	bool			RBAC_HasRole(string role);
		[OperationContract][WebGet(UriTemplate = "/user/isAdmin"				)]	bool			RBAC_IsAdmin();
		[OperationContract][WebGet(UriTemplate = "/user/sessionCookie"			)]	string			RBAC_SessionCookie();	
	 

		//Admin: User Management				
						
		[OperationContract] [WebGet	  (UriTemplate = "/user/{nameOrId}"			)]					TM_User		user(string nameOrId);
		//[OperationContract] [WebGet	  (UriTemplate = "/users/{usersIds}"	)]					List<TM_User>	users(string usersIds);
		[OperationContract] [WebGet	  (UriTemplate = "/users"				)]						List<TM_User>	users();

		[OperationContract] [WebInvoke(UriTemplate = "/user/update"			,	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	bool		user_Update(TM_User user);
		[OperationContract] [WebInvoke(UriTemplate = "/user/new"			,	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	int			user_New(TM_User user);
		[OperationContract] [WebInvoke(UriTemplate = "/user/delete/{userId}",	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	bool		DeleteUser(string userId);
		

		//Admin
		[OperationContract] [WebGet(UriTemplate = "/admin/reloadCache")]		string		admin_ReloadCache();
        [OperationContract] [WebGet(UriTemplate = "/admin/restart")]		    string		admin_RestartCache();

		//Views 
		[OperationContract] [WebGet(UriTemplate = "/users/.html"			)] Stream		users_html();

		//UserActivities 
		[OperationContract] [WebGet(UriTemplate = "/users/activites"		)] Stream		users_Activities();

	}	
}