using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;  
using System.Web.Services;
using System.Security.Permissions;	
using SecurityInnovation.TeamMentor.WebClient;
using O2.DotNetWrappers.ExtensionMethods; 
//using Microsoft.Practices.Unity;
using O2.XRules.Database.APIs;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;

//O2File:../Authentication/TM_Authentication.cs
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
    //[WebService(Namespace = "http://teammentor.securityinnoation.com/")]
	[WebService(Namespace = "http://tempuri.org/")]	 
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public partial class TM_WebServices : System.Web.Services.WebService 
    {
        //[Dependency]
        public TM_Xml_Database_JavaScriptProxy javascriptProxy { get; set; }

        //public ActivityTracking activityTracking { get; set; }

        public TM_Authentication tmAuthentication { get; set; }
		
        public TM_WebServices()
        {			                
            //UnityInjection.resolve(this);
            javascriptProxy = new TM_Xml_Database_JavaScriptProxy(); 
			tmAuthentication = new TM_Authentication(this);
			tmAuthentication.mapUserRoles();					
			
			//Disable Activity Tracking
			//activityTracking = new ActivityTracking();
			//activityTracking.LogRequest();
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
		
	 	
		
        //********  Session Management (& Login)]
        [WebMethod(EnableSession = true)]	public Guid Login(string username, string passwordHash)      		{   return tmAuthentication.sessionID = javascriptProxy.Login(username, passwordHash); }		
		[WebMethod(EnableSession = true)]	public Guid Login_PwdInClearText(string username, string password)	{	return tmAuthentication.sessionID = javascriptProxy.Login_PwdInClearText(username, password); }		
		[WebMethod(EnableSession = true)]	public Guid Logout()      											{																													
																													tmAuthentication.sessionID = Guid.Empty;
                                                                                                                    return tmAuthentication.sessionID;																													
																												}																												
		[WebMethod(EnableSession = true)]	public Guid Current_SessionID()										{	return tmAuthentication.sessionID;  }
		[WebMethod(EnableSession = true)]	public TMUser Current_User()										{	return tmAuthentication.currentUser;  }
		[WebMethod(EnableSession = true)]	public List<string> GetCurrentUserRoles()							{	return tmAuthentication.sessionID.session_UserRoles().toStringList();  }		
		
		//********  Libraries		
		[WebMethod(EnableSession = true)]	public List<TM_Library> GetLibraries()										{	return javascriptProxy.GetLibraries();	}		
		[WebMethod(EnableSession = true)]	public List<Folder_V3> 	GetAllFolders()										{	return javascriptProxy.GetFolders();	}		
		[WebMethod(EnableSession = true)]	public List<View_V3> 	GetAllViews()										{	return javascriptProxy.GetViews();		}		
		[WebMethod(EnableSession = true)]	public List<Folder_V3> GetFolders(Guid libraryId)							{	return javascriptProxy.GetFolders(libraryId);	}
		[WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInFolder(Guid folderId)		{	return javascriptProxy.GetGuidanceItemsInFolder(folderId);	}		
		[WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInView(Guid viewId)			{	return javascriptProxy.GetGuidanceItemsInView(viewId);	}		
		[WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInViews(List<Guid> viewIds)	{	return javascriptProxy.GetGuidanceItemsInViews(viewIds);	}
		//[WebMethod(EnableSession = true)]	public List<TM_GuidanceItem> GetAllGuidanceItemsInViews()					{ 	return javascriptProxy.getAllGuidanceItemsInViews();}
		[WebMethod(EnableSession = true)]	public string GetGuidanceItemHtml(Guid GuidanceItemId)						{	return javascriptProxy.GetGuidanceItemHtml(GuidanceItemId);	}				
		//both methods below will generate a JSON object with more than 1Mb (with the default SI library)
		[WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetAllGuidanceItems()						{	return javascriptProxy.GetAllGuidanceItems();	}
		[WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInLibrary(Guid libraryId)	{	return javascriptProxy.GetGuidanceItemsInLibrary(libraryId); }
		
		[WebMethod(EnableSession = true)]	public List<View_V3> GetViewsInLibraryRoot(string libraryID)		 		{	return javascriptProxy.GetViewsInLibraryRoot(libraryID);		}						
		[WebMethod(EnableSession = true)]	public View_V3 GetViewById(string viewId)		 							{	return javascriptProxy.GetViewById(viewId);		}				
		
		//******* OnlineStorage
		[WebMethod(EnableSession = true)]	public void LogUserGUID(string Guid)										{ javascriptProxy.LogUserGUID(Guid);	}			
		[WebMethod(EnableSession = true)] 	public List<String> GetAllLibraryIds() 										{ return javascriptProxy.GetAllLibraryIds();  }
		[WebMethod(EnableSession = true)] 	public Library GetLibraryById  (Guid libraryId) 						    { return javascriptProxy.GetLibraryById	 (libraryId.str()); }		
		[WebMethod(EnableSession = true)] 	public Library GetLibraryByName(string libraryName) 						{ return javascriptProxy.GetLibraryByName(libraryName); }		
		[WebMethod(EnableSession = true)]	public TeamMentor_Article GetGuidanceItemById(string guidanceItemId)			{ return javascriptProxy.GetGuidanceItemById(guidanceItemId); 	}
		

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
        [WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public Guid CreateArticle(TeamMentor_Article article)					            { this.resetCache(); return javascriptProxy.CreateArticle(article); 	}
        [WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public Guid CreateArticle_Simple(Guid libraryId, string title, string dataType, string htmlCode)					       
                                                                                        { 
                                                                                            this.resetCache(); 
                                                                                            var article = new TeamMentor_Article();
                                                                                            article.Metadata.Library_Id = libraryId;
                                                                                            article.Metadata.Title = title;
                                                                                            article.Content.DataType = dataType;
                                                                                            article.Content.Data.Value = htmlCode;
                                                                                            return javascriptProxy.CreateArticle(article); 
                                                                                        }
        
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool UpdateGuidanceItem(TeamMentor_Article guidanceItem)						
																						{ 
																							this.resetCache();
																							var result = javascriptProxy.UpdateGuidanceItem(guidanceItem); 	
																							return result;
																						}	

        [WebMethod(EnableSession = true)]   [EditArticles(SecurityAction.Demand)]   public bool SetArticleHtml (Guid articleId,string htmlContent)					        
                                                                                        {
                                                                                            return SetArticleContent(articleId, "html", htmlContent);
                                                                                        }
		[WebMethod(EnableSession = true)]   [EditArticles(SecurityAction.Demand)]   public bool SetArticleContent (Guid articleId, string dataType,  string content)					        
                                                                                        { 
                                                                                            resetCache();
                                                                                            var article = javascriptProxy.GetGuidanceItemById(articleId.str());
                                                                                            if (article.notNull())
                                                                                            {
                                                                                                article.Content.Data.Value = content;
                                                                                                article.Content.DataType = dataType;
                                                                                                return javascriptProxy.UpdateGuidanceItem(article);                                                                                                
                                                                                            }
                                                                                            return false;
                                                                                        }

		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteGuidanceItem(Guid guidanceItemId)											{ resetCache(); return javascriptProxy.DeleteGuidanceItem(guidanceItemId); 	}			
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteGuidanceItems(List<Guid> guidanceItemIds)									{ resetCache(); return javascriptProxy.DeleteGuidanceItems(guidanceItemIds); 	}			
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool RenameFolder(Guid libraryId, Guid folderId , string newFolderName) 				{ resetCache(); return javascriptProxy.RenameFolder(libraryId, folderId,newFolderName ); } 		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public Folder_V3 CreateFolder(Guid libraryId, Guid parentFolderId, string newFolderName) 	{ resetCache(); return javascriptProxy.CreateFolder(libraryId ,parentFolderId, newFolderName ); } 		
		[WebMethod(EnableSession = true)]	[EditArticles(SecurityAction.Demand)]	public bool DeleteFolder(Guid libraryId, Guid folderId) 							 	 	{ resetCache(); return javascriptProxy.DeleteFolder(libraryId ,folderId ); } 				

		
		
		//Extra (not in Javascript proxy (move to separate file if more than a couple are needed)
		[WebMethod(EnableSession = true)] [EditArticles(SecurityAction.Demand)]		public bool DeleteLibrary(Guid libraryId)
		{
			this.resetCache();
			if (javascriptProxy.GetLibraryById(libraryId.str()).isNull())
				return false;
			var libraryToDelete = new Library  { id = libraryId.str(), delete = true };
			javascriptProxy.UpdateLibrary(libraryToDelete);
			var libraryDeleted = javascriptProxy.GetLibraryById(libraryId.str());
			return libraryDeleted.isNull();// || libraryDeleted.delete;
		}		
		[WebMethod(EnableSession = true)] [EditArticles(SecurityAction.Demand)]	    public bool RenameLibrary(Guid libraryId, string newName)
		{
			this.resetCache();
			if (javascriptProxy.GetLibraryById(libraryId.str()).isNull())
				return false;
			var libraryToRename = new Library  { id = libraryId.str(), caption = newName };
			return javascriptProxy.UpdateLibrary(libraryToRename);			
		}
		[WebMethod(EnableSession = true)] [EditArticles(SecurityAction.Demand)]		public List<Guid> DeleteTempLibraries()
		{
			var deletedLibraries = new List<Guid>();
			foreach(var library in javascriptProxy.GetLibraries())
				if (library.Caption.contains("temp_lib_", "TempLibrary")  || library.Caption.isGuid())
					if (DeleteLibrary(library.Id))
						deletedLibraries.Add(library.Id);
			return deletedLibraries;
		}				
		
		
/*		[WebMethod(EnableSession = true)]
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
        */
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
