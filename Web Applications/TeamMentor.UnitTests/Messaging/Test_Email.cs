using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_Email : TM_XmlDatabase_InMemory
    {        
        public string       to         = "tm_alerts@securityinnovation.com";        
        public string       subject    = "TM Email Test";
        public string       message    = "This is a test email from TeamMentor";
        public SendEmails   sendEmails;

        [SetUp]
        public void SetUp()
        {
            var secretData = tmXmlDatabase.UserData.SecretData;
            sendEmails = new SendEmails();
            Assert.IsNotNull(sendEmails);
            Assert.IsNull(sendEmails.Smtp_Password , "In UnitTests SendEmails SMTP password should not be set");
            Assert.IsTrue(sendEmails.offlineMode()   , "In UnitTests SendEmails should be in offline mode");            
        }

        [Test]
        public void Check_TM_Server_URL()
        {
            var tmServerUrl = SendEmails.TM_Server_URL;
            Assert.IsNull(tmServerUrl);     //shouldn't be set unless the HttpContext exists
        }

        [Test]
        public void Send_Test_Email()
        {
            var sentEmailMessages   = SendEmails.Sent_EmailMessages;
            var sentMessagesBefore  = sentEmailMessages.size();
            var sendResult          = sendEmails.send(to, subject, message);
            var sentMessagesAfter   = SendEmails.Sent_EmailMessages.size();
            var lastMessage         = sentEmailMessages.last();

            Assert.IsFalse    (sendResult);
            Assert.AreEqual   (sentMessagesBefore + 1, sentMessagesAfter);
            Assert.IsNotNull  (lastMessage);
            Assert.IsNotNull  (lastMessage.To      = to);
            Assert.IsNotNull  (lastMessage.Subject = subject);
            Assert.IsNotNull  (lastMessage.Message = message);

        }

        //    var stmpServerOnline = Mail.isMailServerOnline(sendEmails.Smtp_Server);
        //    Assert.IsTrue(stmpServerOnline);

    }
}
