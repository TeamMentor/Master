using FluentSharp.CoreLib;
using FluentSharp.Moq;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

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
//            SendEmails.Send_Emails_As_Sync = true;            
            
            sendEmails = new SendEmails();
            Assert.IsNotNull(sendEmails);
            Assert.IsEmpty  (sendEmails.Smtp_Password           , "In UnitTests SendEmails SMTP password should not be set");
            Assert.IsTrue   (sendEmails.serverNotConfigured()   , "In UnitTests serverNotConfigured should be in offline mode");            
        }
        [TearDown]
        public void TearDown()                          
        {            
//            SendEmails.Send_Emails_As_Sync = false;             // restore value to its default state
        }

        [Test] public void SendEmails_Ctor()            
        {   
//            SendEmails.Send_Emails_As_Sync = false;             // is false by default
            //static vars
            Assert.IsNotNull(SendEmails.TM_Server_URL);
            Assert.IsNotNull(SendEmails.Sent_EmailMessages);
            Assert.IsFalse  (SendEmails.Disable_EmailEngine);
//            Assert.IsFalse  (SendEmails.Send_Emails_As_Sync);
            Assert.IsFalse  (SendEmails.Dont_Send_Emails);
            
            //instance vars                        
            Assert.AreEqual  (sendEmails.From           , tmConfig.TMSecurity.Default_AdminEmail      );
            Assert.AreEqual  (sendEmails.To             , tmConfig.TMSecurity.Default_AdminEmail      );
            Assert.AreEqual  (sendEmails.Smtp_Server    , userData.SecretData.SmtpConfig.Server       );
            Assert.AreEqual  (sendEmails.Smtp_UserName  , userData.SecretData.SmtpConfig.UserName     );
            Assert.AreEqual  (sendEmails.Smtp_Password  , userData.SecretData.SmtpConfig.Password     );

        }
        [Test] public void Get_TMServer_From_ConfigFile()             
        {
       
            var context = HttpContextFactory.Context.mock();
            var request = HttpContextFactory.Request;
            Assert.IsNotNull(context);
            Assert.IsNotNull(request);
            Assert.IsFalse (request.IsSecureConnection);
            var serverName     = 10.randomLetters();
            var serverPort     = 60000.random().str();             
            request.ServerVariables["Server_Name"] = serverName;
            request.ServerVariables["Server_Port"] = serverPort;
            Assert.AreEqual (request.ServerVariables["Server_Name"], serverName);
            Assert.AreEqual (request.ServerVariables["Server_Port"], serverPort);

            var serverUrl = SendEmails.TM_Server_URL;
            Assert.AreEqual(serverUrl, TMConsts.DEFAULT_TM_LOCALHOST_SERVER_URL);
            Assert.AreEqual(serverUrl, SendEmails.TM_Server_URL);
            
        }
        [Test] public void send_TestEmail()             
        {
            var emailCount = SendEmails.Sent_EmailMessages.size();
            Assert.IsFalse(sendEmails.send_TestEmail());
            var lastEmail = SendEmails.Sent_EmailMessages.last();
            Assert.AreEqual(lastEmail.Subject   , TMConsts.EMAIL_TEST_SUBJECT);
            Assert.AreEqual(lastEmail.Message   , TMConsts.EMAIL_TEST_MESSAGE.line() + TMConsts.EMAIL_DEFAULT_FOOTER);
            Assert.AreEqual(lastEmail.SentStatus, SentStatus.NoConfig);
            Assert.AreEqual(emailCount + 1      , SendEmails.Sent_EmailMessages.size());
        }
        [Test] public void send()                       
        {            
            var emailMessage = new EmailMessage 
                                    {
                                        To          = 10.randomLetters(), 
                                        From        = 10.randomLetters(), 
                                        Subject     = 10.randomLetters(), 
                                        Message     = 10.randomLetters(), 
                                        HtmlMessage = false
                                    };
            sendEmails.Smtp_Server   = 10.randomLetters();
            sendEmails.Smtp_UserName = 10.randomLetters();
            sendEmails.Smtp_Password = 10.randomLetters();

            //try with bad email values
            var result1 = sendEmails.send(emailMessage);
            Assert.IsFalse(sendEmails.serverNotConfigured());
            Assert.IsFalse(result1);            
            Assert.AreEqual(emailMessage.SentStatus, SentStatus.Error);
            //try with good emails values
            SendEmails.Dont_Send_Emails = true;
            emailMessage.To   = "".random_Email();
            emailMessage.From = "from".random_Email();
            var result2 = sendEmails.send(emailMessage);
            Assert.IsTrue(result2);    
            Assert.AreEqual(emailMessage.SentStatus, SentStatus.NotSent);
            SendEmails.Dont_Send_Emails = false;
        }
        [Test] public void Check_TM_Server_URL()        
        {
            var tmServerUrl = SendEmails.TM_Server_URL;
            Assert.IsNotNull(tmServerUrl);
            Assert.AreEqual (tmServerUrl,TMConsts.DEFAULT_TM_LOCALHOST_SERVER_URL);
        }
        [Test] public void Send_Test_Email()            
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
        [Test] public void Send_Welcome_Email()         
        {
            var emailTo   = "qa@teammentor.net";
            var userName  = "username".add_RandomLetters(5);
            var password = "!{0}1234".format(5.randomLetters());
            var tmUser =  userData.newUser(userName, password, emailTo).tmUser(); //new TMUser();
            
            Assert.AreEqual(tmUser.UserName, userName);
            Assert.AreEqual(tmUser.EMail   , emailTo);            
     
            //adding valid serverUrl (email should be sent now)
            //SendEmails.TM_Server_URL = "http://localhost:88/";
            var lastMessageSent1 = SendEmails.Sent_EmailMessages.last();            
            
            Assert.IsTrue (lastMessageSent1.Message.contains("The TEAMMentor team."));
            Assert.IsTrue(lastMessageSent1.Message.contains("It's a pleasure to confirm that a new TeamMentor"));
            
            
        }
        [Test] public void MessageBody_Is_Correct()     
        {
            const string serverUrl = @"https://www.teammentor.net";
            const string username = "tmadmin";
            var tmMessage = TMConsts.EMAIL_BODY_NEW_USER_WELCOME.format(serverUrl, username);
            var expectedMessage =
                "Hello,\r\n\r\nIt's a pleasure to confirm that a new TeamMentor account has been created for you and that you'll now be able to access\r\nthe entire set of guidance available in the TM repository.\r\n\r\nTo access the service:\r\n\r\n- Go to {0} and login at the top right-hand corner of the page.\r\n- Use your username : {1}\r\n\r\nThanks,\r\n\r\n".format(serverUrl,username);
            Assert.IsTrue(tmMessage == expectedMessage);
        }

        [Test]
        public void MessageBody_ServerAndPort_Correct()
        {
            const string serverUrl = @"https://localhost:1337";
            const string username = "tmadmin";
            var tmMessage = TMConsts.EMAIL_BODY_NEW_USER_WELCOME.format(serverUrl, username);
            var expectedMessage =
                "Hello,\r\n\r\nIt's a pleasure to confirm that a new TeamMentor account has been created for you and that you'll now be able to access\r\nthe entire set of guidance available in the TM repository.\r\n\r\nTo access the service:\r\n\r\n- Go to {0} and login at the top right-hand corner of the page.\r\n- Use your username : {1}\r\n\r\nThanks,\r\n\r\n".format(serverUrl, username);
            Assert.IsTrue(tmMessage == expectedMessage);
        }

        [Test]
        public void SendWelcomeEmailToUser_Server_ConfigValue_IsNull()
        {
           
        }
    }
}
