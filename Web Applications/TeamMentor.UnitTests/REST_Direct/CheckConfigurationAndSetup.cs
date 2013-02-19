using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
	[TestFixture]
	public class CheckConfigurationAndSetup 
	{
		public static ITM_REST RestAdmin { get; set; }

		[SetUp]
		public static void Setup()
		{			
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			//TMConfig.Current.UseAppDataFolder = true;
			RestAdmin = new TM_REST();
		}

		[Test]
		public void Test_Version()
		{
			var version = RestAdmin.Version();
			Assert.IsNotNull(version, "Version was null");
			"version (direct access): {0}".info(version);
		}
		[Test]
		public void Test_SessionID()
		{
			var sessionId = RestAdmin.SessionId();
			Assert.IsNotNull(sessionId, "SessionId was null");
			"sessionId (direct access): {0}".info(sessionId);
		}

		[TearDown]
		public static void TearDown() { }
	}
}
