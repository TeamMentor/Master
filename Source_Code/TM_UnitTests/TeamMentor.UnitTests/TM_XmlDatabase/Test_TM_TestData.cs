using System;
using System.Security;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using urn.microsoft.guidanceexplorer;
using View = TeamMentor.CoreLib.View;

namespace TeamMentor.UnitTests.TM_XmlDatabase
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
        public string folder2_In_Folder_Name    = "Another Folder in Folder";
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
            //Create Test Library
            var library = tmXmlDatabase.new_TmLibrary(library_Name);

            Assert.NotNull (library);
            Assert.AreEqual(library_Name    , library.Caption);            
            Assert.AreEqual(library.Caption , tmXmlDatabase.tmLibrary(library.Id).Caption);
            Assert.AreEqual(library.Id      , tmXmlDatabase.tmLibrary(library.Id).Id);

            //Create Folders and Views
            var newFolderInLibrary  = tmXmlDatabase.xmlDB_Add_Folder(library.Id, folder_In_Library_Name);
            var newFolderInFolder   = tmXmlDatabase.xmlDB_Add_Folder(library.Id, newFolderInLibrary.folderId.guid(), folder_In_Folder_Name);
            var newViewInLibrary    = tmXmlDatabase.xmlDB_NewView   (                                    new View { library = library.Id.str(), caption = view_In_Library_Name});
            var newViewInFolder     = tmXmlDatabase.xmlDB_NewView   (newFolderInLibrary.folderId.guid(), new View { library = library.Id.str(), caption = view_In_Folder_Name});

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

           
            //Test serialization

            var serializedXml = guidanceExplorer.toXml();            
            guidanceExplorer = serializedXml.deserialize<guidanceExplorer>(false);

            Assert.IsNotNull (guidanceExplorer);
            Assert.IsNotNull (guidanceExplorer.library);
            Assert.IsNotNull (guidanceExplorer.library.libraryStructure);
            Assert.IsNotEmpty(guidanceExplorer.library.libraryStructure.view);

            var view = guidanceExplorer.library.libraryStructure.view.first();

            Assert.IsNotNull (view);
            Assert.AreEqual  (view.caption , view_In_Library_Name);
            Assert.AreEqual  (view.id      , newViewInLibrary.id);
            Assert.IsNotEmpty(view.items.item);
            Assert.AreEqual  (view.items.item.first(), newArticle.Metadata.Id.str());

            "## For reference, here is the guidanceExplorer Xml ##".line().info();
            serializedXml.info();

            tmXmlDatabase.delete_Library(library);
            var libraryAfterDelete = tmXmlDatabase.tmLibrary(library.Id);
            Assert.IsNull(libraryAfterDelete);
        }

        [Test]
        public void Create_and_Delete_Folders()
        {
            var library = tmXmlDatabase.new_TmLibrary(library_Name);            
            
            //Create Views
            var newFolderInLibrary      = tmXmlDatabase.xmlDB_Add_Folder(library.Id, folder_In_Library_Name);
            var newFolderInFolder       = tmXmlDatabase.xmlDB_Add_Folder(library.Id, newFolderInLibrary.folderId.guid(), folder_In_Folder_Name);
            var anotherFolderInFolder   = tmXmlDatabase.xmlDB_Add_Folder(library.Id, newFolderInLibrary.folderId.guid(), folder2_In_Folder_Name);
            var newViewInFolder         = tmXmlDatabase.xmlDB_NewView   (newFolderInLibrary.folderId.guid(), new View { library = library.Id.str(), caption = view_In_Folder_Name});

            //Check that they are there
            var tmFolder1 = tmXmlDatabase.tmFolder(newFolderInLibrary.folderId.guid());
            var tmFolder2 = tmXmlDatabase.tmFolder(newFolderInFolder.folderId.guid());
            var tmFolder3 = tmXmlDatabase.tmFolder(anotherFolderInFolder.folderId.guid());
            Assert.AreEqual(tmFolder1.name, newFolderInLibrary.caption    , "tmFolder1.caption");
            Assert.AreEqual(tmFolder2.name, newFolderInFolder.caption     , "tmFolder2.caption");
            Assert.AreEqual(tmFolder3.name, anotherFolderInFolder.caption , "tmFolder3.caption");

            //Delete and check that they are not there   (note that when folder 1 is deleted so are the sub folders)          
            var result2               = tmXmlDatabase.xmlDB_Delete_Folder(tmFolder2.libraryId, tmFolder2.folderId);
            var result1               = tmXmlDatabase.xmlDB_Delete_Folder(tmFolder1.libraryId, tmFolder1.folderId);
            var result3               = tmXmlDatabase.xmlDB_Delete_Folder(tmFolder3.libraryId, tmFolder3.folderId);
            var tmFolder1_AfterDelete = tmXmlDatabase.tmFolder(tmFolder1.folderId);
            var tmFolder2_AfterDelete = tmXmlDatabase.tmFolder(tmFolder2.folderId);
            var tmFolder3_AfterDelete = tmXmlDatabase.tmFolder(tmFolder3.folderId);
            var tmView1_AfterDelete   = tmXmlDatabase.tmView(newViewInFolder.id.guid());

            Assert.IsTrue   (result1, "result 1 was false");
            Assert.IsTrue   (result2, "result 2 was false");
            Assert.IsFalse  (result3, "result 3 was true (that folder should not be there after its parent was deleted");
            Assert.IsNull   (tmFolder1_AfterDelete, "tmFolder1 was still there after delete");
            Assert.IsNull   (tmFolder2_AfterDelete, "tmFolder2 was still there after delete");
            Assert.IsNull   (tmFolder3_AfterDelete, "tmFolder3 was still there after delete");
            Assert.IsNull   (tmView1_AfterDelete, "tmView1 was still there after delete");            

            tmXmlDatabase.delete_Library(library);
 
        }

        [Test]
        public void Create_and_Delete_Views()
        {
            var library = tmXmlDatabase.new_TmLibrary(library_Name);

            //Create folders
            var newFolderInLibrary = tmXmlDatabase.xmlDB_Add_Folder(library.Id, folder_In_Library_Name);
            var newViewInLibrary = tmXmlDatabase.xmlDB_NewView(new View { library = library.Id.str(), caption = view_In_Library_Name });
            var newViewInFolder = tmXmlDatabase.xmlDB_NewView(newFolderInLibrary.folderId.guid(), new View { library = library.Id.str(), caption = view_In_Folder_Name });

            //Check that they are there
            var tmView1 = tmXmlDatabase.tmView(newViewInLibrary.id.guid());
            var tmView2 = tmXmlDatabase.tmView(newViewInFolder.id.guid());
            Assert.AreEqual(tmView1.caption, newViewInLibrary.caption, "tmView1.caption");
            Assert.AreEqual(tmView2.caption, newViewInFolder.caption, "tmView2.caption");

            //Delete and check that they are not there
            var result1 = tmXmlDatabase.xmlDB_RemoveView(library, tmView1.viewId);
            var result2 = tmXmlDatabase.xmlDB_RemoveView(tmView1.libraryId, tmView2.viewId);
            var tmView1_AfterDelete = tmXmlDatabase.tmView(newViewInLibrary.id.guid());
            var tmView2_AfterDelete = tmXmlDatabase.tmView(newViewInFolder.id.guid());

            Assert.IsTrue(result1, "result 1 was false");
            Assert.IsTrue(result2, "result 2 was false");
            Assert.IsNull(tmView1_AfterDelete, "tmView1 was still there after delete");
            Assert.IsNull(tmView2_AfterDelete, "tmView2 was still there after delete");
            tmXmlDatabase.delete_Library(library);
        }

    }

}