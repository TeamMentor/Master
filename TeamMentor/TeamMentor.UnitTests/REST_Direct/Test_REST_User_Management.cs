using System;
using System.Security;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
	[TestFixture]
	public class Test_REST_User_Management : TM_Rest_Direct
	{
		[Test] public void Login()
		{
			var tmConfig = TMConfig.Current;
			var credentials = new TM_Credentials { UserName = tmConfig.TMSecurity.Default_AdminUserName, Password = tmConfig.TMSecurity.Default_AdminPassword};
			//login with default value
			var sessionId = TmRest.Login_using_Credentials(credentials);

			Assert.AreNotEqual(sessionId, Guid.Empty);            
			//login with a bad password
			credentials.Password = "AAAA";
			sessionId = TmRest.Login_using_Credentials(credentials);
			Assert.AreEqual(sessionId, Guid.Empty);
		}       
	}
}
