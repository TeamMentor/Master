using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Security.Application;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

//using urn.microsoft.guidanceexplorer.guidanceItem;

namespace TeamMentor.CoreLib
{	
    public partial class TM_Xml_Database 
    {                       
        public  bool loadDataIntoMemory()
        {	
            var tmXmlDatabase = Current;

            if(tmXmlDatabase.Path_XmlDatabase.dirExists().isFalse())
            {
                "[TM_Xml_Database] in loadDataIntoMemory, provided pathXmlDatabase didn't exist: {0}".error(tmXmlDatabase.Path_XmlDatabase);
                return false;
            }            
            tmXmlDatabase.loadLibraryDataFromDisk();            
            tmXmlDatabase.setupGitSupport();
            tmXmlDatabase.UserData.loadTmUserData();
            return true;					
        }        
        

        //move to  extension methods
        [ReadArticles] 
        public TeamMentor_Article getGuidanceItem(Guid guidanceItemId)
        {
            if (Cached_GuidanceItems.hasKey(guidanceItemId).isFalse())
                return null;
                
            return Cached_GuidanceItems[guidanceItemId];			
        }
        
        [ReadArticles] 
        public string getGuidanceItemHtml(Guid sessionId, Guid guidanceItemId)
        {            

            if (Cached_GuidanceItems.hasKey(guidanceItemId).isFalse())
                return null;

            var article = Cached_GuidanceItems[guidanceItemId];
            

            sessionId.session_TmUser().logUserActivity("View Article Html", article.Metadata.Title); 
            
            var articleContent = article.Content.Data.Value;

            if (articleContent.inValid())
                return "";

            switch(article.Content.DataType.lower()) 
            {
                case "raw":
                    return articleContent;
                case "html":
                    {
                        if (TMConfig.Current.TMSecurity.Sanitize_HtmlContent && article.Content.Sanitized.isFalse())
                            return articleContent.sanitizeHtmlContent();
                        
                        return articleContent.fixXmlDoubleEncodingIssue();                            
                    }
                case "safehtml":
                    {
                        return articleContent.sanitizeHtmlContent();
                    }
                case "wikitext":
                    return "<div id ='tm_datatype_wikitext'>{0}</div>".format(articleContent);
                default:
                    return articleContent;
            }			
        }

        [ReadArticles]
        public List<string> getGuidanceItemsHtml(Guid sessionId, List<Guid> guidanceItemsIds)
        {
            var data = new List<string>();
            if (guidanceItemsIds.notNull())
                foreach (var guidanceItemId in guidanceItemsIds)
                {
                    data.add(getGuidanceItemHtml(sessionId, guidanceItemId));
                }
            return data;
        }
    }


    //******************* TM_Library
    
    public static class TM_Xml_Database_ExtensionMethods_TM_Library
    {
        //[ReadArticles] 
        public static List<TM_Library>  tmLibraries(this TM_Xml_Database tmDatabase)
        {
            var libraries = new List<TM_Library>();
            try
            {
                libraries.AddRange(tmDatabase.xmlDB_GuidanceExplorers()
                         .Select(guidanceExplorer => new TM_Library
                                                            {
                                                                Id = guidanceExplorer.library.name.guid(), 
                                                                Caption = guidanceExplorer.library.caption
                                                            }));
            }
            catch(Exception ex)
            {
                ex.log();
            }
            return libraries;
        }		
        public static TM_Library        tmLibrary(this TM_Xml_Database tmDatabase, string caption)
        {
            if (caption.isGuid())   // if the value provided is a guid, then 
                return tmDatabase.tmLibrary(caption.guid());

            var tmLibrary = (from library in tmDatabase.tmLibraries()
                             where library.Caption == caption
                             select library).first();
            //if (tmLibrary.isNull())
            //	"[TM_Xml_Database] couldn't find library with caption: {0}".error(caption);
            return tmLibrary;
        }		
        public static TM_Library        tmLibrary(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var tmLibrary = (from library in tmDatabase.tmLibraries()
                             where library.Id == libraryId
                             select library).first();
            //if (tmLibrary.isNull())
            //	"[TM_Xml_Database] couldn't find library with id: {0}".error(libraryId);
            return tmLibrary;		
        }		
        public static List<Guid>        ids(this List<TM_Library> libraries)
        {
            return (from library in libraries
                    select library.Id).toList();
        }		
        public static List<string>      names(this List<TM_Library> libraries)
        {
            return libraries.captions();
        }
        public static List<string>      captions(this List<TM_Library> libraries)
        {
            return (from library in libraries
                    select library.Caption).toList();
        }	

