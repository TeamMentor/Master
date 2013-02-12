using System.Net.Mail;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using System.Net.Mime;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_Email
    {        
        public string to         = "TM_Security_Dude@securityinnovation.com";        
        public string subject    = "TM Email Test";
        public string message    = "This is a test email from TeamMentor";

        [SetUp]
        public void SetUp()
        {
            if(SendEmails.SmtpServer_Password.notValid())
                Assert.Ignore("SmtpServer_Password is not set, so ignoring test");
        }

        [Test]
        public void Send_Test_Email()
        {                        
            var stmpServerOnline = Mail.isMailServerOnline(SendEmails.SmtpServer);
            Assert.IsTrue(stmpServerOnline);

            var sendResult = new SendEmails().send(to, subject, message);
            Assert.IsTrue(sendResult);
        }

    }
}
