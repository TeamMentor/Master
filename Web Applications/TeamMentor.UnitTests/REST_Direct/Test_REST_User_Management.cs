using System;
using System.Security;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
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
        [Test] public void SingleUseLoginToken()
        {
            var userName    = "admin";
            Assert.Throws<SecurityException>(() => TmRest.GetLoginToken(userName));

            UserGroup.Admin .setThreadPrincipalWithRoles();
            var loginToken  = TmRest.GetLoginToken(userName);

            var sessionId_GoodToken_WrongUser = TmRest.Login_Using_LoginToken(userName +"A", loginToken.str());
            var sessionId_GoodToken_GoodUser  = TmRest.Login_Using_LoginToken(userName     , loginToken.str());
            var sessionId_GoodToken_Again     = TmRest.Login_Using_LoginToken(userName     , loginToken.str());
            var sessionId_EmptyToken          = TmRest.Login_Using_LoginToken(userName     , Guid.Empty.str());
            var sessionId_Null                = TmRest.Login_Using_LoginToken(userName     , null);
            var sessionId_RandomToken         = TmRest.Login_Using_LoginToken(userName     , Guid.NewGuid().str());

            
            Assert.AreNotEqual(Guid.Empty , loginToken                      , "loginToken");
            Assert.AreEqual   (Guid.Empty , sessionId_GoodToken_WrongUser   , "sessionId_GoodToken_WrongUser");
            Assert.AreNotEqual(Guid.Empty , sessionId_GoodToken_GoodUser    , "sessionId_GoodToken_GoodUser");
            Assert.AreEqual   (Guid.Empty , sessionId_GoodToken_Again       , "sessionId_GoodToken_Again");
            Assert.AreEqual   (Guid.Empty , sessionId_EmptyToken            , "sessionId_EmptyToken");
            Assert.AreEqual   (Guid.Empty , sessionId_Null                  , "sessionId_Null");
            Assert.AreEqual   (Guid.Empty , sessionId_RandomToken           , "sessionId_RandomToken");
        }

/*		[Test,Ignore("Was failing in TeamCity")]
		public void UserActivities()
		{
			var expectedHtml = "<h1>UserActivites</h1>Waiting 10 times for user activity events<hr>";
			
			O2Thread.mtaThread(()=>TmRest.users_Activities());
			50.wait();
			var responseHtml = moq_HttpContext.HttpContextBase.response_Read_All();

			Assert.AreEqual(expectedHtml, responseHtml);
			//need better way to do the test bellow so that it doesn't clash with the other tests running in parallel
			/ *
			"Test".logUserActivity("User Activity");
			600.wait();
			responseHtml = moq_HttpContext.HttpContextBase.response_Read_All();
			Assert.AreNotEqual(expectedHtml, responseHtml);* /
		}
*/
	}
}
