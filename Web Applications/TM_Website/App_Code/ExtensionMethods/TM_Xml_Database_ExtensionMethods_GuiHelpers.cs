using System;
using System.Web;
using System.Linq; 
using System.Collections.Generic;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2File:../XmlDatabase/TM_Xml_Database_JavaScriptProxy.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public static class TM_Xml_Database_ExtensionMethods_GuiHelpers
	{
		public static List<Library_V3> getFolderStructure_Libraries(this IJavascriptProxy javascriptProxy, TM_GUI_Objects guiObjects)
		{
			return (from library in javascriptProxy.GetLibraries()
				    select javascriptProxy.getFolderStructure_Library(library.Id,guiObjects)).toList();
		}
		
		public static Library_V3 getFolderStructure_Library(this IJavascriptProxy javascriptProxy, Guid libraryId, TM_GUI_Objects guiObjects)
		{
			//pre-create this mapping since the view retrieval was a massive performance bottle neck
			var allViews = new Dictionary<Guid, View_V3>();
			foreach(var view in javascriptProxy.GetViews())
				allViews.Add(view.viewId, view);
				
			Action<Folder_V3> mapFolderViews =  null;

			Func<List<Guid>, List<View_V3>> mapViews = 
				(viewGuids) => 
							{					
								var views = new List<View_V3>(); 
									
								foreach(var viewGuid in viewGuids)				
								{
									var view = 	allViews[viewGuid];													
									
									/*// compress view.guidanceItems using guiObjects
									view.guidanceItems_Indexes = (from guid in view.guidanceItems
															      select guiObjects.get_Index(guid.str()).str()).toList()
																  .join(",").remove(" ");
									view.guidanceItems.Clear();*/
									views.add(view);
								}
								return views;
							};

			mapFolderViews = 
				(folder) => {		 			 
								 var mappedViews = mapViews(folder.views.guids());
								 folder.views.Clear();
								 folder.views.add(mappedViews);								 
								 foreach(var subFolder in folder.subFolders)
									mapFolderViews(subFolder);
							};
				
			Action<List<Folder_V3>, List<Folder_V3>> mapFolders = (folders, parentFolder) =>
				{					
					foreach(var folder in folders)
					{ 			
						var folderId = folder.folderId;			
						parentFolder.add(folder); 						
						mapFolderViews(folder);
					}				
				};
			 
			Func<Guid,string, Library_V3> mapLibrary = 
				(_libraryId, libraryName)=> {
										var libraryV3 = new Library_V3() 
													{
														libraryId = _libraryId,
														name = libraryName
													};												
										mapFolders(javascriptProxy.GetFolders(_libraryId), libraryV3.subFolders); 
										libraryV3.views.add(mapViews(javascriptProxy.GetViewsInLibraryRoot(libraryId.str()).guids()));
										libraryV3.guidanceItems = javascriptProxy.getGuidanceItemsIds_NotInViews(_libraryId);
										return libraryV3;
									};
					 

			Func<Guid,Library_V3> getLibrary_TreeStructure = 
				(_libraryId)=>{			
								var tmLibrary = javascriptProxy.GetLibraryById(_libraryId.str());
								if (tmLibrary.isNull())
								{
									"[in getLibraryFolderStructure] could not find library with id: {0}".error(_libraryId);
									return null;
								}															
								return mapLibrary(_libraryId, tmLibrary.caption);					
							};
			return getLibrary_TreeStructure(libraryId);
		}
		
		public static List<Guid> getGuidanceItemsIds_NotInViews(this IJavascriptProxy javascriptProxy, Guid libraryId)
		{					
			var guidanceInViews = (from view in javascriptProxy.GetViews()
								   where view.libraryId == libraryId
								   from guidanceItem in view.guidanceItems
								   select guidanceItem).Distinct().toList(); 

					
			var guidanceItemsIdsNotInViews = (from guidanceItem in javascriptProxy.GetGuidanceItemsInLibrary(libraryId)
											where guidanceInViews.contains(guidanceItem.Id).isFalse()
											select guidanceItem.Id).toList();		
			return guidanceItemsIdsNotInViews;
		}
    }
}