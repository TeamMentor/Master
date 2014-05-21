using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;

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
    }
}
