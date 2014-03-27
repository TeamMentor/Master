using FluentSharp;
using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
    [TestFixture]
    public class Test_REST_Admin : TM_Rest_Direct
    {
        public Test_REST_Admin()
        {
            SendEmails.Disable_EmailEngine = false;
            UserGroup.Admin.setThreadPrincipalWithRoles();
        }
        
        [Test] public void SendEmail()
        {            
            var emailsSent_Before = SendEmails.Sent_EmailMessages.size();
            var to                = "tm@si.com";
            var subject           = "a subject";
            var message           = "a message";
            var extraText         = "\r\nSent by TeamMentor. \r\n\r\n";
            var emailMessagePost  = new EmailMessage_Post 
                                        {
                                            To    = to,
                                            Subject = subject,
                                            Message = message
                                        };
            TmRest.SendEmail(emailMessagePost);
            
            var sentMessages      = SendEmails.Sent_EmailMessages;
            var emailsSent_After = sentMessages.size();
            var lastMessage      = sentMessages.last();
   
            Assert.IsTrue     (new SendEmails().serverNotConfigured());
                        
            Assert.AreEqual   (emailsSent_Before +1, emailsSent_After);
            Assert.AreEqual   (lastMessage.To      , emailMessagePost.To);
            Assert.AreEqual   (lastMessage.Subject , emailMessagePost.Subject);
            Assert.AreEqual   (lastMessage.Message , emailMessagePost.Message + extraText );
        }
    }
}
