using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using FluentSharp;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;


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
    
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceItems_Load
    {				
        public static TM_Xml_Database           xmlDB_Load_GuidanceItems_and_Create_CacheFile(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.UsingFileStorage)
            { 
                var pathXmlLibraries = TM_Xml_Database.Current.Path_XmlLibraries;            
                if (pathXmlLibraries.notNull() && pathXmlLibraries.notNull())
                    lock (pathXmlLibraries)
                    {
                        //if (tmDatabase.getCacheLocation().fileExists().isFalse())
                        //{
                            "[TM_Xml_Database] in xmlDB_Load_GuidanceItems, creating cache file".debug();
                            var o2Timer = new O2Timer("loaded GuidanceItems from disk").start();
                            //Load GuidanceItem from the disk				
                            foreach (var item in tmDatabase.GuidanceExplorers_Paths)
                            {
                                var guidanceExplorer = item.Key;
                                var pathToLibraryGuidanceItems = item.Value.parentFolder();
                                var libraryId = guidanceExplorer.library.name.guid();                                
                                "libraryId: {0} : {1}".info(libraryId, pathToLibraryGuidanceItems);                                
                                var filesToLoad = pathToLibraryGuidanceItems.files(true, "*.xml");
                                tmDatabase.xmlDB_Load_GuidanceItemsV3(libraryId, filesToLoad);
                            }

                            //save it to the local cache file (reduces load time from 8s to 0.5s)
                            tmDatabase.save_GuidanceItemsToCache();
                            

                            tmDatabase.ensureFoldersAndViewsIdsAreUnique();
                            tmDatabase.removeMissingGuidanceItemsIdsFromViews();
                            o2Timer.stop();
                        //}
                    }
            }
            return tmDatabase;
        }        
        public static List<TeamMentor_Article>  xmlDB_Load_GuidanceItemsV3(this TM_Xml_Database tmDatabase, Guid libraryId, List<string> guidanceItemsFullPaths)
        {
            var o2Timer = new O2Timer("xmlDB_GuidanceItems").start();
            var itemsLoaded = 0;
            //var maxToLoad = 1000;
            var guidanceItems = new List<TeamMentor_Article>();
            foreach(var fullPath in guidanceItemsFullPaths)
            { 
                var guidanceItemId = fullPath.fileName().remove(".xml");
                if (guidanceItemId.isGuid())
                {
                    var guidanceItem = tmDatabase.xmlDB_GuidanceItem(guidanceItemId.guid(),fullPath);
                    if (guidanceItem.notNull())
                    {
                        guidanceItems.add(guidanceItem);						    
                        guidanceItem.Metadata.Library_Id = libraryId;						
                    }
                    //if (maxToload-- < 1)
                    //	break;
                    if (itemsLoaded++ % 200 == 0)
                        "loaded {0} items".info(itemsLoaded);
                }
                //if (itemsLoaded > maxToLoad)
                //	break; 
                    
            }
            o2Timer.stop();
            return guidanceItems;
        }        
        public static TeamMentor_Article        xmlDB_GuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            return tmDatabase.xmlDB_GuidanceItem(guidanceItemId, null);
        }
        public static TeamMentor_Article        fixGuidanceItemFileDueToGuidConflict(this TM_Xml_Database tmDatabase, Guid original_Guid, string fullPath)
        {			
            var newGuid = Guid.NewGuid();
            var newPath = fullPath.replace(original_Guid.str(), newGuid.str());
            Files.moveFile(fullPath, newPath);
            "[xmlDB_GuidanceItem] resolved GuidanceItem ID conflict for  Id '{0}' was already mapped. \nExisting path: \t{1} \nNew path:  \t{2}".error(original_Guid, fullPath , newPath);
            var guidanceItemV3 = tmDatabase.xmlDB_GuidanceItem(newGuid, newPath);			
            return guidanceItemV3;
        }        
        //[EditArticles]
        public static TeamMentor_Article        xmlDB_GuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId, string fullPath)
        {
            try
            {
                if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(guidanceItemId))
                {
                    //"found match for id: {0} in {1}".info(guidanceItemId, fullPath);
                    if (TM_Xml_Database.Current.GuidanceItems_FileMappings[guidanceItemId] != fullPath)
                    {						
                        //"[xmlDB_GuidanceItem] GuidanceItem ID conflict, the Id '{0}' was already mapped. \nExisting path: \t{1} \nNew path:  \t{2}".error(
                        //	guidanceItemId, TM_Xml_Database.GuidanceItems_FileMappings[guidanceItemId] , fullPath);
                        return tmDatabase.fixGuidanceItemFileDueToGuidConflict(guidanceItemId, fullPath);
                    }
                    return TM_Xml_Database.Current.Cached_GuidanceItems[guidanceItemId]; 
                }
                
                if(fullPath.notNull())
                {
                    //"trying to load id {0} from virtualPath: {1}".info(guidanceItemId, virtualPath);				
                    //var pathXmlLibraries = TM_Xml_Database.Current.Path_XmlLibraries;
                    /*var fullPath = virtualPath.fileExists() 
                                        ? virtualPath
                                        : pathXmlLibraries.pathCombine(virtualPath).fullPath();*/
                    if (fullPath.fileExists())									
                    {
                        //"loading {0}".info(fullPath);

                        var article = fullPath.teamMentor_Article(); //.transform_into_guidanceItem();
                        if (article.isNull())
                        {
                            // _guidanceItem = guidanceItem.Load(fullPath).transform();
                            article = fullPath.load<Guidance_Item_Import>().transform();
                            article.saveAs(fullPath);   // to do an import in place
                        }  
                        if (article.notNull())
                        {
                            if(article.Metadata.Id != guidanceItemId)
                            {
                                "FOUND GUID CHANGE".error();
                                article.Metadata.Id_History += article.Metadata.Id.str() + ",";
                                article.Metadata.Id 		 = guidanceItemId;
                                article.saveAs(fullPath);								
                            }
                        //guidanceItemV3.guidanceItemId		 = original_Guid;		// this gives us the ability to track its source 
                        //guidanceItemV3.source_guidanceItemId = newGuid;				// also provides support for moving GuidanceItems across libraries
                            //var _guidanceItemV3 = _guidanceItem.tmGuidanceItemV3();
                            
                            TM_Xml_Database.Current.Cached_GuidanceItems.Add(guidanceItemId, article);
                            TM_Xml_Database.Current.GuidanceItems_FileMappings.add(guidanceItemId, fullPath);
                            
                            
                            return article;
                        }
                        else
					        "[xmlDB_GuidanceItem] Failed to load article at path: {0} (see errors for reason)".error(fullPath);
                    }
                    else
                        "[xmlDB_GuidanceItem] could not find file: {0}".error(fullPath);
                }
                else
                    "no virtualPath provided for id: {0}".error(guidanceItemId);
            }
            catch(Exception ex)
            {
                "[TM_Xml_Database] in xmlDB_GuidanceItem: {0}".error(ex.Message);
            }
            return null;
        }								
    }

    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceItem
    {
        [EditArticles]  public static TeamMentor_Article xmlDB_RandomGuidanceItem(this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.xmlDB_RandomGuidanceItem(Guid.NewGuid());
        }
        [EditArticles]  public static TeamMentor_Article xmlDB_RandomGuidanceItem(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            return tmDatabase.xmlDB_NewGuidanceItem(Guid.Empty,
                                                    "GI title",
                                                    "GI images", 
            //										DateTime.Now, 
                                                    "Topic..",  
                                                    "Technology....", 
                                                    "Category...", 
                                                    "RuleType...", 
                                                    "Priority...", 
                                                    "Status.." , 
                                                    "Author...", 
                                                    "Phase...",
                                                    "GI HTML content", 
                                                    libraryId);
        }        
        [EditArticles]  public static TeamMentor_Article xmlDB_NewGuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId,  string title, string images, string topic, string technology, string category, string ruleType, string priority, string status, string author,string phase,  string htmlContent, Guid libraryId)
        {			
                
            var article = new TeamMentor_Article
                {
                    Metadata = new TeamMentor_Article_Metadata
                        {
                            Id = (guidanceItemId == Guid.Empty)
                                     ? Guid.NewGuid()
                                     : guidanceItemId,
                            Library_Id = libraryId,
                            Author = author,
                            Category = category,
                            Priority = priority,
                            Type = ruleType,
                            Status = status,
                            Technology = technology,
                            Title = title,
                            Phase = phase,
                        },
                    Content = new TeamMentor_Article_Content
                        {
                            DataType = "html",
                            Data = {Value = htmlContent}
                        }
                };
            if (article.xmlDB_Save_Article(libraryId, tmDatabase))
                return article;
            return null;
        }
        [EditArticles]  public static Guid xmlDB_Create_Article(this TM_Xml_Database tmDatabase, TeamMentor_Article article)
        {             
            article.Metadata.Id = Guid.NewGuid();
            if(article.xmlDB_Save_Article(tmDatabase))
                return article.Metadata.Id;
            return Guid.Empty;
        }
        [EditArticles]  public static bool xmlDB_Save_Article(this TeamMentor_Article article, TM_Xml_Database tmDatabase)
        { 
            return article.xmlDB_Save_Article(article.Metadata.Library_Id, tmDatabase);
        }
        [EditArticles]  public static bool xmlDB_Save_Article(this TeamMentor_Article article, Guid libraryId, TM_Xml_Database tmDatabase)
        {
            if (libraryId == Guid.Empty)                                                // ensure we have a library to put the Article in
            { 
                "[xmlDB_Save_GuidanceItem] no LibraryId was provided".error();
                return false;
            }                         
                        
            if(article.Content.DataType.lower() == "html")                              // tidy the html
            {
                var cdataContent=  article.Content.Data.Value.replace("]]>", "]] >");   // xmlserialization below will break if there is a ]]>  in the text                
                var tidiedHtml = cdataContent.tidyHtml();
                
                article.Content.Data.Value = tidiedHtml;
                if (article.serialize(false).inValid())                                 // see if the tidied content can be serialized  and if not use the original data              
                    article.Content.Data.Value = cdataContent;
            }            
            article.Metadata.Library_Id = libraryId;                                    // ensure the LibraryID is correct

            if (article.serialize(false).notValid())                                    // make sure the article can be serilialized  correctly
                return false;
            
            article.update_Cache_GuidanceItems(tmDatabase);                             // add it to in Memory cache                
            
            if(tmDatabase.UsingFileStorage)                                             // save to disk
            {
                var guidanceXmlPath = tmDatabase.getXmlFilePathForGuidanceId(article.Metadata.Id, libraryId);
                if (guidanceXmlPath.valid())
                {
                    "Saving GuidanceItem {0} to {1}".info(article.Metadata.Id, guidanceXmlPath);
                    article.saveAs(guidanceXmlPath);
                    return guidanceXmlPath.fileExists();
                }
            }
            return true;
        }        
        [EditArticles]  public static bool xmlDB_Delete_GuidanceItems(this TM_Xml_Database tmDatabase, List<Guid> guidanceItemIds)
        {
            var result = true;
            foreach(var guidanceItemId in guidanceItemIds)
            {
                if (tmDatabase.xmlDB_Delete_GuidanceItem(guidanceItemId).isFalse())
                    result = false;
            }
            return result;
        }
        [EditArticles]  public static bool xmlDB_Delete_GuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            var guidanceItemXmlPath = tmDatabase.removeGuidanceItemFileMapping(guidanceItemId);
            "removing GuidanceItem with Id:{0} located at {1}".info(guidanceItemId, guidanceItemXmlPath);
            if (guidanceItemXmlPath.valid())				
            Files.deleteFile(guidanceItemXmlPath);
            if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(guidanceItemId))
                TM_Xml_Database.Current.Cached_GuidanceItems.Remove(guidanceItemId);

            tmDatabase.queue_Save_GuidanceItemsCache();

            //TM_Xml_Database.mapGuidanceItemsViews();
            return true;
        }                
        [ReadArticles]  public static string xmlDB_guidanceItemXml(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            var guidanceXmlPath = tmDatabase.getXmlFilePathForGuidanceId(guidanceItemId);
            return guidanceXmlPath.fileContents();//.xmlFormat();
        }

        [Admin]	        public static string xmlDB_guidanceItemPath(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            if (TM_Xml_Database.Current.GuidanceItems_FileMappings.hasKey(guidanceItemId))                            
                return TM_Xml_Database.Current.GuidanceItems_FileMappings[guidanceItemId];            
            return null;
        }

        public static Guid xmlBD_resolveDirectMapping(this TM_Xml_Database tmDatabase, string mapping)
        {
            if (mapping.inValid())
                return Guid.Empty;


            /*foreach(var item in TM_Xml_Database.Current.Cached_GuidanceItems)
                if(item.Value.Metadata.DirectLink.lower() == mapping ||item.Value.Metadata.Title.lower() == mapping)
                    return item.Key;
            */
            mapping = mapping.lower();

            //first resolve by direct link
            var directLinkResult = (from item in TM_Xml_Database.Current.Cached_GuidanceItems
                                    where (item.Value.Metadata.DirectLink.notNull() && item.Value.Metadata.DirectLink.lower() == mapping)
                                    select item.Key).first();
            if (directLinkResult != Guid.Empty)
                return directLinkResult;

            var mapping_Segments = mapping.split("^");

            //if there are no ^ on the title: resolve by title
            if (mapping_Segments.size() == 1)
            {
                var mapping_Extra = mapping.Replace(" ", "_");
                var titleResult = (from item in TM_Xml_Database.Current.Cached_GuidanceItems
                                   where titleMatch(item.Value, mapping, mapping_Extra)
                                   select item.Key).first();
                if (titleResult != Guid.Empty)
                    return titleResult;
            }
            //if there are ^ on the title: resolve by title^library^technology^phase^type^category
            else
            {
                var title       = mapping_Segments.value(0);
                var title_Extra = title.valid() ? title.Replace(" ", "_") : title;
                var library     = mapping_Segments.value(1);
                var technology  = mapping_Segments.value(2);
                var phase       = mapping_Segments.value(3);
                var type        = mapping_Segments.value(4);
                var category    = mapping_Segments.value(5);
                
                //var libraryNames = tmDatabase.tmLibraries().names().lower();//pre calculate this to make it faster
                    
                foreach (var item in TM_Xml_Database.Current.Cached_GuidanceItems)
                {
                    if (titleMatch(item.Value, title, title_Extra))             // check Title
                    {
                        if (library.inValid())
                            return item.Key;                        
                        if (tmDatabase.tmLibrary(item.Value.Metadata.Library_Id).Caption.lower() == library)                     // check Library
                        {
                            if (technology.inValid())
                                return item.Key;
                            if (item.Value.Metadata.Technology.lower() == technology)   // check Technology  
                            {
                                if (phase.inValid())
                                    return item.Key;
                                if (item.Value.Metadata.Phase.lower() == phase)         // check Phase
                                {
                                    if (type.inValid())
                                        return item.Key;
                                    if (item.Value.Metadata.Type.lower() == type)      // check type
                                    {
                                        if (category.inValid())
                                            return item.Key;
                                        if (item.Value.Metadata.Category.lower() == category) // check category                                                                                 
                                            return item.Key;                                        
                                    }
                                }
                            }
                        }                        
                    }                    
                }   
            }
            return Guid.Empty;
        }
        public static bool titleMatch(TeamMentor_Article article, string title1, string title2)
        {
            var match = (article.Metadata.Title.notNull() && (article.Metadata.Title.lower() == title1) ||
                                                              article.Metadata.Title.lower() == title2);
            if (match)
            { 
            }
            return match;
        }
        public static Guid xmlBD_resolveMappingToArticleGuid(this TM_Xml_Database tmDatabase, string mapping)
        {
            if (mapping.isGuid())
            {
                return tmDatabase.getVirtualGuid_if_MappingExists(mapping.guid());
            }

            mapping = mapping.urlDecode().replaceAllWith(" ", new [] {"_", "+"})
                             .htmlEncode();
            var directMapping = tmDatabase.xmlBD_resolveDirectMapping(mapping);
            if (directMapping != Guid.Empty)
                return directMapping;            

            /*if (mapping.isInt())
            {   
                var pos = mapping.toInt();
                if(pos < TM_Xml_Database.Current.Cached_GuidanceItems.Keys.size())
                    return TM_Xml_Database.Cached_GuidanceItems.Keys.toList()[pos];            
            }*/

            //this was too dangerous
            /*var results = tmDatabase.guidanceItems_SearchTitleAndHtml(mapping);
            if (results.size() >0)
                return results.first();*/
            return Guid.Empty;
        }
    }
}