using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace TeamMentor.CoreLib
{
	[ServiceContract]
	public interface IREST_Admin
	{
		[OperationContract][WebGet(UriTemplate = "/Version"		)]	string Version();				

		//Libraries
		[OperationContract][WebGet(UriTemplate = "/Libraries"				)]	List<Library_V3>	libraries();
		[OperationContract][WebGet(UriTemplate = "/Library/{nameOrId}"		)]	Library_V3			library(string nameOrId);
		[OperationContract][WebGet(UriTemplate = "/Folders/{libraryId}"		)]	List<Folder_V3>		folders(string libraryId);		
		[OperationContract][WebGet(UriTemplate = "/View/{viewId}"			)]	View_V3				view(string viewId);
		[OperationContract][WebGet(UriTemplate = "/article/{articleId}"		)]	string				article(string articleId);
		[OperationContract][WebGet(UriTemplate = "/article/html/{articleId}")]	string				article_Html(string articleId);

		//User Session
		[OperationContract][WebGet	 (UriTemplate = "/SessionId"					)]	string		SessionId();
		[OperationContract][WebGet	 (UriTemplate = "/Logout"						)]	Guid		Logout();
		//[OperationContract][WebGet(UriTemplate = "/Login/{username}/{password}"	)]	Guid		Login(string username, string password);
		[OperationContract][WebInvoke(UriTemplate = "/login", Method = "POST",ResponseFormat = WebMessageFormat.Json)]	Guid		login(TM_Credentials credentials);

		//RBAC
		[OperationContract][WebGet(UriTemplate = "/User_Name"					)]	string			RBAC_CurrentIdentity_Name();
		[OperationContract][WebGet(UriTemplate = "/User_IsAuthenticated"		)]	bool			RBAC_CurrentIdentity_IsAuthenticated();
		[OperationContract][WebGet(UriTemplate = "/User_Roles"					)]	List<string>	RBAC_CurrentPrincipal_Roles();
		[OperationContract][WebGet(UriTemplate = "/User_HasRole/{role}"			)]	bool			RBAC_HasRole(string role);
		[OperationContract][WebGet(UriTemplate = "/User_IsAdmin"				)]	bool			RBAC_IsAdmin();
		[OperationContract][WebGet(UriTemplate = "/User_SessionCookie"			)]	string			RBAC_SessionCookie();	
	 

		//Admin: User Management				
						
		[OperationContract] [WebGet	  (UriTemplate = "/user/{nameOrId}"			)]					TM_User		user(string nameOrId);
		//[OperationContract] [WebGet	  (UriTemplate = "/users/{usersIds}"	)]						List<TM_User>	users(string usersIds);
		[OperationContract] [WebGet	  (UriTemplate = "/users"				)]						List<TM_User>	users();

		[OperationContract] [WebInvoke(UriTemplate = "/user/update"			,	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	bool		user_Update(TM_User user);
		[OperationContract] [WebInvoke(UriTemplate = "/user/new"			,	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	int			user_New(TM_User user);
		[OperationContract] [WebInvoke(UriTemplate = "/user/delete/{userId}",	Method = "PUT", ResponseFormat = WebMessageFormat.Json)]	bool		DeleteUser(string userId);
		

		//Admin
		[OperationContract] [WebInvoke(UriTemplate = "/admin/reloadCache",	Method = "PUT")]		string		admin_ReloadCache();

		//Views 
		[OperationContract] [WebGet(UriTemplate = "/users/.html"			)] Stream		users_html();

		//UserActivities 
		[OperationContract] [WebGet(UriTemplate = "/users/activites"		)] Stream		users_Activities();

	}	
}