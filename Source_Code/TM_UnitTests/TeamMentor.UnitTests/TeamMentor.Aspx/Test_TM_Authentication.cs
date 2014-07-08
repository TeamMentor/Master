using System;
using System.Threading;
using System.Web;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Authentication 
{
    [TestFixture]
    public class Test_TM_Authentication : TM_WebServices_InMemory
    {
        TM_Authentication tmAuthentication;

        public Test_TM_Authentication()
        {             
            tmAuthentication= tmWebServices.tmAuthentication;
            Assert.IsFalse(tmConfig.WindowsAuthentication.Enabled);
            Assert.NotNull(tmAuthentication);
        }

        [SetUp]
        public void setup()
        {
            tmAuthentication.sessionID = Guid.Empty;            
            HttpContextFactory.Session["sessionID"]       = null;                        
            HttpContextFactory.Request.Headers["Session"] = null;   
            HttpContextFactory.Request .Cookies.Clear();
            HttpContextFactory.Response.Cookies.Clear();
            HttpContextFactory.Context.setCurrentUserRoles(UserGroup.None);
        }

        [Test] public void TM_Authentication_Ctor()                     
        {
            Assert.IsFalse(TM_Authentication.Global_Disable_Csrf_Check);
            Assert.IsFalse(tmAuthentication.Disable_Csrf_Check);
            Assert.IsNotNull(tmAuthentication.Current_WindowsIdentity);
            Assert.IsNotNull(tmAuthentication.TmWebServices);
        }
        [Test] public void sessionID_Get()                              
        {            
            //check default (which should return empty guid)
            var sessionID = tmAuthentication.sessionID;
            Assert.NotNull (sessionID);
            Assert.AreEqual(sessionID, Guid.Empty);
            Assert.IsFalse (sessionID.validSession());

            //check sessionID via HttpContextFactory.Session
//            Assert.NotNull    (HttpContextFactory.Session);
//            Assert.IsNull     (HttpContextFactory.Session["sessionID"]);

//            sessionID                               = Guid.NewGuid();
//            HttpContextFactory.Session["sessionID"] = sessionID;

//           Assert.NotNull    (HttpContextFactory.Session["sessionID"]);
//            Assert.AreNotEqual(tmAuthentication.sessionID, Guid.Empty);
//            Assert.AreEqual   (tmAuthentication.sessionID, sessionID);

            //check sessionID via HttpContextFactory.Request.Cookies
            Assert.NotNull    (HttpContextFactory.Request.Cookies);
            Assert.IsNull     (HttpContextFactory.Request.Cookies["Session"]);
         
            sessionID                                = Guid.NewGuid();
            HttpContextFactory.Session["sessionID"]  = null;
            var httpCookie = new HttpCookie("Session", sessionID.str());
            HttpContextFactory.Request.Cookies.Add(httpCookie);

            Assert.NotNull    (HttpContextFactory.Request.Cookies["Session"]);
            Assert.NotNull    (HttpContextFactory.Request.Cookies["Session"].value());
            Assert.AreEqual   (HttpContextFactory.Request.Cookies["Session"].value(),sessionID.str());
            Assert.IsTrue     (HttpContextFactory.Request.Cookies["Session"].value().isGuid());
            Assert.AreNotEqual(tmAuthentication.sessionID, Guid.Empty);            
            Assert.AreEqual   (tmAuthentication.sessionID, sessionID);

            //check sessionID via HttpContextFactory.Request.Header
            HttpContextFactory.Request.Cookies.Remove("Session");
            sessionID                                     = Guid.NewGuid();                        
            HttpContextFactory.Request.Headers["Session"] = sessionID.str();

            Assert.NotNull    (HttpContextFactory.Request.Headers["Session"]);            
            Assert.AreEqual   (HttpContextFactory.Request.Headers["Session"],sessionID.str());
            Assert.IsTrue     (HttpContextFactory.Request.Headers["Session"].isGuid());
            Assert.AreNotEqual(tmAuthentication.sessionID, Guid.Empty);            
            Assert.AreEqual   (tmAuthentication.sessionID, sessionID);

            // if not session, cookie or header is there, it should be null again
            HttpContextFactory.Request.Headers["Session"] = null;
            Assert.AreEqual   (tmAuthentication.sessionID, Guid.Empty);     
        }
/*        [Test] public void sessionID_Set()                              
        {                             
            Assert.IsNull(HttpContextFactory.Session["sessionID"]);
            Assert.IsNull(HttpContextFactory.Response.Cookies["Session"]);
            Assert.IsNull(HttpContextFactory.Request.Cookies["Session"]);
            Assert.IsEmpty(HttpContextFactory.Current.getCurrentUserRoles());
         
            var tmUser1 = userData.newUser().tmUser();
            var tmUser2 = userData.newUser().tmUser();
            var tmUser3 = userData.newUser().tmUser();

            var tmUser1_SessionId = tmUser1.add_NewSession().SessionID;
            var tmUser2_SessionId = tmUser2.add_NewSession().SessionID;
            var tmUser3_SessionId = tmUser3.add_NewSession().SessionID;

            tmUser2.set_UserGroup(UserGroup.Editor);
            tmUser3.set_UserGroup(UserGroup.Admin);

            Action<Guid,UserGroup> checkValues = 
                    (sessionID, userGroup)=>
                        {
                            tmAuthentication.sessionID = sessionID;
                            Assert.AreEqual(HttpContextFactory.Session["sessionID"]                ,sessionID);
                            Assert.AreEqual(HttpContextFactory.Response.Cookies["Session"].value() ,sessionID.str());
                            Assert.AreEqual(HttpContextFactory.Request .Cookies["Session"].value() ,sessionID.str());
                            Assert.AreEqual(HttpContextFactory.Current.getCurrentUserRoles(), userGroup.userRoles().toStringList());                            
                        };

            checkValues(Guid.NewGuid()   , UserGroup.None);
            checkValues(tmUser1_SessionId, UserGroup.Reader);
            checkValues(tmUser2_SessionId, UserGroup.Editor);
            checkValues(tmUser3_SessionId, UserGroup.Admin);
        }*/
        [Test] public void currentUser()                                
        {
            var adminUser = userData.tmUsers().first();
            Assert.NotNull (adminUser);
            Assert.AreEqual(adminUser.UserName, "admin");
                        
            Assert.IsNull(tmAuthentication.currentUser);

            var sessionID = adminUser.login();

            tmAuthentication.sessionID = sessionID;
            var currentUser = tmAuthentication.currentUser;
            Assert.IsNotNull(currentUser);
            Assert.AreEqual (currentUser,adminUser);
            Assert.NotNull  (currentUser.SecretData);
            Assert.NotNull  (currentUser.SecretData.CSRF_Token);
            Assert.AreEqual (currentUser.SecretData.CSRF_Token,sessionID.csrfToken());
        }
        [Test] public void check_CSRF_Token()                           
        {
            Assert.IsFalse(tmAuthentication.check_CSRF_Token());            
            Assert.IsNull(HttpContextFactory.Context.Request.Headers["CSRF-Token"]);

            var sessionID = Guid.NewGuid();
            var csrfToken = sessionID.csrfToken();
            tmAuthentication.sessionID = sessionID;

            HttpContextFactory.Context.Request.Headers["CSRF-Token"] = csrfToken;

            var header_Csrf_Token = HttpContextFactory.Context.Request.Headers["CSRF-Token"];
            Assert.NotNull (header_Csrf_Token);
            Assert.IsTrue  (header_Csrf_Token.valid());
            Assert.AreEqual(header_Csrf_Token , csrfToken);

            Assert.IsTrue(tmAuthentication.check_CSRF_Token()); 

            HttpContextFactory.Context.Request.Headers["CSRF-Token"] = "12345";
            Assert.IsFalse(tmAuthentication.check_CSRF_Token()); 
        }
        [Test] public void check_CSRF_Token_Global_Disable_Csrf_Check() 
        {            
            Assert.IsFalse(tmAuthentication.check_CSRF_Token());
            TM_Authentication.Global_Disable_Csrf_Check = true;
            Assert.IsTrue(tmAuthentication.check_CSRF_Token());
            TM_Authentication.Global_Disable_Csrf_Check = false;
            Assert.IsFalse(tmAuthentication.check_CSRF_Token());
        }
        [Test] public void check_CSRF_Token_Disable_Csrf_Check()        
        {
            Assert.IsFalse(tmAuthentication.check_CSRF_Token());
            tmAuthentication.Disable_Csrf_Check = true;
            Assert.IsTrue(tmAuthentication.check_CSRF_Token());
            tmAuthentication.Disable_Csrf_Check = false;
            Assert.IsFalse(tmAuthentication.check_CSRF_Token());
        }
        [Test] public void mapUserRoles()                               
        {            
            Assert.IsNotNull  (HttpContextFactory.Session["principal"]);
            Assert.AreNotEqual(HttpContextFactory.Session["principal"], Thread.CurrentPrincipal);

            Assert.AreEqual(tmAuthentication.sessionID,  Guid.Empty);
            Assert.IsFalse (tmAuthentication.sessionID.validSession());
            Assert.IsFalse (tmConfig.WindowsAuthentication.Enabled);
            Assert.IsFalse (TMConfig.Current.TMSecurity.Show_ContentToAnonymousUsers);
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.None.userRoles().toStringList());

            
            tmAuthentication.mapUserRoles();
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.Anonymous.userRoles().toStringList());
            
            var adminUser = userData.tmUsers().first();
            tmAuthentication.sessionID = adminUser.login();
            //should not work until the CSRF token is set
            tmAuthentication.mapUserRoles();
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.Anonymous.userRoles().toStringList());

            HttpContextFactory.Context.Request.Headers["CSRF-Token"] = tmAuthentication.sessionID.csrfToken();

            tmAuthentication.mapUserRoles();
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.Admin.userRoles().toStringList());

            Assert.AreEqual   (HttpContextFactory.Session["principal"], Thread.CurrentPrincipal);
   
        }
        [Test] public void mapUserRoles_Show_ContentToAnonymousUsers()  
        {
            tmAuthentication.mapUserRoles();
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.Anonymous.userRoles().toStringList());
            TMConfig.Current.TMSecurity.Show_ContentToAnonymousUsers = true;
            tmAuthentication.mapUserRoles();
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.Reader.userRoles().toStringList());
            TMConfig.Current.TMSecurity.Show_ContentToAnonymousUsers = false;
            tmAuthentication.mapUserRoles();
            Assert.AreEqual(HttpContextFactory.Context.getThreadPrincipalWithRoles(), UserGroup.Anonymous.userRoles().toStringList());
        }
    }
}
