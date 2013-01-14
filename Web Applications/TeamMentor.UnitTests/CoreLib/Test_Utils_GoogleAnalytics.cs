using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
	[TestFixture]
	public class Test_Utils_GoogleAnalytics
	{
		public GoogleAnalytics googleAnalytics;

		[SetUp]
		public void setup()
		{
			googleAnalytics = GoogleAnalytics.Current;
		}

		[Test]
		public void CheckSetup()
		{
			var expectedAccountID = "UA-37594728-3";			
			Assert.AreEqual(googleAnalytics.AccountID,expectedAccountID , "Account id");
			googleAnalytics.SetUserCookie(12345);
			Assert.NotNull(googleAnalytics.UserCookie, "UserCookie was null");
		}

		[Test]
		public void LogTestEntry()
		{
			googleAnalytics.SetUserCookie(12345);
			// at the moment there is no way to verify that the next command will actually work (see note on LogEntry method)
			googleAnalytics.LogEntry("Test_Utils_GoogleAnalytics", "LogTestEntry");			
		}

	}
}
