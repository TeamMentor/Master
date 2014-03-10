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
            UserGroup.Admin.setThreadPrincipalWithRoles();
        }

        //O2 Script Library
        [Test] public void CompileAllScripts()
        {
            PublicDI.log.writeToDebug(true);
            CompileEngine.clearCompilationCache();
            foreach (var method in typeof (O2_Script_Library).methods())
            {
                var code = method.invoke().str();
                var assembly = code.compileCodeSnippet();
                Assert.IsNotNull(assembly, "Failed for compile {0} with code: \n\n {1}".format(method.Name, code));
                "Compiled OK: {0}".info(method.Name);                
            }
        }
        [Test] public void Invoke_O2_Script_Library()
        {
            var result = TmRest.Admin_InvokeScript("AAAAAAAAAA");            
            Assert.AreEqual("script not found", result);
            result = TmRest.Admin_InvokeScript("ping");            
            Assert.AreEqual("pong", result);
            
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
            Assert.Greater    (emailsSent_Before   , 0);
            Assert.AreNotEqual(emailsSent_Before   , emailsSent_After);
            Assert.AreEqual   (lastMessage.To      , emailMessagePost.To);
            Assert.AreEqual   (lastMessage.Subject , emailMessagePost.Subject);
            Assert.AreEqual   (lastMessage.Message , emailMessagePost.Message + extraText );
        }
    }
}
