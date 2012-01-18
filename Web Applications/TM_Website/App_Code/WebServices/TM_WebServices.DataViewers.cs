using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Security.Permissions;	
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using Microsoft.Practices.Unity;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;
//O2File:../IJavascriptProxy.cs
//O2File:../UtilMethods.cs
//O2File:../WebServices/TM_WebServices.asmx.cs
//O2Ref:System.Web.Services.dll 
//O2Ref:Microsoft.Practices.Unity.dll
//O2Ref:System.Xml.Linq.dll
namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{ 					
	//WebServices related to: Data Viewers
    public partial class TM_WebServices 
    {
		//********  DataViewers
		
		[WebMethod(EnableSession = true)]	
		public JsTree JsTreeWithFolders()
		{
			return JsTreeWithFoldersAndGuidanceItems();			
		}
		
		[WebMethod(EnableSession = true)]							
		public JsTree JsTreeWithFoldersAndGuidanceItems()
		{			
    		var jsTree = new JsTree();
			var libraries = GetLibraries();
			Func<Guid, List<Folder_V3>, JsTreeNode, List<Guid>> mapFolders = null;
			Func<Guid, Guid, List<Guid>, JsTreeNode, List<Guid>> mapViews = null;

			//precalculate for performance reasons
			var allViews = new Dictionary<Guid, View_V3>();
			foreach(var view in javascriptProxy.GetViews())
				allViews.Add(view.viewId, view);

			mapViews = (libraryId, folderId, viewIds, targetNode) =>
				{
					var viewsId = new List<Guid>();
					foreach(var viewId in viewIds)
					{
						var view = allViews[viewId];//GetViewById(viewId.str());
						
						var viewNodeText =  view.viewId.str();												
						var viewNode = targetNode.add_Node(view.caption, viewNodeText);						
						viewNode.state = "closed";
						viewNode.data.icon = "/Images/ViewIcon.png";												
						viewsId.add(view.viewId);
					}
					return viewsId;
				};
			
			mapFolders = (libraryId, folders, targetNode)=>
				{
					var folderViewsId = new List<Guid>();
					foreach(var folder in folders)
					{
						var subFolderViewsId  = new List<Guid>();
						var folderId = folder.folderId;
						var folderNode = targetNode.add_Node(folder.name);						
						folderNode.state = "closed";
						folderNode.data.icon = "/Images/FolderIcon.png";
						subFolderViewsId.AddRange(mapFolders(libraryId, folder.subFolders, folderNode));
						var viewIds = (from view in folder.views
									   select view.viewId).toList();
						subFolderViewsId.AddRange(mapViews(libraryId, folderId, viewIds, folderNode));
						
						folderNode.attr.id = folderId.str();												
						folderViewsId.AddRange(subFolderViewsId);					
					}
					return folderViewsId;
					
				};
			
			foreach(var library in libraries)
			{				
				var libraryNode =jsTree.add_Node(library.Caption);
				var mappedFolders = new Dictionary<string, List<Folder_V3>>();				
				mapFolders(library.Id, javascriptProxy.GetFolders(library.Id), libraryNode);
				mapViews(library.Id, Guid.Empty, javascriptProxy.GetViewsInLibraryRoot(library.Id.str()).guids(), libraryNode);
				//libraryNode.state = "open";
				libraryNode.state = "closed";
				libraryNode.data.icon = "/Images/SingleLibrary.png";
				
				var libraryNodeText = library.Id.str();										
				
				libraryNode.attr.id = libraryNodeText;
				
			}					
			return jsTree;
		}
		
		[WebMethod(EnableSession = true)]	
		public JsDataTable JsDataTableWithAllGuidanceItemsInViews()
		{ 
			var rawGuidanceItems = javascriptProxy.GetAllGuidanceItemsInViews_XmlDB();
			//var rawGuidanceItems = tmXmlDatabase.getGuidanceItemsInViews(viewIds);
			return getDataTableFromGuidanceItems(rawGuidanceItems);
		}	
		
		//old mode (to remove)
		[WebMethod(EnableSession = true)]	
		public JsDataTable JsDataTableWithGuidanceItemsInViews(List<Guid> viewIds)
		{ 
			if (viewIds.size()==1)	
			{
				var guidanceInLibrary = javascriptProxy.GetGuidanceItemsInLibrary_XmlDB(viewIds[0]);
				if (guidanceInLibrary.size()>0)
					return getDataTableFromGuidanceItems(guidanceInLibrary);
			}
			var rawGuidanceItems = javascriptProxy.GetGuidanceItemsInViews_XmlDB(viewIds);			
			return getDataTableFromGuidanceItems(rawGuidanceItems);
		}
		
		[WebMethod(EnableSession = true)]
		public JsDataTable JsDataTableWithGuidanceItemsIn_Library(Guid libraryId)
		{
			var rawGuidanceItems = javascriptProxy.GetGuidanceItemsInLibrary_XmlDB(libraryId);			
			return getDataTableFromGuidanceItems(rawGuidanceItems);
		}
		
		[WebMethod(EnableSession = true)]
		public JsDataTable JsDataTableWithGuidanceItemsIn_Folder(Guid folderId)
		{
			var rawGuidanceItems = javascriptProxy.GetGuidanceItemsInFolder_XmlDB(folderId);
			return getDataTableFromGuidanceItems(rawGuidanceItems);
		}
		
		[WebMethod(EnableSession = true)]
		public JsDataTable JsDataTableWithGuidanceItemsIn_View(Guid viewId)
		{
			var rawGuidanceItems = javascriptProxy.GetGuidanceItemsInViews_XmlDB(new List<Guid>().add(viewId));
			return getDataTableFromGuidanceItems(rawGuidanceItems);
		}
		
		
		public JsDataTable getDataTableFromGuidanceItems(List<GuidanceItem_V3> rawGuidanceItems)
		{
			var guidanceItems = rawGuidanceItems.GroupBy((guidanceItem)=>guidanceItem.guidanceItemId)
												.Select((g)=>g.First())
												.ToList();											
								
			var jsDataTable =  new JsDataTable();
			jsDataTable.add_Columns("Check", "Title",  "Technology", "Phase", "Type", "Category", "Id");
			foreach(var guidanceItem in guidanceItems)
					jsDataTable.add_Row("<input type='checkbox' class='GuidanceItemCheckBox' style='text-align: center'/>",
										guidanceItem.title.trim(), 
										guidanceItem.technology.trim() , 
										guidanceItem.phase ?? "", 
										guidanceItem.rule_Type.trim() , 										
										//guidanceItem.type ?? "(TBD2)", 										
										guidanceItem.category.trim() , 
										guidanceItem.guidanceItemId);
			return jsDataTable;
		}
		
		//********  DataEdition
		[WebMethod(EnableSession= true)]
		[EditArticles(SecurityAction.Demand)]
		public bool UpdateGuidanceItemHtml(Guid guidanceItemId, string htmlContent)
		{
			var guidanceItem = javascriptProxy.GetGuidanceItemById(guidanceItemId.str());
			
			new PagesHistory().logPageChange(guidanceItemId, currentUser.UserName, sessionID, guidanceItem.htmlContent.sanitizeHtmlContent());
			
			guidanceItem.htmlContent = htmlContent.sanitizeHtmlContent();
								
			return javascriptProxy.UpdateGuidanceItem(guidanceItem);			
		}
		
		//********  UserActivity
		
		[WebMethod(EnableSession= true)]
		public List<ChangedPage> getPagesHistory_by_PageId(Guid guidanceItemId)
		{
			return new PagesHistory().getPages_by_PageId(guidanceItemId);
		}
		 
		[WebMethod(EnableSession= true)]
		public bool currentUserHasActivityInGuidanceItem(Guid guidanceItemId, string userActivity)
		{
			return userHasActivityInGuidanceItem(guidanceItemId, currentUser.UserName, userActivity);
		}
		
		[WebMethod(EnableSession= true)]
		public bool userHasActivityInGuidanceItem(Guid guidanceItemId, string userName, string userActivity)
		{
			var pages = new PagesHistory().getPages_by_PageId(guidanceItemId);
			return (from page in pages
					where page.UserName == userName && page.UserActivity == userActivity
					select page).size() > 0;  
		}
		
		[WebMethod(EnableSession= true)]
		[EditArticles(SecurityAction.Demand)]
		public ChangedPage logPageUserComment(Guid guidanceItemId, string userComment)
		{					
			return new PagesHistory().logPageChange(guidanceItemId, currentUser.UserName, sessionID, "", userComment, "");
		}
			
		[WebMethod(EnableSession= true)]
		[EditArticles(SecurityAction.Demand)]
		public ChangedPage logPageUserActivity(Guid guidanceItemId, string userActivity)
		{			
			return new PagesHistory().logPageChange(guidanceItemId, currentUser.UserName, sessionID, "", "",userActivity);			
		}		
    }

}
