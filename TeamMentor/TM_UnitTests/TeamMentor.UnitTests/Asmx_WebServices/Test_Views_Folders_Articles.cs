using System;
using System.Security;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;


namespace TeamMentor.UnitTests.Asmx_WebServices
{		  
    [TestFixture] 
    public class Test_OnlineStorage : TM_WebServices_InMemory
    { 
        
        [Test] [Assert_Editor] public void CreateLibrary_and_UpdateLibrary()
        {  
            var newLibrary = new Library { 
                                            id = Guid.NewGuid().str(),
                                            caption = "temp_lib_{0}".format(6.randomLetters())
                                          };										  
            tmWebServices.CreateLibrary(newLibrary);    		     		
            var createdLibrary = tmWebServices.GetLibraryById(newLibrary.id.guid());    		
            Assert.That(createdLibrary.notNull(), "could not fetch new Library with Id: {0}".format(newLibrary.id));    	
            createdLibrary.caption += "_toDelete";    		  
            tmWebServices.UpdateLibrary(createdLibrary);    		    		    		
            var updatedLibrary = tmWebServices.GetLibraryById(createdLibrary.id.guid());
             
            Assert.That(updatedLibrary.id       == createdLibrary.id        , "in updated, id didn't match created library");
            Assert.That(updatedLibrary.caption  == createdLibrary.caption   , "in updated, caption didn't match");
            Assert.That(updatedLibrary.delete   == createdLibrary.delete    , "in updated, delete didn't match");
            Assert.That(updatedLibrary.caption  != newLibrary.caption       , "in updated, caption should be different that newLibrary");
            Assert.That(updatedLibrary.id.contains(newLibrary.id)           , "in updated, id didn't match new library object");
            
            
            updatedLibrary.delete = true;    		     		
            var deleteResult = tmWebServices.UpdateLibrary(updatedLibrary);    		    		    		
            var deletedLibrary = tmWebServices.GetLibraryById(updatedLibrary.id.guid());
            Assert.IsTrue(deleteResult  , "deleteResult was not true");
            Assert.IsNull(deletedLibrary, "deletedLibrary should not be there (GetLibraryById returned it)");    		    		
        }    	    	    	    	           
        [Test] [Assert_Editor] public void CreateView_and_DeleteLibrary()
        {    
            var createdView = createTempView();   	            
            Assert.IsNotNull(createdView, "createdView");    		
            var result = tmWebServices.DeleteLibrary(createdView.library.guid());
            Assert.That(result, "failed to delete library");    		    		
        }          
        [Test] [Assert_Editor] public void GetAllLibraryIds_and_GetLibraryById()
        {
            createTempLibrary();
            var libraries = tmWebServices.GetAllLibraryIds();
            Assert.That(libraries.notNull() , "libraries was null");    		
            Assert.That(libraries.size() > 0 , "libraries was empty");    		
            foreach(var library in libraries)
            { 
                var libraryWithId = tmWebServices.GetLibraryById(library.guid());
                Assert.That(libraryWithId.notNull(), "libraryWithId was null for library with Id: {0}".format(library));
            }       		    
        }            
        [Test] [Assert_Editor] public void UpdateView() 
        {  
            var tempView = createTempView();   	    		
            tempView.caption += "_{0}".format(6.randomLetters());
            tmWebServices.UpdateView(tempView);
            var updatedView = tmWebServices.GetViewById(tempView.id.guid());
            Assert.That(updatedView.viewId == tempView.id.guid(), "ids didn't match");
            Assert.That(updatedView.caption == tempView.caption, "caption didn't match");    		
            tmWebServices.DeleteLibrary(tempView.library.guid());     		    
        }         
        [Test] [Assert_Editor] public void CreateGuidanceItem()
        {  
            var libraryId = createTempLibrary();    		    	 	
            var tempGuidanceItemId = createTempGuidanceItem(libraryId);    		    		
            Assert.That(tempGuidanceItemId != Guid.Empty , "tempGuidanceItem was an EmptyGuid");
            var guidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId);
            
