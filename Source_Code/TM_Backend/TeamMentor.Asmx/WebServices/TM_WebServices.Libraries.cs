using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using System.Web.Services;
using TeamMentor.FileStorage.XmlDatabase;

namespace TeamMentor.CoreLib
{
    
    public partial class TM_WebServices
    {
        //********  Libraries		
        [WebMethod(EnableSession = true)]	[ViewLibrary]        public List<TM_Library>         GetLibraries()										{	return tmXmlDatabase.tmLibraries();         }
        [WebMethod(EnableSession = true)]	[ViewLibrary]        public List<Folder_V3> 	     GetAllFolders()										{	return tmXmlDatabase.tmFolders();           }  	
        [WebMethod(EnableSession = true)]	[ViewLibrary]        public List<View_V3> 	        GetViews()										    {	return tmXmlDatabase.tmViews();	    	    }		
        [WebMethod(EnableSession = true)]	[ViewLibrary]        public List<Folder_V3>          GetFolders(Guid libraryId)							{	return tmXmlDatabase.tmFolders(libraryId);  }  			
        [WebMethod(EnableSession = true)]	[ReadArticles]        public List<TeamMentor_Article> GetGuidanceItemsInFolder(Guid folderId)		        {	return tmXmlDatabase.tmGuidanceItems_InFolder(folderId);    }  			
        [WebMethod(EnableSession = true)]	[ReadArticles]        public List<TeamMentor_Article> GetGuidanceItemsInView(Guid viewId)			        {	return tmXmlDatabase.getGuidanceItemsInView(viewId);        }  	
        [WebMethod(EnableSession = true)]	[ReadArticles]        public List<TeamMentor_Article> GetGuidanceItemsInViews(List<Guid> viewIds)	        {	return tmXmlDatabase.getGuidanceItemsInViews(viewIds);      }  	        
        [WebMethod(EnableSession = true)]	[ViewLibrary]   public List<View_V3>            GetViewsInLibraryRoot(Guid libraryId)		 		{	return tmXmlDatabase.tmViews_InLibraryRoot(libraryId);  }
        [WebMethod(EnableSession = true)]	[ViewLibrary]   public View_V3                  GetViewById(Guid viewId)		 					{	return tmXmlDatabase.tmView(viewId);                    }  	
                
        [WebMethod(EnableSession = true)] 	[ViewLibrary]   public List<String>             GetAllLibraryIds   () 						        { return tmXmlDatabase.tmLibraries().ids().toStringList();  }
        [WebMethod(EnableSession = true)] 	[ViewLibrary]   public Library                  GetLibraryById     (Guid libraryId) 				{ return tmXmlDatabase.tmLibrary(libraryId).library(tmXmlDatabase);	    }  	
        [WebMethod(EnableSession = true)] 	[ViewLibrary]   public Library                  GetLibraryByName   (string libraryName) 			{ return tmXmlDatabase.tmLibrary(libraryName).library(tmXmlDatabase);	}  	
        [WebMethod(EnableSession = true)]	[ReadArticles]  public string                   GetGuidanceItemHtml(Guid guidanceItemId)			{	return tmXmlDatabase.getGuidanceItemHtml (tmAuthentication.sessionID, guidanceItemId);	    }				
        [WebMethod(EnableSession = true)]	[ReadArticles]  public List<String>             GetGuidanceItemsHtml(List<Guid> guidanceItemsIds)	{	return tmXmlDatabase.getGuidanceItemsHtml(tmAuthentication.sessionID, guidanceItemsIds);	}                        
        [WebMethod(EnableSession = true)]	[ReadArticles]  public TeamMentor_Article       GetGuidanceItemById(Guid guidanceItemId)		    { return tmXmlDatabase.tmGuidanceItem(guidanceItemId);                  }  	        
        [WebMethod(EnableSession = true)]   [ReadArticles]  public string                   MarkdownTransform  (string markdownText)            { return markdownText.markdown_Transform(); }


