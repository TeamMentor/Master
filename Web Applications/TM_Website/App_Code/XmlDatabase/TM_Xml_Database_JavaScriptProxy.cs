     using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using Moq;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.XRules.Database.Utils;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
//O2File:../IJavascriptProxy.cs
//O2File:../UtilMethods.cs
//O2File:../Schemas/library.cs
//O2File:../Schemas/guidanceItem.cs
//O2File:TM_Xml_Database.cs
//O2Ref:moq.dll
//O2Ref:System.dll
//O2Ref:System.Web.Abstractions.dll
               
namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public class TM_Xml_Database_JavaScriptProxy : IJavascriptProxy
	{
		public TM_Xml_Database tmXmlDatabase {get;set;}		
		public string ProxyType  { get; set; }    	
		public Guid adminSessionID  { get; set; }
		
		
		public TM_Xml_Database_JavaScriptProxy()
		{			
			ProxyType = "TM Xml Database";
			adminSessionID = Guid.Empty;
			tmXmlDatabase = new TM_Xml_Database();			
		}
		//Misc		
    	public string GetTime() { return DateTime.Now.str(); }  	         
        //User Management
    	public TMUser GetUser_byName(string name)  					{ return tmXmlDatabase.tmUser(name); }  	
    	public TMUser GetUser_byID(int userId)  					{ return tmXmlDatabase.tmUser(userId); }  	
		public List<TMUser> GetUsers_byID(List<int> userIds)  		{ return userIds.tmUsers(); }  	
    	public List<TMUser> GetUsers()								{ return tmXmlDatabase.tmUsers(); }  	
    	public int CreateUser(NewUser newUser)			 			{ return tmXmlDatabase.createTmUser(newUser); }  	
		public TMUser CreateUser_Random()							{ return tmXmlDatabase.tmUser(tmXmlDatabase.newUser()); }  			
    	public List<int> CreateUsers(List<NewUser> newUsers)		{ return tmXmlDatabase.createTmUsers(newUsers); }  	
		public List<int> BatchUserCreation(string batchUserData) { return tmXmlDatabase.createTmUsers(batchUserData); }
    	public bool DeleteUser(int userId)							{ return tmXmlDatabase.deleteTmUser(userId); }  	
    	public List<bool> DeleteUsers(List<int> userIds)			{ return tmXmlDatabase.deleteTmUsers(userIds); }  	
    	public bool UpdateUser(int userId, string userName, 
							   string firstname, string lastname, 
							   string title, string company, 
							   string email, int groupId) 		{ return tmXmlDatabase.updateTmUser(userId, userName, firstname, lastname, title, company, email, groupId); }  	
		public bool SetUserPasswordHash(int userId,  string passwordHash) 					{ return tmXmlDatabase.setUserPassword(userId, passwordHash); }
		public int GetUserGroupId(int userId) 												{ return tmXmlDatabase.getUserGroupId(userId); }  	
		public string GetUserGroupName(int userId) 											{ return tmXmlDatabase.getUserGroupName(userId); }  	
		public bool SetUserGroupId(int userId, int groupId)				 					{ return tmXmlDatabase.setUserGroupId(userId, groupId); }  	
		public List<string> GetUserRoles(int userId)											{ return tmXmlDatabase.getUserRoles(userId); }  	
    	
    	//Session Management
    	public Guid Login(string username, string passwordHash)						{ return tmXmlDatabase.login(username,passwordHash);  }  	
		public Guid Login_PwdInClearText(string username, string passwordHash)		{ return tmXmlDatabase.login_PwdInClearText(username, passwordHash); }  	
		//public Guid Current_SessionID()												{ throw new Exception("not implemented"); }  	
//		public List<string> GetCurrentUserRoles()			/*Not Implemented */	{ throw new Exception("not implemented"); }  	
		
		//Library Data  
		public List<TM_Library> GetLibraries()		 								{ return tmXmlDatabase.tmLibraries(); }  	
		public List<Folder_V3> 	GetFolders()										{ return tmXmlDatabase.tmFolders(); }  	
		public List<View_V3> 	GetViews()											{ return tmXmlDatabase.tmViews(); }  	
		public List<Folder_V3> GetFolders(Guid libraryId)							{ return tmXmlDatabase.tmFolders(libraryId); }  			
		public List<TM_GuidanceItem> GetGuidanceItemsInFolder(Guid folderId)		{ return tmXmlDatabase.tmGuidanceItems_InFolder(folderId).tmGuidanceItems(); }  			
		public List<TM_GuidanceItem> GetGuidanceItemsInView(Guid viewId)			{ return tmXmlDatabase.getGuidanceItemsInView(viewId).tmGuidanceItems(); }  	
		public List<TM_GuidanceItem> GetGuidanceItemsInViews(List<Guid> viewIds)	{ return tmXmlDatabase.getGuidanceItemsInViews(viewIds).tmGuidanceItems(); }  	
		public string GetGuidanceItemHtml(Guid GuidanceItemId)						{ return tmXmlDatabase.getGuidanceItemHtml(GuidanceItemId);  }  	
		public List<TM_GuidanceItem> GetAllGuidanceItems()							{ return tmXmlDatabase.tmGuidanceItems().tmGuidanceItems();}  	
		public List<TM_GuidanceItem> GetGuidanceItemsInLibrary(Guid libraryId)		{ return tmXmlDatabase.tmGuidanceItems(libraryId).tmGuidanceItems();}  	
		
		//OnlineStorage
		public List<string> GetAllLibraryIds()										{ return tmXmlDatabase.tmLibraries().ids().toStringList(); }  	
		public Library GetLibraryById(string libraryId)								{ return tmXmlDatabase.tmLibrary(libraryId.guid()).library(tmXmlDatabase);	 }  	
		public Library GetLibraryByName(string libraryName)							{ return tmXmlDatabase.tmLibrary(libraryName).library(tmXmlDatabase);	 }  	
		public Library_V3 CreateLibrary(Library library)							{ return tmXmlDatabase.xmlDB_NewGuidanceExplorer(library.id.guid(), library.caption).libraryV3(); }  	
		public bool UpdateLibrary(Library library)									{ return tmXmlDatabase.xmlDB_UpdateGuidanceExplorer(library.id.guid(), library.caption, library.delete).notNull(); }  	
		public bool UnDeleteLibrary(Guid libraryId)		/*Not Implemented */		{ throw new Exception("not implemented"); }  	
		public List<TM_Library> GetDeletedLibraries()	/*Not Implemented */		{ throw new Exception("not implemented"); }  	
		public bool DeleteDeletedLibraries()			/*Not Implemented */		{ throw new Exception("not implemented"); }  	
		public View_V3 CreateView(Guid folderId, View view)									{ return tmXmlDatabase.newView(folderId, view); }  	
		public View_V3 GetViewById(string viewId)				  							{ return tmXmlDatabase.tmView(viewId.guid()); }  	
		public List<View_V3> GetViewsInLibraryRoot(string libraryId)						{ return tmXmlDatabase.tmViews_InLibraryRoot(libraryId.guid()); }
		public bool UpdateView(View view)													{ return tmXmlDatabase.xmlDB_UpdateView(view).notNull(); }  	
		public bool AddGuidanceItemsToView(Guid viewId, List<Guid> guidanceItemIds)			{ return tmXmlDatabase.xmlDB_AddGuidanceItemsToView(viewId, guidanceItemIds); }  	
		public bool RemoveGuidanceItemsFromView(Guid viewId, List<Guid> guidanceItemIds)	{ return tmXmlDatabase.xmlDB_RemoveGuidanceItemsFromView(viewId,guidanceItemIds ); }  			
		public GuidanceItem_V3 GetGuidanceItemById(string guidanceItemid)					{ return tmXmlDatabase.tmGuidanceItem(guidanceItemid.guid()); }  	
		public Guid CreateGuidanceItem(GuidanceItem_V3 guidanceItem)						{ return tmXmlDatabase.createGuidanceItem(guidanceItem); }  	
		public bool UpdateGuidanceItem(GuidanceItem_V3 guidanceItem)						{ return tmXmlDatabase.createGuidanceItem(guidanceItem) != Guid.Empty; }  	
		public bool DeleteGuidanceItem(Guid guidanceItemId)									{ return tmXmlDatabase.xmlDB_Delete_GuidanceItem(guidanceItemId); }
		public bool DeleteGuidanceItems(List<Guid> guidanceItemIds)							{ return tmXmlDatabase.xmlDB_Delete_GuidanceItems(guidanceItemIds); }
		public bool RenameFolder(Guid libraryId, Guid folderId,string newFolderName) 		{ return tmXmlDatabase.xmlDB_Rename_Folder(libraryId, folderId,newFolderName ); } 		
		public Folder_V3 CreateFolder(Guid libraryId, Guid parentFolderId, string newFolderName) 	{ return tmXmlDatabase.xmlDB_Add_Folder(libraryId, parentFolderId, newFolderName ).tmFolder(libraryId, tmXmlDatabase); } 		
		public bool DeleteFolder(Guid libraryId, Guid folderId)										{ return  tmXmlDatabase.xmlDB_Delete_Folder(libraryId,  folderId); }
		//XmlDB specific
		public List<GuidanceItem_V3> GetGuidanceItemsInViews_XmlDB(List<Guid> viewIds)	{ return tmXmlDatabase.getGuidanceItemsInViews(viewIds);}  			
		
		public List<GuidanceItem_V3> GetGuidanceItemsInLibrary_XmlDB(Guid libraryId) 	{ return tmXmlDatabase.tmGuidanceItems(libraryId);}
		public List<GuidanceItem_V3> GetGuidanceItemsInFolder_XmlDB(Guid folderId)   	{ return tmXmlDatabase.tmGuidanceItems_InFolder(folderId);}				       
		public List<GuidanceItem_V3> GetAllGuidanceItemsInViews_XmlDB()					{ return tmXmlDatabase.getAllGuidanceItemsInViews();} 
		public List<GuidanceItem_V3> GetAllGuidanceItems_XmlDB()						{ return tmXmlDatabase.tmGuidanceItems();}
		
		
		// new ones to add to main Webservice
		public bool RemoveViewFromFolder(Guid libraryId, Guid viewId)	{ return tmXmlDatabase.xmlDB_RemoveViewFromFolder(libraryId, viewId);}
		public bool MoveViewToFolder(Guid viewId, Guid folderId)    	{ return tmXmlDatabase.xmlDB_MoveViewToFolder(viewId,folderId); }

				
		/*ALL the ones below (including all GuidanceTypes and schema) are Not Implemented at the moment for the TmXmlDatabase*/
		public string GetAllUserLogs()					/*Not Implemented */		{ throw new Exception("not implemented"); }  	
		public void LogUserGUID(string GUID)			/*Not Implemented */		{ throw new Exception("not implemented"); }  	    				
		//bool AuthorizedToUpload();
		public List<GuidanceType> GetGuidanceTypes()								{ throw new Exception("not implemented"); }  	
		public GuidanceType CreateGuidanceType(GuidanceType guidanceType, string[] columns)	{ throw new Exception("not implemented"); }  	
		public GuidanceType GetGuidanceTypeById(string guidanceTypeId)						{ throw new Exception("not implemented"); }  	
		public GuidanceType GetGuidanceTypeByName(string guidanceTypeName)					{ throw new Exception("not implemented"); }  	
		public List<ColumnDefinition> GetGuidanceTypeColumns(Guid guidanceTypeId)			{ throw new Exception("not implemented"); }  	
		public bool DeleteGuidanceType(string guidanceTypeId)								{ throw new Exception("not implemented"); }  	
		public bool DeleteDeletedGuidanceTypes()											{ throw new Exception("not implemented"); }  	
		public void RemoveGuidanceTypeColumns(string schemaId)								{ throw new Exception("not implemented"); }  	
		public void UpdateGuidanceType(GuidanceType guidanceType, string[] columns)			{ throw new Exception("not implemented"); }  	
		public Schema GetSchemaById(string schemaId)									{ return null; }  	
		public List<string> GetGuidanceItemKeywords(string itemId)						{ throw new Exception("not implemented"); }  			
		public void SetGuidanceItemKeywords(string itemId, string[] keywords)			{ throw new Exception("not implemented"); }  			
	}
	
}