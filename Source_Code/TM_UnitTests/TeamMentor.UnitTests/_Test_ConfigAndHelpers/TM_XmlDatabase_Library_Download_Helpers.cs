using FluentSharp.CoreLib;
using FluentSharp.Web35.API;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests
{
    public static class TM_XmlDatabase_Library_Download_Helpers
    {

        //Helpers                
        public static string downloadLibraryIntoTempFolder(this TM_FileStorage tmFileStorage, string libraryName, string downloadPath)
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
        public static string install_LibraryFromZip_Docs(this TM_FileStorage tmFileStorage) 
        {
            var topVulnsZipFile = tmFileStorage.downloadLibraryIntoTempFolder("Lib_Docs.zip",
                "https://github.com/TMContent/Lib_Docs/archive/master.zip");
            return tmFileStorage.install_LibraryFromZip(topVulnsZipFile,"TM Documentation");
        }
        public static string install_LibraryFromZip_OWASP(this TM_FileStorage tmFileStorage)
        {
            var owaspZipFile = tmFileStorage.downloadLibraryIntoTempFolder("Lib_OWASP.zip",
                "https://github.com/TMContent/Lib_OWASP/archive/master.zip");
            return tmFileStorage.install_LibraryFromZip(owaspZipFile,"OWASP");            
        }
        [Assert_Admin]
        public static string install_LibraryFromZip(this TM_FileStorage tmFileStorage, string pathToGitHubZipBall, string libraryName)
        {
            UserGroup.Admin.assert();
            try
            {
                var tmXmlDatabase = tmFileStorage.tmXmlDatabase();
                if (tmFileStorage.isNull() || tmXmlDatabase.isNull())
                    return null;                

                var tmLibrary = tmXmlDatabase.tmLibrary(libraryName);
                if (tmLibrary.notNull())
                {
                    "[Install_LibraryFromZip] Skyping library {0} because it already existed".debug(libraryName);
                    return tmFileStorage.xmlDB_Path_Library_RootFolder(tmLibrary);
                }
                var tmLibraries_Before = tmXmlDatabase.tmLibraries();            

                var result = tmFileStorage.xmlDB_Libraries_ImportFromZip(pathToGitHubZipBall, "");
            
                var tmLibraries_After = tmXmlDatabase.tmLibraries();
                var installedLibrary  = tmXmlDatabase.tmLibrary(libraryName);
                                    
                Assert.IsTrue     (result                                             , "xmlDB_Libraries_ImportFromZip");                        
                Assert.IsNotEmpty (tmLibraries_After                                  , "Libraries should be there after install");
                Assert.AreNotEqual(tmLibraries_After.size(), tmLibraries_Before.size(), "Libraries size should be different before and after");
                Assert.IsNotNull  (installedLibrary                                   , "Could not find installed library: {0}".format(libraryName));
                Assert.AreEqual   (installedLibrary.Caption, libraryName              , "After install library names didn't match");
                return tmFileStorage.xmlDB_Path_Library_RootFolder(installedLibrary);
            }
            finally
            {
                UserGroup.None.assert();
            }
        }
    }
}