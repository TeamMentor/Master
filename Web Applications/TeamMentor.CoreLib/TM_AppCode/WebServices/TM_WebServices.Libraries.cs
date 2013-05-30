using System;
using System.Collections.Generic;
using FluentSharp;
using System.Web.Services;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    
    public partial class TM_WebServices
    {
        //********  Libraries		
        [WebMethod(EnableSession = true)]	public List<TM_Library>         GetLibraries()										{	return tmXmlDatabase.tmLibraries();         }
        [WebMethod(EnableSession = true)]	public List<Folder_V3> 	        GetAllFolders()										{	return tmXmlDatabase.tmFolders();           }  	
        [WebMethod(EnableSession = true)]	public List<View_V3> 	        GetViews()										    {	return tmXmlDatabase.tmViews();	    	    }		
        [WebMethod(EnableSession = true)]	public List<Folder_V3>          GetFolders(Guid libraryId)							{	return tmXmlDatabase.tmFolders(libraryId);  }  			
        [WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInFolder(Guid folderId)		        {	return tmXmlDatabase.tmGuidanceItems_InFolder(folderId);    }  			
        [WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInView(Guid viewId)			        {	return tmXmlDatabase.getGuidanceItemsInView(viewId);        }  	
        [WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInViews(List<Guid> viewIds)	        {	return tmXmlDatabase.getGuidanceItemsInViews(viewIds);      }  	
        [WebMethod(EnableSession = true)]	public string                   GetGuidanceItemHtml(Guid guidanceItemId)			{	return tmXmlDatabase.getGuidanceItemHtml (tmAuthentication.sessionID, guidanceItemId);	    }				
        [WebMethod(EnableSession = true)]	public List<String>             GetGuidanceItemsHtml(List<Guid> guidanceItemsIds)	{	return tmXmlDatabase.getGuidanceItemsHtml(tmAuthentication.sessionID, guidanceItemsIds);	}                
        
        [WebMethod(EnableSession = true)]	public List<View_V3>            GetViewsInLibraryRoot(Guid libraryId)		 		{	return tmXmlDatabase.tmViews_InLibraryRoot(libraryId);  }
        [WebMethod(EnableSession = true)]	public View_V3                  GetViewById(Guid viewId)		 					{	return tmXmlDatabase.tmView(viewId);                    }  	
                
        [WebMethod(EnableSession = true)] 	public List<String>             GetAllLibraryIds   () 						        { return tmXmlDatabase.tmLibraries().ids().toStringList();  }
        [WebMethod(EnableSession = true)] 	public Library                  GetLibraryById     (Guid libraryId) 				{ return tmXmlDatabase.tmLibrary(libraryId).library(tmXmlDatabase);	    }  	
        [WebMethod(EnableSession = true)] 	public Library                  GetLibraryByName   (string libraryName) 			{ return tmXmlDatabase.tmLibrary(libraryName).library(tmXmlDatabase);	}  	
        [WebMethod(EnableSession = true)]	public TeamMentor_Article       GetGuidanceItemById(Guid guidanceItemId)		    { return tmXmlDatabase.tmGuidanceItem(guidanceItemId);                  }  	

        [WebMethod(EnableSession = true)] 	[EditArticles]	                        public Library_V3 CreateLibrary(Library library)	{ resetCache(); return tmXmlDatabase.xmlDB_NewGuidanceExplorer(library.id.guid(), library.caption).libraryV3();                            }  	
        [WebMethod(EnableSession = true)] 	[EditArticles]	                     	public bool UpdateLibrary(Library library) 			{ resetCache(); return tmXmlDatabase.xmlDB_UpdateGuidanceExplorer(library.id.guid(), library.caption, library.delete);                     }  	                        
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public View_V3 CreateView(Guid folderId, View view) { resetCache(); return tmXmlDatabase.newView(folderId, view); }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool UpdateView(View view)													    { resetCache(); return tmXmlDatabase.xmlDB_UpdateView(view).notNull();                                 }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool AddGuidanceItemsToView(Guid viewId,  List<Guid> guidanceItemIds)		    { resetCache(); return tmXmlDatabase.xmlDB_AddGuidanceItemsToView(viewId, guidanceItemIds);            }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool RemoveGuidanceItemsFromView(Guid viewId, List<Guid> guidanceItemIds)	    { resetCache(); return tmXmlDatabase.xmlDB_RemoveGuidanceItemsFromView(viewId,guidanceItemIds );       }  			
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool RemoveViewFromFolder(Guid libraryId, Guid viewId)    					    { resetCache(); return tmXmlDatabase.xmlDB_RemoveView(libraryId, viewId);                              }
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool MoveViewToFolder(Guid viewId, Guid targetFolderId, Guid targetLibraryId)    { resetCache(); return tmXmlDatabase.xmlDB_MoveViewToFolder(viewId,targetFolderId, targetLibraryId);   }
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public Guid CreateGuidanceItem(GuidanceItem_V3 guidanceItem)						    { resetCache(); return tmXmlDatabase.createGuidanceItem(guidanceItem);                                 }  	
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public Guid CreateArticle(TeamMentor_Article article)					                { resetCache(); return tmXmlDatabase.xmlDB_Create_Article(article);                                    }
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public Guid CreateArticle_Simple(Guid libraryId, string title, string dataType, string htmlCode)					       
                                                                                        { 
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
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool UpdateGuidanceItem(TeamMentor_Article guidanceItem)						
                                                                                        { 
                                                                                            resetCache();
                                                                                            
                                                                                            var result = guidanceItem.xmlDB_Save_Article(tmXmlDatabase); 
                                                                                            this.LogUserActivity("Update Article", "{0} - {1}  [{2}".format(guidanceItem.Metadata.Id, guidanceItem.Metadata.Title, result));
                                                                                            return result;
                                                                                        }                                                                                                
        [WebMethod(EnableSession = true)]   [EditArticles]	                        public bool SetArticleHtml (Guid articleId,string htmlContent)					        
                                                                                        {
                                                                                            return SetArticleContent(articleId, "html", htmlContent);
                                                                                        }
        [WebMethod(EnableSession = true)]   [EditArticles]	                        public bool SetArticleContent (Guid articleId, string dataType,  string content)					        
                                                                                        { 
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
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool DeleteGuidanceItem(Guid guidanceItemId)											{ resetCache(); return tmXmlDatabase.xmlDB_Delete_GuidanceItem(guidanceItemId);                }    
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool DeleteGuidanceItems(List<Guid> guidanceItemIds)									{ resetCache(); return tmXmlDatabase.xmlDB_Delete_GuidanceItems(guidanceItemIds);              }
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool RenameFolder(Guid libraryId, Guid folderId , string newFolderName) 				{ resetCache(); return tmXmlDatabase.xmlDB_Rename_Folder(libraryId, folderId,newFolderName );  } 		
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public Folder_V3 CreateFolder(Guid libraryId, Guid parentFolderId, string newFolderName) 	{ resetCache(); return tmXmlDatabase.xmlDB_Add_Folder(libraryId, parentFolderId, newFolderName ).tmFolder(libraryId, tmXmlDatabase); } 		
        [WebMethod(EnableSession = true)]	[EditArticles]	                     	public bool DeleteFolder(Guid libraryId, Guid folderId) 							 	 	{ resetCache(); return tmXmlDatabase.xmlDB_Delete_Folder(libraryId,  folderId);                }

        
                
        [WebMethod(EnableSession = true)] [EditArticles]	                     		public bool DeleteLibrary(Guid libraryId)
        {
            resetCache();
            if (GetLibraryById(libraryId).isNull())
                return false;
            var libraryToDelete = new Library  { id = libraryId.str(), delete = true };
            UpdateLibrary(libraryToDelete);
            var libraryDeleted = GetLibraryById(libraryId);
            return libraryDeleted.isNull();// || libraryDeleted.delete;
        }		
        [WebMethod(EnableSession = true)] [EditArticles]	                     	    public bool RenameLibrary(Guid libraryId, string newName)
        {
            resetCache();
            if (GetLibraryById(libraryId).isNull())
                return false;
            var libraryToRename = new Library  { id = libraryId.str(), caption = newName };
            return UpdateLibrary(libraryToRename);			
        }
        /*[WebMethod(EnableSession = true)] [EditArticles]	                     		public List<Guid> DeleteTempLibraries()
        {
            var deletedLibraries = new List<Guid>();
            foreach(var library in GetLibraries())
                if (library.Caption.contains("temp_lib_", "TempLibrary")  || library.Caption.isGuid())
                    if (DeleteLibrary(library.Id))
                        deletedLibraries.Add(library.Id);
            return deletedLibraries;
        }*/				
        
        //carefull: both methods below will generate a JSON object with more than 1Mb (with the default SI library)
        [WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetAllGuidanceItems()						{	return tmXmlDatabase.tmGuidanceItems();                 }  	
        [WebMethod(EnableSession = true)]	public List<TeamMentor_Article> GetGuidanceItemsInLibrary(Guid libraryId)	{	return tmXmlDatabase.tmGuidanceItems(libraryId);        }  	
    }
}
