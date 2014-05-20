using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    class Test_WebServices_Config : TM_WebServices_InMemory
    {
        [SetUp]
        public void setup()
        {
            tmWebServices.tmXmlDatabase.UsingFileStorage = false;
        }
        [Test][Assert_Admin]
        public void XmlDatabase_UsingFileStorage()
        {
            Assert.IsNotNull(tmWebServices);
            Assert.IsNotNull(tmWebServices.tmXmlDatabase);            
            Assert.IsFalse  (tmWebServices.XmlDatabase_IsUsingFileStorage());
            tmWebServices.tmXmlDatabase.UsingFileStorage = true;
            Assert.IsTrue   (tmWebServices.XmlDatabase_IsUsingFileStorage());
            tmWebServices.tmXmlDatabase.UsingFileStorage = false;
            Assert.IsFalse  (tmWebServices.XmlDatabase_IsUsingFileStorage());
        }
    }
}
