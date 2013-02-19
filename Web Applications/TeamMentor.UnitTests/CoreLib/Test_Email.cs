using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_Email : TM_XmlDatabase_InMemory
    {        
        public string       to         = "TM_Security_Dude@securityinnovation.com";        
        public string       subject    = "TM Email Test";
        public string       message    = "This is a test email from TeamMentor";
        public SendEmails   sendEmails;

        [SetUp]
        public void SetUp()
        {
            var secretData = tmXmlDatabase.UserData.SecretData;
            sendEmails = new SendEmails(secretData.SMTP_Server, secretData.SMTP_UserName, secretData.SMTP_Password);
            Assert.IsNotNull(sendEmails);
            if(sendEmails.Smtp_Password.notValid())
                Assert.Ignore("SmtpServer_Password is not set, so ignoring test");
            if(tmXmlDatabase.ServerOnline.isFalse())
            Assert.Ignore("We are offline, so ignoring test");
        }

        [Test]
        public void Send_Test_Email()
        {            
            var stmpServerOnline = Mail.isMailServerOnline(sendEmails.Smtp_Server);
            Assert.IsTrue(stmpServerOnline);
            var sendResult = new SendEmails().send(to, subject, message);
            Assert.IsTrue(sendResult);
        }

    }
}