            Assert.That(guidanceItem.notNull(), "guidanceItem was null");
            Assert.That(guidanceItem.Metadata.Id == tempGuidanceItemId, "ids didn't match");
            Assert.That(guidanceItem.Metadata.Title.valid(), "title wasn't valid");
            Assert.That(guidanceItem.Content.Data.Value.valid()	, "content wasn't valid"); 
            //Assert.That(guidanceItem.Metadata.Topic.valid(), "topic wasn't valid"); 
            Assert.That(guidanceItem.Metadata.Technology.valid(), "technology wasn't valid");  
            Assert.That(guidanceItem.Metadata.Category.valid(), "category wasn't valid");  
            Assert.That(guidanceItem.Metadata.Type.valid(), "ruleType wasn't valid"); 
            Assert.That(guidanceItem.Metadata.Status.valid(), "ruleType wasn't valid"); 
            Assert.That(guidanceItem.Metadata.Priority.valid(), "priority wasn't valid"); 
            Assert.That(guidanceItem.Metadata.Status.valid(), "status wasn't valid"); 
            Assert.That(guidanceItem.Metadata.Author.valid(), "author wasn't valid");     		
            tmWebServices.DeleteLibrary(libraryId);    		  
        }           
        [Test] [Assert_Editor] public void UpdateGuidanceItem()    
        {      		     		
            var libraryId = createTempLibrary();  			
            "temp library created:{0}".info(libraryId); 
            var tempGuidanceItemId = createTempGuidanceItem(libraryId);
            
            var tempGuidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId);     		
                    
