using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityInnovation.TeamMentor.WebClient;
using TeamMentor.CoreLib.WebServices;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{
	[TestClass]
	public class CheckConfigurationAndSetup 
	{
		public static REST_Admin RestAdmin { get; set; }

		[ClassInitialize]
		public static void Initialize(TestContext context)
		{			
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			TMConfig.Current.UseAppDataFolder = true;
			RestAdmin = new REST_Admin();
		}

		[TestMethod]
		public void Test_Version()
		{
			var version = RestAdmin.Version();
			Assert.IsNotNull(version, "Version was null");
			"version (direct access): {0}".writeLine_Trace(version);
		}
		[TestMethod]
		public void Test_SessionID()
		{
			var sessionId = RestAdmin.SessionId();
			Assert.IsNotNull(sessionId, "SessionId was null");
			"sessionId (direct access): {0}".writeLine_Trace(sessionId);
		}

		[ClassCleanup]
		public static void Cleanup() { }
	}
}
