using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.XRules.Database.APIs;
using TeamMentor.CoreLib.WebServices;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{	
	[TestClass,Ignore]
	public class Test_ConfigAndSetup : RestClass_Hosted
	{
		[TestMethod]
		public void CheckWebServiceHost()
		{
			var html = AdminRestHost.BaseAddress.append("/Version").getHtml();
			html.valid().assert_True("Html fetch failed");
			//test version
			var version = IrestAdmin.Version();
			version.assert_IsNotNull("Version was null");
			"version (hosted access): {0}".writeLine_Trace(version);
			//test sessionID
			var sessionId = IrestAdmin.SessionId();
			sessionId.assert_IsNotNull("sessionID was null");
			"sessionID (hosted access): {0}".writeLine_Trace(sessionId);
		}

		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
			WCFHost_Start(context);
		}
		[ClassCleanup]
		public static void Cleanup()
		{
			WCFHost_Stop();
		}
		
	}
}