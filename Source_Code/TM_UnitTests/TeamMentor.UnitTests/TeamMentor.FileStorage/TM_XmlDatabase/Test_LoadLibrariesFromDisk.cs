using System;
using System.Linq;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Moq;
using FluentSharp.Web;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_LoadLibrariesFromDisk  : TM_XmlDatabase_FileStorage
    {                        
        public Test_LoadLibrariesFromDisk()
        {            
            if (Tests_Consts.offline)
                Assert.Ignore("Ignoring Test because we are offline");   

           if(new O2Kernel_Web().online().isFalse())
                Assert.Ignore("Ignoring Test because we are offline");   

            tmFileStorage.install_LibraryFromZip_OWASP();                  
        }
        
        [Test] [Assert_Admin] public void GetGuidanceExplorerFilesInPath()
        {
            admin.assert();
            var xmlFiles = tmFileStorage.path_XmlLibraries().getGuidanceExplorerFilesInPath();
            Assert.IsNotEmpty(xmlFiles);
            foreach (var xmlFile in xmlFiles)
            {
                var fileContents = xmlFile.fileContents().fix_CRLF();
                var secondLine  = fileContents.lines().second();
                Assert.That(secondLine.starts("<guidanceExplorer"));                                                            
            }
            none.assert();
        }
        [Test] [Assert_Admin] public void LoadGuidanceExplorerFilesDirectly()
        {
            admin.assert();
            foreach (var xmlFile in tmFileStorage.path_XmlLibraries().getGuidanceExplorerFilesInPath())
            {
                "Loading libraryXmlFile: {0}".info(xmlFile.fileName());                
                var guidanceExplorer = xmlFile.getGuidanceExplorerObject();
                Assert.IsNotNull(guidanceExplorer);
            }
            none.assert();
        }
        [Test] [Assert_Admin] public void LoadGuidanceExplorerFiles()
        {            
            admin.assert();
            //tmXmlDatabase.setGuidanceExplorerObjects();
            var xmlFiles    = tmFileStorage.path_XmlLibraries().getGuidanceExplorerFilesInPath();
            var tmLibraries = tmXmlDatabase.tmLibraries();
            Assert.AreEqual(xmlFiles.size(), tmLibraries.size());
            none.assert();
        }
        [Test] [Assert_Admin] public void Test_getGuidanceExplorerObjects()
        {
            admin.assert();
            var guidanceExplorers = tmFileStorage.getGuidanceExplorerObjects(tmFileStorage.path_XmlLibraries());			
            Assert.IsNotNull(guidanceExplorers, "guidanceExplorers");
            Assert.That(guidanceExplorers.size()>0 , "guidanceExplorers was empty");			
            Assert.That(tmXmlDatabase.GuidanceExplorers_XmlFormat.size() > 0, "GuidanceExplorers_XmlFormat was empty");    		
            none.assert();
        }    	    	
        [Test] public void Test_getLibraries()
        {             
            var guidanceExplorers = tmXmlDatabase.GuidanceExplorers_XmlFormat.Values.toList();
            var tmLibraries = tmXmlDatabase.tmLibraries();
            Assert.IsNotNull(tmLibraries,"tmLibraries"); 
            for(var i=0;  i < guidanceExplorers.size() ; i++)
            {
                Assert.AreEqual(tmLibraries[i].Caption,  guidanceExplorers[i].library.caption, "caption");
                Assert.AreEqual(tmLibraries[i].Id, guidanceExplorers[i].library.name.guid(), "caption");
            }
            Assert.That(tmXmlDatabase.GuidanceExplorers_XmlFormat.size()>0, "GuidanceExplorers_XmlFormat empty");    		
        }    	     	   
        [Test] public void Test_getFolders()
        {
           // LoadGuidanceExplorerFiles();
            //var guidanceExplorers = TM_Xml_Database.loadGuidanceExplorerObjects();    		
            var libraryId = tmXmlDatabase.GuidanceExplorers_XmlFormat.Keys.first();
            Assert.AreNotEqual(Guid.Empty, libraryId, "Library id was empty");
            //libraryId = "4738d445-bc9b-456c-8b35-a35057596c16".guid();          // set it to the OWASP library since that has a folder
            var guidanceExplorerFolders = tmXmlDatabase.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure.folder;    		
            Assert.That(guidanceExplorerFolders.size() > 0,"guidanceExplorerFolders was empty");
            
            var tmFolders = tmXmlDatabase.tmFolders(libraryId);
            Assert.IsNotNull(tmFolders,"folders"); 
            Assert.That(tmFolders.size() > 0,"folders was empty");    		

            var mappedById = tmFolders.ToDictionary(tmFolder => tmFolder.folderId);

            //Add checks for sub folders	
            foreach(var folder in guidanceExplorerFolders)
            {
                Assert.That(mappedById.hasKey(folder.folderId.guid()), "mappedById didn't have key: {0}".format(folder.folderId));    				
                var tmFolder = mappedById[folder.folderId.guid()];				
                Assert.That(tmFolder.name == folder.caption);				
                Assert.That(tmFolder.libraryId == libraryId, "libraryId");	
            }      		
        }     	
        [Test][Assert_Editor]
        public void Test_getGuidanceHtml()
        {
            UserGroup.Editor.assert(); 

            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();       // need for the logUserActivity method need to map the IP
            //LoadGuidanceExplorerFiles();            
            //tmXmlDatabase.reloadGuidanceExplorerObjects();                

            var guidanceItems = tmXmlDatabase.tmGuidanceItems();
            var firstGuidanceItem = guidanceItems.first();
            Assert.IsNotNull(firstGuidanceItem,"firstGuidanceItem");
            var guid = firstGuidanceItem.Metadata.Id;
            Assert.IsNotNull(guid,"guid");
            Assert.AreNotEqual(guid, Guid.Empty,"guid.isGuid");
            var sessionId = Guid.Empty;
            var html = tmXmlDatabase.getGuidanceItemHtml(sessionId,guid);
            Assert.IsNotNull(html, "html");    		
            Assert.That(html.valid(), "html was empty");    
		
            UserGroup.None.assert(); 
        }


        

    }
}
