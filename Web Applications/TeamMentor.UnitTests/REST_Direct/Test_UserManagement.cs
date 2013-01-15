using System;
using System.IO;
using NUnit.Framework;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST_Direct
{
	[TestFixture]
	public class Test_UserManagement : RestClass_Direct
	{
		[Test]
		public void Login()
		{
			var tmConfig = TMConfig.Current;
			var credentials = new TM_Credentials() { UserName = tmConfig.DefaultAdminUserName, Password = tmConfig.DefaultAdminPassword};
			//login with default value
			var sessionId = IRESTAdmin.login(credentials);

			Assert.AreNotEqual(sessionId, Guid.Empty);
			//login with a bad password
			credentials.Password = "AAAA";
			sessionId = IRESTAdmin.login(credentials);
			Assert.AreEqual(sessionId, Guid.Empty);
		}


		[Test,Ignore("Was failing in TeamCity")]
		public void UserActivities()
		{
			var expectedHtml = "<h1>UserActivites</h1>Waiting 10 times for user activity events<hr>";
			
			O2Thread.mtaThread(()=>IRESTAdmin.users_Activities());
			50.wait();
			var responseHtml = moq_HttpContext.HttpContextBase.response_Read_All();

			Assert.AreEqual(expectedHtml, responseHtml);
			//need better way to do the test bellow so that it doesn't clash with the other tests running in parallel
			/*
			"Test".logActivity("User Activity");
			600.wait();
			responseHtml = moq_HttpContext.HttpContextBase.response_Read_All();
			Assert.AreNotEqual(expectedHtml, responseHtml);*/
		}

	}
}
