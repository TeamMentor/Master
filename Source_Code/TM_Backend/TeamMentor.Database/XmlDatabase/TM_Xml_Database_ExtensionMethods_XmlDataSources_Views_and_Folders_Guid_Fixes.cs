using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_Views_and_Folders_Guid_Fixes
    {
        public static TM_Xml_Database                               ensureFoldersAndViewsIdsAreUnique       (this TM_Xml_Database tmDatabase, bool verbose = false)
        {
            var conflictsDetected = 0;
            //"[TM_Xml_Database] in ensureFoldersAndViewsIdsAreUnique".info();
            var mappedItems = new List<string>();
            foreach(var view in tmDatabase.xmlDB_Views())
            {
                if(mappedItems.contains(view.id))
                {
                    conflictsDetected++;
                    if (verbose)
                        "[TM_Xml_Database][ensureFoldersAndViewsIdsAreUnique] there was repeated viewId for view {0}: {1}".error(view.caption, view.id);
                    view.id = Guid.NewGuid().str();
                    if (verbose)
                        "[TM_Xml_Database][ensureFoldersAndViewsIdsAreUnique] new Guid assigned to view {0}: {1}".debug(view.caption, view.id);					
                }					
                mappedItems.Add(view.id);
            }
            
            foreach(var folder in tmDatabase.xmlDB_Folders())
            {
                if(mappedItems.contains(folder.folderId))
                {
                    if (verbose)
                        "[TM_Xml_Database][ensureFoldersAndViewsIdsAreUnique] there was repeated folderId for folder {0}: {1}".error(folder.caption, folder.folderId);
                    folder.folderId = Guid.NewGuid().str();
                    if (verbose)
                        "[TM_Xml_Database][ensureFoldersAndViewsIdsAreUnique] new Guid assigned to view {0}: {1}".debug(folder.caption, folder.folderId);
                    conflictsDetected++;
                }					
                mappedItems.Add(folder.folderId);
            }
            if (conflictsDetected > 0)
            {
                "[TM_Xml_Database][ensureFoldersAndViewsIdsAreUnique] There were {0} fixes made: {0}".info(conflictsDetected);	 
                tmDatabase.xmlDB_Save_GuidanceExplorers();
            }
            return tmDatabase;
        }		
        public static TM_Xml_Database                               removeMissingGuidanceItemsIdsFromViews  (this TM_Xml_Database tmDatabase, bool verbose = false)
        {				
            //"[TM_Xml_Database] in removeMissingGuidanceItemsIdsFromViews".info();
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
                                "[TM_Xml_Database][removeMissingGuidanceItemsIdsFromViews] in view {0}:{1}, there was a missing guid: {2}".error(view.caption, view.id, id);
                        }
                    //else
                    //"guid in View: {0}".info(id);					
                }
            }
            if (conflictsDetected >0)
            {
                "[TM_Xml_Database][removeMissingGuidanceItemsIdsFromViews] There were {0} fixes made: {0}".error(conflictsDetected);	 
                tmDatabase.xmlDB_Save_GuidanceExplorers();
            }
            return tmDatabase;
        }
    }
}