using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests
{    
    public class TM_XmlDatabase_FileStorage 
    {        
        public TM_FileStorage   tmFileStorage;
        public TM_UserData      userData;
        public TM_Xml_Database  tmXmlDatabase;    
        public TMConfig         tmConfig;    
        public TM_Server        tmServer;
        
        public TM_XmlDatabase_FileStorage()
        {
            SendEmails.Disable_EmailEngine = true;

            UserGroup.Admin.assert();
            tmFileStorage   = new TM_FileStorage();
            tmXmlDatabase   = tmFileStorage.TMXmlDatabase; //new TM_Xml_Database().setup();
            userData        = tmFileStorage.UserData;
            tmServer        = tmFileStorage.Server;
            tmConfig        = TMConfig.Current;

            CheckDatabaseSetup(); 
            UserGroup.None.assert();
        }

        [Assert_Admin]
        public void CheckDatabaseSetup()
        {           
            Assert.NotNull(tmXmlDatabase);
            Assert.NotNull(userData);
            Assert.NotNull(tmServer);
            Assert.NotNull(tmConfig);
 
            if(tmXmlDatabase.tmLibraries().notEmpty())      // temp while refactoring
            { 
                tmXmlDatabase.delete_Database();                        
            }
            if(tmXmlDatabase.getCacheLocation().fileExists())
                tmXmlDatabase.clear_GuidanceItemsCache();

            if(userData.tmUsers().notEmpty())
                userData.delete_All_Users();
          //  userData.createDefaultAdminUser(); 
            
            Assert.NotNull(tmXmlDatabase.path_XmlDatabase()		    , "path_XmlDatabase()");        // null since we are running TM memory (default setting)
            Assert.NotNull(tmXmlDatabase.path_XmlLibraries()		, "Path_XmlLibraries");         // null since we are running TM memory (default setting)
            Assert.IsEmpty(tmXmlDatabase.Cached_GuidanceItems	    , "Cached_GuidanceItems");
            Assert.IsEmpty (userData.validSessions()                , "ActiveSessions");
            Assert.AreEqual(userData.TMUsers.size()                ,0 , "TMUsers");	                // there should be no users
            
            
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
             
         /*public void Install_LibraryFromZip_TopVulns()  // not public anymore
        {
            var topVulnsZipFile = DownloadLibraryIntoTempFolder("Lib_Top_Vulnerabilities.zip",
                                                                "https://github.com/TMContent/Lib_Top_Vulnerabilities/archive/master.zip");
            Install_LibraryFromZip(topVulnsZipFile,"Top Vulnerabilities");
        } */       
        public void Install_LibraryFromZip_Docs() 
        {
            var topVulnsZipFile = DownloadLibraryIntoTempFolder("Lib_Docs.zip",
                                                                "https://github.com/TMContent/Lib_Docs/archive/master.zip");
            Install_LibraryFromZip(topVulnsZipFile,"TM Documentation");
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
            UserGroup.Admin.assert();
            try
            {                            
                //tmXmlDatabase.useFileStorage(true);      // temp set this so that we can load the files and create the cache
                //tmXmlDatabase.Path_XmlLibraries = "_TempXmlLibraries".tempDir(false);

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
                //tmXmlDatabase.useFileStorage(false);
            }
            finally
            {
                UserGroup.None.assert();
            }
        }
    }
}
