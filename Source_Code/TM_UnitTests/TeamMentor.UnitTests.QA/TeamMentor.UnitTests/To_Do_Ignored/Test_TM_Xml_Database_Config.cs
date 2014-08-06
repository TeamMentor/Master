using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.QA.To_Do_Ignored
{
    [TestFixture]
    public class Test_TM_Xml_Database_Config
    {
        [Test] [Assert_Admin] public void load_UserData()
        {
            admin.assert();

            var tmFileStorage = new TM_FileStorage(false);              // create empty TM_FileStorage
            var tmServer      = tmFileStorage.Server;
            var userData      = tmFileStorage.UserData;            
            
            Assert.IsNotNull(tmServer);
            Assert.IsNotNull(userData);
            
            //tmServer.Users_Create_Default_Admin = true;

            tmFileStorage.load_UserData();

            Assert.IsEmpty (userData.TMUsers);
            
            none.assert();

            Assert.Ignore("Add checks specific to the load_UserData method ");                        
        }
        [Test] [Assert_Admin]public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot_without_Read_Privs()
        {
            admin.assert();

            var tmFileStorage = new TM_FileStorage(false);  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            var baseReadOnlyDir   = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"virtual/path";

            tmFileStorage.WebRoot     = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            //Check that ensure we can write to baseReadOnlyDir
            Assert.IsTrue  (baseReadOnlyDir.dirExists());
            Assert.IsTrue  (tmFileStorage.WebRoot.dirExists());
            Assert.IsTrue  (tmFileStorage.WebRoot.contains(baseReadOnlyDir));
            Assert.IsTrue  (baseReadOnlyDir.canWriteToPath());
            Assert.AreEqual(tmFileStorage.WebRoot.parentFolder().parentFolder(), baseReadOnlyDir);

            //Now remote the write privileges for all users (on baseReadOnlyDir) while keeping  TM_Server.WebRoot writeable
            
            baseReadOnlyDir  .directoryInfo().deny_Write_Users();
            tmFileStorage.WebRoot.directoryInfo().allow_Write_Users();   
            
            Assert.IsFalse(baseReadOnlyDir .canWriteToPath());
            Assert.IsTrue(tmFileStorage.WebRoot.canWriteToPath());

            //Since baseReadOnlyDir can be written, creating an TM_Xml_Database should now default to the App_Data folder (which is on webRootVirualPath )

//            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();
            
            tmFileStorage.set_Path_XmlDatabase();
            
            Assert.IsNotNull(tmFileStorage.path_XmlDatabase());           

            none.assert();

            Assert.Ignore("TO FIX (Refactor Side Effect");
            Assert.IsTrue   (tmFileStorage.path_XmlDatabase().contains("App_Data"));
            Assert.IsTrue   (tmFileStorage.path_XmlDatabase().contains(tmFileStorage.WebRoot));
            Assert.IsTrue   (tmFileStorage.path_XmlDatabase().contains(PublicDI.config.O2TempDir));

            //Finally re enable write so that we can delete the folder
            baseReadOnlyDir.directoryInfo().allow_Write_Users();
            Assert.IsTrue(baseReadOnlyDir.canWriteToPath());
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse  (baseReadOnlyDir.dirExists());

        }
    }
}
