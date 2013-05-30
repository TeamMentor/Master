using System;
using System.Collections.Generic;
using System.Linq;
using O2.DotNetWrappers.ExtensionMethods;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{		 
    [TestFixture]
    public class Test_Libraries : TM_WebServices_InMemory
    {	    
        public Guid OWASP_LIBRARY_GUID = "4738d445-bc9b-456c-8b35-a35057596c16".guid();

        public Test_Libraries() 
        {     		
            Install_LibraryFromZip_OWASP();
        }

        [SetUp]
        public void SetUp()
        {
            tmXmlDatabase.UsingFileStorage = false;
            Assert.IsFalse(tmXmlDatabase.UsingFileStorage, "UsingFileStorage");
        }
        [Test] public void GetLibraries() 
        {        	
            Assert.IsNotNull(tmWebServices, "tmWebServices was null");    	    

            var libraries    = tmWebServices.GetLibraries(); 
            var owaspLibrary = tmWebServices.GetLibraryById(OWASP_LIBRARY_GUID);
            var guidanceExplorers = tmXmlDatabase.GuidanceExplorers_XmlFormat;

            Assert.NotNull  (libraries                                           , "response was null");    		            
            Assert.IsNotNull(guidanceExplorers                                   , "tmXmlDatabase was null");
            Assert.Greater  (guidanceExplorers.Values.size(), 0                  , "GuidanceExplorers_XmlFormat was empty");    		    		
            Assert.Greater  (libraries.size()               , 0                  , "there should be at least  one library");    		    		
            Assert.AreEqual (owaspLibrary.id.guid()         , OWASP_LIBRARY_GUID , "the library 'id' value didn't match");
            Assert.AreEqual (owaspLibrary.caption           , "OWASP"            , "the library 'caption' value didn't match");    		
        }
        [Test] public void GetFolders() 
        {   
            var folderName   = "OWASP Top 10";
            var owaspLibrary = tmWebServices.GetLibraryById(OWASP_LIBRARY_GUID);
            var folders      = tmWebServices.GetFolders(owaspLibrary.id.guid());    		
            Assert.IsNotNull(folders , null , "folders was null");
            Assert.Greater  (folders.size(),0 , "no folders returned");
            Assert.AreEqual (folders.first().name, folderName);                        
            var tmFolder    = tmXmlDatabase.tmFolder(owaspLibrary.id.guid(),folderName);
            
            Assert.That    (tmFolder.notNull(),"could not find folder with name: {0}".format(folderName));    		
            Assert.AreEqual(folderName, tmFolder.name,"expected Name didn't match");    		
        }         
        [Test] public void GetGuidanceItemsInView() 
        {
            var folderName               = "OWASP Top 10";
            var expectedViewId           = "52d2d5f4-170a-4b25-bc92-0e53fd8c11a1";			
            var expectedArticle_Id       = "56b0552d-2ceb-4714-a8f1-20a6a8609874".guid();
            var expectedArticle_Title    = "All Database Input Is Validated";
            var expectedArticle_Category = "Input and Data Validation";

            var owaspLibrary = tmWebServices.GetLibraryById(OWASP_LIBRARY_GUID); 
            var folders = tmWebServices.GetFolders(owaspLibrary.id.guid());
            
            var expectedFolder =  (from folder in folders
                                   where folder.name == folderName
                                   select folder).first();
                                   
            Assert.NotNull (expectedFolder, "couldn't find expected folder: {0}".format(folderName));    		    										   
            Assert.IsTrue  (expectedFolder.views.guids().contains(expectedViewId.guid()), "Folder didn't contain expected view id");														
            
            var articles         = tmWebServices.GetGuidanceItemsInView(expectedViewId.guid());
            
            var searchForArticle = articles.where((article) => article.Metadata.Id == expectedArticle_Id);
            Assert.That    (articles != null          , "articles was null");
            Assert.AreEqual (articles.size()       , 40, "There should be 40 articles in this view");    	 	
            Assert.AreEqual(searchForArticle.size(), 1 , "searchForArticle size()");
                    
            var article_Direct  = tmWebServices.GetGuidanceItemById(expectedArticle_Id);
            var article_ViaView = searchForArticle.first();
            Assert.AreEqual(article_Direct.Metadata.Id        , expectedArticle_Id        , "expected Id didn't match");
            Assert.AreEqual(article_Direct.Metadata.Title     , expectedArticle_Title     , "expected Title didn't match");
            Assert.AreEqual(article_Direct.Metadata.Category  , expectedArticle_Category  , "expected expectedCategory didn't match");
            
            Assert.AreEqual(article_Direct.Metadata.Id, article_ViaView.Metadata.Id , "Comparing article's Metadata.Id");
            Assert.AreEqual(article_Direct.toXml()    , article_ViaView.toXml()     , "Comparing article's toXml");

        }    	 
        [Test] public void GetGuidanceItemsInViews() 
        { 
            var owaspLibrary = tmWebServices.GetLibraryById(OWASP_LIBRARY_GUID); 
            var folders = tmWebServices.GetFolders(owaspLibrary.id.guid());
            var viewIds = folders[0].views;
            var guidanceItems = tmWebServices.GetGuidanceItemsInViews(viewIds.guids());    		    		
            Assert.That(guidanceItems != null , "guidanceItems was null");
            Assert.That(folders.size() > 0 , "no guidanceItems returned");    		    		
        }    
        [Assert_Reader]
        [Test] public void GetGuidanceItemHtml() 
        {    
            var owaspLibrary = tmWebServices.GetLibraryById(OWASP_LIBRARY_GUID); 
            var folders = tmWebServices.GetFolders(owaspLibrary.id.guid());
            var guidanceItems = tmWebServices.GetGuidanceItemsInView(folders[0].views.guids()[0]);    		
            //show.info(guidanceItems);
            var guidanceItem = guidanceItems[0];
            var html = tmWebServices.GetGuidanceItemHtml(guidanceItem.Metadata.Id);    		
            Assert.That(html != null , "GuidanceItemHtml was null");
            Assert.That(html.size() > 0 , "GuidanceItemHtml was empty");    					
        } 		
        [Test] public void GetAllGuidanceItems() 
        {   
            var allGuidanceItems = tmWebServices.GetAllGuidanceItems(); 
            Assert.That(allGuidanceItems != null , "allGuidanceItems was null");
            Assert.That(allGuidanceItems.size()> 0 , "no allGuidanceItems returned");    		
            "There where  {0} items returned".info(allGuidanceItems.size());    		
        }    	    	    	
        [Test] public void GetGuidanceItemsInLibrary() 
        {   		    	
            var guidanceItemsInLibrary = tmWebServices.GetGuidanceItemsInLibrary(OWASP_LIBRARY_GUID);
            Assert.That(guidanceItemsInLibrary != null , "guidanceItemsInLibrary was null");
            Assert.That(guidanceItemsInLibrary.size()> 0 , "no guidanceItemsInLibrary returned");    		
            "There where  {0} items returned".info(guidanceItemsInLibrary.size());    		
        }    	
        [Test] public void GetGuidanceItemsByIds() 
        {       		
            var viewId = "fc1c5b9c-becb-44a2-9812-40090d9bd135".guid(); //A02: Cross-Site Scripting (XSS)
            
            var view = tmWebServices.GetViewById(viewId); 
            var guidanceItemsIds = view.guidanceItems;   
            var guidanceItems = tmXmlDatabase.xmlDB_GuidanceItems(guidanceItemsIds);
             
            Assert.AreEqual(guidanceItems.size(),guidanceItemsIds.size(), "guidanceItemsIds size");
            foreach(var guidanceItem in guidanceItems)
                Assert.That(guidanceItemsIds.contains(guidanceItem.Metadata.Id), "couldn't find guidanceItem by id");

        }    	
        [Test] [Assert_Editor] public void Create_Rename_Delete_Libraries()
        {
            tmXmlDatabase.UsingFileStorage      = true;                                  // need this since we are checking the file paths
            //tmConfig.Git.AutoCommit_LibraryData = false;
            
            var testOwaspViewId                 = "fc1c5b9c-becb-44a2-9812-40090d9bd135".guid();
            var originalName                    = "createAndDelete";  
            var newName 	                    = "_" + originalName + "_new";

            var newLibrary                       = tmWebServices.CreateLibrary(new Library { caption = originalName }); //Create Library
            var tmLibrary                        = tmXmlDatabase.tmLibrary(newLibrary.libraryId);
            //var guidanceItemsPath_NewName_Before = tmXmlDatabase.xmlDB_Path_Library_RootFolder(newName);            
            var libraryPath_OriginalName         = tmXmlDatabase.xmlDB_Path_Library_XmlFile(tmLibrary);
            var guidanceItemsPath_OriginalName   = tmXmlDatabase.xmlDB_Path_Library_RootFolder(tmLibrary);
            var testOwaspview                    = tmWebServices.GetViewById(testOwaspViewId);    	    

            
            
            Assert.AreEqual (testOwaspview.guidanceItems.size()  ,21 , "There should be 21 views in the test OWASP A2 view");						
            Assert.IsNotNull(newLibrary                              , "newLibrary Created OK");
            Assert.IsTrue   (libraryPath_OriginalName.fileExists()   , "libraryPath_originalName should exist after creation");            

            //add a GI to the Library
            var guidanceItem = tmWebServices.CreateGuidanceItem(new GuidanceItem_V3 { libraryId = newLibrary.libraryId });
            var guidanceItemXmlFile = tmXmlDatabase.getXmlFilePathForGuidanceId(guidanceItem);
            Assert.IsTrue(guidanceItemXmlFile.directoryName().contains(guidanceItemsPath_OriginalName) ,  "before rename:  guidanceItemXmlFile not in guidanceItemsPath_originalName");            
            
            //Rename Library
            var renameResult                    = tmWebServices.RenameLibrary(newLibrary.libraryId, newName);  
            var libraryPath_NewName             = tmXmlDatabase.xmlDB_Path_Library_XmlFile(newLibrary.libraryId);			 			
            var guidanceItemsPath_NewNameOldDir = libraryPath_NewName.parentFolder().pathCombine(libraryPath_OriginalName.fileName());
            var guidanceItemsPath_NewName       = tmXmlDatabase.xmlDB_Path_Library_RootFolder(tmLibrary);            

            Assert.IsTrue(renameResult, "renameResult");            
            Assert.IsTrue(libraryPath_OriginalName.fileExists()        , "libraryPath_originalName should exist after rename");
            Assert.IsTrue(libraryPath_NewName	  .fileExists()        , "libraryPath_newName should still not exist after rename");
            Assert.IsTrue(guidanceItemsPath_NewNameOldDir.fileExists() , "old library file should still exists in renamed library");    	    

            var library_By_Id 				= tmWebServices.GetLibraryById(newLibrary.libraryId);    
            var library_By_OriginalName 	= tmWebServices.GetLibraryByName(originalName);
            var library_By_NewName    		= tmWebServices.GetLibraryByName(newName); 
            var library_By_Caption    		= tmWebServices.GetLibraryByName(library_By_Id.caption);
            
            Assert.IsNotNull(library_By_Id			 , "library_by_Id");
            Assert.IsNull   (library_By_OriginalName , "library_by_originalName");
            Assert.IsNotNull(library_By_NewName		 , "library_by_newName");
            Assert.IsNotNull(library_By_Caption		 , "library_by_caption");
            
            Assert.IsTrue(tmWebServices.GetLibraries().names().contains(newName), "new name was not on tmWebServices.GetLibraries()") ;
            
            //check that GI folder was changed
            Assert.IsTrue(guidanceItemsPath_OriginalName.dirExists(), "after rename: guidanceItemsPath_originalName should exist");			
            Assert.IsTrue(guidanceItemsPath_NewName.dirExists()	    , "after rename guidanceItemsPath_originalName should exist");			
        
            //check if GI exists in new folder	
            guidanceItemXmlFile = tmXmlDatabase.getXmlFilePathForGuidanceId(guidanceItem);			
            
            Assert.IsTrue  (guidanceItemXmlFile.directoryName().contains(guidanceItemsPath_OriginalName), "after rename:guidanceItemsPath_OriginalName not in guidanceItemXmlFile");
            Assert.AreEqual(guidanceItemsPath_OriginalName, guidanceItemsPath_NewName                   , "after rename:, guidanceItemsPath_NewName should be equal to guidanceItemsPath_OriginalName");
            
            //Delete Library
            var deleteResult    = tmWebServices.DeleteLibrary(newLibrary.libraryId);  
            var testOwaspview2  = tmWebServices.GetViewById(testOwaspViewId);    	    

            Assert.IsTrue(deleteResult, "deleteResult");
            
            Assert.IsFalse(libraryPath_NewName	   .fileExists(),     "libraryPath_newName should not exist after delete");
            Assert.IsFalse(libraryPath_OriginalName.fileExists(),     "libraryPath_originalName should not exist at the end");            
            Assert.AreEqual(testOwaspview2.guidanceItems.size() , 21, "There should still be 21 views in the test OWASP A2 view");
        }
        [Test] [Assert_Editor] public void Create_Delete_Libraries_with_a_GuidanceItem()
        {
            tmXmlDatabase.UsingFileStorage = true;                                  // need this since we are checking the file paths
            var originalName = "temp_lib".add_RandomLetters(3);    
             
            //Create Library 
            var newLibrary                  = tmWebServices.CreateLibrary                  (new Library { caption = originalName });
            var tmLibrary                   = tmXmlDatabase.tmLibrary(newLibrary.libraryId);
            var libraryPath_OriginalName        = tmXmlDatabase.xmlDB_Path_Library_XmlFile              (tmLibrary);    
            var libraryPath_GuidanceItemsFolder = tmXmlDatabase.xmlDB_Path_Library_RootFolder(tmLibrary); 

            Assert.IsNotNull(newLibrary                                  , "newLibrary");
            Assert.IsTrue   (libraryPath_OriginalName.fileExists() 		 , "libraryPath_originalName should exist after creation");
            Assert.IsTrue   (libraryPath_GuidanceItemsFolder.dirExists() , "libraryPath_GuidanceItemsFolder should exists after library creation");
        
            
            //Create GuidanceItem
            var newGuidanceItem = new GuidanceItem_V3 
                                            {
                                                libraryId = newLibrary.libraryId
                                            };								
            var guidanceItem = tmWebServices.CreateGuidanceItem(newGuidanceItem);
            
            Assert.AreNotEqual(guidanceItem, Guid.Empty, "guidance Item was not created");
            Assert.IsTrue(libraryPath_GuidanceItemsFolder.dirExists()  , "libraryPath_GuidanceItemsFolder should exist after library creation");
            
        
            //Delete Library
            var deleteResult = tmWebServices.DeleteLibrary(newLibrary.libraryId);  
            Assert.IsTrue(deleteResult, "deleteResult");
            Assert.IsFalse(libraryPath_OriginalName.fileExists() , "libraryPath_originalName should not exist at the end");
            
            Assert.IsFalse(libraryPath_GuidanceItemsFolder.dirExists()  , "libraryPath_GuidanceItemsFolder should not exist after delete");
            //tmXmlDatabase.UsingFileStorage = false;
        }    	
        [Test] public void Search_TitleAndHtml()
        {
                        
            var viewId = "fc1c5b9c-becb-44a2-9812-40090d9bd135".guid(); //A02: Cross-Site Scripting (XSS)
            var searchFor = "XSS";	
            var guidanceItemsIds = new List<Guid>();
            
            Func<string, List<Guid>> searchTitleAndHtml =
                (searchText) => tmWebServices.XmlDatabase_GuidanceItems_SearchTitleAndHtml(guidanceItemsIds, searchText);
            
            
            //test searching on all content			
            var matchedIds = searchTitleAndHtml(searchFor);
            Assert.That(matchedIds.size() > 0, "no results when searching all GIs"); 
            
            //test search on view's guidanceItems
            var view = tmWebServices.GetViewById(viewId);  
            guidanceItemsIds = view.guidanceItems;   
            //var guidanceItems = tmXmlDatabase.xmlDB_GuidanceItems(guidanceItemsIds);			 				
            
            matchedIds = searchTitleAndHtml(searchFor);
            Assert.That(matchedIds.size() > 0, "no results");
            var resultGuidanceItems = tmXmlDatabase.xmlDB_GuidanceItems(matchedIds);
            
            foreach(var resultGuidanceItem  in resultGuidanceItems)
                Assert.That(resultGuidanceItem.Metadata.Title.contains(searchFor) || resultGuidanceItem.Content.Data.Value.contains(searchFor), "couldn't find search term in GI");

        }    	
    } 
}    
    