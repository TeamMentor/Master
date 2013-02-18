using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture][Assert_Admin]
    public class Test_Users_Sessions : TM_XmlDatabase_InMemory
    {
        [Test] public void TMUserLoginAndLogout()
        {
            var userId                   = userData.newUser();
            var tmUser                   = userId.tmUser();            
            var sessionId                = tmUser.login();
            var validSessionAfterLogin   = sessionId.validSession();
            var logoutResult             = tmUser.logout();
            var validSessionAfterLogout  = sessionId.validSession();
            var logoutResult2            = tmUser.logout();
            var validSessionAfterLogout2 = sessionId.validSession();

            Assert.Less        (0,userId);
            Assert.IsNotNull   (tmUser);
            Assert.AreNotEqual (Guid.Empty, sessionId);            
            Assert.IsTrue      (logoutResult);
            Assert.IsTrue      (validSessionAfterLogin);
            Assert.IsFalse     (validSessionAfterLogout);            
            Assert.IsFalse     (logoutResult2);
            Assert.IsFalse     (validSessionAfterLogout2);
        }
        [Test] public void GuidLoginAndLogout()
        {
            var sessionId = userData.newUser()
                                    .tmUser()
                                    .login();
            var validSessionAfterLogin  = sessionId.validSession();
            var logoutResult            = sessionId.logout();
            var validSessionAfterLogout = sessionId.validSession();

            Assert.IsTrue      (logoutResult);
            Assert.IsTrue      (validSessionAfterLogin);
            Assert.IsFalse     (validSessionAfterLogout);
        }
        [Test] public void UserStats()
        {
            //test as User
            var tmUser = userData.newUser().tmUser();
            var stats  = tmUser.Stats;

            Assert.AreEqual    (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (0, stats.LoginOk);
            Assert.AreEqual    (0, stats.LoginFail);

            tmUser.login();    //login once

            Assert.AreNotEqual (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (1, stats.LoginOk , "[user] stats.LoginOk should be 1");
            Assert.AreEqual    (0, stats.LoginFail);

            tmUser.login();    // login again

            Assert.AreNotEqual (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (2, stats.LoginOk , "[user] stats.LoginOk should be 2");
            Assert.AreEqual    (0, stats.LoginFail);

            tmUser.logout();
            
            Assert.AreEqual    (2, stats.LoginOk);
            Assert.AreEqual    (0, stats.LoginFail);

            tmUser.login(Guid.Empty);
            
            Assert.AreEqual    (2, stats.LoginOk);
            Assert.AreEqual    (1, stats.LoginFail);

            //test as Admin      
            
            var sessionId =  userData.login(tmConfig.DefaultAdminUserName, tmConfig.DefaultAdminPassword);  // login using user and good pwd
            tmUser = sessionId.session_TmUser();
            stats  = tmUser.Stats;
            var currentLoginOk   = stats.LoginOk;
            var currentLoginFail = stats.LoginFail;
            sessionId =  userData.login(tmConfig.DefaultAdminUserName, tmConfig.DefaultAdminPassword);      // login again using user and good pwd
            Assert.AreNotEqual (Guid.Empty, sessionId, "failed to login as default admin user");
            Assert.AreNotEqual (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (currentLoginOk+1, stats.LoginOk   , "[admin] stats.LoginOk ");
            Assert.AreEqual    (currentLoginFail, stats.LoginFail);

            userData.login(tmUser.UserName, "".add_RandomLetters(10));                                      // login using user and bad pwd

            Assert.AreEqual    (currentLoginOk  + 1, stats.LoginOk);
            Assert.AreEqual    (currentLoginFail +1, stats.LoginFail , "[admin] stats.LoginFail should");
        }

        [Test] public void UserAccount_Expired()
        {
            var tmUser = userData.newUser().tmUser();
            
            Assert.IsFalse(tmUser.account_Expired());

        }        
    }
}
