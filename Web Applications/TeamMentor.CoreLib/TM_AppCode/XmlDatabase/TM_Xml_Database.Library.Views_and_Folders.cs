using System;
using System.Linq;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{	
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_View
    {	
        public static TM_Library                                    tmLibrary                           (this urn.microsoft.guidanceexplorer.View viewToFind, TM_Xml_Database tmDatabase)
        {
            return (from tmLibrary in tmDatabase.tmLibraries()
                    from view in tmLibrary.xmlDB_Views(tmDatabase)					
                    where view == viewToFind
                    select tmLibrary).first();
        }		
        public static urn.microsoft.guidanceexplorer.View           xmlDB_View                          (this TM_Xml_Database tmDatabase, Guid viewId)
        {
            return (from view in tmDatabase.xmlDB_Views()					
                    where view.id == viewId.str()
                    select view).first();
        }			
        public static urn.microsoft.guidanceexplorer.View           xmlDB_View                          (this urn.microsoft.guidanceexplorer.Folder folder, Guid viewId)
        {
            return (from view in folder.view
                    where view.id == viewId.str()
                    select view).first();
        }		
        public static urn.microsoft.guidanceexplorer.View           xmlDB_View                          (this urn.microsoft.guidanceexplorer.Folder folder, string viewCaption)
        {
            return (from view in folder.view
                    where view.caption == viewCaption
                    select view).first();
        }		
        public static List<urn.microsoft.guidanceexplorer.View>     xmlDB_Views                         (this TM_Xml_Database tmDatabase)
        {
            return (from tmLibrary in tmDatabase.tmLibraries()
                    from view in tmLibrary.xmlDB_Views(tmDatabase)					
                    select view).toList();
        }		
        public static List<urn.microsoft.guidanceexplorer.View>     xmlDB_Views                         (this TM_Library tmLibrary , TM_Xml_Database tmDatabase)
        {
            var allViews = tmLibrary.xmlDB_Views_InLibraryRoot(tmDatabase);
            //add the ones from the libraryRoot
            
            //add the ones from the folders
            allViews.AddRange((from folder in tmDatabase.xmlDB_Folders_All(tmLibrary.Id)
                               from view in folder.view					
                               select view).toList());
            return allViews;
        }		
        public static List<urn.microsoft.guidanceexplorer.View>     xmlDB_Views_InLibraryRoot           (this TM_Library tmLibrary , TM_Xml_Database tmDatabase)
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
        public static List<urn.microsoft.guidanceexplorer.View>     xmlDB_Views                         (this urn.microsoft.guidanceexplorer.Folder folder)
        {
            if (folder.notNull())
                return folder.view.toList();
            return new List<urn.microsoft.guidanceexplorer.View>();
        }		
        public static List<Guid>                                    guids                               (this List<View_V3> views)
        {
            return (from view in views
                    select view.viewId).toList();
        }

        [EditArticles] 	public static View_V3                                       newView                             (this TM_Xml_Database tmDatabase, Guid parentFolderId, View tmView)
        {
            var view = tmDatabase.xmlDB_NewView(parentFolderId, tmView);
            return tmDatabase.tmView(view.id.guid());
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_NewView                       (this TM_Xml_Database tmDatabase, View tmView)
        {
            return tmDatabase.xmlDB_NewView(Guid.Empty,  tmView);
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_NewView                       (this TM_Xml_Database tmDatabase, Guid parentFolderId, View tmView)
        {			
            var tmLibrary = tmDatabase.tmLibrary(tmView.library.guid());
            //var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(tmView.library.guid());
            if (tmLibrary.notNull())
            {				
                if (tmView.id.isNull() || (tmView.id.isGuid() && tmView.id == Guid.Empty.str()))
                    tmView.id = Guid.NewGuid().str();
                var view = tmView.view();

                if (parentFolderId == Guid.Empty)
                {
                    var guidanceExplorer = tmLibrary.guidanceExplorer(tmDatabase);
                    guidanceExplorer.library.libraryStructure.view.Add(view);
                    //tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);
                    //return tmDatabase.xmlDB_View(tmView.id.guid()); 					
                }
                else
                { 
                    var folder = tmDatabase.xmlDB_Folder(tmLibrary.Id, parentFolderId);
                    if (folder.isNull())
                    {
                        "[xmlDB_NewView] could not find parent folder (to add view) with id {0}".error(parentFolderId);
                        return null;
                    }
                    folder.view.Add(view);				
                }
                tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
                return view;
                //return folder.xmlDB_View(tmView.id.guid()); 					// I have to get the reference again since the object is different after the calls to xmlDB_Save_GuidanceExplorer				                
            }
            return null;
        }				
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_UpdateView                    (this TM_Xml_Database tmDatabase, View tmView)
        {
            return tmDatabase.xmlDB_UpdateView(tmView, new List<Guid>());
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_UpdateView                    (this TM_Xml_Database tmDatabase, View tmView, List<Guid> guidanceItems)
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
        [EditArticles] 	public static bool                                          xmlDB_RemoveView                    (this TM_Xml_Database tmDatabase, Guid libraryId,  Guid viewId )
        {
            return tmDatabase.xmlDB_RemoveView(tmDatabase.tmLibrary(libraryId), viewId);
        }		
        [EditArticles] 	public static bool                                          xmlDB_RemoveView                    (this TM_Xml_Database tmDatabase, TM_Library tmLibrary, Guid viewId )
        {
            if (tmLibrary.isNull())
                "in xmlDB_RemoveViewFromFolder provided tmLibrary was null".error();
            else
            {
                var view = tmDatabase.xmlDB_View(viewId);
                if (view.notNull())
                {
                    var guidanceExplorer = tmLibrary.guidanceExplorer(tmDatabase);
                    if (guidanceExplorer.library.libraryStructure.view.contains(view))          // the view was in the library root
                    {
                        guidanceExplorer.library.libraryStructure.view.remove(view);
                    }
                    else
                    {
                        foreach (var folder in tmDatabase.xmlDB_Folders_All(tmLibrary.Id))
                        {
                            if (folder.view.contains(view))
                                folder.view.remove(view);
                        }
                    }                                                                        
                    tmLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);						
                    return true;
                }              
            }
            return false;
        }		
        [EditArticles] 	public static bool                                          xmlDB_MoveViewToFolder              (this TM_Xml_Database tmDatabase, Guid viewId, Guid targetFolderId, Guid targetLibraryId)
        {
            try
            {
                var viewToMove = tmDatabase.xmlDB_View(viewId);			
                if (viewToMove.notNull())
                {
                    var tmView =  tmDatabase.tmView(viewToMove.id.guid());  
                    //"found viewToMove : {0}".info(viewToMove.caption);

                    var tmSourceLibrary = tmDatabase.tmLibrary(tmView.libraryId);
                    var tmTargetLibrary = tmDatabase.tmLibrary(targetLibraryId);

                    tmDatabase.xmlDB_RemoveView(tmSourceLibrary, tmView.viewId);
                    
                    tmSourceLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);

                    if(targetFolderId == Guid.Empty)                // add view to Library root
                    {						
                        var guidanceExplorer = tmTargetLibrary.guidanceExplorer(tmDatabase);										
                        guidanceExplorer.library.libraryStructure.view.Add(viewToMove);					
                        "Moved view to library root".info();
                    }	
                    else
                    {
                        var targetFolder = tmDatabase.xmlDB_Folder(targetFolderId);
                        targetFolder.view.Add(viewToMove);
                        "Moved view to folder : {0}".info(targetFolder.caption);                        
                    }

                    //if (tmTargetLibrary.Id != tmSourceLibrary.Id)
                                        
                    tmTargetLibrary.xmlDB_Save_GuidanceExplorer(tmDatabase);				
                    
                    return true;
                }
            }
            catch(Exception ex)
            {
                ex.log();
            }
            return false;
        }				
        [EditArticles] 	public static bool                                          xmlDB_AddGuidanceItemToView         (this TM_Xml_Database tmDatabase, Guid viewId, Guid guidanceItemsId)
        {
            return tmDatabase.xmlDB_AddGuidanceItemsToView(viewId, guidanceItemsId.wrapOnList());
        }		
        [EditArticles] 	public static bool                                          xmlDB_AddGuidanceItemsToView        (this TM_Xml_Database tmDatabase, Guid viewId, List<Guid> guidanceItemsIds)
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
        [EditArticles] 	public static bool                                          xmlDB_RemoveGuidanceItemsFromView   (this TM_Xml_Database tmDatabase, Guid viewId, List<Guid> guidanceItemsIds)
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
        [EditArticles] 	public static bool                                          xmlDB_RemoveAllGuidanceItemsFromView(this TM_Xml_Database tmDatabase, Guid viewId)
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

    }
    
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_Folder
    {
        public static List<urn.microsoft.guidanceexplorer.Folder>   xmlDB_Folders       (this TM_Xml_Database tmDatabase)
        {						
            return (from tmLibrary in tmDatabase.tmLibraries()
                    from folder in tmLibrary.xmlDB_Folders(tmDatabase)
                    select folder).toList();
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders       (this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
        {
            return tmDatabase.xmlDB_Folders(tmLibrary.Id);
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders       (this TM_Xml_Database tmDatabase, Guid libraryId)
        {			
            try
            {			
                if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId))
                {
                    var libraryStructure = TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure;					
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
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders       (this urn.microsoft.guidanceexplorer.Folder folderToMap)
        {            
            return folderToMap.isNull() 
                        ? new List<urn.microsoft.guidanceexplorer.Folder>() 
                        : folderToMap.folder1;
        }
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase)
        {
            return (from tmLibrary in tmDatabase.tmLibraries()
                    from folder in tmDatabase.xmlDB_Folders_All(tmLibrary.Id)
                    select folder).toList();
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase, Guid libraryId)
        {		
            return tmDatabase.xmlDB_Folders_All(tmDatabase.xmlDB_Folders(libraryId));			
        }				
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase, IList<urn.microsoft.guidanceexplorer.Folder> foldersToMap)
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
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase, urn.microsoft.guidanceexplorer.Folder folderToMap)
        {
            var folders = new List<urn.microsoft.guidanceexplorer.Folder> ();			
            if (folderToMap.notNull())
            {
                folders.add(folderToMap);
                folders.AddRange(tmDatabase.xmlDB_Folders_All(folderToMap.folder1));
            }
            return folders;
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid folderId)
        {
            return tmDatabase.xmlDB_Folders_All()
                             .FirstOrDefault(folder => folder.folderId == folderId.str());
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
        {
            return tmDatabase.xmlDB_Folders_All(libraryId)
                             .FirstOrDefault(folder => folder.folderId == folderId.str());
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid libraryId, Guid parentFolder,string folderName)
        {
            return tmDatabase.xmlDB_Folders_All(libraryId)
                             .Where(folder => folder.folderId == parentFolder.str())
                             .SelectMany(folder => folder.folder1)
                             .FirstOrDefault(subFolder => subFolder.caption == folderName);
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid libraryId,  string folderCaption)
        {
            var tmLibrary = tmDatabase.tmLibrary(libraryId); 
            return tmLibrary.xmlDB_Folder(folderCaption, tmDatabase);
        }		
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Library tmLibrary, string folderCaption, TM_Xml_Database tmDatabase)
        {
            return (from folder in tmLibrary.xmlDB_Folders(tmDatabase)
                    where folder.caption == folderCaption
                    select folder).first();				
        }		

        [EditArticles] 	public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder    (this TM_Xml_Database tmDatabase, Guid libraryId, string folderCaption)
        {
            return tmDatabase.xmlDB_Add_Folder(libraryId, Guid.Empty, folderCaption);
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder    (this TM_Xml_Database tmDatabase, Guid libraryId, Guid parentFolderId, string folderCaption)
        {
            var tmLibrary = tmDatabase.tmLibrary(libraryId); 
            return tmLibrary.xmlDB_Add_Folder(parentFolderId, folderCaption, tmDatabase);
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder    (this TM_Library tmLibrary, Guid parentFolderId, string folderCaption, TM_Xml_Database tmDatabase)
        {		
            try
            {	
                var newFolder = new urn.microsoft.guidanceexplorer.Folder
                    { 
                            caption = folderCaption, 
                            folderId = Guid.NewGuid().str()
                        };
                var guidanceExplorer= tmLibrary.guidanceExplorer(tmDatabase);                
                if (parentFolderId == Guid.Empty)
                {
                    //adding a folder to the library
                    guidanceExplorer.library.libraryStructure.folder.Add(newFolder);
                }
                else
                {
                    //adding a folder to folder
                    var folder = tmDatabase.xmlDB_Folder(tmLibrary.Id, parentFolderId);
                    if (folder == null)
                    {
                        "[xmlDB_Add_Folder] couldn't find parent folder (to add folder) with id: {0}".error(parentFolderId);
                        return null;
                    }
                    folder.folder1.Add(newFolder);                                        
                }                        
                guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);	                    
                return tmDatabase.xmlDB_Folder(tmLibrary.Id, newFolder.folderId.guid());                                                
            }
            catch(Exception ex)
            {
                ex.log();
                return null;
            }
        }				
        [EditArticles] 	public static bool                                  xmlDB_Rename_Folder (this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId, string newFolderName)
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
        [EditArticles] 	public static bool                                  xmlDB_Delete_Folder (this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
        {
            try
            {
                if (folderId == Guid.Empty)
                    return false;
                
                var folderToDelete = tmDatabase.xmlDB_Folder(libraryId, folderId);
                if (folderToDelete.isNull())
                    return false;

                var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);                

                //see if the folder is on the library root
                if (guidanceExplorer.library.libraryStructure.folder.contains(folderToDelete))
                {
                    guidanceExplorer.library.libraryStructure.folder.remove(folderToDelete);
                    tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId);
                    return true;
                }
                // see if the folder to delete is a subfolder
                foreach (var folder in tmDatabase.xmlDB_Folders_All(libraryId))
                {
                    if (folder.folder1.contains(folderToDelete))
                    {
                        folder.folder1.remove(folderToDelete);
                        tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId);
                        return true;
                    }
                }
                "[xmlDB_Delete_Folder] could not find folder to delete: {0} {1}".error(libraryId, folderId);
            }
            catch(Exception ex)
            {
                ex.log("in xmlDB_Delete_Folder");                
            }
            return false;
        }		
        /*public static urn.microsoft.guidanceexplorer.View xmlDB_RemoveView(urn.microsoft.guidanceexplorer.Folder , View tmView, TM_Xml_Database tmDatabase)
        {
            
        }*/
    }
    
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_Views_and_Folders_Guid_Fixes
    {
        public static TM_Xml_Database                               ensureFoldersAndViewsIdsAreUnique       (this TM_Xml_Database tmDatabase, bool verbose = false)
        {
            var conflictsDetected = 0;
            "in ensureFoldersAndViewsIdsAreUnique".info();
            var mappedItems = new List<string>();
            foreach(var view in tmDatabase.xmlDB_Views())
            {
                if(mappedItems.contains(view.id))
                {
                    conflictsDetected++;
                    if (verbose)
                        "[ensureFoldersAndViewsIdsAreUnique] there was repeated viewId for view {0}: {1}".error(view.caption, view.id);
                    view.id = Guid.NewGuid().str();
                    if (verbose)
                        "[ensureFoldersAndViewsIdsAreUnique] new Guid assigned to view {0}: {1}".debug(view.caption, view.id);					
                }					
                mappedItems.Add(view.id);
            }
            
            foreach(var folder in tmDatabase.xmlDB_Folders())
            {
                if(mappedItems.contains(folder.folderId))
                {
                    if (verbose)
                        "[ensureFoldersAndViewsIdsAreUnique] there was repeated folderId for folder {0}: {1}".error(folder.caption, folder.folderId);
                    folder.folderId = Guid.NewGuid().str();
                    if (verbose)
                        "[ensureFoldersAndViewsIdsAreUnique] new Guid assigned to view {0}: {1}".debug(folder.caption, folder.folderId);
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
        public static TM_Xml_Database                               removeMissingGuidanceItemsIdsFromViews  (this TM_Xml_Database tmDatabase, bool verbose = false)
        {				
            "in removeMissingGuidanceItemsIdsFromViews".info();
            var conflictsDetected = 0;
            foreach(var view in tmDatabase.xmlDB_Views())
            {					
                //"[removeMissingGuidanceItemsIdsFromViews] mapping view: {0}".info(view.caption);	 
                if (view.items.notNull() && view.items.item.notNull() && view.items.item.Count > 0)
                {					
                    foreach(var id in view.items.item.toList())
                        if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(id.guid()).isFalse())
                        {
                            view.items.item.Remove(id);
                            conflictsDetected++;
                            if (verbose)
                                "[removeMissingGuidanceItemsIdsFromViews] in view {0}:{1}, there was a missing guid: {2}".error(view.caption, view.id, id);
                        }
                        //else
                            //"guid in View: {0}".info(id);					
                }
            }
            if (conflictsDetected >0)
            {
                "[removeMissingGuidanceItemsIdsFromViews] There were {0} fixes made: {0}".error(conflictsDetected);	 
                tmDatabase.xmlDB_Save_GuidanceExplorers();
            }
            return tmDatabase;
        }
    }
}