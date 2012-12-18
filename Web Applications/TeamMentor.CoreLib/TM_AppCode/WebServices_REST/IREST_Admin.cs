using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Web.Routing;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.WebClient.WebServices;

namespace TeamMentor.CoreLib.WebServices
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
		[OperationContract][WebGet(UriTemplate = "/SessionId"	)]	string SessionId();
		[OperationContract][WebGet(UriTemplate = "/Logout"						)]	Guid Logout();
		[OperationContract][WebGet(UriTemplate = "/Login/{username}/{password}"	)]	Guid Login(string username, string password);

		//RBAC
		[OperationContract][WebGet(UriTemplate = "/User_Name"					)]	string			RBAC_CurrentIdentity_Name();
		[OperationContract][WebGet(UriTemplate = "/User_IsAuthenticated"		)]	bool			RBAC_CurrentIdentity_IsAuthenticated();
		[OperationContract][WebGet(UriTemplate = "/User_Roles"					)]	List<string>	RBAC_CurrentPrincipal_Roles();
		[OperationContract][WebGet(UriTemplate = "/User_HasRole/{role}"			)]	bool			RBAC_HasRole(string role);
		[OperationContract][WebGet(UriTemplate = "/User_IsAdmin"				)]	bool			RBAC_IsAdmin();
		[OperationContract][WebGet(UriTemplate = "/User_SessionCookie"			)]	string			RBAC_SessionCookie();	
	 

		//Admin: User Management				

		[OperationContract] [WebGet(UriTemplate = "/users/new"				)]					User		users_New();
		[OperationContract] [WebGet(UriTemplate = "/user/id/{userId}"		)]					User		GetUser_byID(string userId);
		[OperationContract] [WebGet(UriTemplate = "/users/id/{usersIds}"	)]					List<User>	GetUsers_byID(string usersIds);
		[OperationContract] [WebGet(UriTemplate = "/user/{name}"			)]					User		GetUser_byName(string name);
		[OperationContract] [WebGet(UriTemplate = "/users"					)]					List<User>	users();

		[OperationContract] [WebInvoke(UriTemplate = "/user/delete/{userId}",	Method = "PUT")]	bool		DeleteUser(string userId);
		

		//Admin
		[OperationContract] [WebInvoke(UriTemplate = "/admin/reloadCache",	Method = "PUT")]		string		admin_ReloadCache();

		//Views 
		[OperationContract] [WebGet(UriTemplate = "/users/.html"			)] Stream		users_html();
	}

	[DataContract]
	public class User
	{
		[DataMember] public int	   UserId    { get; set; }
		[DataMember] public string UserName  { get; set; }
		[DataMember] public string FirstName { get; set; }
		[DataMember] public string LastName	 { get; set; }
		[DataMember] public string Email	 { get; set; }
		[DataMember] public string Company	 { get; set; }
	}
}