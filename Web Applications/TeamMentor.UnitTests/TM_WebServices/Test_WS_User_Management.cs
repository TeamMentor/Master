using System;
using NUnit.Framework;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    [TestFixture]
    public class Test_WS_User_Management : TM_XmlDatabase_InMemory
    {
        //public TM_Xml_Database tmXmlDatabase;
        public TM_WebServices  tmWebServices;
        public TMConfig        tmConfig;

        public Test_WS_User_Management()
        {
            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
            //tmXmlDatabase    = new TM_Xml_Database();	 
            tmConfig = TMConfig.Current;
            tmWebServices = new TM_WebServices();
        }

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
