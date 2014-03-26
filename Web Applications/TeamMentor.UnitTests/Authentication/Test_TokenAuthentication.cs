using System;
using System.Security;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Authentication
{
    [TestFixture]
    public class Test_TokenAuthentication : TM_UserData_InMemory
    {
        public TokenAuthentication tokenAuth;
        public string              authVar;
        public TMUser              tmUser;
        public AuthToken           user_AuthToken;

        public Test_TokenAuthentication()                   
        {
            tokenAuth = new TokenAuthentication();            
            authVar   = TMConsts.AUTH_TOKEN_REQUEST_VAR_NAME;
        }

        [Assert_Admin][SetUp]
        public void setup()
        {
            tmUser           = userData.createUser();    
            Assert.IsEmpty  (tmUser.AuthTokens);
            user_AuthToken   = tmUser.add_AuthToken();
        }

        [Test] public void TokenAuthentication_Ctor()       
        {
            Assert.NotNull(tokenAuth);
            Assert.NotNull(tokenAuth.userData);
            Assert.AreEqual(tokenAuth.userData, TM_UserData.Current);
        }
        
        // TokenAuthentication methods
        [Test] public void authToken()
        {            
            HttpContextFactory.Context.mock();
            var request = HttpContextFactory.Request;
            var tmAuthentication = new TM_Authentication(null);

            Assert.AreEqual(Guid.Empty,tmAuthentication.authToken);
            Assert.IsNull(request.QueryString[authVar]);
            
            //test with an random string
            request.QueryString[authVar] = 10.randomLetters();
            Assert.IsNotNull(request.QueryString[authVar]);
            Assert.AreEqual (Guid.Empty,tmAuthentication.authToken);

            //test with an random GUID
            request.QueryString[authVar] = Guid.NewGuid().str();
            Assert.IsNotNull  (request.QueryString[authVar]);            
            Assert.AreNotEqual(Guid.Empty,tmAuthentication.authToken);
        }
        [Test] public void validToken()                     
        {   
            Assert.IsTrue(tokenAuth.validToken(user_AuthToken));         
            Assert.IsTrue(tokenAuth.validToken(user_AuthToken.Token));

            //test nulls and empty values
            Assert.IsFalse(tokenAuth.validToken(null));
            Assert.IsFalse(tokenAuth.validToken(new AuthToken()));            
            Assert.IsFalse(tokenAuth.validToken(Guid.Empty));
            Assert.IsFalse((null as TokenAuthentication).validToken(Guid.Empty));
        }
        [Test] public void tmUser_From_AuthToken()          
        {            
            var tmUser_FromToken = tokenAuth.tmUser_From_AuthToken(user_AuthToken);

            Assert.IsNotNull(tmUser_FromToken);
            Assert.AreEqual (tmUser, tmUser_FromToken);
            Assert.IsNull   (tokenAuth.tmUser_From_AuthToken(null));
            Assert.IsNull   (tokenAuth.tmUser_From_AuthToken(Guid.Empty));
        }
 
        [Test] public void login_Using_AuthToken()          
        {                                    
            var sessionId    = user_AuthToken.Token.login_Using_AuthToken();

            Assert.AreNotEqual(Guid.Empty,sessionId);            
            var tmUser_FromSession = sessionId.session_TmUser();

            Assert.IsTrue    (sessionId.validSession());
            Assert.IsNotNull (tmUser_FromSession);
            Assert.AreEqual  (tmUser, tmUser_FromSession);
            Assert.AreEqual  (tmUser.Sessions.last().LoginMethod, "AuthToken");
            
            sessionId.logout();
            Assert.IsFalse   (sessionId.validSession());

            Assert.AreEqual(Guid.Empty, tokenAuth.login_Using_AuthToken(Guid.NewGuid(), Guid.Empty));
        }        
        [Test] public void TM_Authentication_mapUserRoles() 
        {
            HttpContextFactory.Context.mock();
            var request = HttpContextFactory.Request;
            var tmAuthentication = new TM_Authentication(null);
            Assert.NotNull (tmAuthentication.sessionID);
            Assert.AreEqual(tmAuthentication.sessionID,Guid.Empty);
            Assert.IsNull  (request[authVar]);
                                    

            tmAuthentication.mapUserRoles();
            Assert.AreEqual(tmAuthentication.sessionID,Guid.Empty);

            request.QueryString[authVar] = user_AuthToken.Token.str();

            tmAuthentication.mapUserRoles();
            Assert.AreNotEqual(tmAuthentication.sessionID,Guid.Empty);
            Assert.IsTrue     (tmAuthentication.sessionID.validSession());
            Assert.AreEqual   (tmAuthentication.sessionID.session_TmUser(), tmUser);
        }
        
        [Assert_Admin] [Test] public void add_AuthToken()       
        {
            var authToken  = tmUser.add_AuthToken();
            Assert.IsNotNull(authToken);
            Assert.IsNotEmpty(tmUser.AuthTokens);
            Assert.AreEqual  (tmUser.AuthTokens.first(), user_AuthToken);
            Assert.AreEqual  (tmUser.AuthTokens.second(), authToken);
        }
        [Assert_Admin] [Test] public void createUserAuthToken() 
        {
            Assert.AreEqual(1, tmUser.AuthTokens.size());
            var authToken = userData.createUserAuthToken(tmUser.UserID);
            Assert.AreNotEqual(Guid.Empty, authToken);
            Assert.AreEqual   (2         , tmUser.AuthTokens.size());
            Assert.AreEqual   (tmUser    , tokenAuth.tmUser_From_AuthToken(authToken));
            tmUser.deleteTmUser();
            
            //check null and bad values            
            Assert.AreEqual(Guid.Empty, userData.createUserAuthToken(tmUser.UserID));       // this user should not exist anymore
            Assert.AreEqual(Guid.Empty, userData.createUserAuthToken(-1));
            Assert.AreEqual(Guid.Empty, userData.createUserAuthToken(3000.randomNumber()));
        }
        [Assert_Admin] [Test] public void getUserAuthTokens()   
        {
            Assert.AreEqual   (1, tmUser.AuthTokens.size());
            Assert.IsNotEmpty (userData.getUserAuthTokens(tmUser.UserID));
            Assert.AreEqual   (tmUser.AuthTokens.first().Token, userData.getUserAuthTokens(tmUser.UserID).first());
            userData.createUserAuthToken(tmUser.UserID);
            Assert.AreEqual   (2, tmUser.AuthTokens.size());            
            Assert.AreNotEqual(tmUser.AuthTokens.second().Token, tmUser.AuthTokens.first().Token);
            Assert.AreEqual   (tmUser.AuthTokens.second().Token, userData.getUserAuthTokens(tmUser.UserID).second());            
            tmUser.deleteTmUser();

            //check null and bad values  
            Assert.IsEmpty   (userData.getUserAuthTokens(tmUser.UserID));
            Assert.IsEmpty   (userData.getUserAuthTokens(-1));
            Assert.IsEmpty   (userData.getUserAuthTokens(3000.randomNumber()));
        }   
        
        // misc workflows
        [Test] public void Check_SecurityDemands()
        {
            Assert.Throws<SecurityException>(()=> tmUser.add_AuthToken());
            Assert.Throws<SecurityException>(()=> userData.createUserAuthToken(-1));
            Assert.Throws<SecurityException>(()=> userData.getUserAuthTokens(-1));
            UserGroup.Admin.setPrivileges();
            Assert.DoesNotThrow             (()=> tmUser.add_AuthToken());
            Assert.DoesNotThrow             (()=> userData.createUserAuthToken(-1));
            Assert.DoesNotThrow             (()=> userData.getUserAuthTokens(-1));
            UserGroup.Reader.setPrivileges();
            Assert.Throws<SecurityException>(()=> tmUser.add_AuthToken());
            Assert.Throws<SecurityException>(()=> userData.createUserAuthToken(-1));
            Assert.Throws<SecurityException>(()=> userData.getUserAuthTokens(-1));
        }        
        [Test] public void Login_Using_Pwd_and_Login_Using_AuthToken()
        {
            HttpContextFactory.Context.mock();      
                  
            //Create user and login using its username and pwd
            var username         = 10.randomLetters();
            var password         = "!!123".add_RandomLetters(10);
            var userId           = userData.newUser(username,password);
            var loginId          = userData.login(username, password);
            var tmAuthentication = new TM_Authentication(null);

            Assert.Less       (0, userId);
            Assert.AreNotEqual(Guid.Empty, loginId);
            Assert.AreEqual   (Guid.Empty, tmAuthentication.sessionID);
            
            //set current sessionId to user created above
            tmAuthentication.sessionID = loginId;            
            Assert.AreEqual(tmAuthentication.sessionID, loginId);
            Assert.AreEqual(tmAuthentication.sessionID.session_TmUser().UserID   , userId);
            Assert.AreEqual(tmAuthentication.sessionID.session_TmUser().UserName , username);
            
            //set authVar to user_AuthToken and simulate the login process
            HttpContextFactory.Request.QueryString[authVar] = user_AuthToken.Token.str();
            tmAuthentication.mapUserRoles();
            
            //the sessionId should now be mapped to tmUser and not to the user created above
            var sessionId = tmAuthentication.sessionID;
            Assert.AreNotEqual(sessionId, loginId);
            Assert.AreEqual   (tmAuthentication.sessionID.session_TmUser().UserName , tmUser.UserName);

            //another request to tmAuthentication.mapUserRoles(); should not change session or login the user again
            tmAuthentication.mapUserRoles();
            Assert.AreEqual  (sessionId, tmAuthentication.sessionID);
        }
    }
}
