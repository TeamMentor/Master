using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
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
            var sessionId_BeforeLogin = tmWebServices.Current_SessionID();
            var login_SessionId       = tmWebServices.Login       (tmConfig.TMSecurity.Default_AdminUserName, tmConfig.TMSecurity.Default_AdminPassword);
            HttpContextFactory.Context.addCookieFromResponseToRequest("Session");            
            var sessionId_AfterLogin  = tmWebServices.Current_SessionID();
                        
            Assert.AreEqual    (sessionId_BeforeLogin, Guid.Empty          , "sessionId should be empty");
            Assert.AreNotEqual (sessionId_AfterLogin , Guid.Empty          , "sessionId should Not empty");
            Assert.AreNotEqual (login_SessionId      , Guid.Empty          , "login_SessionId  should not be empty");
            Assert.AreEqual    (sessionId_AfterLogin, sessionId_AfterLogin , "sessionsIds should be the same");
        }
    }
}
