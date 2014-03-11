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
    public class Test_WindowsAuthentication //: TM_XmlDatabase_InMemory
    {        
        public WindowsAuthentication windowsAuthentication;
        public TM_UserData           userData;

        public Test_WindowsAuthentication()
        {
            WindowsAuthentication.windowsAuthentication_Enabled = false;
            WindowsAuthentication.readerGroup                   = null;
            windowsAuthentication = new WindowsAuthentication();            
            userData              = new TM_UserData();
        }

        [Test]
        public void WindowsAuthentication_Static_Ctor()             // and loadConfiguration
        {            
            Assert.IsFalse(WindowsAuthentication.windowsAuthentication_Enabled);
            Assert.NotNull(WindowsAuthentication.readerGroup);
            Assert.NotNull(WindowsAuthentication.editorGroup);
            Assert.NotNull(WindowsAuthentication.adminGroup);            
        }
        [Test]
        public void WindowsAuthentication_Ctor()
        {
            Assert.NotNull(windowsAuthentication.CurrentWindowsIdentity);
            var identity = WindowsIdentity.GetCurrent();
            Assert.NotNull (identity);
            Assert.AreEqual(windowsAuthentication.CurrentWindowsIdentity.Name, identity.Name);
        }
    
        [Test]
        public void calculateUserGroupBasedOnWindowsIdentity()
        {
            var identity  = WindowsIdentity.GetCurrent();            
            Assert.NotNull(identity);

            var principal = new WindowsPrincipal(identity);
            Assert.NotNull(principal);

            Assert.AreEqual(identity.AuthenticationType, "NTLM");
            var defaultUserGroup = windowsAuthentication.calculateUserGroupBasedOnWindowsIdentity();
            Assert.AreEqual(UserGroup.None, defaultUserGroup);
            

            var roles = principal.roles();            
        }

        [Test]
        public void authenticateUserBaseOn_ActiveDirectory()
        {
            var identity = windowsAuthentication.CurrentWindowsIdentity;
            Assert.NotNull(identity);
            Assert.IsTrue(identity.IsAuthenticated);            

            var loginName = changeCurrentIndentityToBeImpersonation();
            
            Assert.IsNull(loginName.tmUser());                                 // user shouldn't exist before AD login

            var sessionID = windowsAuthentication.authenticateUserBaseOn_ActiveDirectory();
            var tmUser    = loginName.tmUser();
            Assert.AreNotEqual(sessionID, Guid.Empty);
            Assert.AreEqual (loginName, identity.Name);
            Assert.IsNotNull(tmUser);                                          // user should exist before AD login
            Assert.AreEqual(tmUser, sessionID.session_TmUser());
            
            //check case when identity.Name is null or empty
            identity.field("m_name","");
            Assert.AreEqual   (identity.Name,"");
            Assert.AreEqual(Guid.Empty,windowsAuthentication.authenticateUserBaseOn_ActiveDirectory());
        }


        [Test]
        public void TM_Authentication_mapUserRoles()
        {
            var tmAuthentication = new TM_Authentication(null);
            Assert.NotNull (tmAuthentication.sessionID);
            Assert.AreEqual(tmAuthentication.sessionID,Guid.Empty);
            Assert.IsFalse(WindowsAuthentication.windowsAuthentication_Enabled);

            WindowsAuthentication.windowsAuthentication_Enabled = true;
            
            changeCurrentIndentityToBeImpersonation();
            tmAuthentication.mapUserRoles();

       //     Assert.AreNotEqual(tmAuthentication.sessionID,Guid.Empty); // sessionID should be set
        }


        //Util method
        public string changeCurrentIndentityToBeImpersonation()
        {           
            
            var newName = "{0}\\{1}".format(5.randomLetters(), 5.randomLetters());
            var identity = windowsAuthentication.CurrentWindowsIdentity;
            //by defauld ImpersonationLevel is None 
            Assert.AreNotEqual(identity.ImpersonationLevel, TokenImpersonationLevel.Impersonation);
            Assert.AreEqual   (identity.ImpersonationLevel, TokenImpersonationLevel.None);
            
            //but with reflection we can change it :)
            identity.field("m_impersonationLevel", TokenImpersonationLevel.Impersonation);            
            Assert.AreEqual   (identity.ImpersonationLevel, TokenImpersonationLevel.Impersonation);

            //also change the user
            identity.field("m_name", newName);
            Assert.AreEqual   (identity.Name,newName);
            return newName;
        }
    }
}
