using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityInnovation.TeamMentor.WebClient;

namespace TeamMentor.UnitTests.REST_Direct
{
	[TestClass]
	public class Test_UserManagement : RestClass_Direct
	{
		[TestMethod]
		public void Test_Login()
		{
			var tmConfig = TMConfig.Current;
			//login with default value
			var sessionId = IRESTAdmin.Login(tmConfig.DefaultAdminUserName, tmConfig.DefaultAdminPassword);
			Assert.AreNotEqual(sessionId, Guid.Empty);
			//login with a bad password
			sessionId = IRESTAdmin.Login(tmConfig.DefaultAdminUserName, "AAAAAAA");
			Assert.AreEqual(sessionId, Guid.Empty);
		}
	}
}
