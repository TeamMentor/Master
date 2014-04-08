using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.WebServices
{
    [TestFixture]
    public class Test_TestFixture_WebServices 
    {
        public TestFixture_WebServices   tfWebServices;
        public TM_WebServices_Configured webServices;
           
        public Test_TestFixture_WebServices()
        {            
            tfWebServices = new TestFixture_WebServices();
            webServices   = tfWebServices.webServices;
        }
        [SetUp] public void setup()
        {            
            Assert.IsTrue(webServices.logout());
        }
        
        [Test] public void TestFixture_WebServices_Ctor()
        {
            Assert.IsNotNull(tfWebServices.QAConfig);
            Assert.IsNotNull(tfWebServices.WebSite_Url);            
            Assert.AreEqual(tfWebServices.WebSite_Url, webServices.TargetServer);
            Assert.IsTrue  (tfWebServices.QAConfig.serverOnline());

            //for code coverage on offline server UnitTest Check
            TM_QA_Config.Force_Server_Offline = true;
            Assert.IsFalse  (tfWebServices.QAConfig.serverOnline());            
            Assert.Throws<IgnoreException>(()=> new TestFixture_WebServices());
            TM_QA_Config.Force_Server_Offline = false;
        }
        [Test] public void login_As_Admin()
        {
            //check webServices.Cached_CurrentUser
            var adminUser = tfWebServices.QAConfig.testUser("qa-admin");
            loginAsUser_and_Check_Cached_CurrentUser(tfWebServices.login_As_Admin, adminUser);            
            Assert.IsTrue   (webServices.RBAC_IsAdmin());            
            Assert.IsTrue   (webServices.RBAC_HasRole("Admin"));
            Assert.IsTrue   (webServices.RBAC_HasRole("ManageUsers"));
            Assert.IsTrue   (webServices.RBAC_HasRole("EditArticles"));
            Assert.IsTrue   (webServices.RBAC_HasRole("ReadArticles"));
            Assert.IsFalse   (webServices.RBAC_HasRole("aabbcc"));

            //check webServices.current_User()
            Assert.IsTrue(webServices.logout());                    // logout
            Assert.IsNull   (webServices.current_User());
            Assert.NotNull  (tfWebServices.login_As_Admin());                // login
            Assert.IsTrue   (webServices.RBAC_IsAdmin());
            Assert.IsNotNull(webServices.current_User());
            Assert.AreEqual (webServices.current_User().UserName, adminUser.UserName);            

            //test null
            adminUser.AuthToken = Guid.Empty;
            Assert.IsNull  (tfWebServices.login_As_Admin());

        }        
        [Test] public void login_AsEditor()
        {
            var editorUser = tfWebServices.QAConfig.testUser("qa-editor");
            loginAsUser_and_Check_Cached_CurrentUser(tfWebServices.login_As_Editor, editorUser);
            
            
            Assert.IsFalse              (webServices.RBAC_IsAdmin());
            Assert.Throws<SoapException>(()=>webServices.RBAC_Demand_ManageUsers());
            Assert.IsTrue               (webServices.RBAC_Demand_EditArticles());
            Assert.IsTrue               (webServices.RBAC_Demand_ReadArticles());

            Assert.IsFalse  (webServices.RBAC_HasRole("Admin"));
            Assert.IsFalse  (webServices.RBAC_HasRole("ManageUsers"));
            Assert.IsTrue   (webServices.RBAC_HasRole("EditArticles"));
            Assert.IsTrue   (webServices.RBAC_HasRole("ReadArticles"));
            Assert.IsFalse  (webServices.RBAC_HasRole("aabbcc"));

            //test null
            editorUser.AuthToken = Guid.Empty;
            Assert.IsNull  (tfWebServices.login_As_Editor());
            
        }
        [Test] public void login_AsReader()
        {
            var readerUser = tfWebServices.QAConfig.testUser("qa-reader");
            loginAsUser_and_Check_Cached_CurrentUser(tfWebServices.login_As_Reader, readerUser);            

            Assert.IsFalse              (webServices.RBAC_IsAdmin());
            Assert.Throws<SoapException>(()=>webServices.RBAC_Demand_ManageUsers());
            Assert.Throws<SoapException>(()=>webServices.RBAC_Demand_EditArticles());
            Assert.IsTrue               (webServices.RBAC_Demand_ReadArticles());

            Assert.IsFalse  (webServices.RBAC_HasRole("Admin"));
            Assert.IsFalse  (webServices.RBAC_HasRole("ManageUsers"));
            Assert.IsFalse  (webServices.RBAC_HasRole("EditArticles"));
            Assert.IsTrue   (webServices.RBAC_HasRole("ReadArticles"));
            Assert.IsFalse  (webServices.RBAC_HasRole("aabbcc"));
            
            //test null
            readerUser.AuthToken = Guid.Empty;
            Assert.IsNull  (tfWebServices.login_As_Reader());
        }
        [Test] public void http_GET()
        {
            //make anonymous request (should redirect)
            var html_Login  = tfWebServices.http_GET("tbot");            
            Assert.IsTrue     (html_Login.contains("Login"));
            Assert.IsFalse    (html_Login.contains("TBot"));
             
            //login as Admin and re open TBot page (which should now work)
            var adminUser    = tfWebServices.login_As_Admin();            
                
            Assert.IsNotNull  (adminUser);
            Assert.AreEqual   (adminUser.GroupId, 1);
            var html_Tbot    = tfWebServices.http_GET("tbot");
            
            Assert.IsFalse    (html_Tbot.contains("Login"));
            Assert.IsTrue     (html_Tbot.contains("TBot"));
            Assert.AreNotEqual(html_Tbot, html_Login);

            //login as editor and the Tbot page should redirect to login again
            var editorUser   = tfWebServices.login_As_Editor();            
            Assert.IsNotNull  (editorUser);
            Assert.AreEqual   (editorUser.GroupId, 3);
            var html_Login2  = tfWebServices.http_GET("tbot");
            
            Assert.IsTrue     (html_Login2.contains("Login"));
            Assert.IsFalse    (html_Login2.contains("TBot"));
            Assert.AreNotEqual(html_Login2, html_Tbot);
            Assert.AreEqual   (html_Login2, html_Login);

            //test null
            Assert.AreEqual   ( tfWebServices.http_GET(null              ), "");
            Assert.AreEqual   ( tfWebServices.http_GET(""                ), "");
            Assert.AreNotEqual( tfWebServices.http_GET("!'\""            ), "");
            Assert.AreNotEqual( tfWebServices.http_GET(50.randomLetters()), "");
            Assert.AreNotEqual( tfWebServices.http_GET(50.randomNumbers()), "");           
        }
        [Test] public void http_POST()
        {
            tfWebServices.set_TM_Server("http://local:3187");
            var postUrl = "rest/users/verify";
            var postData = "{}";

            var json_Response1  = tfWebServices.http_POST_JSON(postUrl, postData);
            var json_Response2  = tfWebServices.http_POST_JSON(postUrl + "AAA", postData);
            Assert.IsTrue(json_Response1.contains("Access is denied"));
            Assert.IsTrue(json_Response2.contains("Endpoint not found."));
        }
        [Test] public void http_PUT_JSON()
        {
            var postUrl = "rest/login";
            var postData = "{ \"Password\":\"String content\",\"UserName\":\"String content\"}";

            var json_Response  = tfWebServices.http_PUT_JSON(postUrl, postData);
            Assert.AreEqual(json_Response, "\"{0}\"".format(Guid.Empty));
        }
        //helper method
        public void loginAsUser_and_Check_Cached_CurrentUser(Func<Test_User> loginUser, Test_User testUser)
        {
            Assert.IsNull   (webServices.Cached_CurrentUser);
            Assert.NotNull  (loginUser());  
            Assert.IsNotNull(webServices.Cached_CurrentUser);
            Assert.AreEqual (webServices.Cached_CurrentUser.UserName, testUser.UserName);
            Assert.AreEqual (webServices.Cached_CurrentUser.UserId  , testUser.UserId);
            Assert.AreEqual (webServices.Cached_CurrentUser.GroupID , testUser.GroupId);
        }
    }
}
