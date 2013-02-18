using System;
using NUnit.Framework;
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
            var logoutResult             = tmUser.logout(sessionId);
            var validSessionAfterLogout  = sessionId.validSession();
            var logoutResult2            = tmUser.logout(sessionId);
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
            var tmUser = userData.newUser().tmUser();
            var stats  = tmUser.Stats;

            Assert.AreEqual    (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (0, stats.LoginOk);
            Assert.AreEqual    (0, stats.LoginFail);

            tmUser.login();

            Assert.AreNotEqual (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (1, stats.LoginOk);
            Assert.AreEqual    (0, stats.LoginFail);
        }
    }
}
