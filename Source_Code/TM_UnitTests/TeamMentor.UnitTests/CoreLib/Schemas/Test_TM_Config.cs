using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.Database;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture] 
    public class Test_TM_Config
    {
        [Test]
        public void TM_Config_Ctor()
        {
            var tmConfig = new TMConfig();

            Assert.NotNull(tmConfig);
            Assert.NotNull(tmConfig.TMSetup);
            Assert.NotNull(tmConfig.TMSecurity);
            Assert.NotNull(tmConfig.WindowsAuthentication);
            Assert.NotNull(tmConfig.OnInstallation);
            Assert.NotNull(tmConfig.VirtualArticles);
        }

        [Test]
        public void tmConfig_Location()
        {
            UserGroup.Admin.assert();
            var tmXmlDatabase     = new TM_Xml_Database().useFileStorage().setup();
            var userData          = tmXmlDatabase.UserData;
            var userData_Path     = userData.Path_UserData;
            var expected_Location = userData_Path.pathCombine(TMConsts.TM_CONFIG_FILENAME);

            Assert.NotNull (tmXmlDatabase);
            Assert.NotNull (userData);
            Assert.NotNull (userData_Path);
            Assert.IsTrue  (userData_Path.dirExists());
            Assert.NotNull (expected_Location);
            Assert.AreEqual(expected_Location, userData.tmConfig_Location());
        }

        [Test]
        public void tmConfig_Load()
        {
            TMConfig.Current       = null;
            var temp_Path          = "_tempUserData_Path".tempDir();
            var expected_Location  = temp_Path.pathCombine(TMConsts.TM_CONFIG_FILENAME);

            var userData = new TM_UserData()
                                    {
                                        Path_UserData = temp_Path
                                    };
            
            
            Assert.AreEqual(userData.Path_UserData      , temp_Path);
            Assert.AreEqual(userData.tmConfig_Location(), expected_Location);

            Assert.IsFalse(expected_Location.fileExists());

            Assert.NotNull(userData.tmConfig_Load());

            Assert.NotNull(TMConfig.Current);
            Assert.IsTrue(expected_Location.fileExists());

            //Try loading up an corrupted tmConfig
            "aaaa".saveAs(expected_Location);

            Assert.IsNull(userData.tmConfig_Load());
            Assert.NotNull(TMConfig.Current);

            Files.deleteFolder(temp_Path, true);
            Assert.IsFalse    (temp_Path.dirExists());
        }
    }
}
