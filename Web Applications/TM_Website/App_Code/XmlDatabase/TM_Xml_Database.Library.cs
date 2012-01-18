using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions; 
using Microsoft.Security.Application;
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
//O2Ref:HtmlSanitizationLibrary.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{	
	public partial class TM_Xml_Database 
	{	
		public static bool setLibraryPath(string libraryPath)
		{			
			//"in setLibraryPath: {0}".info(libraryPath);
			if (libraryPath.dirExists().isFalse())						
			{
				libraryPath = TM_Xml_Database.Path_XmlDatabase.pathCombine(libraryPath);
				libraryPath.createDir();  // make sure it exists
			}
			TM_Xml_Database.Path_XmlLibraries = libraryPath;
			return loadDataIntoMemory();
		}
		
		public static bool loadDataIntoMemory()
		{
			return loadDataIntoMemory(Path_XmlDatabase, Path_XmlLibraries);			
		}		
		
		public static bool loadDataIntoMemory(string pathXmlDatabase, string pathXmlLibraries)
		{	
			if(pathXmlDatabase.dirExists().isFalse())
			{
				"[TM_Xml_Database] in loadDataIntoMemory, provided pathXmlDatabase didn't exist: {0}".error(pathXmlDatabase);
				return false;
			}			
			GuidanceExplorers_XmlFormat = pathXmlLibraries.getGuidanceExplorerObjects();
			pathXmlLibraries.loadGuidanceItemsFromCache();
			//mapGuidanceItemsViews();
			loadTmUserObjects(pathXmlDatabase);
			return true;					
		}
		
/*		public static void mapGuidanceItemsViews()
		{		
			//" *** in mapGuidanceItemsViews".error();
			GuidanceItems_InViews.Clear();
			var o2Timer = new O2Timer("mapGuidanceItemsViews").start();
			var foundIDs = 0;
			var notFoundIDs = 0;			
			
			foreach(var library in GuidanceExplorers_XmlFormat.Values)
				foreach(var view in this.tmViews(library.name.guid()))
				{
					view.str().info();
				}
			/*
			foreach(var guidanceExplorer in GuidanceExplorers_XmlFormat.Values)
			{
				try
				{
					if (guidanceExplorer.library.libraryStructure.folder.notNull())					
						foreach(var folder in guidanceExplorer.library.libraryStructure.folder)
							foreach(var view in folder.view)
							{								
								//"mapping view: {0} - view: {1} in folder: {2}".info(view.id, view.caption, folder.caption);								
								if (view.items.notNull())								
									foreach(var item in view.items.item)
										if(Cached_GuidanceItems.hasKey(item.guid()))
										{
											GuidanceItems_InViews.add(view.id.guid(), Cached_GuidanceItems[item.guid()]);
											foundIDs++;								
										}	
										else							
											notFoundIDs++;
									//" --- couldn't find view item with id: {0}".error(item);							
							}							
				}
				catch(Exception ex)
				{
					"in mapGuidanceItemsViews:{0}".error(ex.Message);
					//guidanceExplorer.details();
					return;
				}
			}* /
			//o2Timer.start();
			//"found guidanceItems for: {0} view mappings".info(foundIDs);
			//"couldn't find guidanceItems for: {0} view mappings".error(notFoundIDs);			
		}		
*/		
		//move to  extension methods
		[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 
		public GuidanceItem_V3 getGuidanceItem(Guid guidanceItemId)
		{
			if (Cached_GuidanceItems.hasKey(guidanceItemId).isFalse())
				return null;
				
			return Cached_GuidanceItems[guidanceItemId];			
		}
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 
		public string getGuidanceItemHtml(Guid guidanceItemId)
		{
			if (Cached_GuidanceItems.hasKey(guidanceItemId).isFalse())
				return null;
			return Cached_GuidanceItems[guidanceItemId].htmlContent
													   .sanitizeHtmlContent();
		}				
	}


	//******************* TM_Library
	
	public static class TM_Xml_Database_ExtensionMethods_TM_Library
	{
		//[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 
		public static List<TM_Library> tmLibraries(this TM_Xml_Database tmDatabase)
		{
			var libraries = new List<TM_Library>();
			try
			{
				foreach(var _guidanceExplorer in tmDatabase.xmlDB_GuidanceExplorers())
					libraries.Add(new TM_Library()
										{
											Id = _guidanceExplorer.library.name.guid(), 
											Caption = _guidanceExplorer.library.caption
										});
			}
			catch(Exception ex)
			{
				ex.log();
			}
			return libraries;
		}
		
		public static TM_Library tmLibrary(this TM_Xml_Database tmDatabase, string caption)
		{
			var tmLibrary = (from library in tmDatabase.tmLibraries()
							 where library.Caption == caption
							 select library).first();
			//if (tmLibrary.isNull())
			//	"[TM_Xml_Database] couldn't find library with caption: {0}".error(caption);
			return tmLibrary;
		}
		
		public static TM_Library tmLibrary(this TM_Xml_Database tmDatabase, Guid libraryId)
		{
			var tmLibrary = (from library in tmDatabase.tmLibraries()
							 where library.Id == libraryId
							 select library).first();
			//if (tmLibrary.isNull())
			//	"[TM_Xml_Database] couldn't find library with id: {0}".error(libraryId);
			return tmLibrary;		
		}
		
		public static List<Guid> ids(this List<TM_Library> libraries)
		{
			return (from library in libraries
					select library.Id).toList();
		}
		
		public static List<string> names(this List<TM_Library> libraries)
		{
			return libraries.captions();
		}
		public static List<string> captions(this List<TM_Library> libraries)
		{
			return (from library in libraries
					select library.Caption).toList();
		}	

		public static TM_Library new_TmLibrary(this TM_Xml_Database tmDatabase)
		{
			return tmDatabase.new_TmLibrary("temp_lib_{0}".format(6.randomLetters()));
		}
		
		public static TM_Library new_TmLibrary(this TM_Xml_Database tmDatabase, string libraryCaption )
		{
			var existingLibrary = tmDatabase.tmLibrary(libraryCaption);
			if (existingLibrary.notNull())
			{
				"[TM_Xml_Database] there was already a library called '{0}' to returning it".debug(libraryCaption);
				return existingLibrary;
			}
			tmDatabase.xmlDB_NewGuidanceExplorer(Guid.NewGuid(), libraryCaption);
			return tmDatabase.tmLibrary(libraryCaption);
		}		

	}
	
	//******************* Folder_V3 (was TMFolder)
	
	public static class TM_Xml_Database_ExtensionMethods_Folder_V3
	{
		//gets all folders (recursive search)
		public static List<Folder_V3> tmFolders(this TM_Xml_Database tmDatabase)
		{
			var tmFolders = new List<Folder_V3>();
			foreach(var tmLibrary in tmDatabase.tmLibraries())
				tmFolders.add(tmDatabase.tmFolders_All(tmLibrary.Id));
			return tmFolders;
		}
		
		public static List<Folder_V3> tmFolders(this TM_Library tmLibrary, TM_Xml_Database tmDatabase )
		{
			return tmDatabase.tmFolders(tmLibrary);
		}
		
		public static List<Folder_V3> tmFolders(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
		{
			return tmDatabase.tmFolders(tmLibrary.Id);
		}
		
		public static List<Folder_V3> tmFolders(this TM_Xml_Database tmDatabase, Guid libraryId)
		{
			return tmDatabase.tmFolders(libraryId, tmDatabase.xmlDB_Folders(libraryId));
		}	
		
		public static List<Folder_V3> tmFolders(this TM_Xml_Database tmDatabase, Guid libraryId, IList<urn.microsoft.guidanceexplorer.Folder> folders)
		{
			var tmFolders = new List<Folder_V3>();
			if (libraryId == Guid.Empty || folders.isNull())
				return tmFolders;
			foreach(var folder in folders)
			{								
				var tmFolder = 	folder.tmFolder(libraryId, tmDatabase);
				tmFolders.add(tmFolder);		
			}
			return tmFolders;									
		}
		
		public static Folder_V3 tmFolder(this urn.microsoft.guidanceexplorer.Folder folder, Guid libraryId, TM_Xml_Database tmDatabase)
		{
			if (folder.isNull())
				return null;
			if (folder.folderId.isNull())				// handle legacy case where there is no folderId in the guidanceitems object
					folder.folderId = Guid.NewGuid().str();	
			var tmFolder= new Folder_V3	
					{
						libraryId = libraryId,
						name = folder.caption,						
						folderId = folder.folderId.guid(),
						subFolders = tmDatabase.tmFolders(libraryId, folder.folder1)						
					};
			foreach(var view in folder.view)				
				tmFolder.views.Add(new View_V3 () { viewId = view.id.guid()});	
			return tmFolder;
		}
		public static List<Folder_V3> tmFolders_All(this TM_Xml_Database tmDatabase, Guid libraryId)
		{	
			var tmFolders = new List<Folder_V3>();
			Action<List<Folder_V3>> mapFolder = null;
			mapFolder =
				(tmFoldersToMap)=>{
								foreach(var tmFolder in tmFoldersToMap)
								{
									tmFolders.add(tmFolder);
									mapFolder(tmFolder.subFolders);
								}
							};
			mapFolder(tmDatabase.tmFolders(libraryId));
			return tmFolders;
		}
		
		
		public static Folder_V3 tmFolder(this TM_Library tmLibrary, Guid folderId, TM_Xml_Database tmDatabase )
		{
			return tmDatabase.tmFolder(tmLibrary.Id, folderId);
		}
		
		public static Folder_V3 tmFolder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
		{
			return (from tmFolder in tmDatabase.tmFolders_All(libraryId)
					where tmFolder.folderId == folderId
					select tmFolder).first();
		}
		
		public static Folder_V3 tmFolder(this TM_Xml_Database tmDatabase, Guid libraryId, string name)
		{
			return (from tmFolder in tmDatabase.tmFolders_All(libraryId)
					where tmFolder.name == name
					select tmFolder).first();
		}
		
		public static Folder_V3 tmFolder(this TM_Xml_Database tmDatabase, Guid folderId)
		{
			foreach(var tmLibrary in tmDatabase.tmLibraries())
			{
				var tmFolder = tmDatabase.tmFolder(tmLibrary.Id, folderId);
				if (tmFolder.notNull())
					return tmFolder;
			}
			return null;
		}
	}
	
	//******************* TM_View
	
	public static class TM_Xml_Database_ExtensionMethods_TM_View
	{
		public static List<View_V3> tmViews(this TM_Xml_Database tmDatabase)
		{
			var tmViews = new List<View_V3>();
			foreach(var tmLibrary in tmDatabase.tmLibraries())
				tmViews.add(tmDatabase.tmViews(tmLibrary));
			return tmViews;
		}
		
		public static List<View_V3> tmViews(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
		{
			return tmDatabase.tmViews(tmLibrary.Id);
		}
		
		public static List<View_V3> tmViews(this TM_Xml_Database tmDatabase, Guid libraryId)
		{
			var tmViews = tmDatabase.tmViews(libraryId, tmDatabase.xmlDB_Folders(libraryId));					
			tmViews.AddRange(tmDatabase.tmViews_InLibraryRoot(libraryId));
			return tmViews;			
		}
		public static List<View_V3> tmViews_InLibraryRoot(this TM_Xml_Database tmDatabase, Guid libraryId)		
		{
			var tmViews  = new List<View_V3>();
			var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
			if(guidanceExplorer.notNull() && guidanceExplorer.library.libraryStructure.notNull())
				foreach(var view in guidanceExplorer.library.libraryStructure.view)
					tmViews.add(view.tmView(libraryId, Guid.Empty));
			return tmViews;
			
		}
		
		public static List<View_V3> tmViews(this TM_Xml_Database tmDatabase, Guid libraryId ,  IList<urn.microsoft.guidanceexplorer.Folder> folders)
		{
			var tmViews = new List<View_V3>();							
			foreach(var folder in folders)
			{
				foreach(var view in folder.view)				
					tmViews.add(view.tmView(libraryId, folder.folderId.guid()));
				tmViews.AddRange(tmDatabase.tmViews(libraryId, folder.folder1));	
			}
			return tmViews;									
		}
		
		public static View_V3 tmView(this TM_Xml_Database tmDatabase, Guid viewId)
		{			
			return (from view in tmDatabase.tmViews()
					where view.viewId == viewId
					select view).first();					
		}
		
		public static List<GuidanceItem_V3> getGuidanceItemsInViews(this TM_Xml_Database tmDatabase, List<View> views)
		{
			var viewIds = (from view in views select view.id.guid()).toList();
			return tmDatabase.getGuidanceItemsInViews(viewIds);
		}
		
		public static List<GuidanceItem_V3> getGuidanceItemsInViews(this TM_Xml_Database tmDatabase, List<Guid> viewIds)
		{
			return (from viewId in viewIds
					from guidanceItemV3 in tmDatabase.getGuidanceItemsInView(viewId)
					select guidanceItemV3).toList();
		}
		
		public static List<GuidanceItem_V3> getGuidanceItemsInView(this TM_Xml_Database tmDatabase, Guid viewId)
		{		
			var tmView = tmDatabase.tmView(viewId);
			if (tmView.notNull())
			{
				var guidanceItems = new List<GuidanceItem_V3>();
				foreach(var guidanceItemId in tmView.guidanceItems)
					if (TM_Xml_Database.Cached_GuidanceItems.hasKey(guidanceItemId))
						guidanceItems.add(TM_Xml_Database.Cached_GuidanceItems[guidanceItemId]);
					else
						"[getGuidanceItemsInView]: in view ({0} {1}) could not find guidanceItem for id {2}".error(tmView.caption, tmView.viewId, guidanceItemId);
				return guidanceItems;
			}
			//if (TM_Xml_Database.GuidanceItems_InViews.hasKey(viewId))
			//	return TM_Xml_Database.GuidanceItems_InViews[viewId];
			"[TM_Xml_Database] getGuidanceItemsInView, requested viewId was not mapped: {0}".error(viewId);
			return new List<GuidanceItem_V3>();
		}
		
		public static List<GuidanceItem_V3> getAllGuidanceItemsInViews(this TM_Xml_Database tmDatabase)
		{
			//return (from viewId in TM_Xml_Database.GuidanceItems_InViews.Keys
			//		from guidanceItem in TM_Xml_Database.GuidanceItems_InViews[viewId]
			//		select guidanceItem).toList();
			return new List<GuidanceItem_V3>();
		}
		
/*		public static List<GuidanceItem_V3> getAllGuidanceItemsInLibrary(this TM_Xml_Database tmDatabase, GUID tmLibrary)
		{
			return (from guidanceItem in TM_Xml_Database.Cached_GuidanceItems
					where guidanceItem.guidanceItemId == 
					select guidanceItem).toList();
		}*/
	}
	
	//******************* TM_GuidanceItem
	
	public static class TM_Xml_Database_ExtensionMethods_TM_GuidanceItems
	{
		//[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 	
		[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticlesTitles")] 			
		public static List<GuidanceItem_V3> tmGuidanceItems(this TM_Xml_Database tmDatabase)
		{			
			return tmDatabase.xmlDB_GuidanceItems();						
		}
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 
		//[PrincipalPermission(SecurityAction.Demand, Role = "ReadArticlesTitles")] 	
		public static GuidanceItem_V3 tmGuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
		{			
			return (from guidanceItem in tmDatabase.tmGuidanceItems()
					where guidanceItem.guidanceItemId == guidanceItemId
					select guidanceItem).first();				
		}
		
		public static List<GuidanceItem_V3> tmGuidanceItems(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
		{
			return tmDatabase.tmGuidanceItems(tmLibrary.Id);
		}
		
		public static List<GuidanceItem_V3> tmGuidanceItems(this TM_Xml_Database tmDatabase, Guid libraryId)
		{			
			return (from guidanceItem in TM_Xml_Database.Cached_GuidanceItems.Values
					where guidanceItem.libraryId == libraryId
					select guidanceItem).toList();		
		}				
		
		public static List<GuidanceItem_V3> tmGuidanceItems_InFolder(this TM_Xml_Database tmDatabase, Guid folderId)
		{				
			var folder = tmDatabase.xmlDB_Folder(folderId);			
			var foldersToMap = tmDatabase.xmlDB_Folders_All(folder);			
			return (from folderToMap in foldersToMap
					from view in folderToMap.view
				    from guidanceItem in tmDatabase.getGuidanceItemsInView(view.id.guid())
					select guidanceItem).Distinct().toList();
		}

		public static Guid createGuidanceItem(this TM_Xml_Database tmDatabase, GuidanceItem_V3 guidanceItemV3)
		{
			if (guidanceItemV3.libraryId == Guid.Empty)
			{
				"[createGuidanceItem] no library provided for Guidance Item, stopping creation".error();
				return Guid.Empty;
			}
			var guidanceItem = tmDatabase.xmlDB_NewGuidanceItem(guidanceItemV3.guidanceItemId,
																guidanceItemV3.title, 
																guidanceItemV3.images,
			//													guidanceItemV3.lastUpdate,
																guidanceItemV3.topic,
																guidanceItemV3.technology,
																guidanceItemV3.category,
																guidanceItemV3.rule_Type,
																guidanceItemV3.priority,
																guidanceItemV3.status,
																guidanceItemV3.author,
																guidanceItemV3.phase,
																guidanceItemV3.htmlContent.sanitizeHtmlContent(),
																guidanceItemV3.libraryId);
			return guidanceItem.id.guid();
		}

		public static string sanitizeHtmlContent(this string htmlContent)
		{
			if (htmlContent.valid())
			{
				htmlContent = htmlContent.replace("href=\"ruledisplay:", "href=\"?#ruledisplay:"); // hack to make sure the current xrefs don't get removed by GetSafeHtmlFragment
				var sanitizedContent = Sanitizer.GetSafeHtmlFragment(htmlContent);
				return sanitizedContent;
			}
			return htmlContent;
		}		
	}
	
	
	//******************* Objects Conversion
	
	public static class TM_Xml_Database_ExtensionMethods_ObjectConversion
	{
		public static TM_GuidanceItem tmGuidanceItem(this guidanceItem _guidanceItem)
		{
			return new TM_GuidanceItem()
							{
								Id = _guidanceItem.id.guid(),
								Id_Original = _guidanceItem.id_original.notNull() 
													? _guidanceItem.id_original.guid()
													: Guid.Empty, 
								Author = _guidanceItem.Author,
								Category = _guidanceItem.Category,
								Priority = _guidanceItem.Priority,
								RuleType = _guidanceItem.Rule_Type,
								Status = _guidanceItem.Status,
								Technology = _guidanceItem.Technology,
								Title = _guidanceItem.title,
								Topic = _guidanceItem.Topic
								//,							
								//LastUpdate = DateTime.Parse(_guidanceItem.Date)
							};			
		}
		
		public static GuidanceItem_V3 tmGuidanceItemV3(this guidanceItem _guidanceItem) //, Guid guidanceId)
		{
			var phase = "";
			var ruleType = "";
			if (_guidanceItem.phase.valid())			
			{
				phase = _guidanceItem.phase;
				ruleType = _guidanceItem.Rule_Type ?? "";
			}
			else
			{
				phase = _guidanceItem.Rule_Type ?? "";
				ruleType = _guidanceItem.Type1 ?? "";
			}
						
			return new GuidanceItem_V3()
							{
								guidanceItemId 			= _guidanceItem.id.guid(), //guidanceId,
								guidanceItemId_Original = _guidanceItem.id_original.guid(),
								author 					= _guidanceItem.Author,
								category 				= _guidanceItem.Category,
								priority 				= _guidanceItem.Priority,								
								phase					= phase,
								rule_Type 				= ruleType,								
								status 					= _guidanceItem.Status,
								technology 				= _guidanceItem.Technology,
								title 					= _guidanceItem.title,								
								topic 					= _guidanceItem.Topic,
				//				lastUpdate 				= DateTime.Parse(_guidanceItem.Date),								
								htmlContent				= _guidanceItem.content.sanitizeHtmlContent(), 
								libraryId 				= _guidanceItem.libraryId.guid()
							};			
		}
		
		public static TM_GuidanceItem tmGuidanceItem(this GuidanceItem_V3 guidanceItemV3)
		{
			return new TM_GuidanceItem()
							{
								Id = guidanceItemV3.guidanceItemId,
								Id_Original = guidanceItemV3.guidanceItemId_Original,
								Library = guidanceItemV3.libraryId,
								Author = guidanceItemV3.author,
								Category = guidanceItemV3.category,
								Priority = guidanceItemV3.priority,
								RuleType = guidanceItemV3.rule_Type,
								Status = guidanceItemV3.status,
								Technology = guidanceItemV3.technology,
								Title = guidanceItemV3.title,
								Topic = guidanceItemV3.topic,								
					//			LastUpdate = guidanceItemV3.lastUpdate 
							};			
		}					
		
/*		public static TM_Folder tmFolder(this string folder, Guid libraryId)
		{
			return new TM_Folder()
						{
							Id = Guid.Empty,
							Name = folder,
							Caption = null,
							Library = libraryId
						};
		}
		
		public static TM_Folder tmFolder(this urn.microsoft.guidanceexplorer.View view, Guid libraryId, string folder)
		{
			return new TM_Folder()
						{
							Id = view.id.guid(),
							Name = folder,
							Caption = view.caption,
							Library = libraryId
						};
		}
*/

		public static View_V3 tmView(this urn.microsoft.guidanceexplorer.View view, Guid libraryId, Guid folderId)
		{
			var tmView = new View_V3()
							{	
								libraryId = libraryId,
								folderId = folderId,
								viewId= view.id.guid(),	
								caption = view.caption,
								author = view.author,																															
							};
			if(view.items.notNull())
				foreach(var item in view.items.item)
					tmView.guidanceItems.add(item.guid());
			return tmView;
		}
		
		public static urn.microsoft.guidanceexplorer.View view(this View tmView)
		{
			return new urn.microsoft.guidanceexplorer.View()
						{				
							caption = tmView.caption,							
							author = tmView.creator,
							id = tmView.id,
							creationDate = tmView.lastUpdate
						/*	
							//creatorCaption = view.author,							
							id= view.id,
							lastUpdate  = view.creationDate ?? new DateTime(),
							library = libraryId.str(),
							parentFolder = folder*/
						};
		}	
		//View' to 'urn.microsoft.guidanceexplorer.View'	
		
		public static Library library(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
		{
			if (tmLibrary.isNull())
				return null;
			return new Library()
				{	
					caption = tmLibrary.Caption,  
					id = tmLibrary.Id.str(),
					//Views = tmDatabase.tmViews(tmLibrary).ToArray()
				};
		}		
	}
	
	public static class TM_Xml_Database_ExtensionMethods_ObjectConversion_MiscFilters
	{
		public static TM_GuidanceItem tmGuidanceItem(this List<TM_GuidanceItem> tmGuidanceItems, Guid guidanceItemId)
		{
			return (from tmGuidanceItem in tmGuidanceItems
					where tmGuidanceItem.Id == guidanceItemId
					select tmGuidanceItem).first();
		}
		
		
	}
	
	public static class TM_Xml_Database_ExtensionMethods_ObjectConversion_Lists
	{
		public static List<TM_GuidanceItem> tmGuidanceItems(this List<GuidanceItem_V3> guidanceItemsV3)
		{
			return (from guidanceItemV3 in guidanceItemsV3
					select guidanceItemV3.tmGuidanceItem()).toList();
		}
	}	
}
