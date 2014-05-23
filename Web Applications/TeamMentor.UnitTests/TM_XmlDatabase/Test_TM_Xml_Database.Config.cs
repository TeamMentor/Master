using System.IO;
using System.Security.AccessControl;
using FluentSharp.WinForms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;


namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_TM_Xml_Database_Config
    {
        public Test_TM_Xml_Database_Config()
        {
            UserGroup.Admin.assert();
        }
        [TearDown] public void teardown()
        {
            TM_Server.WebRoot = AppDomain.CurrentDomain.BaseDirectory;            
        }
        
        [Test] public void userData()
        {
            var tmXmlDatabase = new TM_Xml_Database();
            Assert.IsNull(tmXmlDatabase.UserData);
            var userData = tmXmlDatabase.userData();                //first time should create a new instance of TM_UserData
            Assert.NotNull(userData);
            Assert.AreEqual(userData, tmXmlDatabase.UserData);
            Assert.AreEqual(userData, tmXmlDatabase.userData());    // 2nd time should NOT create a new instance
            
            tmXmlDatabase = null;
            Assert.IsNull(tmXmlDatabase.userData(), null);
        }
        [Test] public void load_UserData()
        {
            var tmXmlDatabase = new TM_Xml_Database(true);

            Assert.IsNull(tmXmlDatabase.UserData);
            
            tmXmlDatabase.load_UserData();

            var userData = tmXmlDatabase.userData();

            
            Assert.NotNull (userData);
            Assert.AreEqual(userData, tmXmlDatabase.UserData);
            Assert.NotNull (userData.Path_UserData);            
            Assert.IsEmpty (userData.TMUsers);
            
            tmXmlDatabase.delete_Database();
                        
        }
        [Test] public void load_TM_Config()
        {
            TMConfig.Current = null;
            var tmXmlDatabase = new TM_Xml_Database(true)
                .load_UserData()
                .tmConfig_Load();

            Assert.AreEqual("", tmXmlDatabase.userData().Path_UserData);
            
            Assert.NotNull(tmXmlDatabase.userData().tmConfig_Location());

            Assert.AreEqual("", tmXmlDatabase.userData().tmConfig_Location());
            Assert.IsNull(TMConfig.Current);

        }
        [Test] public void tm_Server_Load__In_Memory()
        {            
            var tmXmlDatabase = new TM_Xml_Database();
            Assert.NotNull (tmXmlDatabase.Server);
            Assert.AreEqual(tmXmlDatabase.Server.toXml(), new TM_Server().toXml()); // check if the value matches a new object of type TM_Server
        }
        [Test] public void tm_Server_Load_UsingFileStorage()
        {            
            
            var baseReadOnlyDir = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"site\wwwroot";        // simulates use of AppData
            TM_Server.WebRoot = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            var tmXmlDatabase    = new TM_Xml_Database(true);
            var tmServerFile     = tmXmlDatabase.tmServer_Location();
            var expectedLocation = tmXmlDatabase.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME);

            Assert.IsNotNull(tmXmlDatabase.Path_XmlDatabase);    
            Assert.IsTrue   (TM_Status.Current.TM_Database_Location_Using_AppData);
            Assert.NotNull  (tmXmlDatabase.Server);
            Assert.IsTrue   (tmServerFile.fileExists());
            Assert.AreEqual(tmServerFile, expectedLocation);
                      
            var tmServer_withDefaultData = new TM_Server().setDefaultData();
            Assert.AreEqual(tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            //make a change, saved it and ensure it gets loaded ok

            var tmpName1 = 10.randomLetters();
            var tmpName2 = 10.randomLetters();
            tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName1;
            tmXmlDatabase.tmServer_Save();
            tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName2;

            tmXmlDatabase.tmServer_Load();
            Assert.AreEqual   (tmXmlDatabase.Server.UserData_Configs.first().Name, tmpName1);
            Assert.AreNotEqual(tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            //Try loading up an corrupted tmServer (whcih will default to load a default TM_Server
            "aaaa".saveAs(tmServerFile);
            tmXmlDatabase.tmServer_Load();
            Assert.AreNotEqual(tmXmlDatabase.Server.UserData_Configs.first().Name, tmpName1);
            Assert.AreEqual   (tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse(baseReadOnlyDir.dirExists());
            tmXmlDatabase.delete_Database();
        }
        [Test] public void tm_Server_Save()
        {                        
            var tmXmlDatabase            = new TM_Xml_Database(true);
            var tmServerLocation         = tmXmlDatabase.tmServer_Location();
            var tmServer_withDefaultData = new TM_Server().setDefaultData();            

            Assert.NotNull(tmXmlDatabase.Path_XmlDatabase);
            
            Assert.IsTrue(tmServerLocation.fileExists());            
            Assert.AreEqual(tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            var tmpName1 = 10.randomLetters();
            
            tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName1;
            Assert.IsTrue(tmXmlDatabase.tmServer_Save());
            Assert.AreEqual(tmServerLocation.load<TM_Server>().UserData_Configs.first().Name, tmpName1);   //check that it was  saved

            /*
             // this works but makes the test run in 10s (and the test being done is if there is no exception)
                
                var tmpName2 = 10.randomLetters();             
                tmServerLocation.fileInfo()
                        .setAccessControl("Users", FileSystemRights.Write, AccessControlType.Deny);

                tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName2;
            
                Assert.IsFalse(tmXmlDatabase.tmServer_Save());

                Assert.AreEqual(tmServerLocation.load<TM_Server>().UserData_Configs.first().Name, tmpName1);   //check that it was not saved

                Assert.IsTrue(tmServerLocation.delete_File());
                Assert.IsFalse(tmServerLocation.fileExists());
             */
            tmXmlDatabase.delete_Database();
            Assert.IsFalse(tmServerLocation.fileExists());
            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.dirExists());

            //check when not UsingFileStorage

            //check for nulls            
            tmXmlDatabase.Path_XmlDatabase = null;
            Assert.IsFalse(tmXmlDatabase.tmServer_Save());
            Assert.IsFalse(new TM_Xml_Database().tmServer_Save());
        }
        [Test] public void set_Path_UserData()
        {
            var tmXmlDatabase = new TM_Xml_Database(true);            

            var expectedPath  = tmXmlDatabase.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
            
            tmXmlDatabase.set_Path_UserData();
            var userData = tmXmlDatabase.UserData;

            //Assert.NotNull (tmServer);
            Assert.NotNull (userData);
            Assert.AreEqual(userData.Path_UserData, expectedPath);
            Assert.True    (userData.Path_UserData.dirExists());

           
            // try with a different Name value
            var tempName = 10.randomLetters(); 
            tmXmlDatabase.Server.userData_Config().Name = tempName;
            tmXmlDatabase.set_Path_UserData();            
            Assert.IsTrue(tmXmlDatabase.userData().Path_UserData.contains(tempName));
            Assert.IsTrue(tmXmlDatabase.UserData.UsingFileStorage);


            tmXmlDatabase.delete_Database();
            Assert.False   (userData.Path_UserData.dirExists());

            //check bad data handling
            tmXmlDatabase.Server.userData_Config().Name = null;
            tmXmlDatabase.set_Path_UserData();                  
            Assert.IsTrue(tmXmlDatabase.userData().Path_UserData.contains(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA));
            

            tmXmlDatabase.Server.userData_Config().Name = "aaa:bbb"; // will fail to create the UserData folder and force memory mode
            tmXmlDatabase.set_Path_UserData();                  
            Assert.IsNotNull (tmXmlDatabase.UserData);
            Assert.IsNull    (tmXmlDatabase.UserData.Path_UserData);
            Assert.IsFalse   (tmXmlDatabase.UserData.UsingFileStorage);


            tmXmlDatabase.Server = null;                    // should handle ok
            tmXmlDatabase.set_Path_UserData();
            Assert.IsTrue(tmXmlDatabase.userData().Path_UserData.contains(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA));

            tmXmlDatabase = null;
            Assert.IsNull(tmXmlDatabase.set_Path_UserData());

            Assert.IsNull(new TM_Xml_Database().set_Path_UserData().UserData.Path_UserData);


        }
        [Test] public void set_Path_XmlDatabase__In_Memory()
        {
            var tmXmlDatabase1 = new TM_Xml_Database();

            Assert.AreEqual(tmXmlDatabase1, tmXmlDatabase1.set_Path_XmlDatabase());
            Assert.AreEqual(tmXmlDatabase1, TM_Xml_Database.Current);
            Assert.IsNull  (tmXmlDatabase1.Path_XmlDatabase);
            Assert.IsFalse (tmXmlDatabase1.UsingFileStorage);

            var tmXmlDatabase2 = new TM_Xml_Database();
            Assert.AreNotEqual(tmXmlDatabase1, tmXmlDatabase2, "A new tmXmlDatabase1 should had been created");            
            Assert.AreEqual   (tmXmlDatabase2, TM_Xml_Database.Current);
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage()
        {            
            var tmXmlDatabase = new TM_Xml_Database(true);                     
            Assert.AreEqual   (tmXmlDatabase, tmXmlDatabase.set_Path_XmlDatabase());
            Assert.AreEqual   (tmXmlDatabase, TM_Xml_Database.Current);
            Assert.IsTrue     (tmXmlDatabase.UsingFileStorage);
            Assert.IsNotNull  (tmXmlDatabase.Path_XmlDatabase);            

            tmXmlDatabase.delete_Database();
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot()
        {
            Assert.AreEqual(TM_Server.WebRoot, AppDomain.CurrentDomain.BaseDirectory);
            TM_Server.WebRoot = "_tmp_webRoot".tempDir().info();            
            
            TM_Server.WebRoot.delete_Folder();
            Assert.IsFalse(TM_Server.WebRoot.dirExists());

            var tmXmlDatabase = new TM_Xml_Database(true);

            Assert.IsFalse(TM_Server.WebRoot.dirExists()             , "db ctor should not create a Web Root (if it doesn't exist)");
            Assert.IsTrue (tmXmlDatabase.Path_XmlDatabase.dirExists(), "db ctor should create a library folder");

            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.contains("App_Data"));
            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.contains(TM_Server.WebRoot));
            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.contains (PublicDI.config.O2TempDir));
            

            "DB path: {0}".info(tmXmlDatabase.Path_XmlDatabase);
            "Lib path: {0}".info(tmXmlDatabase.Path_XmlLibraries);

            tmXmlDatabase.delete_Database();

            Assert.IsFalse(TM_Server.WebRoot.dirExists()             , "still shouldn't exist");
            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.dirExists(), "should had been deleted");            
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot_without_Read_Privs()
        {
            var baseReadOnlyDir   = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"virtual/path";
            TM_Server.WebRoot     = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            //Check that ensure we can write to baseReadOnlyDir
            Assert.IsTrue  (baseReadOnlyDir.dirExists());
            Assert.IsTrue  (TM_Server.WebRoot.dirExists());
            Assert.IsTrue  (TM_Server.WebRoot.contains(baseReadOnlyDir));
            Assert.IsTrue  (baseReadOnlyDir.canWriteToPath());
            Assert.AreEqual(TM_Server.WebRoot.parentFolder().parentFolder(), baseReadOnlyDir);

            //Now remote the write privileges for all users (on baseReadOnlyDir) while keeping  TM_Server.WebRoot writeable
            
            baseReadOnlyDir  .directoryInfo().deny_Write_Users();
            TM_Server.WebRoot.directoryInfo().allow_Write_Users();   
            
            Assert.IsFalse(baseReadOnlyDir .canWriteToPath());
            Assert.IsTrue(TM_Server.WebRoot.canWriteToPath());

            //Since baseReadOnlyDir can be written, creating an TM_Xml_Database should now default to the App_Data folder (which is on webRootVirualPath )

            var tmXmlDatabase = new TM_Xml_Database(true);

            Assert.IsNotNull(tmXmlDatabase.Path_XmlDatabase);
            Assert.IsTrue   (tmXmlDatabase.UsingFileStorage);
            Assert.IsTrue   (tmXmlDatabase.Path_XmlDatabase.contains("App_Data"));
            Assert.IsTrue   (tmXmlDatabase.Path_XmlDatabase.contains(TM_Server.WebRoot));
            Assert.IsTrue   (tmXmlDatabase.Path_XmlDatabase.contains(PublicDI.config.O2TempDir));

            //Finally re enable write so that we can delete the folder
            baseReadOnlyDir.directoryInfo().allow_Write_Users();
            Assert.IsTrue(baseReadOnlyDir.canWriteToPath());
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse  (baseReadOnlyDir.dirExists());

        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_AppData__without_Read_Privs()
        {
            var baseReadOnlyDir = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"site\wwwroot";        // simulates use of AppData
            TM_Server.WebRoot = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            TM_Server.WebRoot.directoryInfo().deny_CreateDirectories_Users(); 
             
            var tmXmlDatabase = new TM_Xml_Database(true);       // usually a true paramater will set UsingFileStorage to true

            Assert.IsNull (tmXmlDatabase.Path_XmlDatabase);      // if we can't write to the AppData folder then this value can't be set automatically            
            Assert.IsFalse(tmXmlDatabase.UsingFileStorage);      // and the offline mode (i.e. UsingFileStorage = false) should be activated
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse(baseReadOnlyDir.dirExists());
        }
/*todo*/[Test] public void set_Path_XmlLibraries()
        {
            var tmXmlDatabase = new TM_Xml_Database(true);
            tmXmlDatabase.delete_Database();
            Assert.Fail("to implement");
            
        }
        [Test]
        public void resetDatabase()
        {            
            var tmDatabase = new TM_Xml_Database();

            tmDatabase.setDefaultValues();

            Assert.NotNull  (tmDatabase);
            Assert.False    (tmDatabase.UsingFileStorage);                        

            Assert.IsEmpty  (tmDatabase.NGits);            
            Assert.IsEmpty  (tmDatabase.Cached_GuidanceItems);
            Assert.IsEmpty  (tmDatabase.GuidanceItems_FileMappings);
            Assert.IsEmpty  (tmDatabase.GuidanceExplorers_XmlFormat);            
            Assert.IsEmpty  (tmDatabase.GuidanceExplorers_Paths);           
            Assert.IsEmpty  (tmDatabase.VirtualArticles);            
            Assert.IsNotNull(tmDatabase.Events);            

            Assert.IsNull   (tmDatabase.Path_XmlDatabase);
            Assert.IsNull   (tmDatabase.Path_XmlLibraries);
            Assert.IsNull   (tmDatabase.UserData);
            Assert.IsNull   (tmDatabase.Server);
        }

    }
}
