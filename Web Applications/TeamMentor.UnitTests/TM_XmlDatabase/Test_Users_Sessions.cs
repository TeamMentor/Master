using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
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

        [Test]
        public void UserSession_Object()
        { 
            userData.resetAllSessions();
            //test empty UserSession Object
            var userSession0 = new UserSession();
            Assert.AreEqual(userSession0.SessionID, Guid.Empty);
            Assert.AreEqual(userSession0.CreationDate, default(DateTime));
            Assert.IsNull  (userSession0.IpAddress);

            // temp user object
            var tmUser = new TMUser();

            //test expected values for 1st session
            var userSession1 = tmUser.add_NewSession();            

            Assert.IsNotNull(userSession1, "New UserSession was null");
            Assert.AreNotEqual(userSession1.SessionID, Guid.Empty);
            Assert.AreNotEqual(userSession1.CreationDate, default(DateTime));
            Assert.IsNotNull  (userSession1.IpAddress);
            Assert.AreEqual   (tmUser.Sessions.size(), 1, "There should only be one sessions here");
            Assert.AreEqual   (tmUser.Sessions.first(), userSession1);

            //test expected values for 2nd session
            var userSession2 = tmUser.add_NewSession();

            Assert.IsNotNull  (userSession2, "New UserSession was null");
            Assert.AreNotEqual(userSession2.SessionID, Guid.Empty);
            Assert.AreNotEqual(userSession2.CreationDate, default(DateTime));
            Assert.IsNotNull  (userSession2.IpAddress);
            Assert.AreEqual   (tmUser.Sessions.size(), 2, "There should only be two sessions here");
            Assert.AreEqual   (tmUser.Sessions.second(), userSession2);
            Assert.AreNotEqual(userSession1, userSession2);
            
            Assert.AreEqual   (userData.validSessions().size(),0); // there should be no sessions here in global sessions handler since the user was created manually
        }

        [Test] public void ResetSessions()
        {
            userData.resetAllSessions();
            Assert.AreEqual(userData.validSessions().size(), 0);

            var tmUser        = userData.newUser().tmUser();                        
            
            tmUser.add_NewSession();

            Assert.AreEqual(userData.validSessions().size(), 1);

            tmUser.Sessions.Add(new UserSession());                 // this should not add a new session 
            Assert.AreEqual(userData.validSessions().size(), 1);

            userData.resetAllSessions();
            Assert.AreEqual(userData.validSessions().size(), 0);
            
            //Remove session using userSession
            var userSession1 = tmUser.add_NewSession();
            Assert.AreEqual(userData.validSessions().size(), 1);
            Assert.IsTrue(tmUser.remove_Session(userSession1));

            //Remove session using SessionID
            var userSession2 = tmUser.add_NewSession();
            Assert.AreEqual(userData.validSessions().size(), 1);
            Assert.IsTrue(tmUser.remove_Session(userSession2.SessionID));

            //Test remove_Session with null values
            Assert.IsFalse(tmUser.remove_Session(null as UserSession));
            Assert.IsFalse(tmUser.remove_Session(Guid.Empty));
            tmUser = null;
            Assert.IsFalse(tmUser.remove_Session(Guid.Empty));
        }

        [Test] public void MultipleLoginSessions_Two_Users()
        {
            userData.resetAllSessions();
            //Testing two separate users
            var tmUser1 = userData.newUser().tmUser();
            var tmUser2 = userData.newUser().tmUser();

            var sessionId1 = tmUser1.login();           // log both in
            var sessionId2 = tmUser2.login();

            Assert.AreNotEqual(sessionId1, Guid.Empty);
            Assert.AreNotEqual(sessionId2, Guid.Empty);

            var validSessions = userData.validSessions();
            Assert.IsTrue(validSessions.contains(sessionId1));
            Assert.IsTrue(validSessions.contains(sessionId2));
            Assert.IsTrue(sessionId1.validSession());
            Assert.IsTrue(sessionId2.validSession());

            tmUser1.logout();                           // logout first
            validSessions = userData.validSessions();

            Assert.IsFalse(validSessions.contains(sessionId1));
            Assert.IsTrue (validSessions.contains(sessionId2));
            Assert.IsFalse(sessionId1.validSession());
            Assert.IsTrue (sessionId2.validSession());

            tmUser2.logout();                           // logout 2nd
            validSessions = userData.validSessions();

            Assert.IsFalse(validSessions.contains(sessionId1));
            Assert.IsFalse(validSessions.contains(sessionId2));
            Assert.IsFalse(sessionId1.validSession());
            Assert.IsFalse(sessionId2.validSession());

            Assert.IsEmpty(userData.validSessions());
        }

        [Test] public void MultipleLoginSessions_One_User()
        {            
            var tmUser = userData.newUser().tmUser();            

            var sessionId1 = tmUser.login();           // log same user with two sessions
            var sessionId2 = tmUser.login();

            Assert.AreNotEqual(sessionId1, Guid.Empty);
            Assert.AreNotEqual(sessionId2, Guid.Empty);

            Assert.IsTrue(sessionId1.validSession());
            Assert.IsTrue(sessionId2.validSession());

            tmUser.logout();                            // this should logout the user on both sessions

            Assert.IsFalse(sessionId1.validSession());
            Assert.IsFalse(sessionId2.validSession());

            var sessionId3 = tmUser.login();           // log same user again with three sessions
            var sessionId4 = tmUser.login();
            var sessionId5 = tmUser.login();

            Assert.IsTrue(sessionId3.validSession());
            Assert.IsTrue(sessionId4.validSession());
            Assert.IsTrue(sessionId5.validSession());
            
            tmUser.logout(sessionId3);                 // logout 1st
            Assert.IsFalse(sessionId3.validSession());
            Assert.IsTrue (sessionId4.validSession());
            Assert.IsTrue (sessionId5.validSession());

            tmUser.logout(sessionId4);                  // logout 2nd
            Assert.IsFalse(sessionId3.validSession());
            Assert.IsFalse(sessionId4.validSession());
            Assert.IsTrue(sessionId5.validSession());

            tmUser.logout(sessionId5);                  // logout 2nd
            Assert.IsFalse(sessionId3.validSession());
            Assert.IsFalse(sessionId4.validSession());
            Assert.IsFalse(sessionId5.validSession());

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

            tmUser.login();
            
            Assert.AreEqual    (3, stats.LoginOk);
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
            Assert.AreEqual(15  , TMConfig.Current.TMSecurity.EvalAccounts_Days         , "Eval_Accounts.Days");
            Assert.AreEqual(true, TMConfig.Current.TMSecurity.EvalAccounts_Enabled      , "Eval_Accounts.Enabled");

            var tmUser = userData.newUser().tmUser();

            var expirationDate              = tmUser.AccountStatus.ExpirationDate                        .ToLongDateString();
            var expectedExpirationDate      = DateTime.Now.AddDays(tmConfig.TMSecurity.EvalAccounts_Days).ToLongDateString();
            var calculatedExpirationDate    = tmConfig.currentExpirationDate()                           .ToLongDateString(); 

            var isExpired_Before_Expire_Account      = tmUser.account_Expired();
            tmUser.expire_Account();
            var isExpired_After_Expire_Account       = tmUser.account_Expired();
            tmConfig.TMSecurity.EvalAccounts_Enabled = false;
            var isExpired_EvalAccounts_Disable       = tmUser.account_Expired();

            Assert.IsFalse(isExpired_Before_Expire_Account          , "before expire, user should Not be expired");
            Assert.IsTrue (isExpired_After_Expire_Account           , "after expired user should be expired");
            Assert.IsTrue (isExpired_EvalAccounts_Disable           , "user account should still be disabled (regardless of the EvalAccounts_Enabled value)");
            Assert.AreEqual(expirationDate, expectedExpirationDate  , "expirationDate != expectedExpirationDate");
            Assert.AreEqual(expirationDate, calculatedExpirationDate, "expirationDate != calculatedExpirationDate");
        }

        [Test] public void UserAccount_EvalAccounts_Behaviour()
        {
            tmConfig.TMSecurity.EvalAccounts_Enabled  = true;
            var tmUser1                               = userData.newUser().tmUser();
            var expirationDate_NewUser_Eval_Enabled   = tmUser1.AccountStatus.ExpirationDate;
            tmConfig.TMSecurity.EvalAccounts_Enabled  = false;
            var tmUser2                               = userData.newUser().tmUser();
            var expirationDate_NewUser_Eval_Disabled  = tmUser2.AccountStatus.ExpirationDate;

            Assert.IsNotNull(tmUser2, "tmUser1");
            Assert.IsNotNull(tmUser2, "tmUser2");
            Assert.AreNotEqual(expirationDate_NewUser_Eval_Enabled , default(DateTime) , "ExpirationDate should be set when EvalAccounts_Enabled is true");
            Assert.AreEqual   (expirationDate_NewUser_Eval_Disabled, default(DateTime) , "ExpirationDate should NOT be set when EvalAccounts_Enabled is false");

            "tmUser1: {0}".info(tmUser1.toXml());
            "tmUser2: {0}".info(tmUser2.toXml());
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
