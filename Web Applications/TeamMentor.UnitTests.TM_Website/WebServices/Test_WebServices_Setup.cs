using System;
using NUnit.Framework;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.TM_Website.WebServices
{
    [TestFixture]
    public class Test_WebServices_Setup : API_TM_WebServices
    {
        public Test_WebServices_Setup()
        {            
            if (WebSite_Url.HEAD().isFalse())
                Assert.Ignore("TM server is offline");
            Assert.NotNull(webServices);
        }

        [Test] public void WS_Method_Ping()
        {
            var message  = 10.randomLetters();
            var expected = "received ping: {0}".format(message);
            var response = webServices.Ping(message);
            Assert.AreEqual(expected, response);                    
        }
        [Test] public void WS_Method_GetTime()
        {
            var response = webServices.GetTime();
            var expected = "...Via Proxy:{0}".format(DateTime.Now.str());
            Assert.AreEqual(expected, response);
        }
    }
}
