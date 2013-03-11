using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{    
    public class TM_XmlDatabase_InMemory
    {
        public TM_Xml_Database  tmXmlDatabase;
        public TM_UserData      userData;
        public TMConfig         tmConfig;
        
        public TM_XmlDatabase_InMemory()
        {
            SetupDatabase();            
        }

        [Assert_Admin]
        public void SetupDatabase()
        {
            //1.set_DEFAULT_PBKDF2_INTERACTIONS();                // improve performance of tests that created users
            tmXmlDatabase   = new TM_Xml_Database();
            userData        = tmXmlDatabase.UserData;
            tmConfig        = TMConfig.Current = new TMConfig();
            //new TM_TestLibrary().CreateTestDatabase(tmXmlDatabase);

            //all these values should be null since we are running TM memory (default setting)
            Assert.IsNull(tmXmlDatabase.Path_XmlDatabase		    , "Path_XmlDatabase");
            Assert.IsNull(tmXmlDatabase.Path_XmlLibraries		    , "Path_XmlLibraries");
            Assert.IsEmpty(tmXmlDatabase.Cached_GuidanceItems	    , "Cached_GuidanceItems");
            Assert.IsEmpty(tmXmlDatabase.UserData.ActiveSessions    , "ActiveSessions");
            Assert.AreEqual(tmXmlDatabase.UserData.TMUsers.size()   ,1 , "TMUsers");				// there should be admin
        }

        [TestFixtureSetUp]		
        public void TestFixtureSetUp()
        {			            
        }


        //Helpers                
        public void Install_LibraryFromZip_TopVulns()
        {
            Install_LibraryFromZip("https://github.com/TeamMentor/Library_Top_Vulnerabilities/zipball/master", "Top Vulnerabilities");
        }        
        public void Install_LibraryFromZip_OWASP()
        {
            Install_LibraryFromZip("https://github.com/TeamMentor/OWASP_Library/zipball/master", "OWASP");
        }
        [Assert_Admin]
        public void Install_LibraryFromZip(string pathToGitHubZipBall, string libraryName)
        {
            tmXmlDatabase.Path_XmlLibraries = "_TempXmlLibraries".tempDir(false);

            if (tmXmlDatabase.tmLibrary(libraryName).notNull())
            {
                "[Install_LibraryFromZip] Skyping library {0} because it already existed".debug(libraryName);
                return;
            }
            var tmLibraries_Before = tmXmlDatabase.tmLibraries();            

            var result = tmXmlDatabase.xmlDB_Libraries_ImportFromZip(pathToGitHubZipBall, "");
            tmXmlDatabase.xmlDB_Load_GuidanceItems();  // extra step required to reload the guidance items
            
            var tmLibraries_After = tmXmlDatabase.tmLibraries();
            var installedLibrary  = tmXmlDatabase.tmLibrary(libraryName);
            
            Assert.IsTrue     (result                                             , "xmlDB_Libraries_ImportFromZip");                        
            Assert.IsNotEmpty (tmLibraries_After                                  , "Libraries should be there after install");
            Assert.AreNotEqual(tmLibraries_After.size(), tmLibraries_Before.size(), "Libraries size should be different before and after");
            Assert.IsNotNull  (installedLibrary                                   , "Could not find installed library: {0}".format(libraryName));
            Assert.AreEqual   (installedLibrary.Caption, libraryName              , "After install library names didn't match");
        }
    }
}
