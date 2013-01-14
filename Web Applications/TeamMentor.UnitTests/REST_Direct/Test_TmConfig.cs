using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST_Direct
{
	[TestClass]
	public class Admin_REST_Class : RestClass_Direct
	{
		[TestMethod]
		public void Test_LoadTmConfig()
		{
			var tmConfig = TMConfig.Current;
			Assert.IsNotNull(tmConfig);
		}

		[TestMethod]
		public void Test_DefaultSettings()
		{
			var tmConfig = TMConfig.Current;
			Assert.IsFalse(tmConfig.ShowContentToAnonymousUsers, "ShowContentToAnonymousUsers");
			Assert.IsFalse(tmConfig.WindowsAuthentication.Enabled, "tmConfig.WindowsAuthentication.Enabled");
		}
			
		[TestMethod]
		public void Test_CrossDomainAccess()
		{
			var response = HttpContextFactory.Response;
			var headers = response.Headers;			
			Assert.AreEqual(0, headers.size());
			ResponseHeaders.addDefaultResponseHeaders();
			Assert.AreEqual(2, headers.size()	, "two headers expected");

			TMConfig.Current.REST.AllowCrossDomainAccess = true;
			ResponseHeaders.addDefaultResponseHeaders();
			Assert.AreEqual(5, headers.size()	, "five headers expected");
			Assert.AreEqual("Access-Control-Allow-Origin", headers.Keys[2]);
			Assert.AreEqual("*", headers[2]);
			Assert.AreEqual("*", headers["Access-Control-Allow-Origin"]);
		}
	}
}