        [EditArticles]  public static TM_Library        new_TmLibrary   (this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.new_TmLibrary("Default_Library_{0}".format(6.randomNumbers()));
        }		
        [EditArticles]  public static TM_Library        new_TmLibrary   (this TM_Xml_Database tmDatabase, string libraryCaption )
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
        [EditArticles]  public static TM_Xml_Database   delete_Library  (this TM_Xml_Database tmDatabase, TM_Library library)
        {
            tmDatabase.xmlDB_DeleteGuidanceExplorer(library.Id);
            return tmDatabase;
        }
    }
    
    //******************* Folder_V3 (was TMFolder)
    
    public static class TM_Xml_Database_ExtensionMethods_Folder_V3
    {
        //gets all folders (recursive search)
        public static List<Folder_V3>   tmFolders(this TM_Xml_Database tmDatabase)
        {
            var tmFolders = new List<Folder_V3>();
            foreach(var tmLibrary in tmDatabase.tmLibraries())
                tmFolders.add(tmDatabase.tmFolders_All(tmLibrary.Id));
            return tmFolders;
        }		
        public static List<Folder_V3>   tmFolders(this TM_Library tmLibrary, TM_Xml_Database tmDatabase )
        {
            return tmDatabase.tmFolders(tmLibrary);
        }		
        public static List<Folder_V3>   tmFolders(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
        {
            return tmDatabase.tmFolders(tmLibrary.Id);
        }		
        public static List<Folder_V3>   tmFolders(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            return tmDatabase.tmFolders(libraryId, tmDatabase.xmlDB_Folders(libraryId));
        }			
        public static List<Folder_V3>   tmFolders(this TM_Xml_Database tmDatabase, Guid libraryId, IList<urn.microsoft.guidanceexplorer.Folder> folders)
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
        public static Folder_V3         tmFolder(this urn.microsoft.guidanceexplorer.Folder folder, Guid libraryId, TM_Xml_Database tmDatabase)
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
                tmFolder.views.Add(new View_V3  { viewId = view.id.guid()});	
            return tmFolder;
        }
        public static List<Folder_V3>   tmFolders_All(this TM_Xml_Database tmDatabase, Guid libraryId)
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
        public static Folder_V3         tmFolder(this TM_Library tmLibrary, Guid folderId, TM_Xml_Database tmDatabase )
        {
            return tmDatabase.tmFolder(tmLibrary.Id, folderId);
        }		
        public static Folder_V3         tmFolder(this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
        {
            return (from tmFolder in tmDatabase.tmFolders_All(libraryId)
                    where tmFolder.folderId == folderId
                    select tmFolder).first();
        }		
        public static Folder_V3         tmFolder(this TM_Xml_Database tmDatabase, Guid libraryId, string name)
        {
            return (from tmFolder in tmDatabase.tmFolders_All(libraryId)
                    where tmFolder.name == name
                    select tmFolder).first();
        }		
        public static Folder_V3         tmFolder(this TM_Xml_Database tmDatabase, Guid folderId)
        {
            return tmDatabase.tmLibraries()
                             .Select        (tmLibrary => tmDatabase.tmFolder(tmLibrary.Id, folderId))
                             .FirstOrDefault(tmFolder  => tmFolder.notNull());
        }
    }
    
    //******************* TM_View
    
