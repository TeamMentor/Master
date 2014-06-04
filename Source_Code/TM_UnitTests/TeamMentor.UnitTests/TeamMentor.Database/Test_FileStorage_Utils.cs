using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.Database;

namespace TeamMentor.UnitTests.TeamMentor.Database
{
    [TestFixture]
    public class Test_FileStorage_Utils
    {
        [Test]
        public void useFileStorage()
        {
            var tmXmlDatabase = new TM_Xml_Database();
            
            Assert.NotNull(tmXmlDatabase.tmServer());
            Assert.IsFalse(tmXmlDatabase.usingFileStorage());

            Assert.AreEqual(tmXmlDatabase.useFileStorage()              , tmXmlDatabase);
            Assert.IsTrue  (tmXmlDatabase.usingFileStorage());   

            Assert.AreEqual(tmXmlDatabase.useFileStorage(false)         , tmXmlDatabase);
            Assert.IsFalse (tmXmlDatabase.usingFileStorage());

            Assert.AreEqual(tmXmlDatabase.useFileStorage(true)          , tmXmlDatabase);
            Assert.IsTrue  (tmXmlDatabase.usingFileStorage());

            //test that nulls have no effect 
            var tmServer = tmXmlDatabase.Server;
            tmXmlDatabase.Server = null;
            Assert.AreEqual(tmXmlDatabase            .useFileStorage() , tmXmlDatabase);
            Assert.IsNull((null as TM_Xml_Database)  .useFileStorage());

            Assert.IsTrue  (TM_Server.UseFileStorage);
        }

        [Test]
        public void usingFileStorage()
        {
            var tmXmlDatabase = new TM_Xml_Database();

            Assert.IsFalse(tmXmlDatabase.usingFileStorage());
            
            tmXmlDatabase.useFileStorage();

            Assert.IsTrue(tmXmlDatabase.usingFileStorage());

            tmXmlDatabase.Server = null;
            Assert.IsFalse(tmXmlDatabase.usingFileStorage());
            Assert.IsFalse((null as TM_Xml_Database).usingFileStorage());
        }

    }
}
