using System;
using System.Security;
using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_WebServices_Authentication : TestFixture_WebServices
    {

        public Test_WebServices_Authentication()
        {                        
            Assert.NotNull(webServices);                        
        }

        [SetUp]
        public void setup()                         
        {                        
            Assert.AreEqual(webServices.Logout(), Guid.Empty);
        }
        [Test] public void WS_Method_CurrentUSer()  
        {
            Assert.AreEqual(webServices.Current_SessionID(), Guid.Empty);
            Assert.AreEqual(webServices.Current_User()     , null);
        }        
        [Test] public void Check_AuthTokens()       
        {
            var userId = webServices.add_User();
            Assert.Less(-1, userId);

            Assert.NotNull(this.login_As_Admin());
                        
            var authTokens1 = webServices.GetUser_AuthTokens(userId);            
            Assert.IsEmpty(authTokens1);
                        
            var authToken1 = webServices.CreateUser_AuthToken(userId);
            var authToken2 = webServices.CreateUser_AuthToken(userId);            
            var authTokens2 = webServices.GetUser_AuthTokens(userId).toList();
            Assert.AreNotEqual(Guid.Empty, authTokens1);
            Assert.AreNotEqual(Guid.Empty, authTokens2);
            Assert.AreEqual(authTokens2.size(),2);
            Assert.AreEqual(authTokens2.first(),authToken1);
            Assert.AreEqual(authTokens2.second(),authToken2);
            
            //check that user exists and delete it
            var tmUser = webServices.user(userId);
            Assert.AreEqual(tmUser.UserId, userId);
            Assert.IsTrue(webServices.delete_User(userId));
            Assert.IsNull(webServices.user(userId));
        }
        [Test] public void Login_Using_AuthTokens() 
        {
            var testUser                = this.login_As_Admin();
            var userId                  = webServices.add_User();
            var tmUser                  = webServices.user(userId);
            var authToken               = webServices.CreateUser_AuthToken(userId);
            var current_User_Before     = webServices.current_User();
            var before_SessionId        = webServices.current_SessionId();

            webServices.logout();

            var current_User_LoggedOut  = webServices.current_User();            
            var loggedOut_SessionId     = webServices.current_SessionId();
            var authToken_SessionId     = this.webServices.Login_Using_AuthToken(authToken);
            var current_User_After      = webServices.current_User();
            var after_SessionId         = webServices.current_SessionId();

            Assert.NotNull    (testUser);
            Assert.NotNull    (current_User_Before);
            Assert.IsNull     (current_User_LoggedOut);
            Assert.NotNull    (current_User_After);
            Assert.AreNotEqual(before_SessionId   ,Guid.Empty);
            Assert.AreNotEqual(before_SessionId   ,after_SessionId);
            Assert.AreEqual   (loggedOut_SessionId,Guid.Empty);
            Assert.AreNotEqual(after_SessionId    ,Guid.Empty);
            Assert.NotNull    (authToken_SessionId           );
            Assert.AreNotEqual(authToken_SessionId,Guid.Empty);
            Assert.AreEqual   (current_User_Before.UserName, testUser.UserName);
            Assert.AreEqual   (current_User_After .UserName, tmUser.UserName);
            Assert.Throws<SoapException>(()=>webServices.delete_User(userId));
            
            this.login_As_Admin();
            Assert.IsTrue(webServices.delete_User(userId));
        }
        [Test] public void Login_Admin_CurrentUser()
        {
            var sessionId        = webServices.Login(Tests_Consts.DEFAULT_ADMIN_USERNAME, Tests_Consts.DEFAULT_ADMIN_PASSWORD);
            var currentSessionId = webServices.Current_SessionID();
            var currentUser      = webServices.Current_User();            
            Assert.AreNotEqual(sessionId            , Guid.Empty);            
            Assert.AreNotEqual(currentUser          , null);
            Assert.AreNotEqual(currentSessionId     , Guid.Empty);
            Assert.AreEqual   (currentSessionId     , sessionId);
            Assert.AreEqual   (currentUser.UserName , Tests_Consts.DEFAULT_ADMIN_USERNAME);            
            Assert.IsTrue     (webServices.RBAC_IsAdmin());
        }

    }
}
