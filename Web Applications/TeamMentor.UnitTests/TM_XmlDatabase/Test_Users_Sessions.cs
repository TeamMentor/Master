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
            Assert.AreEqual    (0, stats.LoginFail, "Login fail should be 0");

            tmUser.login(Guid.Empty);
            
            Assert.AreEqual    (2, stats.LoginOk);
            Assert.AreEqual    (0, stats.LoginFail, "LoginFail should still be 0");

            //test as Admin      
            var adminName        = tmConfig.TMSecurity.Default_AdminUserName;
            var adminPwd         = tmConfig.TMSecurity.Default_AdminPassword;
            var sessionId        =  userData.login(adminName, adminPwd);  // login using user and good pwd
            tmUser               = sessionId.session_TmUser();
            stats                = tmUser.Stats;
            var currentLoginOk   = stats.LoginOk;
            var currentLoginFail = stats.LoginFail;
            sessionId            =  userData.login(adminName, adminPwd);      // login again using user and good pwd

            Assert.AreNotEqual (Guid.Empty, sessionId, "failed to login as default admin user");
            Assert.AreNotEqual (default(DateTime), stats.LastLogin);
            Assert.AreEqual    (currentLoginOk+1, stats.LoginOk   , "[admin] stats.LoginOk ");
            Assert.AreEqual    (currentLoginFail, stats.LoginFail);

            userData.login(tmUser.UserName, "".add_RandomLetters(10));                                      // login using user and bad pwd

            Assert.AreEqual    (currentLoginOk  + 1, stats.LoginOk);
            Assert.AreEqual    (currentLoginFail +1, stats.LoginFail , "[admin] stats.LoginFail should be higher");
        }
        [Test] public void UserAccount_Enabled_and_Disabled()
        {            
            var testUser  = "user".add_RandomLetters(10);                       
            var testPwd   = "!!Pwd".add_RandomLetters(10);
            var tmUser    = userData.newUser(testUser, testPwd).tmUser();                           //create test user     
            var sessionId = userData.login(testUser, testPwd);

            Assert.IsTrue     (tmUser.account_Enabled(), "New user account should be enabled");
            Assert.AreNotEqual(Guid.Empty, sessionId   , "login should work");

            tmUser.disable_Account();                                                               // disable account
            sessionId = userData.login(testUser, testPwd);

            Assert.IsFalse    (tmUser.account_Enabled(), "Account should be disabled now");
            Assert.AreEqual   (Guid.Empty, sessionId   , "After disable, login should not work");

            tmUser.enable_Account();                                                               // re-enable account
            sessionId = userData.login(testUser, testPwd);

            Assert.IsTrue     (tmUser.account_Enabled(), "Account should be enabled now");
            Assert.AreNotEqual(Guid.Empty, sessionId   , "After enable, login should not work");
        }
        [Test] public void UserAccount_Expired()
        {
            tmConfig.TMSecurity.EvalAccounts_Enabled  = true;
            tmConfig.TMSecurity.EvalAccounts_Days     = 15;
            Assert.AreEqual(15  , TMConfig.Current.TMSecurity.EvalAccounts_Days    , "Eval_Accounts.Days");
            Assert.AreEqual(true, TMConfig.Current.TMSecurity.EvalAccounts_Enabled , "Eval_Accounts.Enabled");

            var tmUser = userData.newUser().tmUser();
            
            Assert.IsFalse (tmUser.account_Expired(), "New user should not be expired");

            Assert.AreEqual(tmUser.AccountStatus.ExpirationDate                        .ToLongDateString(), 
                            DateTime.Now.AddDays(tmConfig.TMSecurity.EvalAccounts_Days).ToLongDateString());

            tmUser.expire_Account();    

            Assert.IsTrue (tmUser.account_Expired(), "user should be expired");

            tmConfig.TMSecurity.EvalAccounts_Enabled = false;
            Assert.IsFalse (tmUser.account_Expired() ,"expiration check should be disabled");
        }
        [Test] public void UserPassword_Expired()
        {
            var tmUser        = userData.newUser().tmUser();

            Assert.IsFalse    (tmUser.password_Expired());

            tmUser            .expire_Password();

            Assert.IsTrue     (tmUser.password_Expired());

            var newPassword   = "QAsdf1234!";
            tmUser            .setPassword(newPassword);    
  
            Assert.IsFalse    (tmUser.password_Expired(), "Password expiry should not be set after password change");
            Assert.AreNotEqual(Guid.Empty               , userData.login(tmUser.UserName, newPassword));
        }        
        [Test, Ignore("under dev")]
        public void PasswordComplexity()
        {
            
        }
    }
}
