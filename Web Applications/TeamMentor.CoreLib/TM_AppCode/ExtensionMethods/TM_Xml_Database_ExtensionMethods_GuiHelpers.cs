using System;
using System.Linq; 
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
	public static class TM_Xml_Database_ExtensionMethods_GuiHelpers
	{
		public static List<Library_V3> getFolderStructure_Libraries(this TM_WebServices tmWebServices, TM_GUI_Objects guiObjects)
		{
			return (from library in tmWebServices.GetLibraries()
				    select tmWebServices.getFolderStructure_Library(library.Id,guiObjects)).toList();
		}
		
		public static Library_V3 getFolderStructure_Library(this TM_WebServices tmWebServices, Guid libraryId, TM_GUI_Objects guiObjects)
		{
            // ReSharper disable AccessToModifiedClosure
			//pre-create this mapping since the view retrieval was a massive performance bottle neck
			var allViews = new Dictionary<Guid, View_V3>();
			foreach(var view in tmWebServices.GetViews())
			    if (allViews.hasKey(view.viewId))
			        "[getFolderStructure_Library] duplicate viewID: {0} from Library {0}".format(view.viewId, view.libraryId);  // this should be moved into a TM Library health check
                else
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
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable ImplicitlyCapturedClosure
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
						parentFolder.add(folder); 						
						mapFolderViews(folder);
					}				
				};            
			Func<Guid,string, Library_V3> mapLibrary = 
				(library_Id, libraryName)=> {
										var libraryV3 = new Library_V3
										    {
														libraryId = library_Id,
														name = libraryName
													};												
										mapFolders(tmWebServices.GetFolders(library_Id), libraryV3.subFolders); 
										libraryV3.views.add(mapViews(tmWebServices.GetViewsInLibraryRoot(libraryId).guids()));
										libraryV3.guidanceItems = tmWebServices.getGuidanceItemsIds_NotInViews(library_Id);
										return libraryV3;
									};			

			Func<Guid,Library_V3> getLibrary_TreeStructure = 
				(id)=>{			
								var tmLibrary = tmWebServices.GetLibraryById(id);
								if (tmLibrary.isNull())
								{
									"[in getLibraryFolderStructure] could not find library with id: {0}".error(id);
									return null;
								}															
								return mapLibrary(id, tmLibrary.caption);					
							};
			return getLibrary_TreeStructure(libraryId);
            // ReSharper restore PossibleNullReferenceException				            
			// ReSharper restore ImplicitlyCapturedClosure		 
            // ReSharper restore AccessToModifiedClosure
		}
		
		public static List<Guid> getGuidanceItemsIds_NotInViews(this TM_WebServices tmWebServices, Guid libraryId)
		{					
			var guidanceInViews = (from view in tmWebServices.GetViews()
								   where view.libraryId == libraryId
								   from guidanceItem in view.guidanceItems
								   select guidanceItem).Distinct().toList(); 

					
			var guidanceItemsIdsNotInViews = (from guidanceItem in tmWebServices.GetGuidanceItemsInLibrary(libraryId)
											where guidanceInViews.contains(guidanceItem.Metadata.Id).isFalse()
											select guidanceItem.Metadata.Id).toList();		
			return guidanceItemsIdsNotInViews;
		}
    }
}