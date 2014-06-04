using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.Database;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    class Test_WebServices_Config : TM_WebServices_InMemory
    {
        [SetUp]
        public void setup()
        {
            tmWebServices.tmXmlDatabase.useFileStorage(false);
        }
        [Test][Assert_Admin]
        public void XmlDatabase_UsingFileStorage()
        {
            UserGroup.Admin.assert();
            Assert.IsNotNull(tmWebServices);
            Assert.IsNotNull(tmWebServices.tmXmlDatabase);            
            Assert.IsFalse  (tmWebServices.XmlDatabase_IsUsingFileStorage());
            tmWebServices.tmXmlDatabase.useFileStorage(true);
            Assert.IsTrue   (tmWebServices.XmlDatabase_IsUsingFileStorage());
            tmWebServices.tmXmlDatabase.useFileStorage(false);
            Assert.IsFalse  (tmWebServices.XmlDatabase_IsUsingFileStorage());
        }
    }
}
