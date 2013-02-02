using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
	[TestClass]
	public class CheckConfigurationAndSetup 
	{
		public static ITM_REST RestAdmin { get; set; }

		[ClassInitialize]
		public static void Initialize(TestContext context)
		{			
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			TMConfig.Current.UseAppDataFolder = true;
			RestAdmin = new TM_REST();
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