    public static class TM_Xml_Database_ExtensionMethods_TM_View
    {
        public static List<View_V3>     tmViews(this TM_Xml_Database tmDatabase)
        {
            var tmViews = new List<View_V3>();
            foreach(var tmLibrary in tmDatabase.tmLibraries())
                tmViews.add(tmDatabase.tmViews(tmLibrary));
            return tmViews;
        }		
        public static List<View_V3>     tmViews(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
        {
            return tmDatabase.tmViews(tmLibrary.Id);
        }		
        public static List<View_V3>     tmViews(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var tmViews = tmDatabase.tmViews(libraryId, tmDatabase.xmlDB_Folders(libraryId));					
            tmViews.AddRange(tmDatabase.tmViews_InLibraryRoot(libraryId));
            return tmViews;			
        }
        public static List<View_V3>     tmViews_InLibraryRoot(this TM_Xml_Database tmDatabase, Guid libraryId)		
        {
            var tmViews  = new List<View_V3>();
            var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
            if(guidanceExplorer.notNull() && guidanceExplorer.library.libraryStructure.notNull())
                foreach(var view in guidanceExplorer.library.libraryStructure.view)
                    tmViews.add(view.tmView(libraryId, Guid.Empty));
            return tmViews;
            
        }		
        public static List<View_V3>     tmViews(this TM_Xml_Database tmDatabase, Guid libraryId ,  IList<urn.microsoft.guidanceexplorer.Folder> folders)
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
        public static View_V3           tmView(this TM_Xml_Database tmDatabase, Guid viewId)
        {			
            return (from view in tmDatabase.tmViews()
                    where view.viewId == viewId
                    select view).first();					
        }		
        public static List<TeamMentor_Article>  getGuidanceItemsInViews(this TM_Xml_Database tmDatabase, List<View> views)
        {
            var viewIds = (from view in views select view.id.guid()).toList();
            return tmDatabase.getGuidanceItemsInViews(viewIds);
        }		
        public static List<TeamMentor_Article>  getGuidanceItemsInViews(this TM_Xml_Database tmDatabase, List<Guid> viewIds)
        {
            return (from viewId in viewIds
                    from guidanceItemV3 in tmDatabase.getGuidanceItemsInView(viewId)
                    select guidanceItemV3).toList();
        }		
        public static List<TeamMentor_Article>  getGuidanceItemsInView(this TM_Xml_Database tmDatabase, Guid viewId)
        {		
            var tmView = tmDatabase.tmView(viewId);
            if (tmView.notNull())
            {
                var guidanceItems = new List<TeamMentor_Article>();
                foreach(var guidanceItemId in tmView.guidanceItems)
                    if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(guidanceItemId))
                        guidanceItems.add(TM_Xml_Database.Current.Cached_GuidanceItems[guidanceItemId]);
                    else
                        "[getGuidanceItemsInView]: in view ({0} {1}) could not find guidanceItem for id {2}".error(tmView.caption, tmView.viewId, guidanceItemId);
                return guidanceItems;
            }
            //if (TM_Xml_Database.Current.GuidanceItems_InViews.hasKey(viewId))
            //	return TM_Xml_Database.Current.GuidanceItems_InViews[viewId];
            "[TM_Xml_Database] getGuidanceItemsInView, requested viewId was not mapped: {0}".error(viewId);
            return new List<TeamMentor_Article>();
        }		
        public static List<TeamMentor_Article>  getAllGuidanceItemsInViews(this TM_Xml_Database tmDatabase)
        {
            //return (from viewId in TM_Xml_Database.Current.GuidanceItems_InViews.Keys
            //		from guidanceItem in TM_Xml_Database.Current.GuidanceItems_InViews[viewId]
            //		select guidanceItem).toList();
            return new List<TeamMentor_Article>();
        }		
    }
    
    //******************* TM_GuidanceItem
    
    public static class TM_Xml_Database_ExtensionMethods_TM_GuidanceItems
    {	
        //needs the ReadArticlesTitles privildge because of the GetGuiObjects method
        //[ReadArticlesTitles]  // this was allowing anonynimous viewing of TM articles
        [ReadArticles]          public static TeamMentor_Article        tmGuidanceItem (this TM_Xml_Database tmDatabase, Guid id)
        {
            if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(id))
            {
                var article = TM_Xml_Database.Current.Cached_GuidanceItems[id];
                return article;
            }
            var externalArticle = tmDatabase.getExternalTeamMentorArticle_if_MappingExists(id);
            if (externalArticle.notNull())
                return externalArticle;

            return null;
        }        
        [ReadArticlesTitles]    public static List<TeamMentor_Article>  tmGuidanceItems(this TM_Xml_Database tmDatabase)
        {			
            return tmDatabase.xmlDB_GuidanceItems();						
        }                
        [ReadArticlesTitles] 	public static List<TeamMentor_Article>  tmGuidanceItems(this TM_Xml_Database tmDatabase, TM_Library tmLibrary)
        {
            return tmDatabase.tmGuidanceItems(tmLibrary.Id);
        }        
        [ReadArticlesTitles] 	public static List<TeamMentor_Article>  tmGuidanceItems(this TM_Xml_Database tmDatabase, Guid libraryId)
        {			
            return (from guidanceItem in TM_Xml_Database.Current.Cached_GuidanceItems.Values.toList()
                    where guidanceItem.Metadata.Library_Id == libraryId
                    select guidanceItem).toList();		
        }				        

