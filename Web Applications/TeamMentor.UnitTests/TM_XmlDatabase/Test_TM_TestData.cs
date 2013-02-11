using System;
using System.Security;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    public class TM_TestLibrary
    {
        public TM_Xml_Database CreateTestDatabase(TM_Xml_Database tmXmlDatabase)
        {
            return tmXmlDatabase;
        }
    }

    [TestFixture]
    public class Test_TM_TestLibrary : TM_XmlDatabase_InMemory
    {        
        public string library_Name              = "Test Library";
        public string folder_In_Library_Name    = "A Folder in Library";
        public string folder_In_Folder_Name     = "A Folder in Folder";
        public string view_In_Library_Name      = "A View in Library";
        public string view_In_Folder_Name       = "A View in Folder";
        
        [SetUp]
        public void SetUp()
        {            
            Assert.Throws<SecurityException>(() => tmXmlDatabase.new_TmLibrary(library_Name));            
            UserGroup.Editor.setThreadPrincipalWithRoles(); // impersonate an Editor
        }
        [TearDown]
        public void TearDown()
        {
            UserGroup.Anonymous.setThreadPrincipalWithRoles(); //revent premissions
        }

        [Test]
        public void CreateLibrary()
        {
            
            //Create Library
            var library = tmXmlDatabase.new_TmLibrary(library_Name);

            Assert.NotNull (library);
            Assert.AreEqual(library_Name    , library.Caption);            
            Assert.AreEqual(library.Caption , tmXmlDatabase.tmLibrary(library.Id).Caption);
            Assert.AreEqual(library.Id      , tmXmlDatabase.tmLibrary(library.Id).Id);

            //Create Folders and Views
            var newFolderInLibrary  = tmXmlDatabase.xmlDB_Add_Folder(library.Id, folder_In_Library_Name);
            var newFolderInFolder   = tmXmlDatabase.xmlDB_Add_Folder(library.Id, newFolderInLibrary.folderId.guid(), folder_In_Folder_Name);
            var newViewInLibrary    = tmXmlDatabase.xmlDB_NewView(new View { library = library.Id.str(), caption = view_In_Library_Name});
            var newViewInFolder     = tmXmlDatabase.xmlDB_NewView (newFolderInLibrary.folderId.guid(), new View {  library = library.Id.str(), caption = view_In_Library_Name});

            Assert.NotNull (newFolderInLibrary);
            Assert.NotNull (newFolderInFolder);
            Assert.NotNull (newViewInLibrary);
            Assert.NotNull (newViewInFolder);

            Assert.AreNotEqual(Guid.Empty,newFolderInLibrary.folderId.guid());
            Assert.AreNotEqual(Guid.Empty,newFolderInFolder.folderId.guid());
            Assert.AreNotEqual(Guid.Empty,newViewInLibrary.id.guid());
            Assert.AreNotEqual(Guid.Empty,newViewInFolder.id.guid());

            var guidanceExplorer          = tmXmlDatabase.xmlDB_GuidanceExplorer(library.Id);
            var libraryStructure          = guidanceExplorer.library.libraryStructure;
            var foldersInLibraryStructure = libraryStructure.folder;
            var viewsInLibraryStructure   = libraryStructure.view;
            var foldersInFolder           = foldersInLibraryStructure.first().folder1;

            Assert.NotNull (guidanceExplorer);
            Assert.NotNull (libraryStructure);
            Assert.NotNull (foldersInLibraryStructure);
            Assert.NotNull (viewsInLibraryStructure);
            Assert.NotNull (foldersInFolder);

            Assert.AreEqual(1, foldersInLibraryStructure.size());
            Assert.AreEqual(1, viewsInLibraryStructure.size());
            Assert.AreEqual(1, foldersInFolder.size());

            
            //Create Articles
            var newArticle = tmXmlDatabase.xmlDB_RandomGuidanceItem(library.Id);
            var result     = tmXmlDatabase.xmlDB_AddGuidanceItemToView(newViewInLibrary.id.guid(), newArticle.Metadata.Id);
            
            Assert.NotNull (newArticle);
            Assert.IsTrue  (result);

            "****** Here is the guidanceExplorer Xml".info();
            guidanceExplorer.toXml().info();

            
        }
    }

}