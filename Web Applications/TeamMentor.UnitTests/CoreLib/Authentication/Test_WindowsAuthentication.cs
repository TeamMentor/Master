using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Authentication
{
    [TestFixture]
    public class Test_WindowsAuthentication : TM_Config_InMemory//: TM_XmlDatabase_InMemory
    {        
        public WindowsAuthentication windowsAuthentication;
        public TM_UserData           userData;        
       
        [SetUp]
        public void setup()
        {
            tmConfig.WindowsAuthentication.Enabled = false;
            WindowsAuthentication.readerGroup      = null;
            windowsAuthentication                  = new WindowsAuthentication();            
            userData                               = new TM_UserData();
            
        }
        [Test]
        public void WindowsAuthentication_Static_Ctor()             // and loadConfiguration
        {            
            Assert.IsFalse(tmConfig.WindowsAuthentication.Enabled);
            Assert.NotNull(WindowsAuthentication.readerGroup);
            Assert.NotNull(WindowsAuthentication.editorGroup);
            Assert.NotNull(WindowsAuthentication.adminGroup);            
        }
        [Test]
        public void WindowsAuthentication_Ctor()
        {                        
            Assert.NotNull(WindowsAuthentication.readerGroup);
            Assert.NotNull(WindowsAuthentication.editorGroup);
            Assert.NotNull(WindowsAuthentication.adminGroup);            
        }
    
        [Test]
        public void calculateUserGroupBasedOnWindowsIdentity()
        {
            Action<UserGroup> testMappings = (expectedUserGroup)=>
                {
                    var identity  = WindowsIdentity.GetCurrent();            
                    Assert.NotNull(identity);

                    var principal = new WindowsPrincipal(identity);
                    Assert.NotNull(principal);

                    Assert.AreEqual(identity.AuthenticationType, "NTLM");
                    var defaultUserGroup = windowsAuthentication.calculateUserGroupBasedOnWindowsIdentity(identity);
                    Assert.AreEqual(expectedUserGroup, defaultUserGroup);   
                };
         
            testMappings(UserGroup.None);
            
            WindowsAuthentication.readerGroup = "Users";
            testMappings(UserGroup.Reader);
            
            WindowsAuthentication.editorGroup = "Users";
            testMappings(UserGroup.Editor);

            WindowsAuthentication.adminGroup = "Users";
            testMappings(UserGroup.Admin);

            WindowsAuthentication.loadConfiguration();  // rest
        }

        [Test]
        public void login_Using_WindowsAuthentication()
        {

            var identity = WindowsIdentity.GetCurrent();
            Assert.NotNull(identity);
            Assert.IsTrue(identity.IsAuthenticated);            
            Assert.AreEqual(Guid.Empty, windowsAuthentication.login_Using_WindowsAuthentication(identity));            
            Assert.AreEqual(Guid.Empty, windowsAuthentication.login_Using_WindowsAuthentication(null));            

            var loginName = changeIndentityToBeImpersonation(identity);
            
            Assert.IsNull(loginName.tmUser());                                 // user shouldn't exist before AD login

            var sessionID = windowsAuthentication.login_Using_WindowsAuthentication(identity);
            var tmUser    = loginName.tmUser();
            Assert.AreNotEqual(sessionID, Guid.Empty);
            Assert.AreEqual (loginName, identity.Name);
            Assert.IsNotNull(tmUser);                                          // user should exist before AD login
            Assert.AreEqual(tmUser, sessionID.session_TmUser());
            Assert.AreEqual(tmUser.Sessions.last().LoginMethod, "WindowsAuth");
            //check case when identity.Name is null or empty
            identity.field("m_name","");
            Assert.AreEqual   (identity.Name,"");
            Assert.AreEqual(Guid.Empty,windowsAuthentication.login_Using_WindowsAuthentication(identity));

            //at the moment we are not testing the mode to get the user from the live HttpContext server variable (the line below)
            //userName = HttpContextFactory.Current.field("_context").field("_wr").invoke("GetServerVariable", "LOGON_USER") as string;                 
        }


        [Test]
        public void TM_Authentication_mapUserRoles()
        {
            HttpContextFactory.Context.mock();
            var tmAuthentication = new TM_Authentication(null);
            Assert.NotNull (tmAuthentication.sessionID);
            Assert.AreEqual(tmAuthentication.sessionID,Guid.Empty);
            Assert.IsFalse(tmConfig.WindowsAuthentication.Enabled);

            
            tmConfig.WindowsAuthentication.Enabled = true;
            var identity = tmAuthentication.Current_WindowsIdentity;
            changeIndentityToBeImpersonation(identity);
            
            tmAuthentication.mapUserRoles();

            var tmUser_fromSession = tmAuthentication.sessionID.session_TmUser();

            Assert.AreNotEqual(tmAuthentication.sessionID,Guid.Empty); // sessionID should be set
            Assert.IsTrue     (tmAuthentication.sessionID.validSession());
            Assert.IsNotNull  (tmUser_fromSession);
            Assert.AreEqual   (tmUser_fromSession.UserName,identity.Name);
            
            tmConfig.WindowsAuthentication.Enabled = false;
        }


        //Util method        
        public string changeIndentityToBeImpersonation(WindowsIdentity identity)
        {                       
            var newName = "{0}\\{1}".format(5.randomLetters(), 5.randomLetters());            
            //by defauld ImpersonationLevel is None 
            Assert.AreNotEqual(identity.ImpersonationLevel, TokenImpersonationLevel.Impersonation);
            Assert.AreEqual   (identity.ImpersonationLevel, TokenImpersonationLevel.None);
            
            //but with reflection we can change it :)
            identity.field("m_impersonationLevel", TokenImpersonationLevel.Impersonation);            
            Assert.AreEqual   (identity.ImpersonationLevel, TokenImpersonationLevel.Impersonation);
            identity.field("m_isAuthenticated", 1);            
            Assert.IsTrue (identity.IsAuthenticated);
            
            //also change the user
            identity.field("m_name", newName);
            Assert.AreEqual   (identity.Name,newName);
            return newName;
        }
    }
}