            tempGuidanceItem.Metadata.Title += "_{0}".format(6.randomLetters());
            tempGuidanceItem.Content.Data.Value += "_{0}".format(6.randomLetters());
            //tempGuidanceItem.Metadata.Topic += "_{0}".format(6.randomLetters());
            tempGuidanceItem.Metadata.Technology += "_{0}".format(6.randomLetters());			        		    		
            var result = tmWebServices.UpdateGuidanceItem(tempGuidanceItem);    		
            Assert.That(result, "UpdateGuidanceItem failed");
            var updatedGuidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId);    		    		
            Assert.That(updatedGuidanceItem.Metadata.Id == tempGuidanceItem.Metadata.Id, "ids didn't match");
            Assert.That(updatedGuidanceItem.Metadata.Title == tempGuidanceItem.Metadata.Title, "title didn't match");
            Assert.That(updatedGuidanceItem.Content.Data.Value == tempGuidanceItem.Content.Data.Value, "newHtmlContent didn't match");    		
            //Assert.That(updatedGuidanceItem.Metadata.Topic == tempGuidanceItem.Metadata.Topic, "topic didn't match");    		
            Assert.That(updatedGuidanceItem.Metadata.Technology == tempGuidanceItem.Metadata.Technology, "newHtmlContent didn't match");    		
            
            tmWebServices.DeleteLibrary(libraryId);     		    		    		    
        } 
        [Test] [Assert_Editor] public void IJavascriptProxy_LiveWS_DeleteGuidanceItem()
        {
            var libraryId = createTempLibrary(); 
            var tempGuidanceItemId = createTempGuidanceItem(libraryId);    		
            var newGuidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId);    		            
            var result = tmWebServices.DeleteGuidanceItem(newGuidanceItem.Metadata.Id);    			    
            Assert.That(result, "result was false");
            var deletedGuidanceItem = tmWebServices.GetGuidanceItemById(newGuidanceItem.Metadata.Id);    		
            Assert.IsNull(deletedGuidanceItem, "deletedGuidanceItem should be null");    		
            tmWebServices.DeleteLibrary(libraryId);    		    		
        }           
        [Test] [Assert_Editor] public void AddGuidanceItemsToView_and_RemoveGuidanceItemsFromView()
        { 
            var createdView = createTempView();  
            var viewId = createdView.id.remove("view:").guid();
            var guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId);
            Assert.That(guidanceItemsInView.size()==0, "here guidanceItemsInView should be zero, and it was '{0}'".format(guidanceItemsInView.size()));
            var tempGuidanceItemId_1 = createTempGuidanceItem(createdView.library.guid());    		
            var tempGuidanceItemId_2 = createTempGuidanceItem(createdView.library.guid());    		
            var guidanceItemIds = new List<Guid> { tempGuidanceItemId_1,tempGuidanceItemId_2 };    		 
            var result = tmWebServices.AddGuidanceItemsToView(viewId, guidanceItemIds);
            Assert.That(result, "result was false");
            "view ID: {0}".info(viewId);
            guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId); 
            Assert.That(guidanceItemsInView.size()==2, "here guidanceItemsInView should be two, and it was '{0}'".format(guidanceItemsInView.size()));
             
            var tempGuidanceItemId_3 = createTempGuidanceItem(createdView.library.guid());    		
            var tempGuidanceItemId_4 = createTempGuidanceItem(createdView.library.guid());    		
            guidanceItemIds.Clear(); 
            guidanceItemIds.add(tempGuidanceItemId_3).add(tempGuidanceItemId_4);
            
            result = tmWebServices.AddGuidanceItemsToView(createdView.id.remove("view:").guid(), guidanceItemIds);    		    		
            Assert.That(result, "2nd result was false");
            guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId);    		
            Assert.That(guidanceItemsInView.size()==4, "here guidanceItemsInView should be 4, and it was '{0}'".format(guidanceItemsInView.size()));
                        
            var guidanceIdsToRemove = ( from guidanceItem in guidanceItemsInView 
                                        select guidanceItem.Metadata.Id).toList();
                                        
            tmWebServices.RemoveGuidanceItemsFromView(viewId, guidanceIdsToRemove);    		
             
            guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId);
            Assert.That(guidanceItemsInView.size()==0, "after remove the guidanceItemsInView should be zero, and it was '{0}'".format(guidanceItemsInView.size()));
        }    	    	
        [Test] [Assert_Editor] public void GetGuiObjects()
        {   
            var tempView = createTempView();
            createTempGuidanceItem(tempView.library.guid());
            var guiObjects = tmWebServices.GetGUIObjects();
            Assert.That(guiObjects.notNull(), "guiObjects was null"); 		
            Assert.That(guiObjects.UniqueStrings.size() > 0 , "empty UniqueStrings");
            Assert.That(guiObjects.GuidanceItemsMappings.size() > 0 , "empty GuidanceItemsMappings");    		
        }        
        [Test] [Assert_Editor] public void GetFolderStructure_Libraries()
        {
            createTempView();
            var librariesStructure = tmWebServices.GetFolderStructure_Libraries();
            Assert.That(librariesStructure.notNull(), "librariesStructure was null"); 		
            Assert.That(librariesStructure.size() > 0 , "empty librariesStructure");
            var folders = from libraryStucture in librariesStructure
                          select libraryStucture.subFolders;
            var views =   from libraryStucture in librariesStructure
                          from folder in libraryStucture.subFolders
                          select folder.views;
            Assert.Greater(folders.size(),0 , "no folders");
            Assert.Greater(views.size()  ,0 , "no views");            
        }
        [Test] public void GetGuidanceItemById()
        {  
            var sessionID = userData.newUser().tmUser()             // create a temp user
                                 .make_Editor()                     // make it an editor
                                 .login();                          // log it in

            tmWebServices.tmAuthentication.sessionID = sessionID;   // set the current session to the logged in used 
            var currentUser = tmWebServices.Current_User();         // check that the current user is correctly set
            Assert.IsNotNull(currentUser , "there should be a user object here");
            
            
            var libraryId = createTempLibrary();    		    	 	                    // create a temp library
            var tempGuidanceItemId = createTempGuidanceItem(libraryId);                     // create temp article
            
            var guidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId);
            Assert.That(guidanceItem.Content.Data.Value.valid()	, "content wasn't valid");  // check that the article had data
            
            tmWebServices.Logout();                                                         // log out current user

            currentUser = tmWebServices.Current_User();                                                      // which should result in a null current user
            Assert.IsNull(currentUser , "there shouldn't be a user object here");

            Assert.Throws<SecurityException>(()=> tmWebServices.GetGuidanceItemById(tempGuidanceItemId));  // this should thrown an security exception            
        }

        //***********        
        //  helper methods    Imported from previews version of this test (see if we still need them here)
        public Guid createTempLibrary()
        {
            "creating temp library".info();
            var newLibrary = new Library { 	id = Guid.NewGuid().str(),
                                            caption = "temp_lib_{0}".format(6.randomLetters()) };										  
            tmWebServices.CreateLibrary(newLibrary);  
            return newLibrary.id.guid();
        }         
        public Folder_V3 createTempFolder(Guid libraryId)
        {
            return tmWebServices.CreateFolder(libraryId,default(Guid), "test folder");	
        }        
        public View createTempView()
        { 
            "creating temp view".info();
            var newView = new View  {
                                        caption = "test view", 
                                        creatorCaption = "guidanceLibrary", 	    								
                                        parentFolder = "a folder",
                                        criteria = "",	    								
                                        id = Guid.NewGuid().str(),
                                        library = createTempLibrary().str()
                                     };
            var newFolder = createTempFolder(newView.library.guid());	    							 
            tmWebServices.CreateView(newFolder.folderId, newView);   
            var createdView = tmWebServices.GetViewById(newView.id.guid());
            Assert.That(createdView.viewId.str().remove("view:") == newView.id.str(), "ids didn't match");
            Assert.That(createdView.caption == newView.caption, "captions didn't match");
            
            return newView;
            //return createdView;
        } 
        // there methods are a variation of the ones found in OnlineStorageHelpers.cs file
        public Guid createTempGuidanceItem( Guid libraryIdGuid)
        { 
            return createTempGuidanceItem(	libraryIdGuid, 
                                            "GI title",
                                            "GI images",  
                                            DateTime.Now, 
                                            "Topic..",  
                                            "Technology....", 
                                            "Category...",   
                                            "RuleType...", 
                                            "Phase...",
                                            "Priority...", 
                                            "Status.." , 
                                            "Author...", "GI HTML content");
        }        	        
        public Guid createTempGuidanceItem(Guid libraryIdGuid, string title, string images, DateTime lastUpdate, string topic, string technology, string category, string ruleType, string phase, string priority, string status, string author, string htmlContent)
        {
            var guidanceType = Guid.Empty; //  tmWebServices.GetGuidanceTypeByName("Guideline").id.guid();   
            Guid creatorId = Guid.Empty; 
            var creatorCaption = "guidanceLibrary"; 					     // we can use either one of these creator values				 					  			
            var guidanceItem =  createTempGuidanceItem(libraryIdGuid, guidanceType, creatorId, creatorCaption, title, images, lastUpdate, topic, technology, category, ruleType, phase, priority, status, author, htmlContent);			
            var guidanceItemV3 = new GuidanceItem_V3(guidanceItem);																	
            
            var newGuidanceItemId = tmWebServices.CreateGuidanceItem(guidanceItemV3);			
            Assert.AreEqual(newGuidanceItemId, guidanceItemV3.guidanceItemId, "GuidanceItemId");
            "newGuidanceItemId : {0}".debug(newGuidanceItemId);
            Assert.That(newGuidanceItemId != Guid.Empty, "in createTempGuidanceItem, newGuidanceItemId was empty");
            return newGuidanceItemId;
        } 
        public GuidanceItem createTempGuidanceItem(Guid libraryIdGuid, Guid guidanceType, Guid creatorId, string creatorCaption, string title, string images, DateTime lastUpdate, string topic, string technology, string category, string ruleType, string phase, string priority, string status, string author, string htmlContent)
        {
            var newGuidanceId= Guid.Empty.next(8.randomNumbers().toInt());			
            
            var guidanceItem = newGuidanceItemObject(newGuidanceId, title, guidanceType , libraryIdGuid, creatorId, creatorCaption ,htmlContent,images,lastUpdate);
            guidanceItem.AnyAttr = new List<XmlAttribute>()
                .add_XmlAttribute("Topic", topic )
                .add_XmlAttribute("Technology", technology)
                .add_XmlAttribute("Category", category)
                .add_XmlAttribute("Rule_Type", ruleType)
                .add_XmlAttribute("Phase", phase)
                .add_XmlAttribute("Priority", priority) 
                .add_XmlAttribute("Status", status) 
                .add_XmlAttribute("Author", author)
                .ToArray();
            
            //liveWS_tmWebServices.CreateGuidanceItem(guidanceItem, htmlContent);  
            "Created GuidanceItem with the title'{0}' ".info(title);
            return guidanceItem;
            //return newGuidanceId; 
        }        
        public static GuidanceItem newGuidanceItemObject(Guid id, string title, Guid guidanceType, Guid library, Guid creator, string creatorCaption, string content, string images, DateTime lastUpdate)
        {
            var guidanceItem = new GuidanceItem  { id =id.str(),  													
                                                    title = title, 
                                                    guidanceType = guidanceType.str(),	
                                                    library = library.str(),
                                                    creator = creator.str(),
                                                    creatorCaption = creatorCaption, 
                                                    content = content,
                                                    images = images, 
                                                    lastUpdate = lastUpdate
                                                  };														  
            return guidanceItem;
        }        
    }       
}