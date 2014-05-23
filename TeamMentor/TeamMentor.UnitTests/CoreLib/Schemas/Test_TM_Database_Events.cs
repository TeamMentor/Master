using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Schemas
{
    [TestFixture]
    public class Test_TM_Database_Events
    {
        [Test]
        public void TM_Database_Events_Ctor()
        {
            var tmDBEvents = new TM_Database_Events();

//            Assert.AreEqual(0, tmDBEvents.On_TM_Server_Config.size());
        }
    }
}
