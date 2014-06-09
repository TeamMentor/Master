using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceItems_Search
    {				
        public static List<Guid> guidanceItems_SearchTitleAndHtml(this TM_Xml_Database tmDatabase, string searchText)
        {
            return tmDatabase.guidanceItems_SearchTitleAndHtml(tmDatabase.xmlDB_GuidanceItems() , searchText);
        }        
        public static List<Guid> guidanceItems_SearchTitleAndHtml(this TM_Xml_Database tmDatabase, List<Guid> guidanceItemsIds, string searchText)
        {
            List<TeamMentor_Article> guidanceItems;
            var searchTitleAndBody = true;

            //first see if there are special tags in the seach text
            if (searchText.starts("all:")) // means we want to do a full search
            {
                guidanceItemsIds.Clear();
                searchText = searchText.remove("all:");
            }
            else if (searchText.starts("title:"))
            {
                searchTitleAndBody = false;
                searchText = searchText.remove("title:");
            }

            //figure out which guidanceItems to search on
            switch (guidanceItemsIds.size())  
            {
                case 0:         // if there are no guidanceItemsIds provided, search on all of them
                    guidanceItems = tmDatabase.xmlDB_GuidanceItems() ;
                    break;
                case 1:         // handle special case where the ID provided is from a library, folder or view
                    var id = guidanceItemsIds.first();
                    guidanceItems =tmDatabase.getGuidanceItems_from_LibraryFolderOrView(id);
                    if (guidanceItems.isNull())     // if there was no mapping, use the id as a GuidanceItem ID
                        guidanceItems = tmDatabase.xmlDB_GuidanceItems(guidanceItemsIds);
                    break;
                default:
                    guidanceItems = tmDatabase.xmlDB_GuidanceItems(guidanceItemsIds);
                    break;
            }

            if (searchTitleAndBody)
                return tmDatabase.guidanceItems_SearchTitleAndHtml(guidanceItems, searchText);
            return tmDatabase.guidanceItems_SearchTitle(guidanceItems, searchText);
        }
        
        public static List<Guid> guidanceItems_SearchTitleAndHtml(this TM_Xml_Database tmDatabase, List<TeamMentor_Article> guidanceItems, string searchText)
        {
            var searchTextEncoded = HttpUtility.HtmlEncode(searchText).lower();   
            
            //var maxNumberOfItemsToReturn = 100;			
            "There are {0} GIs to search".error(guidanceItems.size());
            return 	(from guidanceItem in guidanceItems
                where guidanceItem.Metadata.Title.valid() &&
                      (guidanceItem.Metadata.Title.lower().contains(searchTextEncoded)       ||
//					        guidanceItem.title.regEx	   				(searchText) 	 ||
                       guidanceItem.Content.Data.Value.lower().contains(searchTextEncoded) )
//                       || guidanceItem.htmlContent.regEx			(searchText)           )									
                select guidanceItem.Metadata.Id
                ).toList(); 
        }

        public static List<Guid> guidanceItems_SearchTitle(this TM_Xml_Database tmDatabase, List<TeamMentor_Article> guidanceItems, string searchText)
        {
            var searchTextEncoded = HttpUtility.HtmlEncode(searchText).lower();
                       
            return (from guidanceItem in guidanceItems
                where guidanceItem.Metadata.Title.valid() &&
                      guidanceItem.Metadata.Title.lower().contains(searchTextEncoded)            
                select guidanceItem.Metadata.Id
                ).toList();
        }        
        
/*		public static List<Guid> guidanceItem_SearchTitle(this TM_Xml_Database tmDatabase, string searchText)
        {
            var maxNumberOfItemsToReturn = 250000;
            var lowercaseSearchText = searchText.lower();
            return 	(from guidanceItem in tmDatabase.GuidanceItems
                     where guidanceItem.title.lower().contains(lowercaseSearchText) ||
                           guidanceItem.title.regEx(searchText)
                     select new Item() { Key =guidanceItem.title, 
                                        Value= guidanceItem.guidanceItemId.str() }
                    ).Take(maxNumberOfItemsToReturn)
                     .toList(); 
        }*/				
        public static List<TeamMentor_Article> getGuidanceItems_from_LibraryFolderOrView(this TM_Xml_Database tmDatabase, Guid id)
        {
            if (tmDatabase.tmLibrary(id).notNull())                         // first search on the library
                return tmDatabase.tmGuidanceItems(id);
            if (tmDatabase.tmFolder(id).notNull())                          // the on the folders
                return tmDatabase.xmlDB_GuidanceItems(tmDatabase.tmFolder(id));
            if (tmDatabase.tmView(id).notNull())                            // then on the views
                return tmDatabase.xmlDB_GuidanceItems(tmDatabase.tmView(id).guidanceItems);
            return null;
        }

        public static List<TeamMentor_Article> xmlDB_GuidanceItems(this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.Cached_GuidanceItems.Values.toList();
        }		
        
        public static List<TeamMentor_Article> xmlDB_GuidanceItems(this TM_Xml_Database tmDatabase, List<Guid> guidanceItemsIds)
        {
            return (from guidanceItem in tmDatabase.Cached_GuidanceItems.Values
                where guidanceItemsIds.contains(guidanceItem.Metadata.Id)
                select guidanceItem).toList();
        }
        public static List<TeamMentor_Article> xmlDB_GuidanceItems(this TM_Xml_Database tmDatabase, Folder_V3 folder)
        { 
            return tmDatabase.tmGuidanceItems_InFolder(folder.folderId);                       
        }
    }
}