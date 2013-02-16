using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.UnitTests.REST

{	
    [TestFixture,Ignore]
    public class Test_ConfigAndSetup : TM_Rest_Hosted
    {
        [SetUp]
        public static void Initialize()
        {
            WCFHost_Start();
        }

        [Test]
        public void CheckWebServiceHost()
        {
            var html = TmRestHost.BaseAddress.append("/Version").getHtml();
            Assert.IsTrue(html.valid(), "Html fetch failed");
            //test version
            var version = IrestAdmin.Version();
            Assert.NotNull(version,"Version was null");
            "version (hosted access): {0}".info(version);
            //test sessionID
            var sessionId = IrestAdmin.SessionId();
            Assert.NotNull(sessionId, "sessionID was null");
            "sessionID (hosted access): {0}".info(sessionId);
        }
        
        [TearDown]
        public static void Cleanup()
        {
            WCFHost_Stop();
        }
        
    }
}