using System;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    public class Test_WebServices_User_Management : TM_WebServices_InMemory
    {        
        [Test] public void Login()
        {
            tmWebServices.Logout();
            var sessionId_BeforeLogin = tmWebServices.Current_SessionID();
            var login_SessionId       = tmWebServices.Login       (tmConfig.TMSecurity.Default_AdminUserName, tmConfig.TMSecurity.Default_AdminPassword);
            HttpContextFactory.Context.addCookieFromResponseToRequest("Session");            
            var sessionId_AfterLogin  = tmWebServices.Current_SessionID();
                        
            Assert.AreEqual    (sessionId_BeforeLogin, Guid.Empty          , "sessionId should be empty");
            Assert.AreNotEqual (sessionId_AfterLogin , Guid.Empty          , "sessionId should Not empty");
            Assert.AreNotEqual (login_SessionId      , Guid.Empty          , "login_SessionId  should not be empty");
            Assert.AreEqual    (sessionId_AfterLogin, sessionId_AfterLogin , "sessionsIds should be the same");
        }
        [Test] public void CheckThatCurrentUserUserMatchesNewUser()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);
            var tmUser      = userId.tmUser();

            Assert.IsTrue   (newUser.validation_Ok(), "NewUser object failed validation");
            Assert.Greater  (userId, 0);
            Assert.NotNull  (tmUser);
            Assert.AreEqual (userId, tmUser.UserID);

            var sessionId   = tmWebServices.Login(newUser.Username, newUser.Password);
            HttpContextFactory.Context     .addCookieFromResponseToRequest("Session");
            var currentUser = tmWebServices.Current_User();
            
            Assert.AreNotEqual(Guid.Empty,sessionId);                
            Assert.NotNull    (currentUser , "Current User Not Set");
            Assert.AreEqual   (currentUser.Company      , newUser.Company);
            Assert.AreEqual   (DateTime.FromFileTimeUtc(currentUser.CreatedDate).ToLongDateString()  ,
                               DateTime.Now                                     .ToLongDateString());
            Assert.AreEqual   (currentUser.Email        , newUser.Email);
            Assert.AreEqual   (currentUser.FirstName    , newUser.Firstname);
            Assert.AreEqual   (currentUser.LastName     , newUser.Lastname);
            Assert.AreEqual   (currentUser.Title        , newUser.Title);
            Assert.AreEqual   (currentUser.UserId       , userId);
            Assert.AreEqual   (currentUser.UserName     , newUser.Username);            
        }
        [Test] public void ChangeCurrentUserPassword()        
        {                     
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);

            var originalPassword = newUser.Password;
            var newPassword      = "Abcmfl!@#";
            
            var sessionId_OriginalPassword    = tmWebServices.Login(newUser.Username, originalPassword);
            HttpContextFactory.Context       .addCookieFromResponseToRequest("Session");
            var currentUser_OriginalPassword  = tmWebServices.Current_User();
            var changePasswordResult          = tmWebServices.SetCurrentUserPassword(originalPassword,newPassword);
            var sessionId_NewPassword         = tmWebServices.Login(newUser.Username, newPassword);
            var currentUser_NewPassword       = tmWebServices.Current_User();
            var sessionId_OriginalPassword2   = tmWebServices.Login(newUser.Username, originalPassword);            
            var currentUser_OriginalPassword2 = tmWebServices.Current_User();

            Assert.Greater    (userId, 0);
            Assert.AreNotEqual(Guid.Empty,sessionId_OriginalPassword  , "Login with original password");   
            Assert.NotNull    (currentUser_OriginalPassword           , "Current User Not Set (original password)");
            Assert.IsTrue     (changePasswordResult                   , "Change password result");
            Assert.AreNotEqual(Guid.Empty,sessionId_NewPassword       , "Login with new password");   
            Assert.NotNull    (currentUser_NewPassword                , "Current User Not Set (new password)");
            Assert.AreEqual   (Guid.Empty,sessionId_OriginalPassword2 , "Login with original password after change");   
            Assert.IsNull     (currentUser_OriginalPassword2          , "Current User Not Set (original password after change)");
        }
        [Test] public void NewPasswordMustBeDifferentFromOlderPassword()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);
            var sessionId   = tmWebServices.Login(newUser.Username, newUser.Password );
            var changePasswordResult  = tmWebServices.SetCurrentUserPassword(newUser.Password , newUser.Password );

            Assert.Greater    (userId, 0);     
            Assert.AreNotEqual(Guid.Empty,sessionId  , "Login with original password");   
            Assert.IsFalse    (changePasswordResult  , "Change password should be false if passwords are the same");
        }
        [Test] public void RequireOldPasswordBeforeChangingIntoNewOne()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);

            var originalPassword = newUser.Password;
            var newPassword      = "Abcmfl!@#";
            
            var sessionId_OriginalPassword           = tmWebServices.Login(newUser.Username, originalPassword);
            var changePasswordResult_NoOriginalPwd   = tmWebServices.SetCurrentUserPassword(newPassword     , newPassword);
            var changePasswordResult_WithOriginalPwd = tmWebServices.SetCurrentUserPassword(originalPassword, newPassword);

            Assert.Greater    (userId, 0);
            Assert.AreNotEqual(Guid.Empty,sessionId_OriginalPassword  , "Login with original password");   
            Assert.IsFalse    (changePasswordResult_NoOriginalPwd     , "Change password result (no original password");
            Assert.IsTrue     (changePasswordResult_WithOriginalPwd   , "Change password result (with original password");
        }
        [Test] public void CheckCurrentUserCSRFToken()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);
            var sessionId   = tmWebServices.Login(newUser.Username, newUser.Password);
            HttpContextFactory.Context     .addCookieFromResponseToRequest("Session");
            var currentUser = tmWebServices.Current_User();
            
            Assert.Greater  (userId, 0);
            Assert.IsNotNull(currentUser.CSRF_Token                                 , "CSRF_Token was not set");
            Assert.AreEqual (sessionId.str().hash().str(), currentUser.CSRF_Token   , "CSRF_Token didn't match");
            
        }  
        [Test] public void PasswordResetToken()
        {            
            var newUser       = newTempUser();
            var newPassword   = "123SAFsi!";
            var oldPassword   = newUser.Password;
            var tmUser        = tmWebServices.CreateUser(newUser).tmUser();

            var hash_BeforeSet      = tmUser.SecretData.PasswordResetToken;            
            var token1              = tmUser.passwordResetToken_getTokenAndSetHash();                        
            var token2              = tmUser.passwordResetToken_getTokenAndSetHash();            
            var hash_AfterSet       = tmUser.SecretData.PasswordResetToken;            
            var hash_FromToken2     = tmUser.passwordResetToken_getHash(token2);
            var result              = tmWebServices.PasswordReset(tmUser.UserName, token2,newPassword);
            var hash_AfterUse       = tmUser.SecretData.PasswordResetToken;
            var token3              = tmUser.passwordResetToken_getTokenAndSetHash();
            var hash_AfterUseAndSet = tmUser.SecretData.PasswordResetToken;
            var sessionId_NewPwd    = tmWebServices.Login(tmUser.UserName, newPassword);
            var sessionId_OldPwd    = tmWebServices.Login(tmUser.UserName, oldPassword);            
            var result_EmptyToken   = tmWebServices.PasswordReset(tmUser.UserName, Guid.Empty ,newPassword);
            var result_BadToken     = tmWebServices.PasswordReset(tmUser.UserName, Guid.NewGuid() ,newPassword);

            Assert.IsNull       (hash_BeforeSet                 , "New users should have a empty PasswordResetToken");
            Assert.IsNotNull    (hash_AfterSet                  , "PasswordResetToken should set (until it is used)");
            Assert.AreNotEqual  (Guid.Empty, token1             , "First call to passwordResetToken_getTokenAndSetHash failed");
            Assert.AreNotEqual  (Guid.Empty, token2             , "2nd call to passwordResetToken_getTokenAndSetHash failed");
            Assert.AreNotEqual  (token1, token2                 , "token1 and token 2 should not be equal");
            Assert.AreEqual     (hash_AfterSet,hash_FromToken2  , "PasswordResetToken and 'hash from Guid' didn't match");
            Assert.IsTrue       (result                         , "PasswordReset with token 1 faled");
            Assert.IsNull       (hash_AfterUse                  , "PasswordResetToken should be null after use");
            Assert.IsNotNull    (hash_AfterUseAndSet            , "PasswordResetToken should be set (after reset for the 2nd time)");
            Assert.AreNotEqual  (Guid.Empty, token3             , "token3 was not set");
            Assert.AreNotEqual  (token2, token3                 , "token 2 and 3 should not be equal");
            Assert.AreNotEqual  (Guid.Empty, sessionId_NewPwd   , "sessionId with new password");
            Assert.AreEqual     (Guid.Empty, sessionId_OldPwd   , "sessionId with old password");                       
            Assert.IsFalse      (result_EmptyToken);
            Assert.IsFalse      (result_BadToken);
            
        }
        [Test] public void CreateUser_Validate()
        {
            //try with an empty NewUser object
            var newUser = new NewUser();
            var result = tmWebServices.CreateUser_Validate(newUser);
            Assert.NotNull(result);
            Assert.AreEqual(result.size(), 10);
            Assert.AreEqual(result.first(), "Company:The Company field is required.");            
            Assert.AreEqual(result.second(), "Country:The Country field is required.");            

            //try with an a fully populated NewUser object
            newUser       = newTempUser();
            result = tmWebServices.CreateUser_Validate(newUser);
            Assert.NotNull(result);
            Assert.AreEqual(result.size(), 0);
        }
        
        //Helper methods
        public NewUser newTempUser()
        {
            var password1 = "Sdimfl!@#".add_RandomLetters(10);
            
            var newUser = new NewUser
                {
                    Username    = 10.randomLetters(),
                    Password    = password1,
                    Company     = 10.randomLetters(),
                    Email       = "{0}@{0}.{0}".format(3.randomLetters()),
                    Firstname   = 10.randomLetters(),
                    Lastname    = 10.randomLetters(),
                    Note        = 10.randomLetters(),
                    Title       = 10.randomLetters(),
                    Country     = 10.randomLetters(),
                    State       = 10.randomLetters()
                };
            return newUser;            
        }
    }
}
