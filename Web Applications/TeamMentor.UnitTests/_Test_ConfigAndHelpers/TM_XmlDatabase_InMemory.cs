using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{    
    public class TM_XmlDatabase_InMemory : TM_UserData_InMemory
    {
        public TM_Xml_Database  tmXmlDatabase;                
        
        public TM_XmlDatabase_InMemory()
        {
            SetupDatabase();            
        }

        [Assert_Admin]
        public void SetupDatabase()
        {                        

            tmXmlDatabase   = new TM_Xml_Database();
            userData        = tmXmlDatabase.UserData;      

            Assert.IsNull(tmXmlDatabase.Path_XmlDatabase		    , "Path_XmlDatabase");          // null since we are running TM memory (default setting)
            Assert.IsNull(tmXmlDatabase.Path_XmlLibraries		    , "Path_XmlLibraries");         // null since we are running TM memory (default setting)
            Assert.IsEmpty(tmXmlDatabase.Cached_GuidanceItems	    , "Cached_GuidanceItems");
            Assert.IsEmpty(tmXmlDatabase.UserData.validSessions()   , "ActiveSessions");
            Assert.AreEqual(tmXmlDatabase.UserData.TMUsers.size(),1 , "TMUsers");	                 // there should be admin            
            
        }

        [TestFixtureSetUp]		
        public void TestFixtureSetUp()
        {			            
        }



        //Helpers                
        public string DownloadLibraryIntoTempFolder(string libraryName, string downloadPath)
        {
            var tmpDownloadDir = System.IO.Path.GetTempPath().pathCombine("_TeamMentor_TempLibraries")
                                                             .createDir();
            Assert.IsTrue(tmpDownloadDir.dirExists(), "Could find tmpDownloadDir: {0}".format(tmpDownloadDir));
            var downloadedFile = tmpDownloadDir.pathCombine(libraryName);
            if (downloadedFile.fileExists())
                return downloadedFile;            
            new Web().downloadBinaryFile(downloadPath,downloadedFile);
            Assert.IsTrue(downloadedFile.fileExists());
            return downloadedFile;
        }

        public void Install_LibraryFromZip_TopVulns()
        {
            var topVulnsZipFile = DownloadLibraryIntoTempFolder("Lib_Top_Vulnerabilities.zip",
                                                                "https://github.com/TMContent/Lib_Top_Vulnerabilities/archive/master.zip");
            Install_LibraryFromZip(topVulnsZipFile,"Top Vulnerabilities");
        }        
        public void Install_LibraryFromZip_OWASP()
        {
            var owaspZipFile = DownloadLibraryIntoTempFolder("Lib_OWASP.zip",
                                                             "https://github.com/TMContent/Lib_OWASP/archive/master.zip");
            Install_LibraryFromZip(owaspZipFile,"OWASP");            
        }
        [Assert_Admin]
        public void Install_LibraryFromZip(string pathToGitHubZipBall, string libraryName)
        {
            tmXmlDatabase.UsingFileStorage = true;      // temp set this so that we can load the files and create the cache
            tmXmlDatabase.Path_XmlLibraries = "_TempXmlLibraries".tempDir(false);

            if (tmXmlDatabase.tmLibrary(libraryName).notNull())
            {
                "[Install_LibraryFromZip] Skyping library {0} because it already existed".debug(libraryName);
                return;
            }
            var tmLibraries_Before = tmXmlDatabase.tmLibraries();            

            var result = tmXmlDatabase.xmlDB_Libraries_ImportFromZip(pathToGitHubZipBall, "");
            //tmXmlDatabase.xmlDB_Load_GuidanceItems();  // extra step required to reload the guidance items
            
            var tmLibraries_After = tmXmlDatabase.tmLibraries();
            var installedLibrary  = tmXmlDatabase.tmLibrary(libraryName);
            
            Assert.IsTrue     (result                                             , "xmlDB_Libraries_ImportFromZip");                        
            Assert.IsNotEmpty (tmLibraries_After                                  , "Libraries should be there after install");
            Assert.AreNotEqual(tmLibraries_After.size(), tmLibraries_Before.size(), "Libraries size should be different before and after");
            Assert.IsNotNull  (installedLibrary                                   , "Could not find installed library: {0}".format(libraryName));
            Assert.AreEqual   (installedLibrary.Caption, libraryName              , "After install library names didn't match");
            tmXmlDatabase.UsingFileStorage = false;
        }
    }
}
