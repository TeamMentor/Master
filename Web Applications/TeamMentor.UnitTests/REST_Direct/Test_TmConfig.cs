using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
	[TestFixture]
	public class Admin_REST_Class : TM_Rest_Direct
	{
		[Test]
		public void Test_LoadTmConfig()
		{
			var tmConfig = TMConfig.Current;
			Assert.IsNotNull(tmConfig);
		}

		[Test]
		public void Test_DefaultSettings()
		{            
			var tmConfig = TMConfig.Current;
			Assert.IsFalse(tmConfig.TMSecurity.Show_ContentToAnonymousUsers, "ShowContentToAnonymousUsers");
			Assert.IsFalse(tmConfig.WindowsAuthentication.Enabled          , "tmConfig.WindowsAuthentication.Enabled");
		}
			
		[Test]
		public void Test_CrossDomainAccess()
		{
			var response = HttpContextFactory.Response;
			var headers = response.Headers;			
			Assert.AreEqual(0, headers.size());
			ResponseHeaders.addDefaultResponseHeaders();
			Assert.AreEqual(3, headers.size()	, "two headers expected");

			TMConfig.Current.TMSecurity.REST_AllowCrossDomainAccess = true;
			ResponseHeaders.addDefaultResponseHeaders();
			Assert.AreEqual(6, headers.size()	, "five headers expected");
			Assert.AreEqual("Access-Control-Allow-Origin", headers.Keys[3]);
			Assert.AreEqual("*", headers[3]);
			Assert.AreEqual("*", headers["Access-Control-Allow-Origin"]);
		}
	}
}
