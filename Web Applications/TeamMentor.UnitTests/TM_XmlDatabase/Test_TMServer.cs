using System;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{    
    [TestFixture]
    public class Test_TMServer
    {        
        [SetUp]
        [Assert_Admin]
        public void setup() 
        {
            
        }

        [TearDown]
        public void tearDown()
        {
          
        }


        [Test]
        public void TMServer_Ctor()
        {
            var tmServer = new TM_Server();
            Assert.NotNull(tmServer);
            Assert.NotNull(tmServer.UserData);
            Assert.NotNull(tmServer.SiteData);
            Assert.IsEmpty(tmServer.UserData_Repos);
            Assert.IsEmpty(tmServer.SiteData_Repos);
            Assert.IsFalse(tmServer.Create_Default_Admin_Account);
        }

        //TM_Server Extension methods

        [Test]
        public void resetData()
        {
            var tmServer = new TM_Server();
            var userData = tmServer.UserData;
            var sideData = tmServer.SiteData;
            Assert.IsEmpty(tmServer.UserData_Repos);
            Assert.IsEmpty(tmServer.SiteData_Repos);
            Assert.NotNull(userData);
            Assert.NotNull(sideData);
            Assert.AreEqual(userData.Active_Repo_Name   , TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
            Assert.AreEqual(userData.Use_FileSystem     , false);
            Assert.AreEqual(userData.Enable_Git_Support , false);
                
            Assert.AreEqual(sideData.Active_Repo_Name   , TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA);
            Assert.AreEqual(sideData.Use_FileSystem     , false);
            Assert.AreEqual(sideData.Enable_Git_Support , false);
        }

        [Test]
        public void add_UserData_Repo()
        {
            var tmServer           = new TM_Server();
            var userData_GitRepo1a = new TM_Server.GitRepo { Name = 10.randomLetters()      };
            var userData_GitRepo1b = new TM_Server.GitRepo { Name = userData_GitRepo1a.Name };
            var userData_GitRepo2  = new TM_Server.GitRepo { Name = 10.randomLetters()      };

            Assert.IsEmpty(tmServer.UserData_Repos);          // should add userData_GitRepo1a

            tmServer.add_UserData_Repo(userData_GitRepo1a);

            Assert.AreEqual( tmServer.UserData_Repos.size()   ,1);
            Assert.AreEqual( tmServer.UserData_Repos.first()  ,userData_GitRepo1a);

            tmServer.add_UserData_Repo(userData_GitRepo2);   // should add userData_GitRepo2

            Assert.AreEqual( tmServer.UserData_Repos.size()   ,2);
            Assert.AreEqual( tmServer.UserData_Repos.first()  ,userData_GitRepo1a);            
            Assert.AreEqual( tmServer.UserData_Repos.second() ,userData_GitRepo2);

            tmServer.add_UserData_Repo(userData_GitRepo1b);  // should reaplce userData_GitRepo1a with userData_GitRepo1b
            Assert.AreEqual(tmServer.UserData_Repos.size()   , 2);            
            Assert.AreEqual(tmServer.UserData_Repos.first()  , userData_GitRepo2);
            Assert.AreEqual(tmServer.UserData_Repos.second() , userData_GitRepo1b);
        }
        [Test]
        public void getActive_UserData_Repo_Remote_GitPath()
        {
            var tmServer = new TM_Server();
            
            Assert.IsNull(tmServer.getActive_UserData_Repo());

            var userData_GitRepo = new TM_Server.GitRepo 
                { 
                    Name           = 10.randomLetters(), 
                    Remote_GitPath = 10.randomLetters()
                };
            tmServer.setActive_UserData_Rep(userData_GitRepo);
            
            var remote_GitPath = tmServer.getActive_UserData_Remote_Repo_GitPath();
            var active_GitRepo = tmServer.getActive_UserData_Repo();
            
            Assert.NotNull (active_GitRepo);
            Assert.AreEqual(userData_GitRepo, active_GitRepo);
            Assert.AreEqual(userData_GitRepo.Remote_GitPath, remote_GitPath); ;            
        }

        [Test]
        public void load_TMServer_Config()
        {
         //   TM_Status.In_Setup_XmlDatabase = false;
            UserGroup.Admin.assert();

            //first test with in memory version of TM_Xml_Database
            TM_Xml_Database tmXmlDatabase = new TM_Xml_Database();
            var tmServer = tmXmlDatabase.load_TMServer_Config();

            Assert.NotNull (tmServer);
            Assert.AreEqual(tmServer, tmXmlDatabase.tmServer());

            //now test with filebase
            tmXmlDatabase = new TM_Xml_Database();

            tmXmlDatabase.Path_XmlDatabase = "_tmpLocation".tempDir();
            tmXmlDatabase.UsingFileStorage = true;
        }
        [Test]        
        public void tmServer()
        {
            TM_Xml_Database tmXmlDatabase = new TM_Xml_Database();  
            Assert.NotNull(tmXmlDatabase.tmServer());
        }        

        [Test][Assert_Admin]
        public void LoadAndSave_TMServer_To_Disk()
        {
            TM_Xml_Database tmXmlDatabase = null;

            var tmServer      = tmXmlDatabase.TM_Server_Config;
            var tmServerPath  = tmXmlDatabase.get_Path_TMServer_Config();

            Assert.IsNotNull(tmServer                   , "tmServer");
            Assert.IsNotNull(tmServerPath               , "tmServerPath");
            Assert.AreEqual (tmServerPath.parentFolder(), tmXmlDatabase.Path_XmlDatabase , "tmServerPath.folderName()");
        }

        //Utils
        /*
          TM_Xml_Database tmXmlDatabase;
            //Create a temp filebase TMXmlDatabase with a Temp UserData repo
            tmXmlDatabase = new TM_Xml_Database();

            tmXmlDatabase.Path_XmlDatabase = "_tmpLocation".tempDir();
            tmXmlDatabase.UsingFileStorage = true;

            Assert.IsTrue(tmXmlDatabase.Path_XmlDatabase.dirExists());

            var tmServer = tmXmlDatabase.TM_Server_Config;
            var tmServerPath = tmXmlDatabase.get_Path_TMServer_Config();

            Assert.IsNotNull(tmServer);
            Assert.IsNotNull(tmServerPath);
            Assert.IsNotNull(tmServer.UserDataRepos);
            Assert.IsEmpty(tmServer.UserDataRepos);

            Assert.NotNull(tmXmlDatabase.UserData.Path_UserData);

            var userDataRepo = new TM_Server.UserDataRepo()
            {
                Name = "TestRepo",
                GitPath = "http://github.com/TeamMentor/XYZ_REPO"
            };
            tmServer.setActive_UserData_Rep(userDataRepo);

            Assert.IsNotEmpty(tmServer.UserDataRepos);
            Assert.AreEqual(tmServer.UserDataRepos.first(), userDataRepo);
            Assert.AreEqual(tmServer.ActiveRepo, userDataRepo.Name);
            Assert.AreEqual(tmServer.getActive_UserData_Rep(), userDataRepo);
            Assert.AreEqual(tmServer.getActive_UserData_Repo_GitPath(), userDataRepo.GitPath);

            Assert.IsTrue(tmXmlDatabase.Path_XmlDatabase.dirExists());            
            Files.deleteFolder(tmXmlDatabase.Path_XmlDatabase, true);
            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.dirExists());
         * */
    }
}