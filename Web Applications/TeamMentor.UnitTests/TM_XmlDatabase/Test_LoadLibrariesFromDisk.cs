using System;
using System.Linq;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_LoadLibrariesFromDisk  : TM_XmlDatabase_InMemory
    {
        public string LibraryPath { get; set; }

        public Test_LoadLibrariesFromDisk()
        {            
            var assembly		 = this.type().Assembly;

            var dllLocation		 = assembly.CodeBase.subString(8);
            var webApplications  = dllLocation.parentFolder()
                                              .pathCombine(@"\..\..\..\..");
            LibraryPath          = webApplications.pathCombine(@"Library_Data\XmlDatabase\TM_Libraries");
            "calculated libraryPath: {0}".info(LibraryPath);            
        }

        [SetUp]
        public void Setup()
        {      
            UserGroup.Admin.setThreadPrincipalWithRoles();          // impersonate an Admin  
            if (LibraryPath.dirExists().isFalse())
                Assert.Ignore("Couldn't find library path, so skipping assemby load tests");                                    
            tmXmlDatabase.ResetDatabase();                          // reset Xml Database
        }        
        [TearDown]
        public void TearDown()
        {
            tmXmlDatabase.ResetDatabase();                         // reset Xml Database
        }

        [Test] public void GetGuidanceExplorerFilesInPath()
        {
            var xmlFiles = LibraryPath.getGuidanceExplorerFilesInPath();
            Assert.IsNotEmpty(xmlFiles);
            foreach (var xmlFile in xmlFiles)
            {                
                var fileContents = xmlFile.fileContents();
                var secondLine  = fileContents.lines().second();
                Assert.That(secondLine.starts("<guidanceExplorer"));                                                            
            }
        }
        [Test] public void LoadGuidanceExplorerFilesDirectly()
        {
            foreach (var xmlFile in LibraryPath.getGuidanceExplorerFilesInPath())
            {
                "Loading libraryXmlFile: {0}".info(xmlFile.fileName());                
                var guidanceExplorer = xmlFile.getGuidanceExplorerObject();
                Assert.IsNotNull(guidanceExplorer);
            }
        }
        [Test] public void LoadGuidanceExplorerFiles()
        {
            TM_Xml_Database.Current.Path_XmlLibraries = LibraryPath;
            tmXmlDatabase.setGuidanceExplorerObjects();
            var xmlFiles    = LibraryPath.getGuidanceExplorerFilesInPath();
            var tmLibraries = tmXmlDatabase.tmLibraries();
            Assert.AreEqual(xmlFiles.size(), tmLibraries.size());
        }
        [Test] public void Test_getGuidanceExplorerObjects()
        {
            LoadGuidanceExplorerFiles();

            var guidanceExplorers = tmXmlDatabase.Path_XmlLibraries.getGuidanceExplorerObjects();			
            Assert.IsNotNull(guidanceExplorers, "guidanceExplorers");
            Assert.That(guidanceExplorers.size()>0 , "guidanceExplorers was empty");			
            Assert.That(tmXmlDatabase.GuidanceExplorers_XmlFormat.size() > 0, "GuidanceExplorers_XmlFormat was empty");    		
        }    	    	
        [Test] public void Test_getLibraries()
        { 
            LoadGuidanceExplorerFiles();

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
            LoadGuidanceExplorerFiles();
            //var guidanceExplorers = TM_Xml_Database.loadGuidanceExplorerObjects();    		
            var libraryId = tmXmlDatabase.GuidanceExplorers_XmlFormat.Keys.first();
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
        [Test] public void Test_getGuidanceHtml()
        {
            //LoadGuidanceExplorerFiles();
            TM_Xml_Database.Current.Path_XmlLibraries = LibraryPath;
            tmXmlDatabase.setGuidanceExplorerObjects();
            tmXmlDatabase.xmlDB_Load_GuidanceItems();            

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
        }

    }
}