        [WebMethod(EnableSession = true)] 	[EditArticles]	public Library_V3 CreateLibrary(Library library)	                                    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_NewGuidanceExplorer(library).libraryV3();                            }  	
        [WebMethod(EnableSession = true)] 	[EditArticles]	public bool UpdateLibrary(Library library) 			                                    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_UpdateGuidanceExplorer(library);                                     }  	                        
        [WebMethod(EnableSession = true)]	[EditArticles]	public View_V3 CreateView(Guid folderId, View view)                                     { editArticles.demand(); resetCache(); return tmXmlDatabase.newView(folderId, view); }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool UpdateView(View view)													    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_UpdateView(view).notNull();                                 }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool AddGuidanceItemsToView(Guid viewId,  List<Guid> guidanceItemIds)		    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_AddGuidanceItemsToView(viewId, guidanceItemIds);            }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool RemoveGuidanceItemsFromView(Guid viewId, List<Guid> guidanceItemIds)	    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_RemoveGuidanceItemsFromView(viewId,guidanceItemIds );       }  			
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool RemoveViewFromFolder(Guid libraryId, Guid viewId)    					    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_RemoveView(libraryId, viewId);                              }
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool MoveViewToFolder(Guid viewId, Guid targetFolderId, Guid targetLibraryId)    { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_MoveViewToFolder(viewId,targetFolderId, targetLibraryId);   }
        [WebMethod(EnableSession = true)]	[EditArticles]	public Guid CreateGuidanceItem(GuidanceItem_V3 guidanceItem)						    { editArticles.demand(); resetCache(); return tmXmlDatabase.createGuidanceItem(guidanceItem);                                 }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	public Guid CreateArticle(TeamMentor_Article article)					                { editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_Create_Article(article);                                    }
        [WebMethod(EnableSession = true)]	[EditArticles]	public Guid CreateArticle_Simple(Guid libraryId, string title, string dataType, string htmlCode)		
                                                            { 
                                                                editArticles.demand(); 
                                                                resetCache(); 
                                                                var article = new TeamMentor_Article
                                                                    {
                                                                        Metadata =
                                                                            {
                                                                                Library_Id  = libraryId,
                                                                                Title       = title
                                                                            },
                                                                        Content =
                                                                            {
                                                                                DataType    = dataType,
                                                                                Data        = { Value = htmlCode }
                                                                            }
                                                                    };
                                                                return CreateArticle(article); 
                                                            }        
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool UpdateGuidanceItem(TeamMentor_Article guidanceItem)						                    
                                                            { 
                                                                editArticles.demand(); 
                                                                if (guidanceItem.isNull())
                                                                    return false;
                                                                resetCache();    
                                                                var result = guidanceItem.xmlDB_Save_Article(tmXmlDatabase); 
                                                                this.logUserActivity("Update Article", "{0} ({1})".format(guidanceItem.Metadata.Title, guidanceItem.Metadata.Id));
                                                                return result;
                                                            }                                                                                                
        [WebMethod(EnableSession = true)]   [EditArticles]	public bool SetArticleHtml (Guid articleId,string htmlContent)					                        
                                                            {
                                                                editArticles.demand(); 
                                                                return SetArticleContent(articleId, "html", htmlContent);
                                                            }
        [WebMethod(EnableSession = true)]   [EditArticles]	public bool SetArticleContent (Guid articleId, string dataType,  string content)					    
                                                            { 
                                                                editArticles.demand(); 
                                                                resetCache();
                                                                var article = GetGuidanceItemById(articleId);
                                                                if (article.notNull())
                                                                {
                                                                    article.Content.Data.Value = content;
                                                                    article.Content.DataType = dataType;
                                                                    return UpdateGuidanceItem(article);                                                                                                
                                                                }
                                                                return false;
                                                            }
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool DeleteGuidanceItem(Guid guidanceItemId)											{ editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_Delete_GuidanceItem(guidanceItemId);                }    
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool DeleteGuidanceItems(List<Guid> guidanceItemIds)									{ editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_Delete_GuidanceItems(guidanceItemIds);              }
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool RenameFolder(Guid libraryId, Guid folderId , string newFolderName) 				{ editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_Rename_Folder(libraryId, folderId,newFolderName );  } 		
        [WebMethod(EnableSession = true)]	[EditArticles]	public Folder_V3 CreateFolder(Guid libraryId, Guid parentFolderId, string newFolderName) 	{ editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_Add_Folder(libraryId, parentFolderId, newFolderName ).tmFolder(libraryId, tmXmlDatabase); } 		
        [WebMethod(EnableSession = true)]	[EditArticles]	public bool DeleteFolder(Guid libraryId, Guid folderId) 							 	 	{ editArticles.demand(); resetCache(); return tmXmlDatabase.xmlDB_Delete_Folder(libraryId,  folderId);                }        
        [WebMethod(EnableSession = true)]   [EditArticles]	public bool DeleteLibrary(Guid libraryId)
                                                            {
                                                                editArticles.demand(); 
                                                                resetCache();
                                                                if (GetLibraryById(libraryId).isNull())
                                                                    return false;
                                                                var libraryToDelete = new Library  { id = libraryId.str(), delete = true };
                                                                UpdateLibrary(libraryToDelete);
                                                                var libraryDeleted = GetLibraryById(libraryId);
                                                                return libraryDeleted.isNull();// || libraryDeleted.delete;
                                                            }		
        [WebMethod(EnableSession = true)]   [EditArticles]	public bool RenameLibrary(Guid libraryId, string newName)
                                                            {
                                                                editArticles.demand(); 
                                                                resetCache();
                                                                if (GetLibraryById(libraryId).isNull())
                                                                    return false;
                                                                var libraryToRename = new Library  { id = libraryId.str(), caption = newName };
                                                                return UpdateLibrary(libraryToRename);			
                                                            }
        
                
        //Xml Database Specific
        [WebMethod(EnableSession = true)]   [ViewLibrary]   public List<Guid> XmlDatabase_GuidanceItems_SearchTitleAndHtml(List<Guid> guidanceItemsIds, string searchText)
                                                                                                                                {
                                                                                                                                    var results = TM_Xml_Database.Current.guidanceItems_SearchTitleAndHtml(guidanceItemsIds,searchText);
                                                                                                                                    this.logUserActivity("User Search", "on {0} item(s) for '{1}' with {2} results".format(guidanceItemsIds.size(), searchText,results.size()));
                                                                                                                                    return results;
                                                                                                                                }		
		//Article Guid Mappings
        [WebMethod(EnableSession = true)]		                public Guid getGuidForMapping(string mapping)
                                                                            {
                                                                                return TM_Xml_Database.Current.xmlBD_resolveMappingToArticleGuid(mapping);
                                                                            }
        [WebMethod(EnableSession = true)]		                public bool IsGuidMappedInThisServer(Guid guid)
                                                                            {
                                                                                if (GetGuidanceItemById(guid).notNull())
                                                                                    return true;
                                                                                if (TM_Xml_Database.Current.get_GuidRedirect(guid).valid())
                                                                                    return true;
                                                                                return false;
                                                                            }																														
        [WebMethod(EnableSession = true)] public string         XmlDatabase_GetGuidanceItemXml(Guid guidanceItemId)	    {	return  tmFileStorage.xmlDB_guidanceItemXml(guidanceItemId); }        
        [WebMethod(EnableSession = true)] public string         XmlDatabase_GetGuidanceItemPath(Guid guidanceItemId)	{	return  tmFileStorage.xmlDB_guidanceItemPath(guidanceItemId); }                
                                                                    

        // not exposed as WebServices any more 
        // note: to move into another class
        //carefull: both methods below will generate a JSON object with more than 1Mb (with the default SI library)
        public List<TeamMentor_Article> GetAllGuidanceItems()						{	return tmXmlDatabase.tmGuidanceItems();                 }  	
        public List<TeamMentor_Article> GetGuidanceItemsInLibrary(Guid libraryId)	{	return tmXmlDatabase.tmGuidanceItems(libraryId);        }  	
    }
}
