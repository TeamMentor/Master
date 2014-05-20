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
        TM_Xml_Database tmXmlDatabase;

        [SetUp]
        [Assert_Admin]
        public void setup() 
        {
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
            Assert.IsEmpty  (tmServer.UserDataRepos);

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
        }

        [TearDown]
        public void tearDown()
        {
            Assert.IsTrue(tmXmlDatabase.Path_XmlDatabase.dirExists());            
            Files.deleteFolder(tmXmlDatabase.Path_XmlDatabase, true);
            Assert.IsFalse(tmXmlDatabase.Path_XmlDatabase.dirExists());
        }


        [Test]
        public void TMServer_Ctor()
        {
            var tmServer = new TM_Server();
            Assert.NotNull(tmServer);
            Assert.NotNull(tmServer.UserDataRepos);
            Assert.IsEmpty(tmServer.UserDataRepos);
        }

        //TM_Server Extension methods

        [Test]
        public void getActive_UserData_Repo_GitPath()
        {
            Assert.NotNull(tmXmlDatabase);            
            Assert.NotNull(tmXmlDatabase.tmServer());
            tmXmlDatabase.tmServer().toXml().info();
            var activeUserRepo = tmXmlDatabase.tmServer().getActive_UserData_Rep();
            var path = tmXmlDatabase.tmServer().getActive_UserData_Repo_GitPath();
            Assert.NotNull(tmXmlDatabase.tmServer().getActive_UserData_Repo_GitPath());

        }

        [Test]        
        public void tmServer()
        {
            Assert.NotNull(tmXmlDatabase.tmServer());
        }        

        [Test][Assert_Admin]
        public void LoadAndSave_TMServer_To_Disk()
        {
            

            var tmServer      = tmXmlDatabase.TM_Server_Config;
            var tmServerPath  = tmXmlDatabase.get_Path_TMServer_Config();

            Assert.IsNotNull(tmServer                   , "tmServer");
            Assert.IsNotNull(tmServerPath               , "tmServerPath");
            Assert.AreEqual (tmServerPath.parentFolder(), tmXmlDatabase.Path_XmlDatabase , "tmServerPath.folderName()");
        }
    }
}