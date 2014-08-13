using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Schemas
{
    [TestFixture]
    public class Test_TM_Status
    {        

        [SetUp]
        public void setup()
        {
            
        }
        [Test]
        public void TM_Status_Ctor()
        {
            var tmStatus = new TM_Status();

            Assert.IsFalse(tmStatus.TM_Database_In_Setup_Workflow);
            Assert.IsFalse(tmStatus.TM_Database_Location_Using_AppData);
            Assert.AreEqual(tmStatus.TM_Database_Status, TM_Status.Database_Status.Not_Initialized);                        
        }
    }
}
