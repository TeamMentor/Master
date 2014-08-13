using System;
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;

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
            UserRole.EditArticles.demand();
            var view = tmDatabase.xmlDB_NewView(parentFolderId, tmView);
            return (view.notNull()) 
                ? tmDatabase.tmView(view.id.guid())
                : null;
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_NewView                       (this TM_Xml_Database tmDatabase, View tmView)
        {
            UserRole.EditArticles.demand();
            return tmDatabase.xmlDB_NewView(Guid.Empty,  tmView);
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_NewView                       (this TM_Xml_Database tmDatabase, Guid parentFolderId, View tmView)
        {
            UserRole.EditArticles.demand();
            if (tmView.isNull())
                return null;
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
            UserRole.EditArticles.demand();
            return tmDatabase.xmlDB_UpdateView(tmView, new List<Guid>());
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.View           xmlDB_UpdateView                    (this TM_Xml_Database tmDatabase, View tmView, List<Guid> guidanceItems)
        {
            UserRole.EditArticles.demand();
            if (tmView.isNull())
                return null;
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
            UserRole.EditArticles.demand();
            return tmDatabase.xmlDB_RemoveView(tmDatabase.tmLibrary(libraryId), viewId);
        }		
        [EditArticles] 	public static bool                                          xmlDB_RemoveView                    (this TM_Xml_Database tmDatabase, TM_Library tmLibrary, Guid viewId )
        {
            UserRole.EditArticles.demand();
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
            UserRole.EditArticles.demand();
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
            UserRole.EditArticles.demand();
            return tmDatabase.xmlDB_AddGuidanceItemsToView(viewId, guidanceItemsId.wrapOnList());
        }		
        [EditArticles] 	public static bool                                          xmlDB_AddGuidanceItemsToView        (this TM_Xml_Database tmDatabase, Guid viewId, List<Guid> guidanceItemsIds)
        {	
            UserRole.EditArticles.demand();
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
            UserRole.EditArticles.demand();
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
            UserRole.EditArticles.demand();
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
}