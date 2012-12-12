using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.WebClient;
using TeamMentor.CoreLib.WebServices;

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
			Admin_REST_Config.setCrossDomainAccess();
			Assert.AreEqual(0, headers.size()	, "no headers expected");

			TMConfig.Current.REST.AllowCrossDomainAccess = true;
			Admin_REST_Config.setCrossDomainAccess();
			Assert.AreEqual(1, headers.size()	, "header expected");
			Assert.AreEqual("Access-Control-Allow-Origin", headers.Keys[0]);
			Assert.AreEqual("*", headers[0]);
			Assert.AreEqual("*", headers["Access-Control-Allow-Origin"]);
		}
	}
}
