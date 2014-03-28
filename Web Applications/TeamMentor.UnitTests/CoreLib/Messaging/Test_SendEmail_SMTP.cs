using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests._Test_ConfigAndHelpers;

namespace TeamMentor.UnitTests.CoreLib.Messaging
{
    [TestFixture]
    public class Test_SendEmail_SMTP : TM_UserData_InMemory
    {         
        public TM_QA_ConfigFile tmQAConfig;
        public SendEmails       sendEmails;

        public Test_SendEmail_SMTP()              
        {                    
            tmQAConfig = TM_QA_ConfigFile.Current;
            if (tmQAConfig.isNull())
                Assert.Ignore("TM_QA_ConfigFile.Current was null (so no SMTP config values");
            
            userData.SecretData.SMTP_Server        = tmQAConfig.SMTP_Server;
            userData.SecretData.SMTP_UserName      = tmQAConfig.SMTP_UserName;
            userData.SecretData.SMTP_Password      = tmQAConfig.SMTP_Password;
            tmConfig.TMSecurity.Default_AdminEmail = tmQAConfig.Default_AdminEmail;

            sendEmails = new SendEmails();
        }

        [SetUp]
        public void setup()
        {
            SendEmails.Send_Emails_As_Sync = true;
            SendEmails.Disable_EmailEngine = false;
        }

        [TearDown]
        public void tearDown()
        {
            SendEmails.Send_Emails_As_Sync = false;
            SendEmails.Disable_EmailEngine = true;
        }
        //SendEmails methods
        
        //SendEmails workflows
        [Test] public void Check_If_SMTP_Server_Is_Configured()
        {
            Assert.IsTrue(userData.SecretData.SMTP_Server  .valid());
            Assert.IsTrue(userData.SecretData.SMTP_UserName.valid());
            Assert.IsTrue(userData.SecretData.SMTP_Password.valid());
            Assert.AreNotEqual(tmConfig.TMSecurity.Default_AdminEmail, new TMConfig.TMSecurity_Config().Default_AdminEmail);

            Assert.IsNotNull(sendEmails);
            Assert.IsFalse(sendEmails.serverNotConfigured());
            
        }
        [Test][Ignore]
        public void send_TestEmail()
        {
            Assert.IsTrue(sendEmails.send_TestEmail());
        }
        [Test][Ignore]
        public void Send_Email_On_New_Account_Creation()
        {            
            var sentEmails  = SendEmails.Sent_EmailMessages;
            var beforeCount = sentEmails.size();
            var tmUser      = userData.createUser();
            Assert.IsTrue(tmUser.AccountStatus.UserEnabled);
            var afterCount  = sentEmails.size();
            
            Assert.IsNotNull(tmUser);
            Assert.AreEqual(beforeCount + 3, afterCount);
        }
        [Test] public void Check_Workflow_for_New_User_Admin_Enable()
        {
            Assert.IsTrue(tmConfig.TMSecurity.NewAccounts_Enabled);
            tmConfig.TMSecurity.NewAccounts_Enabled = false;
            Assert.IsFalse(tmConfig.newAccountsEnabled());

            var tmUser      = userData.createUser();
            Assert.IsFalse(tmUser.AccountStatus.UserEnabled);

            tmConfig.TMSecurity.NewAccounts_Enabled = true;
        }
    }
}
