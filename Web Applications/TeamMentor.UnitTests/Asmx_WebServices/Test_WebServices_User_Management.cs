using System;
using NUnit.Framework;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    public class Test_WebServices_User_Management : TM_WebServices_InMemory
    {        
        [Test]
        public void Login()
        {
            var sessionId = tmWebServices.Current_SessionID();
            Assert.AreEqual(sessionId, Guid.Empty , "sessionId should be empty");
            tmWebServices.Login(tmConfig.DefaultAdminUserName, tmConfig.DefaultAdminPassword);
            Assert.AreEqual(sessionId, Guid.Empty , "sessionId should be empty");
        }
    }
}
