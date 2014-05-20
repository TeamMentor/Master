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
            TM_Status.reset();
        }
        [Test]
        public void TM_Status_Ctor()
        {
            Assert.IsFalse(TM_Status.Loading_UserData);
            Assert.IsFalse(TM_Status.Loading_XmlDatabase);
            Assert.IsFalse(TM_Status.Loaded_UserData);
            Assert.IsFalse(TM_Status.Loaded_XmlDatabase);
            Assert.IsFalse(TM_Status.In_Setup_XmlDatabase);
            
        }
    }
}
