using NUnit.Framework;
using System;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.FileStorage;


namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_TM_Xml_Database_Config
    {
        public Test_TM_Xml_Database_Config()
        {
            UserGroup.Admin.assert();
        }
        [SetUp] public void setup()
        {
            TM_FileStorage.Current = null;            
        }
        [TearDown] public void teardown()
        {
            //TM_Server.WebRoot = AppDomain.CurrentDomain.BaseDirectory;            
        }
        
        /*[Test] public void userData()
        {
            var tmXmlDatabase = new TM_Xml_Database();
            Assert.IsNull(tmXmlDatabase.UserData);
            var userData = tmXmlDatabase.userData();                //first time should create a new instance of TM_UserData
            Assert.NotNull(userData);
            Assert.AreEqual(userData, tmXmlDatabase.UserData);
            Assert.AreEqual(userData, tmXmlDatabase.userData());    // 2nd time should NOT create a new instance
            
            tmXmlDatabase = null;
            Assert.IsNull(tmXmlDatabase.userData(), null);
        }*/
        [Test] public void load_UserData()
        {
            var tmFileStorage = new TM_FileStorage(false);              // create empty TM_FileStorage
            var tmServer      = tmFileStorage.Server;
            var userData      = tmFileStorage.UserData;            
            
            Assert.IsNotNull(tmServer);
            Assert.IsNotNull(userData);
            
            tmServer.Users_Create_Default_Admin = true;

            tmFileStorage.load_UserData();

            Assert.IsEmpty (userData.TMUsers);
            
            Assert.Ignore("Add checks specific to the load_UserData method ");
                        
        }
        [Test] public void tmConfig_Load()
        {
            var tmFileStorage = new TM_FileStorage(false);              // create empty TM_FileStorage
            var tmServer      = tmFileStorage.Server;
            var userData      = tmFileStorage.UserData;                                                              
            
            tmFileStorage.tmConfig_Load();

            //if Path_UserData is null, then tmConfig_Location is also null
            
            Assert.IsNull(tmFileStorage.Path_UserData);            
            Assert.IsNull(tmFileStorage.tmConfig_Location());
            Assert.NotNull(TMConfig.Current);

        }
        [Test] public void tm_Server_Load__In_Memory()
        {            
            var tmFileStorage           = new TM_FileStorage(false);
            var tmServer_defaultData    = new TM_Server();//.setDefaultData();                     

            Assert.NotNull (tmFileStorage.Server);
            Assert.AreEqual(tmFileStorage.Server.toXml(), tmServer_defaultData.toXml()); // check if the value matches a new object of type TM_Server
        }
        [Test] public void tm_Server_Load_UsingFileStorage()
        {            
            var tmFileStorage = new TM_FileStorage(false);            
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            var baseReadOnlyDir = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"site\wwwroot";        // simulates use of AppData
            tmFileStorage.WebRoot = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();
            
            tmFileStorage.set_Path_XmlDatabase()
                         .set_Path_UserData()
                         .tmServer_Load();
            
            var tmServerFile     = tmFileStorage.tmServer_Location();
            var expectedLocation = tmFileStorage.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME);

            Assert.IsNotNull(tmFileStorage.path_XmlDatabase());    
            Assert.IsTrue   (TM_Status.Current.TM_Database_Location_Using_AppData);
            Assert.NotNull  (tmFileStorage.Server);
            Assert.IsTrue   (tmServerFile.fileExists());
            Assert.AreEqual(tmServerFile, expectedLocation);
                     
//            Assert.Ignore("TO FIX (Refactor Side Effect");
     //       Assert.Ignore("TO FIX (Refactor Side Effect");
            var tmServer_withDefaultData = new TM_Server().setDefaultData();
            Assert.AreEqual(tmFileStorage.Server.toXml(), tmServer_withDefaultData.toXml());

            //make a change, saved it and ensure it gets loaded ok

            var tmpName1 = 10.randomLetters();
            var tmpName2 = 10.randomLetters();
            tmFileStorage.Server.UserData_Configs.first().Name = tmpName1;
            tmFileStorage.tmServer_Save();
            tmFileStorage.Server.UserData_Configs.first().Name = tmpName2;

            tmFileStorage.tmServer_Load();
            Assert.AreEqual   (tmFileStorage.Server.UserData_Configs.first().Name, tmpName1);
            Assert.AreNotEqual(tmFileStorage.Server.toXml(), tmServer_withDefaultData.toXml());

            //Try loading up an corrupted tmServer (whcih will default to load a default TM_Server
            "aaaa".saveAs(tmServerFile);
            tmFileStorage.tmServer_Load();
            Assert.AreNotEqual(tmFileStorage.Server.UserData_Configs.first().Name, tmpName1);
            Assert.AreEqual   (tmFileStorage.Server.toXml(), tmServer_withDefaultData.toXml());
            
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse(baseReadOnlyDir.dirExists());

            //tmXmlDatabase.delete_Database();
        }
        [Test] public void tm_Server_Save()
        {                       
            var tmFileStorage = new TM_FileStorage();  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            //tmXmlDatabase.set_Path_XmlDatabase()
            //             .tmServer_Load();
            Assert.NotNull(tmXmlDatabase.path_XmlDatabase());      

            var tmServerLocation         = tmFileStorage.tmServer_Location();
           
            var tmServer_withDefaultData = new TM_Server().setDefaultData();             

                  
            Assert.IsTrue(tmServerLocation.fileExists());    
        
           // Assert.Ignore("TO FIX (Refactor Side Effect"); 

            Assert.AreEqual(tmFileStorage.Server.toXml(), tmServer_withDefaultData.toXml());

            var tmpName1 = 10.randomLetters();
            
            tmFileStorage.Server.UserData_Configs.first().Name = tmpName1;
            Assert.IsTrue(tmFileStorage.tmServer_Save());
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
            Assert.IsFalse(tmXmlDatabase.path_XmlDatabase().dirExists());

            //check when not UsingFileStorage

            //check for nulls            
            tmFileStorage.Path_XmlDatabase = null;
            Assert.IsFalse(tmFileStorage.tmServer_Save());
            Assert.IsFalse(new TM_FileStorage(false).tmServer_Save());
        }
        [Test] public void set_Path_UserData()
        {
            var tmFileStorage = new TM_FileStorage();  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            var expectedPath  = tmXmlDatabase.path_XmlDatabase().pathCombine(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
            
            tmFileStorage.set_Path_UserData();
            var userData = tmFileStorage.UserData;

            //Assert.NotNull (tmServer);
            Assert.NotNull (userData);
            Assert.AreEqual(tmFileStorage.Path_UserData, expectedPath);
            Assert.True    (tmFileStorage.Path_UserData.dirExists());

           
            // try with a different Name value
            var tempName = 10.randomLetters(); 
            tmFileStorage.Server.userData_Config().Name = tempName;
            tmFileStorage.set_Path_UserData();            
            Assert.IsTrue(tmFileStorage.Path_UserData.contains(tempName));
            Assert.IsTrue(tmFileStorage.UserData.usingFileStorage());


            tmXmlDatabase.delete_Database();
            Assert.False   (tmFileStorage.Path_UserData.dirExists());

            //check bad data handling
            tmFileStorage.Server.userData_Config().Name = null;
            tmFileStorage.set_Path_UserData();                  
            Assert.IsTrue(tmFileStorage.Path_UserData.contains(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA));
            

            tmFileStorage.Server.userData_Config().Name = "aaa:bbb"; // will fail to create the UserData folder and force memory mode
            tmFileStorage.set_Path_UserData();                  
            Assert.IsNotNull (tmFileStorage.UserData);
            Assert.IsNull    (tmFileStorage.Path_UserData);

            

            

            //test nulls
            tmFileStorage.Server         = null;     
            tmFileStorage.UserData       = null;
            tmFileStorage.TMXmlDatabase  = null;
            tmFileStorage.set_Path_UserData();

            Assert.IsFalse(tmFileStorage.UserData.usingFileStorage());
            Assert.IsNull (tmFileStorage.Path_UserData);

            //Assert.IsNull (new TM_FileStorage(false).set_Path_UserData().Path_UserData);


        }

         [Test] public void set_Path_SiteData()
        {
            var tmFileStorage = new TM_FileStorage();  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            var expectedPath  = tmXmlDatabase.path_XmlDatabase().pathCombine(TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA);
            
            tmFileStorage.set_Path_SiteData();            
            
            Assert.AreEqual(tmFileStorage.Path_SiteData, expectedPath);
            Assert.True    (tmFileStorage.Path_SiteData.dirExists());

           
            // try with a different Name value
            var tempName = 10.randomLetters(); 
            tmFileStorage.Server.siteData_Config().Name = tempName;
            tmFileStorage.set_Path_SiteData();            
            Assert.IsTrue(tmFileStorage.Path_SiteData.contains(tempName));            
            
            //check bad data handling
            tmFileStorage.Server.siteData_Config().Name = null;
            tmFileStorage.set_Path_SiteData();                  
            Assert.IsTrue(tmFileStorage.Path_SiteData.contains(TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA));
            

            tmFileStorage.Server.siteData_Config().Name = "aaa:bbb"; // will fail to create the SiteData folder and force memory mode
            tmFileStorage.set_Path_SiteData();                              
            Assert.IsNull    (tmFileStorage.Path_SiteData);
            
        }
        [Test] public void set_Path_XmlDatabase__In_Memory()
        {
            var tmFileStorage = new TM_FileStorage(false);  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;
            
            var tmServer = tmFileStorage.Server;
            Assert.AreEqual(tmFileStorage, tmFileStorage.set_Path_XmlDatabase());
            Assert.AreEqual(tmXmlDatabase, TM_Xml_Database.Current);
            Assert.IsNull  (tmFileStorage.path_XmlDatabase());
            Assert.IsFalse (tmXmlDatabase.usingFileStorage());

            var tmXmlDatabase2 = new TM_Xml_Database();
            Assert.AreNotEqual(tmXmlDatabase, tmXmlDatabase2, "A new tmXmlDatabase1 should had been created");            
            Assert.AreEqual   (tmXmlDatabase2, TM_Xml_Database.Current);
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage()
        {            
            var tmFileStorage = new TM_FileStorage();  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;
                  
            tmFileStorage.set_Path_XmlDatabase();

            Assert.AreEqual   (tmXmlDatabase, TM_Xml_Database.Current);
            Assert.IsTrue     (tmXmlDatabase.usingFileStorage());
            Assert.IsNotNull  (tmXmlDatabase.path_XmlDatabase());            

            tmXmlDatabase.delete_Database();
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot()
        {
            var tmFileStorage = new TM_FileStorage(false);  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            //Assert.AreEqual(tmFileStorage.WebRoot, AppDomain.CurrentDomain.BaseDirectory);
            tmFileStorage.WebRoot = "_tmp_webRoot".tempDir().info();            
            
            tmFileStorage.WebRoot.delete_Folder();
            Assert.IsFalse(tmFileStorage.WebRoot.dirExists());
            
            tmFileStorage.set_Path_XmlDatabase();

            Assert.IsTrue  (tmFileStorage.path_XmlDatabase().dirExists(), "db ctor should create a library folder");   

            var usingAppDataFolder = TM_Status.Current.TM_Database_Location_Using_AppData;
            
            "*** DB path: {0}".info(tmXmlDatabase.path_XmlDatabase());
            "*** Lib path: {0}".info(tmFileStorage.Path_XmlLibraries);
            "*** Current WebRoot: {0}".debug(tmFileStorage.WebRoot);
            "*** Current WebRoot exists: {0}".debug(tmFileStorage.WebRoot.dirExists());
            "*** TM_Status.Current.TM_Database_Location_Using_AppData: {0}".debug(TM_Status.Current.TM_Database_Location_Using_AppData);

            Assert.AreEqual(usingAppDataFolder, tmFileStorage.WebRoot.dirExists()       , "db ctor should not create a Web Root (if it doesn't exist)");
            Assert.AreEqual(usingAppDataFolder, tmFileStorage.path_XmlDatabase().contains ("App_Data"));
            Assert.AreEqual(usingAppDataFolder, tmFileStorage.path_XmlDatabase().contains (tmFileStorage.WebRoot));
            Assert.AreEqual(usingAppDataFolder, tmFileStorage.path_XmlDatabase().contains (PublicDI.config.O2TempDir));

            //tmXmlDatabase.delete_Database();

            Assert.AreEqual(usingAppDataFolder, tmFileStorage.WebRoot.dirExists()  , "if not usingAppDataFolder the TM_Server.WebRoot shouldn't exist");
            Assert.IsFalse(tmXmlDatabase.path_XmlDatabase().dirExists()          , "should had been deleted");            
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot_without_Read_Privs()
        {
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
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_AppData__without_Read_Privs()
        {
            var tmFileStorage = new TM_FileStorage(false);  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            var baseReadOnlyDir = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"site\wwwroot";        // simulates use of AppData
            tmFileStorage.WebRoot = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            tmFileStorage.WebRoot.directoryInfo().deny_CreateDirectories_Users(); 
             
            //var tmXmlDatabase = new TM_Xml_Database().useFileStorage();       // usually a true paramater will set UsingFileStorage to true
            
            tmFileStorage.set_Path_XmlDatabase();

            Assert.IsNull (tmFileStorage.path_XmlDatabase());      // if we can't write to the AppData folder then this value can't be set automatically            
            Assert.IsFalse(tmXmlDatabase.usingFileStorage());      // and the offline mode (i.e. UsingFileStorage = false) should be activated
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse(baseReadOnlyDir.dirExists());
        }
        [Test] public void set_Path_XmlLibraries()
        {
            var tmFileStorage = new TM_FileStorage(false);  
            
            TMConfig.Current = null;

            //this is the sequence that needs to be loaded in order to have a Path for Xml Libraries
            tmFileStorage.set_WebRoot()                 
                         .set_Path_XmlDatabase()        
                         .tmServer_Load()               
                         .set_Path_UserData()           //  
                         .tmConfig_Load()               // 
                         .set_Path_XmlLibraries();

            Assert.NotNull(tmFileStorage.path_XmlDatabase());
            Assert.NotNull(tmFileStorage.path_XmlLibraries());
            Assert.IsTrue (tmFileStorage.path_XmlDatabase().dirExists());
            Assert.IsTrue (tmFileStorage.path_XmlLibraries().dirExists());
            Assert.NotNull(TMConfig.Current);

            //test nulls
            tmFileStorage.Path_XmlLibraries =null;  

            //in the scenarios below the tmFileStorage.Path_XmlLibraries should not be set
            TMConfig.Current.TMSetup = null;           
            tmFileStorage.set_Path_XmlLibraries();
            TMConfig.Current = null;
            tmFileStorage.set_Path_XmlLibraries();
            Assert.IsNull(tmFileStorage.Path_XmlLibraries);
            
            //tmXmlDatabase.delete_Database();
            //TMConfig.Current = new TMConfig();
            
        }
        [Test]
        public void set_Default_Values()
        {            
            var tmFileStorage = new TM_FileStorage(false);  
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;            

            var events     = tmXmlDatabase.Events;           // this value should not change

            tmXmlDatabase.set_Default_Values();

            Assert.NotNull  (tmXmlDatabase);                                
            
            Assert.IsEmpty  (tmXmlDatabase.Cached_GuidanceItems);
            Assert.IsEmpty  (tmFileStorage.GuidanceItems_FileMappings);
            Assert.IsEmpty  (tmXmlDatabase.GuidanceExplorers_XmlFormat);                        
            Assert.IsEmpty  (tmXmlDatabase.VirtualArticles);            
            Assert.AreEqual (tmXmlDatabase.Events, events);                               
            Assert.IsNull   (tmXmlDatabase.path_XmlDatabase());

            Assert.IsEmpty  (tmFileStorage.GuidanceExplorers_Paths);           
            Assert.IsNull   (tmFileStorage.Path_XmlLibraries);
            Assert.IsNotNull(tmFileStorage.UserData);
            Assert.IsNotNull(tmFileStorage.Server);
        }

    }
}
