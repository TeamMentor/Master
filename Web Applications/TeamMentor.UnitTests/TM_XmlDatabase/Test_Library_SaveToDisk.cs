using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture][Assert_Admin]
    public class Test_Library_SaveToDisk
    {
        public TM_Xml_Database tmDatabase;

        public Test_Library_SaveToDisk()
        {
            TMConfig.BaseFolder = "temp_BaseFolder".tempDir();  // set temp folder for UnitTests
            tmDatabase = new TM_Xml_Database(true);          // with the useFileStorage set to true
        }

        [Test]
        public void TestUseOfTempFolders()
        {
            var databaseFolder      = tmDatabase.Path_XmlDatabase;
            var libraryFolder       = tmDatabase.Path_XmlLibraries;
            var appDomainFolder     = AppDomain.CurrentDomain.BaseDirectory;

            Assert.IsFalse(libraryFolder.contains(appDomainFolder), "libraryFolder should not be inside appDomainFolder");
            Assert.IsFalse(databaseFolder.contains(appDomainFolder), "databaseFolder should not be inside appDomainFolder");            
        }

        public void Test_xmlDB_LibraryPath()
        {
            var newLibraryName1 = "Test_new_Library";
            var testLibrary1    = tmDatabase.new_TmLibrary(newLibraryName1);

            var libraryPath_Null_GuidanceExplorer = tmDatabase.xmlDB_Path_Library_XmlFile(null as guidanceExplorer);
            var libraryPath_Null_TM_Library       = tmDatabase.xmlDB_Path_Library_XmlFile(null as TM_Library);
            var libraryPath_Null_EmptyGuid        = tmDatabase.xmlDB_Path_Library_XmlFile(Guid.Empty);
            var libraryPath_TM_Library            = tmDatabase.xmlDB_Path_Library_XmlFile(testLibrary1);
            var libraryPath_Library_Id            = tmDatabase.xmlDB_Path_Library_XmlFile(testLibrary1.Id);
            var libraryPath_GuidanceExplorer      = tmDatabase.xmlDB_Path_Library_XmlFile(testLibrary1.guidanceExplorer(tmDatabase));            

            Assert.IsNull(libraryPath_Null_GuidanceExplorer , "libraryPath_NullValue");
            Assert.IsNull(libraryPath_Null_TM_Library       , "libraryPath_NullValue");
            Assert.IsNull(libraryPath_Null_EmptyGuid        , "libraryPath_Null_EmptyGuid");
            
            Assert.NotNull(libraryPath_TM_Library           , "libraryPath_TM_Library");
            Assert.NotNull(libraryPath_Library_Id           , "libraryPath_Library_Id");
            Assert.NotNull(libraryPath_GuidanceExplorer     , "libraryPath_GuidanceExplorer");

            Assert.IsTrue(libraryPath_TM_Library.valid() && libraryPath_Library_Id.valid() && libraryPath_GuidanceExplorer.valid());
            Assert.AreEqual(libraryPath_TM_Library, libraryPath_Library_Id);
            Assert.AreEqual(libraryPath_TM_Library, libraryPath_GuidanceExplorer);
        }


        [Test]
        public void CreateLibrary_OnDisk()
        {
            var newLibraryName1  = "Test_new_Library";
            var newLibraryName2  = "C++";

            var testLibrary1      = tmDatabase.new_TmLibrary     (newLibraryName1);
            var libraryPath1      = tmDatabase.xmlDB_Path_Library_XmlFile (testLibrary1);
            var testLibrary2      = tmDatabase.new_TmLibrary     (newLibraryName2);
            var libraryPath2      = tmDatabase.xmlDB_Path_Library_XmlFile (testLibrary2);

            Assert.IsTrue(libraryPath1.fileExists());
            Assert.IsTrue(libraryPath2.fileExists());
        }

        [Test]
        public void CreateArticle_OnLibraryWithDiferentNameThanFolder()
        {
            var libraryId                   = Guid.NewGuid();
            var libraryName                 = "library_Name".add_RandomLetters(4);
            var libraryFolderAndXmlFile     = "FolderXml_Name".add_RandomLetters(4);
            var newGuidanceExplorer         = new guidanceExplorer { library = { name = libraryId.str(), caption = libraryName } };
            var newGuidanceExplorerXmlFile  = tmDatabase.Path_XmlLibraries.pathCombine(@"{0}\{0}.xml".format(libraryFolderAndXmlFile));

            //manually add the new newGuidanceExplorer to the database
            tmDatabase.GuidanceExplorers_XmlFormat.add(libraryId, newGuidanceExplorer);
            //manually add the new guidanceExplorer Path
            tmDatabase.GuidanceExplorers_Paths.add(newGuidanceExplorer, newGuidanceExplorerXmlFile);
            //save guidanceExplorer (which should save in the new path)
            tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId);

            //get new values
            var libraryXmlFile = tmDatabase.xmlDB_Path_Library_XmlFile      (libraryId);
            var libraryRootFolder = tmDatabase.xmlDB_Path_Library_RootFolder(newGuidanceExplorer);

            Assert.IsTrue     (newGuidanceExplorerXmlFile.fileExists(), "newGuidanceExplorerXmlFile.fileExists()");
            Assert.IsTrue     (libraryRootFolder.dirExists()          , "libraryRootFolder.dirExists()");
            Assert.AreEqual   (libraryXmlFile, newGuidanceExplorerXmlFile, "libraryXmlFile and libraryFolderAndXmlFile");
            Assert.AreNotEqual(libraryXmlFile.fileName_WithoutExtension(), libraryName);

            //Now that we have confirmed that the library Name is different from the folder name, we can add an article
            //which was not working ok in 3.3. ( https://github.com/TeamMentor/Master/issues/482 )
            var newArticle         = tmDatabase.xmlDB_RandomGuidanceItem(libraryId);            
            var articlesInLibrary  = tmDatabase.getGuidanceItems_from_LibraryFolderOrView(libraryId);
            var articlePath        = tmDatabase.xmlDB_guidanceItemPath(newArticle.Metadata.Id);
            var articlePath_Manual = libraryRootFolder.pathCombine(TMConsts.DEFAULT_ARTICLE_FOLDER_NAME).pathCombine("{0}.xml".format(newArticle.Metadata.Id));

            Assert.NotNull  (newArticle                  , "newArticle was null");
            Assert.AreEqual (1, articlesInLibrary.size() , "There should be one article in this Library");
            Assert.IsTrue   (articlePath.fileExists()    , "articlePath.fileExists()");
            Assert.IsTrue   (articlePath_Manual.fileExists(), "articlePath_Manual.fileExists()");
            Assert.AreEqual (articlePath, articlePath_Manual, "articlePath vs articlePath_Manual");

            articlePath.info();
            articlePath_Manual.info();
        }

        [Test]
        public void CreateLibrary_DirectlyOnDatabase_CheckExpectedPaths()
        {
            var libraryId   = Guid.NewGuid();
            var libraryName =  "test_Library".add_RandomLetters(4);
            var newGuidanceExplorer = new guidanceExplorer {library = {name = libraryId.str(), caption = libraryName}};
            
            //manually add the new newGuidanceExplorer to the database
            tmDatabase.GuidanceExplorers_XmlFormat.add(libraryId, newGuidanceExplorer);
            newGuidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);

            var tmLibrary = tmDatabase.tmLibrary(libraryId);

            Assert.IsNotNull(tmLibrary, "tmLibrary was null for libraryId: {0}".format(libraryId));
            Assert.AreEqual(tmLibrary.Id, libraryId, "tmLibrary.Id");
            Assert.AreEqual(tmLibrary.Caption, libraryName, "tmLibrary.Caption");

            var libraryXml_Via_LibraryId        = tmDatabase.xmlDB_Path_Library_XmlFile(libraryId);
            var libraryXml_Via_GuidanceExplorer = tmDatabase.xmlDB_Path_Library_XmlFile(newGuidanceExplorer);
            var libraryRootFolder               = tmDatabase.xmlDB_Path_Library_RootFolder(newGuidanceExplorer);

            Assert.IsTrue(libraryXml_Via_LibraryId.valid(), "libraryXml_Via_LibraryId");
            Assert.AreEqual(libraryXml_Via_LibraryId, libraryXml_Via_GuidanceExplorer, "libraryXml_Via_LibraryId and libraryXml_Via_GuidanceExplorer");

            Assert.AreEqual(libraryName, libraryXml_Via_LibraryId.fileName_WithoutExtension(),
                            libraryXml_Via_LibraryId.fileName_WithoutExtension());
            Assert.AreEqual(libraryName, libraryRootFolder.folderName(), libraryRootFolder.folderName());
        }
    }
}
