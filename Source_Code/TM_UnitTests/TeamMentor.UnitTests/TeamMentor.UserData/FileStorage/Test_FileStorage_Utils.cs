using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.Database;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.UserData
{
    [TestFixture]
    public class Test_FileStorage_Utils
    {
        [Test]
        public void useFileStorage()
        {
            var userData = new TM_UserData();
            
            Assert.NotNull(userData.tmServer());
            Assert.IsFalse(userData.usingFileStorage());

            Assert.AreEqual(userData.useFileStorage()              , userData);
            Assert.IsTrue  (userData.usingFileStorage());   

            Assert.AreEqual(userData.useFileStorage(false)         , userData);
            Assert.IsFalse (userData.usingFileStorage());

            Assert.AreEqual(userData.useFileStorage(true)          , userData);
            Assert.IsTrue  (userData.usingFileStorage());

            //test that nulls have no effect 
            var tmServer = userData.Server;
            userData.Server = null;
            Assert.AreEqual(userData            .useFileStorage() , userData);
            Assert.IsNull((null as TM_Xml_Database)  .useFileStorage());

            Assert.IsTrue  (TM_Server.UseFileStorage);
        }

        [Test]
        public void usingFileStorage()
        {
            var userData = new TM_UserData();

            Assert.IsFalse(userData.usingFileStorage());
            
            userData.useFileStorage();

            Assert.IsTrue(userData.usingFileStorage());

            userData.Server = null;
            Assert.IsFalse(userData.usingFileStorage());
            Assert.IsFalse((null as TM_Xml_Database).usingFileStorage());
        }

    }
}