        [ReadArticles] 	public static List<TeamMentor_Article>  tmGuidanceItems_InFolder(this TM_Xml_Database tmDatabase, Guid folderId)
        {				
            var folder = tmDatabase.xmlDB_Folder(folderId);			
            var foldersToMap = tmDatabase.xmlDB_Folders_All(folder);			
            return (from folderToMap in foldersToMap
                    from view in folderToMap.view
                    from guidanceItem in tmDatabase.getGuidanceItemsInView(view.id.guid())
                    select guidanceItem).Distinct().toList();
        }        
        [ReadArticles] 	public static string                    sanitizeHtmlContent(this string htmlContent)
        {
            return Sanitizer.GetSafeHtmlFragment(htmlContent);
        }		

        [EditArticles] 	public static Guid                      createGuidanceItem(this TM_Xml_Database tmDatabase, GuidanceItem_V3 guidanceItemV3)
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
            return guidanceItem.Metadata.Id;
        }
    }
    
    
    //******************* Objects Conversion
    
    public static class TM_Xml_Database_ExtensionMethods_ObjectConversion
    {        
        public static TM_GuidanceItem tmGuidanceItem(this GuidanceItem_V3 guidanceItemV3)
        {
            return new TM_GuidanceItem
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
        public static View_V3 tmView(this urn.microsoft.guidanceexplorer.View view, Guid libraryId, Guid folderId)
        {
            var tmView = new View_V3
                            {	
                                libraryId = libraryId,
                                folderId = folderId,
                                viewId= view.id.guid(),	
                                caption = view.caption,
                                author = view.author,																															
                            };            
            if(view.items.notNull())
                if(view.items.item.notNull())
                    foreach(var item in view.items.item)
                        tmView.guidanceItems.add(item.guid());
            return tmView;
        }      
        public static urn.microsoft.guidanceexplorer.View view(this View tmView)
        {
            return new urn.microsoft.guidanceexplorer.View
                        {				
                            caption = tmView.caption,							
                            author = tmView.creator,
                            id = tmView.id,
                            creationDate = DateTime.Now
                        };
        }	        
        
        public static Library library(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
        {
            if (tmLibrary.isNull())
                return null;
            return new Library
                {	
                    caption = tmLibrary.Caption,  
                    id = tmLibrary.Id.str(),
                    //Views = tmDatabase.tmViews(tmLibrary).ToArray()
                };
        }		
        public static TM_GuidanceItem tmGuidanceItem(this List<TM_GuidanceItem> tmGuidanceItems, Guid guidanceItemId)
        {
            return (from tmGuidanceItem in tmGuidanceItems
                    where tmGuidanceItem.Id == guidanceItemId
                    select tmGuidanceItem).first();
        }
        public static List<TM_GuidanceItem> tmGuidanceItems(this List<GuidanceItem_V3> guidanceItemsV3)
        {
            return (from guidanceItemV3 in guidanceItemsV3
                    select guidanceItemV3.tmGuidanceItem()).toList();
        }
    }
    
    public static class TM_Xml_Database_ExtensionMethods_TMConfig
    {
        public static TMConfig tmConfig(this  TM_Xml_Database tmDatabase)
        {
            return TMConfig.Current;
        }
    }
    public static class TM_Xml_Database_ExtensionMethods_OnInstallationActions
    {
/*        public static TM_Xml_Database handleDefaultInstallActions(this TM_Xml_Database tmDatabase)
        {
            try
            {
                "In handleDefaultInstallActions".info();
                var tmConfig = tmDatabase.tmConfig();
                var defaultLibrary = tmConfig.OnInstallation.DefaultLibraryToInstall_Name;
                if (defaultLibrary.valid())
                {
                    var tmLibraries = tmDatabase.tmLibraries();
                    if (tmLibraries.notEmpty()) // don't install default library if there are already libraries there                        
                    {
                        "There were {0} libraries found, so skipping handleDefaultInstallActions".info(tmLibraries.size());
                        return tmDatabase;
                    }
                    var library = tmDatabase.tmLibrary(defaultLibrary); // check if default library exists, and if it doesn't, download and install it
                    if (library.isNull())
                    {
                        var installUrl = tmConfig.OnInstallation.DefaultLibraryToInstall_Location;
                        "[handleDefaultInstallActions] installing default Library {0} from {1}".info(defaultLibrary, installUrl);
                        if (tmDatabase.xmlDB_Libraries_ImportFromZip(installUrl, ""))
                        {
                            "[handleDefaultInstallActions]  library {0} installed ok".info(defaultLibrary);
                            //tmDatabase.ReloadData();
                        }
                        else
                            "[handleDefaultInstallActions]  failed to install default library {0}".error(defaultLibrary);
                    }                                
                }
            }
            catch (Exception ex)
            {
                ex.log("[in handleDefaultInstallActions]");
            }
            return tmDatabase;
        }*/
    }	
}
