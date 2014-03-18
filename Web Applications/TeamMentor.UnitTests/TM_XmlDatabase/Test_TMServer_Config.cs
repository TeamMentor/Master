using System;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_TMServer_Config: TM_XmlDatabase_InMemory
    {
        [Test]
        public void CheckDefaultValuesAndBehaviour()
        {
            var tmServer     = tmXmlDatabase.TM_Server_Config;
            var tmServerPath = tmXmlDatabase.get_Path_TMServer_Config();

            Assert.IsNotNull(tmServer);
            Assert.IsNull   (tmServerPath);
            Assert.IsNotNull(tmServer.UserDataRepos);
            Assert.IsEmpty  (tmServer.UserDataRepos);
            
            var userDataRepo = new TMServer_UserDataRepo()
                {
                    Name = "TestRepo",
                    GitPath = "http://github.com/TeamMentor/XYZ_REPO"
                };
            tmServer.setActive_UserData_Rep(userDataRepo);

            Assert.IsNotEmpty(tmServer.UserDataRepos);
            Assert.AreEqual  (tmServer.UserDataRepos.first()            , userDataRepo);
            Assert.AreEqual  (tmServer.ActiveRepo                       , userDataRepo.Name);
            Assert.AreEqual  (tmServer.getActive_UserData_Rep()         , userDataRepo);
            Assert.AreEqual  (tmServer.getActive_UserData_Repo_GitPath(), userDataRepo.GitPath);
        }
    }

    [TestFixture]
    public class Test_TMServer_Config_Disk
    {
        [Test][Assert_Admin]
        public void LoadAndSave_TMServer_To_Disk()
        {
            var tmXmlDatabase              = new TM_Xml_Database();
            tmXmlDatabase.Path_XmlDatabase = "_tmpLocation".tempDir();
            tmXmlDatabase.UsingFileStorage = true;

            var tmServer      = tmXmlDatabase.TM_Server_Config;
            var tmServerPath  = tmXmlDatabase.get_Path_TMServer_Config();

            Assert.IsNotNull(tmServer                   , "tmServer");
            Assert.IsNotNull(tmServerPath               , "tmServerPath");
            Assert.AreEqual (tmServerPath.parentFolder(), tmXmlDatabase.Path_XmlDatabase , "tmServerPath.folderName()");
        }
    }
}