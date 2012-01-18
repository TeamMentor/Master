using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using Moq;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Utils;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
//O2File:TM_Xml_Database.cs
//O2File:../O2_Scripts_APIs/_O2_Scripts_Files.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{	
	public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_View
	{	
		public static TM_Library tmLibrary(this urn.microsoft.guidanceexplorer.View viewToFind, TM_Xml_Database tmDatabase)
		{
			return (from tmLibrary in tmDatabase.tmLibraries()
					from view in tmLibrary.xmlDB_Views(tmDatabase)					
					where view == viewToFind
					select tmLibrary).first();
		}
		
		public static urn.microsoft.guidanceexplorer.View xmlDB_View(this TM_Xml_Database tmDatabase, Guid viewId)
		{
			return (from view in tmDatabase.xmlDB_Views()					
					where view.id == viewId.str()
					select view).first();
		}
			
		public static urn.microsoft.guidanceexplorer.View xmlDB_View(this urn.microsoft.guidanceexplorer.Folder folder, Guid viewId)
		{
			return (from view in folder.view
					where view.id == viewId.str()
					select view).first();
		}
		
		public static urn.microsoft.guidanceexplorer.View xmlDB_View(this urn.microsoft.guidanceexplorer.Folder folder, string viewCaption)
		{
			return (from view in folder.view
					where view.caption == viewCaption
					select view).first();
		}
		
		public static List<urn.microsoft.guidanceexplorer.View> xmlDB_Views(this TM_Xml_Database tmDatabase)
		{
			return (from tmLibrary in tmDatabase.tmLibraries()
					from view in tmLibrary.xmlDB_Views(tmDatabase)					
					select view).toList();
		}
		
		public static List<urn.microsoft.guidanceexplorer.View> xmlDB_Views(this TM_Library tmLibrary , TM_Xml_Database tmDatabase)
		{
			var allViews = tmLibrary.xmlDB_Views_InLibraryRoot(tmDatabase);
			//add the ones from the libraryRoot
			
			//add the ones from the folders
			allViews.AddRange((from folder in tmDatabase.xmlDB_Folders_All(tmLibrary.Id)
							   from view in folder.view					
							   select view).toList());
			return allViews;
		}
		
		public static List<urn.microsoft.guidanceexplorer.View> xmlDB_Views_InLibraryRoot(this TM_Library tmLibrary , TM_Xml_Database tmDatabase)
		{
			try
			{
				return tmLibrary.guidanceExplorer(tmDatabase).library.libraryStructure.view.toList();
			}
			catch
			{
				return new List<urn.microsoft.guidanceexplorer.View>();
			}			
		}
		public static List<urn.microsoft.guidanceexplorer.View> xmlDB_Views(this urn.microsoft.guidanceexplorer.Folder folder)
		{
			if (folder.notNull())
				return folder.view.toList();
			return new List<urn.microsoft.guidanceexplorer.View>();
		}
		
		public static View_V3 newView(this TM_Xml_Database tmDatabase, Guid parentFolderId, View tmView)
		{
			var view = tmDatabase.xmlDB_NewView(parentFolderId, tmView);
			return tmDatabase.tmView(view.id.guid());
		}
		
		public static urn.microsoft.guidanceexplorer.View xmlDB_NewView(this TM_Xml_Database tmDatabase, View tmView)
		{
			return tmDatabase.xmlDB_NewView(Guid.Empty,  tmView);
		}
		
		public static urn.microsoft.guidanceexplorer.View xmlDB_NewView(this TM_Xml_Database tmDatabase, Guid parentFolderId, View tmView)
		{			
			var tmLibrary = tmDatabase.tmLibrary(tmView.library.guid());
			//var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(tmView.library.guid());
			if (tmLibrary.notNull())
			{				
				if (parentFolderId == Guid.Empty)
				{					
					var guidanceExplorer = tmLibrary.guidanceExplorer(tmDatabase);
					if (tmView.id == Guid.Empty.str())
						tmView.id = Guid.NewGuid().str();
					var view = tmView.view();			
					guidanceExplorer.library.libraryStructure.view.Add(view);
					tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);
					return tmDatabase.xmlDB_View(tmView.id.guid()); 					
				}
				else
				{
					var folder = tmDatabase.xmlDB_Folder(tmLibrary.Id, parentFolderId);
					if (folder.isNull())
						return null;
					//var folder = tmLibrary.xmlDB_Add_Folder(parentFolderId, tmView.parentFolder,tmDatabase);
/*					var existingView = folder.xmlDB_View(tmView.caption);
					if (existingView.notNull())
					{
						"[TM_Xml_Database] in xmlDB_NewView ,there was already a view called '{0}' in folder '{1}' so returning existing one".debug(tmView.caption,tmView.parentFolder);
						return existingView;
					}*/
					
					if (tmView.id == Guid.Empty.str())
						tmView.id = Guid.NewGuid().str();
					var view = tmView.view();								
					folder.view.Add(view);				
					tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
					return folder.xmlDB_View(tmView.id.guid()); 					// I have to get the reference again since the object is different after the calls to xmlDB_Save_GuidanceExplorer				
				}
			}
			return null;
		}		
		
		public static urn.microsoft.guidanceexplorer.View xmlDB_UpdateView(this TM_Xml_Database tmDatabase, View tmView)
		{
			return tmDatabase.xmlDB_UpdateView(tmView, new List<Guid>());
		}
		
		public static urn.microsoft.guidanceexplorer.View xmlDB_UpdateView(this TM_Xml_Database tmDatabase, View tmView, List<Guid> guidanceItems)
		{
			".... in  xmlDB_UpdateView".info();
			var tmLibrary = tmDatabase.tmLibrary(tmView.library.guid());
			if (tmLibrary.isNull())
			{
				"[TM_Xml_Database] in xmlDB_UpdateView, could not find library with id: {0}".error(tmView.library);
				return null;
			}
			/*var targetFolder = tmLibrary.xmlDB_Folder(tmView.parentFolder, tmDatabase);
			if (targetFolder.isNull())
			{
				"[TM_Xml_Database] in xmlDB_UpdateView, could not find target Folder with name: {0}".error(tmView.parentFolder);
				return null;
			}*/
			var targetView = tmDatabase.xmlDB_View(tmView.id.guid());
			if (targetView.isNull())
			{
				"[TM_Xml_Database] in xmlDB_UpdateView, could not find view with id: {0}".error(tmView.id);
				return null;
			}
			//"Updating view with caption {0} in folder {1} with id {2}".info(targetView.caption, targetFolder.caption, targetView.id);
			targetView.caption = tmView.caption;							
			targetView.author = tmView.creator;
			
			//foreach(var guid in guidanceItems)
			
			tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);			
			return targetView;
			//existingView.creationDate = tmView.lastUpdate // should we also update this?			
		}				
		
		public static bool xmlDB_RemoveViewFromFolder(this TM_Xml_Database tmDatabase, Guid libraryId,  Guid viewId )
		{
			return tmDatabase.xmlDB_RemoveViewFromFolder(tmDatabase.tmLibrary(libraryId), viewId);
		}
		
		public static bool xmlDB_RemoveViewFromFolder(this TM_Xml_Database tmDatabase, TM_Library tmLibrary, Guid viewId )
		{
			if (tmLibrary.isNull())
				"in xmlDB_RemoveViewFromFolder provided tmLibrary was null".error();
			else
			{			
				var view = tmDatabase.xmlDB_View(viewId);
				if (view.notNull())
				{
					view.Untyped.Remove();
					tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);						
					return true;
				}
				/*var folder = tmLibrary.xmlDB_Folder(folderName, tmDatabase);
				if (folder.isNull())
					"in xmlDB_RemoveViewFromFolder could not find folder '{0}' in library '{1}'".error(folderName, tmLibrary.Caption);
				else
				{
					var view = folder.xmlDB_View(viewId);
					if (view.isNull())
						"in xmlDB_RemoveViewFromFolder could not find view '{0}' in folder '{1}'".error(viewId, folderName);
					else
					{
						folder.view.Remove(view);
						"in xmlDB_RemoveViewFromFolder removed  view '{0}' from folder '{1}' in library '{2}'".info(view.caption, folderName, tmLibrary.Caption);
						tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);						
						return true;
					}
				}*/
			}
			return false;
		}
		
		public static bool xmlDB_MoveViewToFolder(this TM_Xml_Database tmDatabase, Guid viewId, Guid folderId)
		{
			try
			{
				var viewToMove = tmDatabase.xmlDB_View(viewId);			
				if (viewToMove.notNull())
				{
					var tmView =  tmDatabase.tmView(viewToMove.id.guid());  
					var tmLibrary = tmDatabase.tmLibrary(tmView.libraryId);
					"found viewToMove : {0}".info(viewToMove.caption);
					viewToMove.Untyped.Remove();							// remove from current location
					var targetFolder = tmDatabase.xmlDB_Folder(folderId);
					if (targetFolder == null)						// add view to Library root
					{
						
						var guidanceExplorer = tmLibrary.guidanceExplorer(tmDatabase);										
						guidanceExplorer.library.libraryStructure.view.Add(viewToMove);					
						"Moved view to library root".info();
					}	
					else
					{
						targetFolder.view.Add(viewToMove);
						"Moved view to folder : {0}".info(targetFolder.caption);
					}

					tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
					return true;
				}
			}
			catch(Exception ex)
			{
				ex.log();
			}
			return false;
		}
		
		
		public static bool xmlDB_AddGuidanceItemsToView(this TM_Xml_Database tmDatabase, Guid viewId, List<Guid> guidanceItemsIds)
		{	
			var view = tmDatabase.xmlDB_View(viewId); 
			if (view.isNull())
				"in xmlDB_AddGuidanceItemsToView, could not resolve view: {0}".error(viewId);
			else
			{
				if(view.items.isNull()) 
					view.items = new urn.microsoft.guidanceexplorer.Items(); 
				foreach(var guidanceItemsId in guidanceItemsIds)	
					if (view.items.item.Contains(guidanceItemsId.str()).isFalse())
						view.items.item.Add(guidanceItemsId.str());
				var tmLibrary = view.tmLibrary(tmDatabase);
				tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
				return true;
			}
			return false;
		}
		
		public static bool  xmlDB_RemoveGuidanceItemsFromView(this TM_Xml_Database tmDatabase, Guid viewId, List<Guid> guidanceItemsIds)
		{	
			var view = tmDatabase.xmlDB_View(viewId); 
			if (view.isNull())
				"in xmlDB_AddGuidanceItemsToView, could not resolve view: {0}".error(viewId);
			else
			{
				if(view.items.isNull()) 
					view.items = new urn.microsoft.guidanceexplorer.Items(); 
				foreach(var guidanceItemsId in guidanceItemsIds)	
					if (view.items.item.Contains(guidanceItemsId.str()))
						view.items.item.Remove(guidanceItemsId.str()); 
				var tmLibrary = view.tmLibrary(tmDatabase);
				tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
				return true;
			}
			return false;
		}
		
		public static bool  xmlDB_RemoveAllGuidanceItemsFromView(this TM_Xml_Database tmDatabase, Guid viewId)
		{	
			var view = tmDatabase.xmlDB_View(viewId); 
			if (view.isNull())
				"in xmlDB_AddGuidanceItemsToView, could not resolve view: {0}".error(viewId);
			else
			{
				
				view.items = new urn.microsoft.guidanceexplorer.Items(); 				
				
				var tmLibrary = view.tmLibrary(tmDatabase);
				tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
				return true;
			}
			return false;
		}
		
		public static List<Guid> guids(this List<View_V3> views)
		{
			return (from view in views
					select view.viewId).toList();
		}
	}
	
	public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_Folder
	{
		public static List<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders(this TM_Xml_Database tmDatabase)
		{						
			return (from tmLibrary in tmDatabase.tmLibraries()
					from folder in tmLibrary.xmlDB_Folders(tmDatabase)
					select folder).toList();
		}
		
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
		{
			return tmDatabase.xmlDB_Folders(tmLibrary.Id);
		}
		
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders(this TM_Xml_Database tmDatabase, Guid libraryId)
		{			
			try
			{			
				if (TM_Xml_Database.GuidanceExplorers_XmlFormat.hasKey(libraryId))
				{					
					var libraryStructure = TM_Xml_Database.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure;					
					if (libraryStructure.notNull())		
						return libraryStructure.folder;
		//				folders.AddRange(libraryStructure.folder.xmlDB_Folders());												
				}
				else
					"[xmlDB_Folders] library with id {0} was not present".error(libraryId);
			}
			catch(Exception ex)
			{
				"[xmlDB_Folders] xmlDB_Folders: {0}".error(ex.Message);				
			}
			return new List<urn.microsoft.guidanceexplorer.Folder>() ;
		}				
		
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders(this urn.microsoft.guidanceexplorer.Folder folderToMap)
		{						
			var folders = new List<urn.microsoft.guidanceexplorer.Folder>() ;
			if (folderToMap.isNull())
				return new List<urn.microsoft.guidanceexplorer.Folder>() ;;
			return folderToMap.folder1;
			/*return folderToMap;
			foreach(var folder in foldersToMap)
			{
				folders.add(folder);								
				folders.AddRange(folder.folder1.xmlDB_Folders());					
			}
			return folders;*/
		}
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders_All(this TM_Xml_Database tmDatabase)
		{
			return (from tmLibrary in tmDatabase.tmLibraries()
					from folder in tmDatabase.xmlDB_Folders_All(tmLibrary.Id)
					select folder).toList();
		}
		
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders_All(this TM_Xml_Database tmDatabase, Guid libraryId)
		{		
			return tmDatabase.xmlDB_Folders_All(tmDatabase.xmlDB_Folders(libraryId));			
		}
				
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders_All(this TM_Xml_Database tmDatabase, IList<urn.microsoft.guidanceexplorer.Folder> foldersToMap)
		{
			var folders = new List<urn.microsoft.guidanceexplorer.Folder> ();
			
			if (foldersToMap.notNull())
				foreach(var folderToMap in foldersToMap)
				{
					folders.add(folderToMap);
					folders.AddRange(tmDatabase.xmlDB_Folders_All(folderToMap.folder1));
				}
			return folders;
		}
		
		public static IList<urn.microsoft.guidanceexplorer.Folder> xmlDB_Folders_All(this TM_Xml_Database tmDatabase, urn.microsoft.guidanceexplorer.Folder folderToMap)
		{
			var folders = new List<urn.microsoft.guidanceexplorer.Folder> ();			
			if (folderToMap.notNull())
			{
				folders.add(folderToMap);
				folders.AddRange(tmDatabase.xmlDB_Folders_All(folderToMap.folder1));
			}
			return folders;
		}
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Folder(this TM_Xml_Database tmDatabase, Guid folderId)
		{
			foreach(var folder in tmDatabase.xmlDB_Folders_All())			
				if(folder.folderId == folderId.str())
					return folder;									
			return null;	
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Folder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
		{
			foreach(var folder in tmDatabase.xmlDB_Folders_All(libraryId))			
				if(folder.folderId == folderId.str())
					return folder;									
			return null;
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Folder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid parentFolder,string folderName)
		{
			foreach(var folder in tmDatabase.xmlDB_Folders_All(libraryId))									
				if(folder.folderId == parentFolder.str())
					foreach(var subFolder in folder.folder1)
						if (subFolder.caption == folderName)
							return subFolder;						
			return null;		
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Folder(this TM_Xml_Database tmDatabase, Guid libraryId,  string folderCaption)
		{
			var tmLibrary = tmDatabase.tmLibrary(libraryId); 
			return tmLibrary.xmlDB_Folder(folderCaption, tmDatabase);
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Folder(this TM_Library tmLibrary, string folderCaption, TM_Xml_Database tmDatabase)
		{
			return (from folder in tmLibrary.xmlDB_Folders(tmDatabase)
					where folder.caption == folderCaption
					select folder).first();				
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder(this TM_Xml_Database tmDatabase, Guid libraryId, string folderCaption)
		{
			return tmDatabase.xmlDB_Add_Folder(libraryId, Guid.Empty, folderCaption);
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid parentFolderId, string folderCaption)
		{
			var tmLibrary = tmDatabase.tmLibrary(libraryId); 
			return tmLibrary.xmlDB_Add_Folder(parentFolderId, folderCaption, tmDatabase);
		}
		
		public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder(this TM_Library tmLibrary, Guid parentFolderId, string folderCaption, TM_Xml_Database tmDatabase)
		{		
			try
			{	
				if (parentFolderId != Guid.Empty)
				{
					var folder = tmDatabase.xmlDB_Folder(tmLibrary.Id,parentFolderId);
					if (folder != null)
					{
/*						var subFolder = tmDatabase.xmlDB_Folder(tmLibrary.Id,parentFolderId,folderCaption);
						if (subFolder.notNull())			// there was already a subfolder with this caption
							return subFolder;
*/							
						var newFolder = new urn.microsoft.guidanceexplorer.Folder() 
						{ 
							caption = folderCaption, 
							folderId = Guid.NewGuid().str()
						};
						folder.folder1.Add(newFolder);
						
						var _guidanceExplorer= tmLibrary.guidanceExplorer(tmDatabase);
						_guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);		
						return tmDatabase.xmlDB_Folder(tmLibrary.Id, newFolder.folderId.guid());
					}
					return null;
				}
				else
				{
/*					var folder = tmLibrary.xmlDB_Folder(folderCaption, tmDatabase);
					if (folder.notNull())
					{
						"[TM_Xml_Database] in xmlDB_Add_Folder folder already existed, so returning existing one".debug();
						return folder;
					}
*/					
					var _guidanceExplorer= tmLibrary.guidanceExplorer(tmDatabase);
					var newFolder = new urn.microsoft.guidanceexplorer.Folder() 
						{ 
							caption = folderCaption, 
							folderId = Guid.NewGuid().str()
						};
					_guidanceExplorer.library.libraryStructure.folder.Add(newFolder);
					_guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);	
					
					return tmDatabase.xmlDB_Folder(tmLibrary.Id, newFolder.folderId.guid());
					//return tmLibrary.xmlDB_Folder(folderCaption, tmDatabase);
				}
			}
			catch(Exception ex)
			{
				ex.log();
				return null;
			}
		}
		
		
		public static bool xmlDB_Rename_Folder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId, string newFolderName)
		{
			//if (orginalFolderName.inValid() || newFolderName.inValid())
			//	return false;
			var folder = tmDatabase.xmlDB_Folder(libraryId, folderId);
			if (folder.isNull())
				return false;
			folder.caption = newFolderName;
			tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId); 
			return true;
		}
		
		public static bool xmlDB_Delete_Folder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
		{
			try
			{
				if (folderId == Guid.Empty)
					return false;
				
				var folder = tmDatabase.xmlDB_Folder(libraryId, folderId);
				if (folder.isNull())
					return false;
				//"found folder".info();	
				//var libraryStructure = TM_Xml_Database.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure;
				//libraryStructure.folder.Remove(folder); 
				folder.Untyped.Remove();
				
				tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId); 
				return true;
			}
			catch(Exception ex)
			{
				ex.log("in xmlDB_Delete_Folder");
				return false;
			}
			
		}
		
		/*public static urn.microsoft.guidanceexplorer.View xmlDB_RemoveView(urn.microsoft.guidanceexplorer.Folder , View tmView, TM_Xml_Database tmDatabase)
		{
			
		}*/
	}
	
	public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_Views_and_Folders_Guid_Fixes
	{
		public static TM_Xml_Database ensureFoldersAndViewsIdsAreUnique(this TM_Xml_Database tmDatabase)
		{
			var conflictsDetected = 0;
			"in ensureFoldersAndViewsIdsAreUnique".info();
			var mappedItems = new List<string>();
			foreach(var view in tmDatabase.xmlDB_Views())
			{
				if(mappedItems.contains(view.id))
				{
					conflictsDetected++;
					//"[ensureFoldersAndViewsIdsAreUnique] there was repeated viewId for view {0}: {1}".error(view.caption, view.id);
					view.id = Guid.NewGuid().str();
					//"[ensureFoldersAndViewsIdsAreUnique] new Guid assigned to view {0}: {1}".debug(view.caption, view.id);					
				}					
				mappedItems.Add(view.id);
			}
			
			foreach(var folder in tmDatabase.xmlDB_Folders())
			{
				if(mappedItems.contains(folder.folderId))
				{
					//"[ensureFoldersAndViewsIdsAreUnique] there was repeated folderId for folder {0}: {1}".error(folder.caption, folder.folderId);
					folder.folderId = Guid.NewGuid().str();
					//"[ensureFoldersAndViewsIdsAreUnique] new Guid assigned to view {0}: {1}".debug(folder.caption, folder.folderId);
					conflictsDetected++;
				}					
				mappedItems.Add(folder.folderId);
			}
			if (conflictsDetected > 0)
			{
				"[ensureFoldersAndViewsIdsAreUnique] There were {0} fixes made: {0}".info(conflictsDetected);	 
				tmDatabase.xmlDB_Save_GuidanceExplorers();
			}
			return tmDatabase;
		}
		
		public static TM_Xml_Database removeMissingGuidanceItemsIdsFromViews(this TM_Xml_Database tmDatabase)
		{				
			"in removeMissingGuidanceItemsIdsFromViews".debug();
			var conflictsDetected = 0;
			foreach(var view in tmDatabase.xmlDB_Views())
			{					
				//"[removeMissingGuidanceItemsIdsFromViews] mapping view: {0}".info(view.caption);	 
				if (view.items.notNull() && view.items.item.notNull() && view.items.item.Count > 0)
				{					
					foreach(var id in view.items.item.toList())
						if (TM_Xml_Database.Cached_GuidanceItems.hasKey(id.guid()).isFalse())
						{
							view.items.item.Remove(id);
							conflictsDetected++;
							//"missing guid: {0}".info(id);
						}
						//else
							//"guid in View: {0}".info(id);					
				}
			}
			if (conflictsDetected >0)
			{
				"[removeMissingGuidanceItemsIdsFromViews] There were {0} fixes made: {0}".info(conflictsDetected);	 
				tmDatabase.xmlDB_Save_GuidanceExplorers();
			}
			return tmDatabase;
		}
	}
}