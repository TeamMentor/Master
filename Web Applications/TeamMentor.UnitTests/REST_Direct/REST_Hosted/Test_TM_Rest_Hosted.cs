using System;
using System.ServiceModel;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST

{	    
    [TestFixture]
    [Ignore]                // can't run these on TeamCity due to security protection for binding WCF address into Port
                            // need to find a better solution or a way to automate the setup
                            // references:http://blogs.msdn.com/b/paulwh/archive/2007/05/04/addressaccessdeniedexception-http-could-not-register-url-http-8080.aspx
                            //            http://stackoverflow.com/questions/885744/wcf-servicehost-access-rights
    public class Test_TM_Rest_Hosted : TM_Rest_Hosted
    {
        [SetUp] public static void Initialize()
        {
            WCFHost_Start();
        }

        [Test]  public void CheckWebServiceHost()
        {
            var html = TmRestHost.BaseAddress.append("/Version").getHtml();
            Assert.IsTrue(html.valid(), "Html fetch failed");
            //test version
            var version = TmRest.Version();
            Assert.NotNull(version,"Version was null");
            "version (hosted access): {0}".info(version);
            //test sessionID
            var sessionId = TmRest.SessionId();
            Assert.NotNull(sessionId, "sessionID was null");
            "sessionID (hosted access): {0}".info(sessionId);
        }
        
        [Test]  public void CheckLogin()
        {
            var identity_IsAuthenticated = TmRest.RBAC_CurrentIdentity_IsAuthenticated();
            var identity_Name            = TmRest.RBAC_CurrentIdentity_Name();
            var identity_Roles           = TmRest.RBAC_CurrentPrincipal_Roles();
            var identity_IsAdmin         = TmRest.RBAC_IsAdmin();

            Assert.IsTrue (identity_IsAuthenticated);
            Assert.IsNull  (identity_Name);
            Assert.AreEqual(identity_Roles.size(), 1);
            Assert.IsFalse (identity_IsAdmin);
            var username   = tmConfig.TMSecurity.Default_AdminUserName;
            var pwd        = tmConfig.TMSecurity.Default_AdminPassword;
            var sessionId  = TmRest.Login(username, pwd);
            
            Assert.AreNotEqual(Guid.Empty,sessionId);
            
            identity_IsAuthenticated = TmRest.RBAC_CurrentIdentity_IsAuthenticated();
            identity_Name            = TmRest.RBAC_CurrentIdentity_Name();
            identity_Roles           = TmRest.RBAC_CurrentPrincipal_Roles();
            identity_IsAdmin         = TmRest.RBAC_IsAdmin();

            Assert.IsTrue  (identity_IsAuthenticated);
            Assert.AreEqual(username, identity_Name);
            Assert.AreEqual(identity_Roles.size(), 5);
            Assert.IsTrue  (identity_IsAdmin);
            
        }

        [Test]
        public void CheckLogin_Again() // make sure there are no session values left behind
        {
            CheckLogin();
        }

        //[Assert_Admin]
        [Test]  public void CheckErrorHandling()
        {
            Assert.Throws<ProtocolException >(()=>TmRest.users());
            CheckLogin();       //logs in as admin

            TmRest.users();
        }

        [TearDown] public static void Cleanup()
        {
            WCFHost_Stop();
        }
        
    }
}