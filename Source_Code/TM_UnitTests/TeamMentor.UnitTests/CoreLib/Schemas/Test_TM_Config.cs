using NUnit.Framework;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture] 
    public class Test_TM_Config  : TM_XmlDatabase_FileStorage
    {
        [Test]
        public void TM_Config_Ctor()
        {
            //var tmConfig = new TMConfig(); 

            Assert.NotNull(tmConfig);
            Assert.NotNull(tmConfig.TMSetup);
            Assert.NotNull(tmConfig.TMSecurity);
            Assert.NotNull(tmConfig.WindowsAuthentication);
            Assert.NotNull(tmConfig.OnInstallation);
            Assert.NotNull(tmConfig.VirtualArticles);
            Assert.NotNull(tmConfig.TMErrorMessages);
        }

        [Test]
        public void tmConfig_Location() 
        {
            UserGroup.Admin.assert();
            
            var userData_Path     = tmFileStorage.Path_UserData.createDir();
            var expected_Location = userData_Path.pathCombine(TMConsts.TM_CONFIG_FILENAME);

            Assert.NotNull (tmXmlDatabase);
            Assert.NotNull (userData);
            Assert.NotNull (userData_Path); 
            Assert.IsTrue  (userData_Path.dirExists());
            Assert.NotNull (expected_Location);
            Assert.AreEqual(expected_Location, tmFileStorage.tmConfig_Location());
        }

        [Test]
        public void tmConfig_Load()
        {
            TMConfig.Current       = null;
            var temp_Path          = "_tempUserData_Path".tempDir();
            var expected_Location  = temp_Path.pathCombine(TMConsts.TM_CONFIG_FILENAME);

            tmFileStorage.Path_UserData = temp_Path;
            Assert.AreEqual(tmFileStorage.Path_UserData      , temp_Path);
            Assert.AreEqual(tmFileStorage.tmConfig_Location(), expected_Location);

            Assert.IsFalse(expected_Location.fileExists());

            Assert.NotNull(tmFileStorage.tmConfig_Load());

            Assert.NotNull(TMConfig.Current);
            Assert.IsTrue(expected_Location.fileExists());

            //Try loading up an corrupted tmConfig
            "aaaa".saveAs(expected_Location);

            Assert.IsNull(tmFileStorage.tmConfig_Load());
            Assert.NotNull(TMConfig.Current);

            Files.deleteFolder(temp_Path, true);
            Assert.IsFalse    (temp_Path.dirExists());
        }
    }
}
