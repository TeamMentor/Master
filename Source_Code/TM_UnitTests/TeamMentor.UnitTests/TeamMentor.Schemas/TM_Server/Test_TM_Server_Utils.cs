using System;
using FluentSharp.NUnit;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Schemas
{    
    [TestFixture]
    public class Test_TM_Server_Utils
    {        
        [SetUp]    public void setup() 
        {
            
        }
        [TearDown] public void tearDown()
        {
          
        }

        [Test] public void TMServer_Ctor()
        {
            // see setDefaultValues();   
        }
        
        //TM_Server Extension methods
        [Test] public void setDefaultValues()                       
        {
            var tmServer = new TM_Server();
            Assert.NotNull(tmServer);            
            Assert.IsEmpty(tmServer.UserData_Configs);
            Assert.IsEmpty(tmServer.SiteData_Configs);
            
            tmServer.assert_Equals(TM_Server.Current);

//            Assert.IsTrue (tmServer.Users_Create_Default_Admin);
            //Assert.IsFalse(tmServer.TM_Database_Use_AppData_Folder);            
            Assert.IsFalse(tmServer.UserActivities_Disable_Logging);    
        }        
        [Test] public void setDefaultData()                              
        {
            var tmServer = new TM_Server();    
            tmServer.setDefaultData();
            Assert.IsNotEmpty(tmServer.UserData_Configs);
            Assert.IsNotEmpty(tmServer.SiteData_Configs);     
            Assert.AreEqual(1, tmServer.UserData_Configs.size());
            Assert.AreEqual(1, tmServer.SiteData_Configs.size());
            
            var userData = tmServer.userData_Config();
            Assert.AreEqual(userData.Name               , TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
            Assert.AreEqual(userData.Active             , true);
//            Assert.AreEqual(userData.Use_FileSystem     , false);
//            Assert.AreEqual(userData.Enable_Git_Support , false);

            var siteData = tmServer.siteData_Config();
            Assert.AreEqual(siteData.Name               , TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA);
            Assert.AreEqual(siteData.Active             , true);
//            Assert.AreEqual(siteData.Use_FileSystem     , false);
//            Assert.AreEqual(siteData.Enable_Git_Support , false);
        }
        [Test] public void add_UserData_Repo()                      
        {
            var tmServer          = new TM_Server().setDefaultData();
            var userData_Config1a = new TM_Server.Config { Name = 10.randomLetters() };
            var userData_Config1b = new TM_Server.Config { Name = userData_Config1a.Name };
            var userData_Config2  = new TM_Server.Config { Name = 10.randomLetters() };

            Assert.IsNotEmpty(tmServer.UserData_Configs);          // there should be at least one default UserData config
            
            tmServer.UserData_Configs.clear();

            tmServer.add_UserData(userData_Config1a);

            Assert.AreEqual( tmServer.UserData_Configs.size()   ,1);
            Assert.AreEqual( tmServer.UserData_Configs.first()  ,userData_Config1a);

            tmServer.add_UserData(userData_Config2);            // should add userData_Config2

            Assert.AreEqual( tmServer.UserData_Configs.size()   ,2);
            Assert.AreEqual( tmServer.UserData_Configs.first()  ,userData_Config1a);            
            Assert.AreEqual( tmServer.UserData_Configs.second() ,userData_Config2);

            tmServer.add_UserData(userData_Config1b);           // should reaplce userData_GitRepo1a with userData_GitRepo1b
            Assert.AreEqual(tmServer.UserData_Configs.size()   , 2);            
            Assert.AreEqual(tmServer.UserData_Configs.first()  , userData_Config2);
            Assert.AreEqual(tmServer.UserData_Configs.second() , userData_Config1b);
        }
        [Test] public void getActive_UserData_Repo_Remote_GitPath() 
        {
            var tmServer = new TM_Server();
                        

            var userData_Config = new TM_Server.Config 
                { 
                    Name           = 10.randomLetters(), 
                    Remote_GitPath = 10.randomLetters()
                };
            tmServer.add_UserData(userData_Config);
            tmServer.active_UserData(userData_Config);
            
            var active_GitConfig = tmServer.userData_Config();
            var remote_GitPath   = tmServer.userData_Config().Remote_GitPath;

            Assert.NotNull (active_GitConfig);
            Assert.AreEqual(userData_Config, active_GitConfig);
            Assert.AreEqual(userData_Config.Remote_GitPath, remote_GitPath); ;            
        }
              
        

        //Utils
        /*
          TM_Xml_Database tmXmlDatabase;
            //Create a temp filebase TMXmlDatabase with a Temp UserData repo
            tmXmlDatabase = new TM_Xml_Database();

            tmXmlDatabase.Path_XmlDatabase = "_tmpLocation".tempDir();
            tmXmlDatabase.useFileStorage(true);

            Assert.IsTrue(tmXmlDatabase.Path_XmlDatabase.dirExists());

            var tmServer = tmXmlDatabase.TM_Server_Config;
            var tmServerPath = tmXmlDatabase.tmServer_Location();

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