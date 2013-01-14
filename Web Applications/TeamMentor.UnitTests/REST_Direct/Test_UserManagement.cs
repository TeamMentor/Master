using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST_Direct
{
	[TestClass]
	public class Test_UserManagement : RestClass_Direct
	{
		[TestMethod]
		public void Test_Login()
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
	}
}
