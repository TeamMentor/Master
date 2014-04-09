using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.CoreLib
{    
    public class SendEmails
    {   
        public static List<EmailMessage> Sent_EmailMessages  { get; set; }
        public static string             TM_Server_URL       { get; set; }
        public static bool               Disable_EmailEngine { get; set; }
        public static bool               Dont_Send_Emails    { get; set; }
        public static bool               Send_Emails_As_Sync { get; set; }

        public        string             From                { get; set; }
        public        string             Smtp_Server         { get; set; }
        public        string             Smtp_UserName       { get; set; }
        public        string             Smtp_Password       { get; set; }

        static SendEmails()
        {
            Sent_EmailMessages = new List<EmailMessage>();                     
        }


        public SendEmails()
        {
            mapTMServerUrl();  
            From = TMConfig.Current.TMSecurity.Default_AdminEmail;
            if (TM_UserData.Current.notNull())
            { 
                var secretData = TM_UserData.Current.SecretData;
                if (secretData.notNull())
                {
                    Smtp_Server   = secretData.SMTP_Server;
                    Smtp_UserName = secretData.SMTP_UserName;
                    Smtp_Password = secretData.SMTP_Password;
                }
            }
        }
        public static string mapTMServerUrl()           // this should be set by an live HTTP request
        {       
            if (TM_Server_URL.isNull())
            {
                TM_Server_URL = TMConsts.DEFAULT_TM_LOCALHOST_SERVER_URL;
                if (HttpContextFactory.Context.isNotNull() && HttpContextFactory.Request.isNotNull())
                {
                    var request = HttpContextFactory.Request;
                    var scheme = request.IsSecureConnection ? "https" : "http";
                    var serverName = request.ServerVariables["Server_Name"];
                    var serverPort = request.ServerVariables["Server_Port"];
                    if (serverName.notNull() && serverPort.notNull())                
                        TM_Server_URL = "{0}://{1}:{2}".format(scheme, serverName, serverPort);                                    
                }                            
            }
            return TM_Server_URL;
        }

        /*public SendEmails(string smtpServer, string smtpUserName, string smtpPassword ) : this()
        {
            Smtp_Server     = smtpServer;
            Smtp_UserName   = smtpUserName;
            Smtp_Password   = smtpPassword;
        }*/
        

        public bool send_TestEmail()
        {
            return send(TMConsts.EMAIL_TEST_SUBJECT, TMConsts.EMAIL_TEST_MESSAGE);
        }        
        
        public bool send(string subject, string message)
        {
            return send(From, subject, message, false);
        }
        public bool send(string to, string subject, string message)
        {
            return send(to, subject, message, false);
        }
        public bool send(string to, string subject, string message, bool htmlMessage)
        {
            var emailMessage = new EmailMessage 
                                    {
                                        To = to, 
                                        From = this.From, 
                                        Subject = subject, 
                                        Message = message, 
                                        HtmlMessage = htmlMessage
                                    };
            return send(emailMessage);
        }
        
        //Refactor into SMTP class
        public bool send(EmailMessage_Post emailMessagePost)
        {
            if (emailMessagePost.isNull())
                return false;
            var emailMessage = new EmailMessage(emailMessagePost);
            return send(emailMessage);
        }
        public bool send(EmailMessage emailMessage)
        {
            if (emailMessage.isNull())
                return false;
            emailMessage.Message += TMConsts.EMAIL_DEFAULT_FOOTER;
            Sent_EmailMessages.Add(emailMessage);

            if (emailMessage.From.notValid())
                emailMessage.From = this.From;
            TM_UserData.Current.logTBotActivity("Send Email","From: {0}    To: {1}    Subject: {2}".format(emailMessage.From, emailMessage.To, emailMessage.Subject));

            try
            {
                if (this.serverNotConfigured())
                {                    
                    emailMessage.SentStatus = SentStatus.NoConfig;
                    return false;   
                }
            
                emailMessage.SentStatus = SentStatus.Sending;
                "Sending email:\n  to: {0}\n  from: {0}\n  subject: {0} ".info(emailMessage.To, emailMessage.Subject, emailMessage.Message);
                var mailMsg = new MailMessage();
                // To
                mailMsg.To.Add(new MailAddress(emailMessage.To));
                // From
                mailMsg.From = new MailAddress(emailMessage.From);
                // Subject and multipart/alternative Body
                mailMsg.Subject = emailMessage.Subject;                
                if (emailMessage.HtmlMessage)
                {
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailMessage.Message, null, MediaTypeNames.Text.Plain));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailMessage.Message, null, MediaTypeNames.Text.Html));
                }
                else
                {
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailMessage.Message, null, MediaTypeNames.Text.Plain));
                }
                // Init SmtpClient and send
                var smtpClient = new SmtpClient(Smtp_Server, 587);
                var credentials = new System.Net.NetworkCredential(Smtp_UserName, Smtp_Password);
                
                smtpClient.EnableSsl = true;

                smtpClient.Credentials = credentials;
                if (Dont_Send_Emails)
                {
                    emailMessage.SentStatus = SentStatus.NotSent;
                }
                else
                {
                    smtpClient.Send(mailMsg);
                
                    emailMessage.SentStatus = SentStatus.Sent;
                    emailMessage.Sent_Date = DateTime.Now.ToFileTimeUtc();
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[Error sending email");
                emailMessage.SentStatus = SentStatus.Error;
                return false;
            }
        }

        //sendEmail Helpers
        public static void SendNewUserEmails(string subject, TMUser tmUser)
        {
            if(TMConfig.Current.newAccountsEnabled().isFalse())
            {
                SendNewUserEmails_UserDisabled__Workflow(subject, tmUser);
                return;
            }

            if (Disable_EmailEngine)
                return;
            var tmMessage =
@"New TeamMentor User Created:

    UserId: {0}
    UserName: {1}
    Company: {2}
    Email: {3}
    FirstName: {4}
    LastName: {5}
    Title: {6}
    Creation Date: {7}".format(tmUser.UserID, 
                              tmUser.UserName,
                              tmUser.Company,
                              tmUser.EMail,
                              tmUser.FirstName,
                              tmUser.LastName,
                              tmUser.Title,
                              tmUser.Stats.CreationDate.ToLongDateString());

            SendEmailToTM(subject, tmMessage);
            SendWelcomeEmailToUser(tmUser);
        }

        public static void SendWelcomeEmailToUser(TMUser tmUser)
        {
            if (tmUser.EMail.valid())
            {
                var subj = TMConsts.EMAIL_SUBJECT_NEW_USER_WELCOME;
                var userMessage = TMConsts.EMAIL_BODY_NEW_USER_WELCOME.format(TM_Server_URL, tmUser.UserName);
                SendEmailToEmail(tmUser.EMail, subj, userMessage);
                //userMessage = "(sent to: {0})\n\n{1}".format(tmUser.EMail, userMessage);
                //SendEmailToTM("(user email) Welcome to TeamMentor", userMessage);
            }
        }
        public static void SendNewUserEmails_UserDisabled__Workflow(string subject, TMUser tmUser)
        { 
            var subj = "Thanks for creating an TeamMentor Account"; //TMConsts.EMAIL_SUBJECT_NEW_USER_WELCOME;
            var userMessage = TMConsts.EMAIL_BODY_NEW_USER_NEEDS_APPROVAL;
            SendEmailToEmail(tmUser.EMail, subj, userMessage);

            var enableUserUrl = "{0}/aspx_pages/EnableUser.aspx?token={1}".format(SendEmails.TM_Server_URL, tmUser.enableUser_Token());
            var tmMessage =
@"Hello TeamMentor admin, the following new User request as been received:

    UserId: {0}
    UserName: {1}
    Company: {2}
    Email: {3}
    FirstName: {4}
    LastName: {5}
    Title: {6}
    Creation Date: {7}

Please click on this link if you want to approve it: {8}
".format(tmUser.UserID, 
                              tmUser.UserName,
                              tmUser.Company,
                              tmUser.EMail,
                              tmUser.FirstName,
                              tmUser.LastName,
                              tmUser.Title,
                              tmUser.Stats.CreationDate.ToLongDateString(),
                              enableUserUrl);

            //userMessage = "(sent to: {0})\n\n{1}".format(tmUser.EMail, userMessage);
            SendEmailToTM("New TeamMentor Account Request", tmMessage);
            
        }
        [Assert_Admin]
        public static void SendEmailAboutUserToTM(string action, TMUser tmUser)
        {
            var subject = "User {0} {1}".format(tmUser.UserName, action);
            var message =
@"The user {0} has just {1}

Stats:

- last  login: {2}
- login fails: {3}
- login Oks  : {4}

                ".format(tmUser.UserName, 
                         action, 
                         tmUser.Stats.LastLogin,
                         tmUser.Stats.LoginFail,
                         tmUser.Stats.LoginOk);
            SendEmailToTM(subject, message);
        }

        [Assert_Admin]
        public static bool SendPasswordReminderToUser(TMUser tmUser, Guid passwordResetToken)
        {
            try
            {                 
var userMessage =
@"Hi {0} {1},  a password reminder was requested for your account.

You can change the password of your {2} account using {4}/passwordReset/{2}/{3}

If you didn't make this request, please let us know at support@teammentor.net.
             ".format(tmUser.FirstName, tmUser.LastName, tmUser.UserName, passwordResetToken,TM_Server_URL);
             
                SendEmailToEmail(tmUser.EMail, "TeamMentor Password Reset", userMessage);
             
                userMessage = "(sent to: {0})\n\n{1}".format(tmUser.EMail, userMessage);
                SendEmailToTM("(user email) TeamMentor Password Reset", userMessage);
                return true;
            }
            catch (Exception ex)
            {
                ex.log();
            }
            return false;

        }

        public static void SendEmailToTM(string subject, string message)
        {
            if (Disable_EmailEngine)
                return;
            var thread = O2Thread.mtaThread(
                ()=>{
                        try
                        {
                            new SendEmails().send(subject, message);
                        }
                        catch (Exception ex)
                        {
                            ex.log("in SendEmailToTM");
                        }
                    });
           if(Send_Emails_As_Sync)
               thread.Join();
        }
        public static void SendEmailToEmail(string to, string subject, string message)
        {
            if (Disable_EmailEngine)
                return;
            var thread = O2Thread.mtaThread(
                ()=>{
                        try
                        {
                            new SendEmails().send(to, subject, message);
                        }
                        catch (Exception ex)
                        {
                            ex.log("in SendEmailToTM");
                        }
                    });
            if(Send_Emails_As_Sync)
               thread.Join();
        }
        
    }

    public static class SendEmail_ExtensionMethods
    {
        public static bool serverNotConfigured(this SendEmails sendEmails)
        {
            return sendEmails.Smtp_Server.notValid() ||
                   sendEmails.Smtp_UserName.notValid() ||
                   sendEmails.Smtp_Password.notValid();
        }
    }
}
