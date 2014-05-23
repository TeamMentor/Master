using System;
using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.UnitTests.TM_Website.WebServices;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_WebServices_Users: TestFixture_WebServices
    {
        [SetUp] public void setup()
        {                        
            //Assert.AreEqual(webServices.current_User().UserName, Tests_Consts.DEFAULT_ADMIN_USERNAME);
        }
        [TearDown] public void tearDown()
        {
            webServices.logout();
        }
      
        //Extension methods
        [Test] public void add_User()
        {
            var userId = webServices.add_User();            
            Assert.Greater(userId, 0);            
            
            Assert.AreEqual(webServices.add_User(null,null),-1);

            this.login_As_Admin();
            var tmUser = webServices.user(userId);
            Assert.NotNull(tmUser);
            Assert.IsTrue (webServices.delete_User(tmUser.UserName));
        }
        [Test] public void delete_User()
        {
            this.login_As_Admin();
            Assert.IsFalse(webServices.delete_User(null as TM_User));
            Assert.IsFalse(webServices.delete_User(null as string));
            Assert.IsFalse(webServices.delete_User(-1));            
        }
        [Test] public void login_with_AuthToken()
        {
            Assert.IsFalse(webServices.login_with_AuthToken(Guid.Empty));
            Assert.IsFalse((null as TM_WebServices).login_with_AuthToken(Guid.Empty));
        }
        [Test] public void login_with_Pwd()
        {
            Assert.IsFalse(webServices.login_with_Pwd(null, null));
            Assert.IsFalse((null as TM_WebServices).login_with_Pwd(null, null));            
        }
        [Test] public void loggedIn()
        {
            Assert.IsFalse(webServices.loggedIn());
            this.login_As_Reader();
            Assert.IsTrue(webServices.loggedIn());
            webServices.logout();
            Assert.IsFalse(webServices.loggedIn());
        }
        [Test] public void users()
        {            
            Assert.NotNull(this.login_As_Admin());
            var users = webServices.users();
            Assert.Greater(users.size(), 1);            
        }  
        [Test] public void user_AuthToken()
        {            
            Assert.Throws<SoapException>(()=>webServices.user_AuthToken(0));
            this.login_As_Reader();
            Assert.Throws<SoapException>(()=>webServices.user_AuthToken(0));
            this.login_As_Editor();
            Assert.Throws<SoapException>(()=>webServices.user_AuthToken(0));
            this.login_As_Admin();
            Assert.DoesNotThrow             (()=>webServices.user_AuthToken(0));

            var adminUser = QAConfig.testUser("qa-admin");
            var authToken = webServices.user_AuthToken(adminUser.UserId);
            
            Assert.IsNotNull(adminUser);
            Assert.IsNotNull(authToken);
            Assert.AreEqual (authToken, adminUser.AuthToken);
        }

        //Workflows
        [Test] public void TM_QA_Config_Check_Test_Accounts_AuthTokens()
        {
            foreach(var testUser in QAConfig.testUsers())
            {
                Assert.IsNotNull(testUser);
                Assert.IsNotNull(testUser.UserName);

                Assert.IsNotNull(testUser.AuthToken);  // should be IsNotNull
                var sessionId = webServices.Login_Using_AuthToken(testUser.AuthToken);
                Assert.AreNotEqual(Guid.Empty, sessionId, "userId: {0} , userName: {1} token: {2}".format(testUser.UserId,testUser.UserName,testUser.AuthToken));
                var currentUser = webServices.Current_User();
                Assert.AreEqual   (currentUser.UserName,testUser.UserName);
                Assert.AreEqual   (currentUser.UserId  ,testUser.UserId);
                Assert.AreEqual   (currentUser.GroupID ,testUser.GroupId);
                Assert.AreEqual   (currentUser.Email   ,testUser.Email.lower());
            }            
        }
        [Test] public void TM_QA_Config_Setup_TestUsers()                     // ensures that authtokens are set
        {            
            Assert.IsTrue   (QAConfig.Url_Target_TM_Site.valid());        // target server is set
            Assert.IsTrue   (QAConfig.Default_Admin_User.valid());        // we have an admin username and password
            Assert.IsTrue   (QAConfig.Default_Admin_Pwd .valid());
            Assert.AreEqual (QAConfig.TestUsers.size() , 4      );        // there are test users     
            Assert.IsNull   (webServices.Cached_CurrentUser     );        // not logged in
            
            //login as admin
            Assert.IsTrue   (webServices.login_with_Pwd(QAConfig.Default_Admin_User, QAConfig.Default_Admin_Pwd));
            Assert.IsTrue   (webServices.RBAC_IsAdmin());
                        
            var adminUser  = webServices.Cached_CurrentUser;

            Assert.IsNotNull(adminUser);
            Assert.AreEqual (adminUser.UserName,QAConfig.Default_Admin_User);
            
            foreach(var testUser in QAConfig.testUsers())
            {
                var tmUser = webServices.user(testUser.UserName);
                if(tmUser.notNull())
                    webServices.delete_User(tmUser);

                var newUser = new NewUser
                    {
                        Username = testUser.UserName,
                        Password = testUser.Password,
                        Email    = testUser.Email,
                        GroupId  = testUser.GroupId, 
                    };
                var userId = webServices.add_User(newUser);
                Assert.Greater(userId,0);

                tmUser = webServices.user(testUser.UserName);
                                
                Assert.IsNotNull(tmUser);
                
                testUser.UserId    = tmUser.UserId; 
                testUser.AuthToken = webServices.user_AuthToken(tmUser.UserId);                                
            }
            QAConfig.save_QAConfig();
        }
        //helper methods

        
    }
}
