using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;  
using System.Web.Services;
using System.Security.Permissions;	
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using O2.Kernel.ExtensionMethods;
using Microsoft.Practices.Unity;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;


//O2File:../Authentication/ExtensionMethods/TeamMentorUserManagement_ExtensionMethods.cs
//O2File:../Authentication/UserRoleBaseSecurity.cs
//O2File:../Authentication/WindowsAndLDAP.cs
//O2File:../UnityInjection.cs
//O2File:../DataViewers/JsTreeNode.cs
//O2File:../O2_Scripts_APIs/_O2_Scripts_Files.cs

//O2File:TM_WebServices.ActivityTracking.cs

//O2File:TM_WebServices.Config.cs
//O2File:TM_WebServices.DataViewers.cs
//O2File:TM_WebServices.GuiHelpers.cs

//O2Ref:System.Web.dll
//O2Ref:System.Web.Services.dll
//O2Ref:System.Web.Extensions.dll 
//O2Ref:Microsoft.Practices.Unity.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    /// <summary>
    /// Summary description for Authentication
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public partial class TM_WebServices : System.Web.Services.WebService 
    {
		[Dependency]
		public IJavascriptProxy javascriptProxy {get;set;}
		
		public ActivityTracking activityTracking {get;set;}
		
		//public new HttpContextBase MyContext { get; set;}
		
		//This stores the adminSessionID value in a session variable
		//MOVE THIS TO AN HTTPMODULE
		private Guid _sessionID;	// for unit tests
				
		public Guid sessionID
		{
			get 
			{
				try
				{
					// first check if there s a session variable already set
					if (Session["sessionID"].notNull())
						return (Guid)Session["sessionID"];
					// then check the cookie
					var sessionCookie = System.Web.HttpContext.Current.Request.Cookies["Session"];
					if (sessionCookie.notNull() && sessionCookie.Value.isGuid())
						return sessionCookie.Value.guid();							
					var sessionHeader = System.Web.HttpContext.Current.Request.Headers["Session"];
					if (sessionHeader.notNull() && sessionHeader.isGuid())
						return sessionHeader.guid();								
					//if none is set, return an empty Guid	
					return Guid.Empty;
				}
				catch//(Exception ex) // this will happen on the unit tests
				{					
					//"sessionID.get: {0}".error(ex.Message);
					//System.Web.HttpContext.Current.Response.Write("\n\nERROR: {0} ---\n\n".format(ex.Message));
					return _sessionID;
				}                 
			 	
			}
			
			set
			{				
			//	MyContext.Session["sessionID"] = value;
				
				try
				{					
					//if (Session.notNull())
					Session["sessionID"] = value;
					//var sessionCookie = System.Web.HttpContext.Current.Request.Cookies["Session"];
					//if (sessionCookie.isNull())
					//{
					var sessionCookie = new HttpCookie("Session", value.str());
					sessionCookie.HttpOnly = true;
					System.Web.HttpContext.Current.Response.Cookies.Add(sessionCookie);
					//}
				}
				catch//(Exception ex) // this will happen on the unit tests
				{
					_sessionID = value;
					//"sessionID.set: {0}".error(ex.Message);
				}				
				if (value!= Guid.Empty)
					new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(value);
			}
		}
		
		public TMUser currentUser 
		{
			get 
			{
				try
				{
					return sessionID.session_TmUser();
				}
				catch
				{
					return new TMUser();
				}
			}			
		}
		
        public TM_WebServices()
        {			
            UnityInjection.resolve(this);
			try
			{                
                javascriptProxy.adminSessionID = sessionID;                                    
			}
			catch(Exception ex)	// this will happen on the unit tests
			{
				"TM_WebServices.ctor: {0}".error(ex.Message);
			}
			if (sessionID!= Guid.Empty)
				new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(sessionID);						
				
			if (GetCurrentUserRoles().size()==0)
				if (TMConfig.Current.ShowContentToAnonymousUsers)
					UserGroup.Reader.setThreadPrincipalWithRoles();
				else
					UserGroup.Anonymous.setThreadPrincipalWithRoles();		
			
			activityTracking = new ActivityTracking();
			activityTracking.LogRequest();
        }        
        

		//******** javascriptProxy User Management   no admin privs needed
        [WebMethod(EnableSession = true)]											public int CreateUser(NewUser newUser)      				{   return javascriptProxy.CreateUser(newUser); 	}
		[WebMethod(EnableSession = true)]											public TMUser CreateUser_Random()      						{   return javascriptProxy.CreateUser_Random(); 	}		        
		
        //******** javascriptProxy User Management   (all demand admin privs)     
        [WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public TMUser GetUser_byID(int userId)        				{   return javascriptProxy.GetUser_byID(userId); ;        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public List<TMUser> GetUsers_byID(List<int> userIds)      	{   return javascriptProxy.GetUsers_byID(userIds); ;       }
		[WebMethod(EnableSession = true)]   [Admin(SecurityAction.Demand)]			public TMUser GetUser_byName(string name)					{   return javascriptProxy.GetUser_byName(name);        }
        [WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public List<TMUser> GetUsers()        						{   return javascriptProxy.GetUsers();        		}         

		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public List<TMUser> CreateUsers(List<NewUser> newUsers)    	{	return javascriptProxy.CreateUsers(newUsers).tmUsers();        }        
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public List<TMUser> BatchUserCreation(string batchUserData) {	return javascriptProxy.BatchUserCreation(batchUserData).tmUsers();  }        
        [WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public bool DeleteUser(int userId)	        				{	return javascriptProxy.DeleteUser(userId);        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public List<bool> DeleteUsers(List<int> userIds)        	{	return javascriptProxy.DeleteUsers(userIds);        }
        [WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public bool UpdateUser(int userId, string userName, 
																										   string firstname, string lastname, 
																										   string title, string company, 
																										   string email, int groupId) 		{	return javascriptProxy.UpdateUser(userId, userName, firstname, lastname, title, company, email, groupId);        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public bool SetUserPasswordHash(int userId,  string passwordHash) 					{ 	return javascriptProxy.SetUserPasswordHash(userId, passwordHash) ;        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public int GetUserGroupId(int userId)        			{	return javascriptProxy.GetUserGroupId(userId);        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public string GetUserGroupName(int userId)        		{	return javascriptProxy.GetUserGroupName(userId);        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public bool SetUserGroupId(int userId, int roleId)  	{	return javascriptProxy.SetUserGroupId(userId, roleId);        }
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public List<string> GetUserRoles(int userId)			{	return javascriptProxy.GetUserRoles(userId);        }
		
	 	
		
        //********  Session Management (& Login)
        [WebMethod(EnableSession = true)]	public Guid LoginToWindows(string username, string password)  		{
																													return sessionID = SecurityInnovation.TeamMentor.Authentication.WindowsAndLDAP.loginOnLocalMachine(username,password);
																												}		

        
        [WebMethod(EnableSession = true)]	public Guid Login(string username, string passwordHash)      		{   return sessionID = javascriptProxy.Login(username, passwordHash); }		
		[WebMethod(EnableSession = true)]	public Guid Login_PwdInClearText(string username, string password)	{	return sessionID = javascriptProxy.Login_PwdInClearText(username, password); }		
		[WebMethod(EnableSession = true)]	public Guid Logout()      											{																													
																													sessionID = Guid.Empty;			
																													return sessionID;																													
																												}																												
		[WebMethod(EnableSession = true)]	public Guid Current_SessionID()										{	return sessionID;  }
		[WebMethod(EnableSession = true)]	public TMUser Current_User()										{	return currentUser;  }
		[WebMethod(EnableSession = true)]	public List<string> GetCurrentUserRoles()							{	return sessionID.session_UserRoles().toStringList();  }		
		
		//********  Libraries		
		[WebMethod(EnableSession = true)]	public List<TM_Library> GetLibraries()										{	return javascriptProxy.GetLibraries();	}		
		[WebMethod(EnableSession = true)]	public List<Folder_V3> 	GetAllFolders()										{	return javascriptProxy.GetFolders();	}		
		[WebMethod(EnableSession = true)]	public List<View_V3> 	GetAllViews()										{	return javascriptProxy.GetViews();		}		
		[WebMethod(EnableSession = true)]	public List<Folder_V3> GetFolders(Guid libraryId)							{	return javascriptProxy.GetFolders(libraryId);	}
		[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetGuidanceItemsInFolder(Guid folderId)		{	return javascriptProxy.GetGuidanceItemsInFolder(folderId);	}		
		[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetGuidanceItemsInView(Guid viewId)			{	return javascriptProxy.GetGuidanceItemsInView(viewId);	}		
		[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetGuidanceItemsInViews(List<Guid> viewIds)	{	return javascriptProxy.GetGuidanceItemsInViews(viewIds);	}
		//[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetAllGuidanceItemsInViews()					{ 	return javascriptProxy.getAllGuidanceItemsInViews();}
		[WebMethod(EnableSession = true)]	public string GetGuidanceItemHtml(Guid GuidanceItemId)						{	return javascriptProxy.GetGuidanceItemHtml(GuidanceItemId);	}				
		//both methods below will generate a JSON object with more than 1Mb (with the default SI library)
		[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetAllGuidanceItems()							{	return javascriptProxy.GetAllGuidanceItems();	}
		[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetGuidanceItemsInLibrary(Guid libraryId)		{	return javascriptProxy.GetGuidanceItemsInLibrary(libraryId); }
		
		[WebMethod(EnableSession = true)]	public List<View_V3> GetViewsInLibraryRoot(string libraryID)		 		{	return javascriptProxy.GetViewsInLibraryRoot(libraryID);		}						
		[WebMethod(EnableSession = true)]	public View_V3 GetViewById(string viewId)		 							{	return javascriptProxy.GetViewById(viewId);		}				
		
		//******* OnlineStorage
		[WebMethod(EnableSession = true)]	public void LogUserGUID(string Guid)										{ javascriptProxy.LogUserGUID(Guid);	}			
		[WebMethod(EnableSession = true)] 	public List<String> GetAllLibraryIds() 										{ return javascriptProxy.GetAllLibraryIds();  }
		[WebMethod(EnableSession = true)] 	public Library GetLibraryById  (Guid libraryId) 						    { return javascriptProxy.GetLibraryById	 (libraryId.str()); }		
		[WebMethod(EnableSession = true)] 	public Library GetLibraryByName(string libraryName) 						{ return javascriptProxy.GetLibraryByName(libraryName); }		
		[WebMethod(EnableSession = true)]	public GuidanceItem_V3 GetGuidanceItemById(string guidanceItemId)			{ return javascriptProxy.GetGuidanceItemById(guidanceItemId); 	}
		

		//demand Admin privs
		[WebMethod(EnableSession = true)]	[Admin(SecurityAction.Demand)]			public string GetAllUserLogs()		{				return javascriptProxy.GetAllUserLogs();		}		
		[WebMethod(EnableSession = true)] 	[Admin(SecurityAction.Demand)]			public List<Guid> GetActiveSessions()									{ 	return new TM_Xml_Database().activeSessions().Keys.ToList(); }
		[WebMethod(EnableSession = true)] 	[Admin(SecurityAction.Demand)]			public TMUser	  GetActiveSession(Guid sessionID)						
																						{ 	
																							var activeSessions = new TM_Xml_Database().activeSessions();
																							if (activeSessions.ContainsKey(sessionID))
																								return activeSessions[sessionID];
																							return null;
																						}
		
		
		//demand EditArticles privs
		[WebMethod(EnableSession = true)] 	[EditArticles(SecurityAction.Demand)]	public Library_V3 CreateLibrary(Library library)	{ this.resetCache(); return javascriptProxy.CreateLibrary(library);		}				
		[WebMethod(EnableSession = true)] 	[EditArticles(SecurityAction.Demand)]	public bool UpdateLibrary(Library library) 			{ this.resetCache(); return javascriptProxy.UpdateLibrary(library); }		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool UnDeleteLibrary(Guid libraryId)			{ return javascriptProxy.UnDeleteLibrary(libraryId);				}
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public List<TM_Library> GetDeletedLibraries() 		{ return javascriptProxy.GetDeletedLibraries();  }
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteDeletedLibraries()				{ return javascriptProxy.DeleteDeletedLibraries();  }		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public View_V3 CreateView(Guid folderId, View view) { this.resetCache(); return javascriptProxy.CreateView(folderId,view);  }
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool UpdateView(View view)													{ this.resetCache(); return javascriptProxy.UpdateView(view);		}
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool AddGuidanceItemsToView(Guid viewId,  List<Guid> guidanceItemIds)		{ this.resetCache(); return javascriptProxy.AddGuidanceItemsToView(viewId, guidanceItemIds);		}
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool RemoveGuidanceItemsFromView(Guid viewId, List<Guid> guidanceItemIds)	{ this.resetCache(); return javascriptProxy.RemoveGuidanceItemsFromView(viewId, guidanceItemIds);		}		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool RemoveViewFromFolder(Guid libraryId, Guid viewId)    					{ this.resetCache(); return javascriptProxy.RemoveViewFromFolder(libraryId, viewId); }  	
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool MoveViewToFolder(Guid viewId, Guid folderId) 							{ this.resetCache(); return javascriptProxy.MoveViewToFolder(viewId, folderId); }  	
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public Guid CreateGuidanceItem(GuidanceItem_V3 guidanceItem)						{ this.resetCache(); return javascriptProxy.CreateGuidanceItem(guidanceItem); 	}		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool UpdateGuidanceItem(GuidanceItem_V3 guidanceItem)						
																						{ 
																							this.resetCache();
																							var result = javascriptProxy.UpdateGuidanceItem(guidanceItem); 	
																							try
																							{																								
																								if (result)
																								{																									
																									new PagesHistory().logPageChange(guidanceItem.guidanceItemId, 
																																	 currentUser.notNull() 
																																		? currentUser.UserName 
																																		: "[tm error: no user]", 
																																	 sessionID, 
																																	 guidanceItem.htmlContent);
																									return true;																																	
																								}
																								return false;																																															
																							}
																							catch(Exception ex)
																							{
																								"Error in new PagesHistory(): {0} \n\n {1}".error(ex.Message, ex.StackTrace);
																							}
																							return result;
																						}																																
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteGuidanceItem(Guid guidanceItemId)											{ resetCache(); return javascriptProxy.DeleteGuidanceItem(guidanceItemId); 	}			
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteGuidanceItems(List<Guid> guidanceItemIds)									{ resetCache(); return javascriptProxy.DeleteGuidanceItems(guidanceItemIds); 	}			
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool RenameFolder(Guid libraryId, Guid folderId , string newFolderName) 				{ resetCache(); return javascriptProxy.RenameFolder(libraryId, folderId,newFolderName ); } 		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public Folder_V3 CreateFolder(Guid libraryId, Guid parentFolderId, string newFolderName) 	{ resetCache(); return javascriptProxy.CreateFolder(libraryId ,parentFolderId, newFolderName ); } 		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteFolder(Guid libraryId, Guid folderId) 							 	 	{ resetCache(); return javascriptProxy.DeleteFolder(libraryId ,folderId ); } 				

		
		
		//Extra (not in Javascript proxy (move to separate file if more than a couple are needed)
		[WebMethod(EnableSession = true)] [EditArticles(SecurityAction.Demand)]	
		public bool DeleteLibrary(Guid libraryId)
		{
			this.resetCache();
			if (javascriptProxy.GetLibraryById(libraryId.str()).isNull())
				return false;
			var libraryToDelete = new Library  { id = libraryId.str(), delete = true };
			javascriptProxy.UpdateLibrary(libraryToDelete);
			var libraryDeleted = javascriptProxy.GetLibraryById(libraryId.str());
			return libraryDeleted.isNull();// || libraryDeleted.delete;
		}
		
		[WebMethod(EnableSession = true)] [EditArticles(SecurityAction.Demand)]	
		public bool RenameLibrary(Guid libraryId, string newName)
		{
			this.resetCache();
			if (javascriptProxy.GetLibraryById(libraryId.str()).isNull())
				return false;
			var libraryToRename = new Library  { id = libraryId.str(), caption = newName };
			return javascriptProxy.UpdateLibrary(libraryToRename);			
		}
		
		
		[WebMethod(EnableSession = true)] [EditArticles(SecurityAction.Demand)]	
		public List<Guid> DeleteTempLibraries()
		{
			var deletedLibraries = new List<Guid>();
			foreach(var library in javascriptProxy.GetLibraries())
				if (library.Caption.contains("temp_lib_", "TempLibrary")  || library.Caption.isGuid())
					if (DeleteLibrary(library.Id))
						deletedLibraries.Add(library.Id);
			return deletedLibraries;
		}				
		
		
		[WebMethod(EnableSession = true)]
		[EditArticles(SecurityAction.Demand)]	
		public bool RenameGuidanceItemTitle(Guid guidanceItemId, string title)
		{			
			this.resetCache();
			try
			{
				var guidanceItem = GetGuidanceItemById(guidanceItemId.str());
				guidanceItem.title = title;
				return UpdateGuidanceItem(guidanceItem);
			}
			catch//(Exception ex)
			{				
				return false;
			}
		}

		//not implemented in TM3
		[WebMethod(EnableSession = true)]	public List<GuidanceType> GetGuidanceTypes() { 		return javascriptProxy.GetGuidanceTypes(); }
		[WebMethod(EnableSession = true)]	public GuidanceType CreateGuidanceType(GuidanceType guidanceType, string[] columns)	{ return javascriptProxy.CreateGuidanceType(guidanceType,columns) ; }		
		[WebMethod(EnableSession = true)]	public GuidanceType GetGuidanceTypeById(string guidanceTypeId)						{ return javascriptProxy.GetGuidanceTypeById(guidanceTypeId); 	}
		[WebMethod(EnableSession = true)]	public GuidanceType GetGuidanceTypeByName(string guidanceTypeName)					{ return javascriptProxy.GetGuidanceTypeByName(guidanceTypeName); 	}
		[WebMethod(EnableSession = true)]	public List<ColumnDefinition> GetGuidanceTypeColumns(Guid guidanceTypeId)			{ return javascriptProxy.GetGuidanceTypeColumns(guidanceTypeId); } 
		[WebMethod(EnableSession = true)]	public void RemoveGuidanceTypeColumns(string schemaId)	 							{ javascriptProxy.RemoveGuidanceTypeColumns(schemaId);}
		[WebMethod(EnableSession = true)]	public void UpdateGuidanceType(GuidanceType guidanceType, string[] columns)			{ javascriptProxy.UpdateGuidanceType(guidanceType,columns) ;		}
		[WebMethod(EnableSession = true)]	public bool DeleteGuidanceType(string guidanceTypeId)								{ return javascriptProxy.DeleteGuidanceType(guidanceTypeId) ;		}
		[WebMethod(EnableSession = true)]	public bool DeleteDeletedGuidanceTypes()											{ return javascriptProxy.DeleteDeletedGuidanceTypes(); }
		[WebMethod(EnableSession = true)]	public Schema GetSchemaById(string schemaId) 										{ return javascriptProxy.GetSchemaById(schemaId);		}
		[WebMethod(EnableSession = true)]	public List<string> GetGuidanceItemKeywords(string itemsId)							{ return javascriptProxy.GetGuidanceItemKeywords(itemsId);		}
		[WebMethod(EnableSession = true)]	public void SetGuidanceItemKeywords(string itemId, string[] keywords) 				{ javascriptProxy.SetGuidanceItemKeywords(itemId, keywords);		}						
    }
}
