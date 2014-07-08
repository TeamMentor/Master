using System;
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Web35;
using TeamMentor.XmlDatabase;

namespace TeamMentor.CoreLib
{
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
            if (article.notNull())
            {
                article.Metadata.Id = Guid.NewGuid();
                if(article.xmlDB_Save_Article(tmDatabase))
                    return article.Metadata.Id;
            }
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

            tmDatabase.Events.Article_Saved.raise(article);                             // TODO find way to identify save issues (like the return value of the save action)

            return true;
        }                
        [EditArticles]  public static bool xmlDB_Delete_GuidanceItems(this TM_Xml_Database tmDatabase, List<Guid> guidanceItemIds)
        {
            if (guidanceItemIds.isNull())
                return false;
            var result = true;
            foreach(var guidanceItemId in guidanceItemIds)            
                if (tmDatabase.xmlDB_Delete_GuidanceItem(guidanceItemId).isFalse())
                    result = false;            
            return result;
        }
        [EditArticles]  public static bool xmlDB_Delete_GuidanceItem(this TM_Xml_Database tmDatabase, Guid articleId)
        {            
            if (articleId !=  Guid.Empty && tmDatabase.Cached_GuidanceItems.hasKey(articleId))
            {                                                 
                var tmArticle = tmDatabase.Cached_GuidanceItems[articleId];         // get article object from Cache
                tmDatabase.Cached_GuidanceItems.Remove(articleId);                  // remove article from cache
                tmDatabase.Events.Article_Deleted.raise(tmArticle);                 // raise event
                return true;
            }
            return false;
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
            if (mapping.notValid())
                return Guid.Empty;

            if (mapping.isGuid())
            {
                return tmDatabase.getVirtualGuid_if_MappingExists(mapping.guid());
            }

            mapping = mapping.urlDecode().replaceAllWith(" ", new [] {"+"})
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