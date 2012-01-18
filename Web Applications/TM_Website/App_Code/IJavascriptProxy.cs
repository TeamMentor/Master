using System; 
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;

//O2File:DataViewers/JsTreeNode.cs
//O2File:DataViewers/JsDataTable.cs

//O2File:O2_Scripts_APIs/_O2_Scripts_Files.cs


namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public interface IJavascriptProxy
    {    
    	string ProxyType  { get; set; }
    	
		Guid adminSessionID  { get; set; }
		//Misc
		
    	string GetTime(); 	     
        
        //User Management
    	TMUser GetUser_byName(string name);    	 
    	TMUser GetUser_byID(int userId);    			
		List<TMUser> GetUsers_byID(List<int> userIds);    			
    	List<TMUser> GetUsers();    	     	
    	int CreateUser(NewUser newUser);     	
		TMUser CreateUser_Random();
    	List<int> CreateUsers(List<NewUser> newUser);     	
		List<int> BatchUserCreation(string batchUserData);
    	bool DeleteUser(int userId);    	
    	List<bool> DeleteUsers(List<int> userIds);    	
    	bool UpdateUser(int userId, string userName, string firstname, string lastname, string title, string company, string email, int groupId);
		bool SetUserPasswordHash(int userId,  string passwordHash);
		string GetUserGroupName(int userId);
		int GetUserGroupId(int userId);		
		bool SetUserGroupId(int userId, int groupId);
		List<string> GetUserRoles(int userId);
		
		
    	
    	//Session Management
    	Guid Login(string username, string passwordHash);
		Guid Login_PwdInClearText(string username, string passwordHash);		
		//Guid Current_AdminSessionID();
		//List<string> GetCurrentUserRoles();
		
		//Library Data
		List<TM_Library> GetLibraries();
		List<Folder_V3>  GetFolders();
		List<View_V3> 	 GetViews();
		List<Folder_V3> GetFolders(Guid libraryId);
		List<TM_GuidanceItem> GetGuidanceItemsInFolder(Guid folderId);
		List<TM_GuidanceItem> GetGuidanceItemsInView(Guid viewId);
		List<TM_GuidanceItem> GetGuidanceItemsInViews(List<Guid> viewIds);			
		string GetGuidanceItemHtml(Guid GuidanceItemId);
		List<TM_GuidanceItem> GetAllGuidanceItems();
		List<TM_GuidanceItem> GetGuidanceItemsInLibrary(Guid libraryId);
		
		//OnlineStorage
		string GetAllUserLogs();
		void LogUserGUID(string GUID);
    	//bool AuthorizedToUpload();
		List<string> GetAllLibraryIds();
		Library GetLibraryById  (string libraryId);
		Library GetLibraryByName(string libraryName);		
		Library_V3 CreateLibrary(Library library);
		bool UpdateLibrary(Library library);
		bool UnDeleteLibrary(Guid libraryId);
		List<TM_Library> GetDeletedLibraries();
		bool DeleteDeletedLibraries();
		View_V3 CreateView(Guid folderId, View view);
		View_V3 GetViewById(string viewId);
		List<View_V3> GetViewsInLibraryRoot(string libraryId);		
		List<GuidanceType> GetGuidanceTypes();
		GuidanceType CreateGuidanceType(GuidanceType guidanceType, string[] columns);		
		GuidanceType GetGuidanceTypeById(string guidanceTypeId);
		GuidanceType GetGuidanceTypeByName(string guidanceTypeName);
		List<ColumnDefinition> GetGuidanceTypeColumns(Guid guidanceTypeId);
		bool DeleteGuidanceType(string guidanceTypeId);
		bool DeleteDeletedGuidanceTypes();
		void RemoveGuidanceTypeColumns(string schemaId);
		void UpdateGuidanceType(GuidanceType guidanceType, string[] columns);
		Schema GetSchemaById(string schemaId);
		List<string> GetGuidanceItemKeywords(string itemId);
		bool UpdateView(View view);
		bool AddGuidanceItemsToView(Guid viewId, List<Guid> guidanceItemIds);
		bool RemoveGuidanceItemsFromView(Guid viewId, List<Guid> guidanceItemIds);
		bool RemoveViewFromFolder(Guid libraryId,  Guid viewId);
		bool MoveViewToFolder(Guid viewId, Guid folderId);
		//void CreateGuidanceItem(GuidanceItem item, string content);
		//Guid CreateGuidanceItem(Guid libraryIdGuid, Guid guidanceType, Guid creatorId, string creatorCaption, string title, string images, DateTime lastUpdate, string topic, string technology, string category, string ruleType, string priority, string status, string author, string htmlContent) ;
		//bool UpdateGuidanceItem(Guid id, string title, Guid guidanceType, Guid library, Guid creator, string creatorCaption, string content, string images, DateTime lastUpdate, string htmlContent);
		GuidanceItem_V3 GetGuidanceItemById(string guidanceItemid);
		Guid CreateGuidanceItem(GuidanceItem_V3 guidanceItem);
		bool UpdateGuidanceItem(GuidanceItem_V3 guidanceItem);
		bool DeleteGuidanceItem(Guid guidanceItemId);
		bool DeleteGuidanceItems(List<Guid> guidanceItemIds);
		
		void SetGuidanceItemKeywords(string itemId, string[] keywords);	
		
		bool RenameFolder(Guid libraryId, Guid folderId, string newFolderName);
		Folder_V3 CreateFolder(Guid libraryId, Guid parentFolderId, string newFolderName);
		bool DeleteFolder(Guid libraryId, Guid folderId);
		//XmlDB V3.0 specific
		
		List<GuidanceItem_V3> GetGuidanceItemsInViews_XmlDB(List<Guid> viewIds);		
		
		
		List<GuidanceItem_V3> GetAllGuidanceItems_XmlDB();	
		List<GuidanceItem_V3> GetGuidanceItemsInLibrary_XmlDB(Guid libraryId);	
		List<GuidanceItem_V3> GetGuidanceItemsInFolder_XmlDB(Guid folderId);	
		List<GuidanceItem_V3> GetAllGuidanceItemsInViews_XmlDB();	
			
    }    
	
	//move this so a separate file
	public class GuidanceItem_V3
	{
		public Guid guidanceItemId { get; set; }
		public Guid guidanceItemId_Original { get; set; }
		public Guid source_guidanceItemId { get; set; }
		public Guid libraryId { get; set; }
		public Guid guidanceType { get; set; }
		public Guid creatorId { get; set; }
		public string creatorCaption { get; set; }
		public string title { get; set; }
		public string images { get; set; }
//		public DateTime lastUpdate { get; set; }
		public string topic { get; set; }
		public string technology { get; set; }
		public string category { get; set; }
		public string phase { get; set; }
		public string rule_Type { get; set; }
		public string priority { get; set; }
		public string status { get; set; }
		public string author { get; set; }
		public bool delete { get; set; }
		public string htmlContent { get; set; }
		 
		public GuidanceItem_V3()
		{
			guidanceItemId= Guid.Empty.next(7.randomNumbers().toInt());			
		}
		
		public GuidanceItem_V3(GuidanceItem guidanceItem)
		{			
			guidanceItemId 			= guidanceItem.id.guid();
			guidanceItemId_Original	= guidanceItem.id_original.guid();
			libraryId 		= guidanceItem.library.guid();
			guidanceType 	= guidanceItem.guidanceType.guid();
			creatorId 		= guidanceItem.creator.guid();
			creatorCaption 	= guidanceItem.creatorCaption;
			title			= guidanceItem.title;
			images			= guidanceItem.images;
//			lastUpdate 		= guidanceItem.lastUpdate;
			delete 			= guidanceItem.delete;
			htmlContent		= guidanceItem.content.sanitizeHtmlContent();
			
			//use reflection to set these values
			foreach(var attribute in guidanceItem.AnyAttr)			
				this.prop(attribute.Name.lowerCaseFirstLetter(), attribute.Value);							
		}
		
		public GuidanceItem getGuidanceItem()
		//this one has quite a bit of logic (some of it hard-coded). Note that JSON was not able to handle the XMLDocument
		{						
			var guidanceItem = newGuidanceItemObject(guidanceItemId, title, guidanceType , libraryId, creatorId, creatorCaption ,htmlContent ,images );
			guidanceItem.AnyAttr = new List<XmlAttribute>()
				.add_XmlAttribute("Topic", topic )
				.add_XmlAttribute("Technology", technology)
				.add_XmlAttribute("Category", category)
				.add_XmlAttribute("Rule_Type", rule_Type)
				.add_XmlAttribute("Priority", priority)
				.add_XmlAttribute("Status", status)
				.add_XmlAttribute("Author", author)
				.ToArray();			
			return guidanceItem;			
		}
		
		private GuidanceItem newGuidanceItemObject(Guid id, string title, Guid guidanceType, Guid library, Guid creator, string creatorCaption, string content, string images) //, DateTime lastUpdate)
		{
			var guidanceItem = new GuidanceItem() { id =id.str(),  													
													title = title, 
													guidanceType = guidanceType.str(),	
													library = library.str(),
													creator = creator.str(),
													creatorCaption = creatorCaption, 
													content = content,
													images = images, 
													//lastUpdate = lastUpdate
												  };
			return guidanceItem;
		}
	}
}
