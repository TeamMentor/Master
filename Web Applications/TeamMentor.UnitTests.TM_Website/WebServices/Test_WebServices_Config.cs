using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.WebServices
{
    [TestFixture]
    public class Test_WebServices_Config : API_TM_WebServices
    {
        [Test][Ignore]
        public void Get_TM_QA_Config()
        {
            var tmQaConfig_Path = this.webServices.Get_TM_QA_Config_Path();
            var tmQaConfig      = this.webServices.Get_TM_QA_Config();
            Assert.IsNotNull(tmQaConfig_Path);
            Assert.IsNotNull(tmQaConfig);
            Assert.AreEqual(tmQaConfig.toXml(), tmQaConfig_Path.load<TM_QA_Config>().toXml());
        }
    }
}
