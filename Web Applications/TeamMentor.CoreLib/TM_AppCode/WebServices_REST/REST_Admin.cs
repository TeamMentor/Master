using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	[ServiceBehavior				(InstanceContextMode = InstanceContextMode.PerCall			  ), 
     AspNetCompatibilityRequirements(RequirementsMode	 = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class REST_Admin : IREST_Admin
	{
		public HttpContextBase		Context			 { get; set; }	
		public HttpSessionStateBase Session			 { get; set; }	
		public TM_WebServices		TmWebServices	 { get; set; }	

		[LogUrl("REST")]
		public REST_Admin()
		{
			Context = HttpContextFactory.Current;
			Session = HttpContextFactory.Session;
									
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
		public Library_V3		library(string nameOrId)
		{
			var library = (nameOrId.isGuid())
							? TmWebServices.GetLibraryById(nameOrId.guid()).libraryV3()
							: TmWebServices.GetLibraryByName(nameOrId).libraryV3();
			return (library.notNull())
				        ? TmWebServices.GetFolderStructure_Library(library.libraryId)
				        : null;
		}
		public List<Folder_V3>	folders(string libraryId)
		{
			return TmWebServices.GetFolders(libraryId.guid());
		}
		public View_V3			view(string viewId)
		{
			return TmWebServices.GetViewById(viewId);
		}

		//public TeamMentor_Article article(string articleId)
		public string			article(string articleId)
		{
			var article = TmWebServices.GetGuidanceItemById(articleId);
			return article.serialize(false);
			//			return article;		// this was failing
		}
		public string			article_Html(string articleId)
		{			
			return TmWebServices.GetGuidanceItemHtml(articleId.guid());
		}

		//User Session 
		public string			SessionId()
		{			
			return Session.SessionID;			
		}
		public Guid			login(TM_Credentials credentials)
		{
			return TmWebServices.Login_PwdInClearText(credentials.UserName, credentials.Password);
		}
		public Guid				Logout()
		{
			return TmWebServices.Logout();
		}

		//RBAC
		public string			RBAC_CurrentIdentity_Name()
		{
			return TmWebServices.RBAC_CurrentIdentity_Name();
		}
		public bool				RBAC_CurrentIdentity_IsAuthenticated()
		{
			return TmWebServices.RBAC_CurrentIdentity_IsAuthenticated();
		}
		public List<string>		RBAC_CurrentPrincipal_Roles()
		{
			return TmWebServices.RBAC_CurrentPrincipal_Roles();
		}
		public bool				RBAC_HasRole(string role)
		{
			return TmWebServices.RBAC_HasRole(role);
		}
		public bool				RBAC_IsAdmin()
		{
			return TmWebServices.RBAC_IsAdmin();
		}
		public string			RBAC_SessionCookie()
		{
			return TmWebServices.RBAC_SessionCookie();
		}

		//Admin: User Management

		/*public TM_User				users_New()
		{
			return TmWebServices.CreateUser_Random().user();
		}*/
		public int user_New(TM_User user)
		{
			return TmWebServices.CreateUser(user.newUser());
		}
		public TM_User				user(string userNameOrId)
		{
			var user = TmWebServices.GetUser_byID(userNameOrId.toInt()).user();
			if (user.notNull())
				return user;
			return TmWebServices.GetUser_byName(userNameOrId).user();
		}
		public List<TM_User>		users(string usersIds)
		{
			var ids = usersIds.split(",").Select((id) => id.toInt()).toList();
			return TmWebServices.GetUsers_byID(ids).users();
		}				
		public List<TM_User>		users()
		{
			return TmWebServices.GetUsers().users();
		}

		public bool				DeleteUser(string userId)
		{
			return TmWebServices.DeleteUser(userId.toInt());
		}

		public string admin_ReloadCache()
		{
			UserGroup.Admin.setThreadPrincipalWithRoles();
			var response = TmWebServices.XmlDatabase_ReloadData();
			UserGroup.Anonymous.setThreadPrincipalWithRoles();
			return response;
		}
	}

	public static class TMUser_ExtensionMethod
	{
		public static List<TM_User> users(this List<TMUser> tmUsers)
		{
			return (from tmUser in tmUsers select tmUser.user()).toList();
		}
		public static TM_User user(this TMUser tmUser)
		{
			if (tmUser.isNull())
				return null;
			return new TM_User()
				{
					UserId		= tmUser.UserID,
					UserName	= tmUser.UserName,
					Email		= tmUser.EMail,
					FirstName	= tmUser.FirstName,
					LastName	= tmUser.LastName,
					Company		= tmUser.Company,
					Title		= tmUser.Title
				};
		}
		public static NewUser newUser(this TM_User user)
		{
			return new NewUser()
				{
					username = user.UserName,
					email = user.Email,
					firstname = user.FirstName,
					lastname = user.LastName,
					title = user.Title,
					company = user.Company
				};
		}
	}

	public static class IREST_Admin_ExtensionMethods
	{
		public static IREST_Admin response_ContentType_Html(this IREST_Admin iRest_Admin)
		{
			return iRest_Admin.response_ContentType("text/html");
		}

		public static IREST_Admin response_ContentType(this IREST_Admin iRest_Admin, string contentType)
		{
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = contentType;
			return iRest_Admin;
		}
	}
}
	