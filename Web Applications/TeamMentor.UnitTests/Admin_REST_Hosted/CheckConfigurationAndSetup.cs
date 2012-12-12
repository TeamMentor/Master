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
		public void CheckVersion()
		{
			var version = RestAdmin.Version();
			version.assert_IsNotNull("Version was null");
			"version (direct access): {0}".writeLine_Trace(version);
		}

		[ClassCleanup]
		public static void Cleanup() { }
	}
}
