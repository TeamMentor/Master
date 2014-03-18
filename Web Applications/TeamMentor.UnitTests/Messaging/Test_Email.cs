using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_Email : TM_XmlDatabase_InMemory
    {        
        public string       to         = TMConfig.Current.TMSecurity.Default_AdminEmail; 
        public string       subject    = "TM Email Test";
        public string       message    = "This is a test email from TeamMentor";
        public SendEmails   sendEmails;

        [SetUp]
        public void SetUp()
        {
            SendEmails.Disable_EmailEngine = false;
            SendEmails.Send_Emails_As_Sync = true;

            
            sendEmails = new SendEmails();
            Assert.IsNotNull(sendEmails);
            Assert.IsNull(sendEmails.Smtp_Password , "In UnitTests SendEmails SMTP password should not be set");
            Assert.IsTrue(sendEmails.serverNotConfigured()   , "In UnitTests serverNotConfigured should be in offline mode");            
        }

        [Test]
        public void Check_TM_Server_URL()
        {
            var tmServerUrl = SendEmails.TM_Server_URL;
            Assert.IsEmpty (tmServerUrl);
            Assert.AreEqual(tmServerUrl,"http://localhost");
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
        
        [Test]
        public void Send_Welcome_Email()
        {
            var emailTo   = "qa@teammentor.net";
            var userName  = "username".add_RandomLetters(5);
            var firstName = "Jonh";
            var lastName  = "Smith";
            var tmUser = new TMUser();
            var sentEmailsCount = SendEmails.Sent_EmailMessages.size();
            
            //try with no values
            var emailThread1 = tmUser.email_NewUser_Welcome();
            Assert.IsNull(emailThread1);
            Assert.AreEqual(sentEmailsCount, SendEmails.Sent_EmailMessages.size());

            //adding valid Email
            tmUser.EMail = emailTo;
            var emailThread2 = tmUser.email_NewUser_Welcome();
            Assert.IsNull(emailThread2);
            Assert.AreEqual(sentEmailsCount, SendEmails.Sent_EmailMessages.size());

            //adding valid username
            tmUser.UserName = userName;
            var emailThread3 = tmUser.email_NewUser_Welcome();
            Assert.IsNull(emailThread3);
            Assert.AreEqual(sentEmailsCount, SendEmails.Sent_EmailMessages.size());

            //adding valid serverUrl (email should be sent now)
            SendEmails.TM_Server_URL = "http://localhost:88/";
            tmUser.email_NewUser_Welcome().Join();                      // join will wait until the email thread completes execution
            var lastMessageSent1 = SendEmails.Sent_EmailMessages.last();
            
            Assert.AreEqual(sentEmailsCount+1, SendEmails.Sent_EmailMessages.size());
            Assert.AreEqual(lastMessageSent1.To, emailTo);
            Assert.AreEqual(lastMessageSent1.Subject, TMConsts.EMAIL_SUBJECT_NEW_USER_WELCOME);
            Assert.IsTrue (lastMessageSent1.Message.contains("Sent by TeamMentor."));
            Assert.IsTrue (lastMessageSent1.Message.contains("Hi , welcome to TeamMentor."));
            
            //adding a valid firstName
            tmUser.FirstName = firstName;
            tmUser.email_NewUser_Welcome().Join();                     
            var lastMessageSent2 = SendEmails.Sent_EmailMessages.last();
            lastMessageSent2.toXml().info();
            Assert.IsTrue(lastMessageSent2.Message.contains("Hi Jonh, welcome to TeamMentor."), "Couldn't Found John (firstname)");

            //adding a valid firstName
            tmUser.LastName = lastName;
            tmUser.email_NewUser_Welcome().Join();
            var lastMessageSent3 = SendEmails.Sent_EmailMessages.last();
            Assert.IsTrue(lastMessageSent3.Message.contains("Hi Jonh Smith, welcome to TeamMentor."), "Found John Smith (firstname + lastname)");
        }

        [Test]
        public void MessageBody_Is_Correct()
        {
            const string serverUrl = @"https://www.teammentor.net";
            const string username = "tmadmin";
            var tmMessage = TMConsts.EMAIL_BODY_NEW_USER_WELCOME.format(serverUrl, username);
            var expectedMessage =
                "Hello,\r\n\r\nIt's a pleasure to confirm that a new TeamMentor account has been created for you and that you'll now be able to access\r\nthe entire set of guidance available in the TM repository.\r\n\r\nTo access the service:\r\n\r\n- Go to {0} and login at the top right-hand corner of the page.\r\n- Use your username : {1}.\r\n\r\nThanks,\r\n\r\n".format(serverUrl,username);
            Assert.IsTrue(tmMessage == expectedMessage);
        }

        //    var stmpServerOnline = Mail.isMailServerOnline(sendEmails.Smtp_Server);
        //    Assert.IsTrue(stmpServerOnline);

    }
}
