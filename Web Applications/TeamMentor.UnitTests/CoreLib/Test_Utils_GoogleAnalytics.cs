using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
	[TestFixture]
	public class Test_Utils_GoogleAnalytics
	{
		public GoogleAnalytics	googleAnalytics;
		public int				logCount_AtStart;

		[SetUp]
		public void setup()
		{
			googleAnalytics		= GoogleAnalytics.Current;
			logCount_AtStart	= GoogleAnalytics.Current.LogCount;
			
			//TM CI account
			googleAnalytics.AccountID			= "UA-37594728-3";
			googleAnalytics.Enabled				= true;
			googleAnalytics.LogWebServicesCalls = true;
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
		public void LogTestEntry_Directly()
		{
			googleAnalytics.SetUserCookie(12345);
			// at the moment there is no way to verify that the next command will actually work (see note on LogEntry method)
			googleAnalytics.LogEntry("Test_Utils_GoogleAnalytics", "LogTestEntry");			
		}

		[Test, Ignore("Not working")]
		public void LogTestEntry_ViaAttribue()
		{			
			new GA_PostSharp_Test().Test_LogTo_GoogleAnalytics();		    
			Assert.AreEqual(logCount_AtStart + 1, GoogleAnalytics.Current.LogCount);
		}

		[Test]
		public void LogUserActivity()
		{
			new TMUser().logUserActivity("UnitTest","LogUserActivity");
			Assert.AreEqual(logCount_AtStart + 1, GoogleAnalytics.Current.LogCount);
		}

	}

	public class GA_PostSharp_Test
	{
		[LogTo_GoogleAnalytics]
		public void Test_LogTo_GoogleAnalytics()
		{			
		}
	}
}
