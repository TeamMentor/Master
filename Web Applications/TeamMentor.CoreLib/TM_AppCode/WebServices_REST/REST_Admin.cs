using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Web.Routing;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;

namespace TeamMentor.CoreLib.WebServices
{
	[ServiceBehavior				(InstanceContextMode = InstanceContextMode.PerCall			  ), 
     AspNetCompatibilityRequirements(RequirementsMode	 = AspNetCompatibilityRequirementsMode.Allowed)]

	public class REST_Admin : IREST_Admin
	{
		public HttpContextBase		Context			 { get; set; }	
		public HttpSessionStateBase Session			 { get; set; }	
		public TM_WebServices		TmWebServices	 { get; set; }	

		public REST_Admin()
		{
			Context = HttpContextFactory.Current;
			Session = HttpContextFactory.Session;
						

			

			//Context.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			TmWebServices = new TM_WebServices(true);	//Disabling CSRF

			UserGroup.Admin.setThreadPrincipalWithRoles();
					
		}

		public string Version()
		{			
			return this.type().Assembly.version();
		}		

		//Libraries
		public List<Library_V3> libraries()
		{
			return TmWebServices.GetLibraries().librariesV3();
		}
		public Library_V3 library(string nameOrId)
		{
			var library = (nameOrId.isGuid())
							? TmWebServices.GetLibraryById(nameOrId.guid()).libraryV3()
							: TmWebServices.GetLibraryByName(nameOrId).libraryV3();
			return (library.notNull())
				        ? TmWebServices.GetFolderStructure_Library(library.libraryId)
				        : null;
		}
		public List<Folder_V3> folders(string libraryId)
		{
			return TmWebServices.GetFolders(libraryId.guid());
		}
		public View_V3 view(string viewId)
		{
			return TmWebServices.GetViewById(viewId);
		}
		//public TeamMentor_Article article(string articleId)
		public string article(string articleId)
		{
			var article = TmWebServices.GetGuidanceItemById(articleId);
			return article.serialize(false);
			//			return article;		// this was failing
		}
		public string article_Html(string articleId)
		{			
			return TmWebServices.GetGuidanceItemHtml(articleId.guid());
		}

		//User Session 
		public string SessionId()
		{			
			return Session.SessionID;			
		}
		public Guid Login(string username, string password)
		{
			return TmWebServices.Login_PwdInClearText(username, password);
		}
		public Guid Logout()
		{
			return TmWebServices.Logout();
		}




		//RBAC
		public string		RBAC_CurrentIdentity_Name()
		{
			return TmWebServices.RBAC_CurrentIdentity_Name();
		}
		public bool			RBAC_CurrentIdentity_IsAuthenticated()
		{
			return TmWebServices.RBAC_CurrentIdentity_IsAuthenticated();
		}
		public List<string> RBAC_CurrentPrincipal_Roles()
		{
			return TmWebServices.RBAC_CurrentPrincipal_Roles();
		}
		public bool			RBAC_HasRole(string role)
		{
			return TmWebServices.RBAC_HasRole(role);
		}
		public bool			RBAC_IsAdmin()
		{
			return TmWebServices.RBAC_IsAdmin();
		}
		public string		RBAC_SessionCookie()
		{
			return TmWebServices.RBAC_SessionCookie();
		}

		//Admin: User Management

		public User CreateUser_Random()
		{
			return TmWebServices.CreateUser_Random().user();
		}

		public User GetUser_byID(string userId)
		{
			return TmWebServices.GetUser_byID(userId.toInt()).user();
		}

		public List<User> GetUsers_byID(string usersIds)
		{
			var ids = usersIds.split(",").Select((id) => id.toInt()).toList();
			return TmWebServices.GetUsers_byID(ids).users();
		}

		public User GetUser_byName(string name)
		{
			return TmWebServices.GetUser_byName(name).user();
		}

		public List<User> GetUsers()
		{
			return TmWebServices.GetUsers().users();
		}

		/*		public List<TMUser> CreateUsers(List<NewUser> newUsers)
		{
			return TmWebServices.RBAC_CurrentIdentity_Name();
		}

		public List<TMUser> BatchUserCreation(string batchUserData)
		{
			return TmWebServices.RBAC_CurrentIdentity_Name();
		}*/

		public bool DeleteUser(string userId)
		{
			return TmWebServices.DeleteUser(userId.toInt());
		}		
	}

	public static class TMUser_ExtensionMethod
	{
		public static List<User> users(this List<TMUser> tmUsers)
		{
			return (from tmUser in tmUsers select tmUser.user()).toList();
		}

		public static User user(this TMUser tmUser)
		{
			return new User()
				{
					UserId = tmUser.UserID,
					UserName = tmUser.UserName,
					Email = tmUser.EMail,
					FirstName = tmUser.FirstName,
					LastName = tmUser.LastName,
					Company = tmUser.Company
				};
		}
	}
}
	