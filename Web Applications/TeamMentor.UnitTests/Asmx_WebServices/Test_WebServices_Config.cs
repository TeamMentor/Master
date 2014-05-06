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

        [Test][Assert_Admin][Ignore("WithoutFileStorage is not enabled on the current version")]
        public void XmlDatabase_WithoutFileStorage()
        {
            var original_TM_XmlDatabase = tmXmlDatabase;

            Assert.IsFalse  (tmWebServices.XmlDatabase_IsUsingFileStorage());
            tmWebServices.tmXmlDatabase.UsingFileStorage = true;

            Assert.IsTrue   (tmWebServices.XmlDatabase_IsUsingFileStorage());

            var result = tmWebServices.XmlDatabase_WithoutFileStorage();
            var new_TM_XmlDatabase = TM_Xml_Database.Current;
            
            
            Assert.IsTrue (result);
            Assert.IsFalse(new_TM_XmlDatabase.UsingFileStorage);
            Assert.AreNotEqual(original_TM_XmlDatabase, new_TM_XmlDatabase);
            Assert.AreNotEqual(original_TM_XmlDatabase.UsingFileStorage, new_TM_XmlDatabase.UsingFileStorage);
        }
    }
}
