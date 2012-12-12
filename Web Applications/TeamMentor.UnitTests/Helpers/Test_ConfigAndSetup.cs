using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.XRules.Database.APIs;
using TeamMentor.CoreLib.WebServices;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{
	//TO DO
	//[TestClass]
	public class Test_ConfigAndSetup
	{
		public static Admin_REST_Host AdminRestHost { get; set; }
		public static IREST_Admin IrestAdmin { get; set; }

		
		public static void Initialize(TestContext context)
		{
			"Starting Host".writeLine_Trace();
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			AdminRestHost = new Admin_REST_Host().StartHost();
			IrestAdmin = AdminRestHost.GetProxy();
		}

		//[TestMethod]
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

		[ClassCleanup]
		public static void Cleanup()
		{
			"Stopping Host".writeLine_Trace();
			AdminRestHost.StoptHost();
		}
	}
}