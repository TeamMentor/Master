using System;
using System.Net.Mail;
using System.Net.Mime;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class SendEmails
    {                
        public string From          { get; set; }
        public string Smtp_Server   { get; set; }
        public string Smtp_UserName { get; set; }
        public string Smtp_Password { get; set; }

        public SendEmails()
        {            
            From = "TM_Security_Dude@securityinnovation.com";
            var secretData = TM_Xml_Database.Current.UserData.SecretData;
            if (secretData.notNull())
            {
                Smtp_Server = secretData.SMTP_Server;
                Smtp_UserName = secretData.SMTP_UserName;
                Smtp_Password = secretData.SMTP_Password;
            }
        }
        public SendEmails(string smtpServer, string smtpUserName, string smtpPassword ) : this()
        {
            Smtp_Server     = smtpServer;
            Smtp_UserName   = smtpUserName;
            Smtp_Password   = smtpPassword;
        }
        public bool send_TestEmail()
        {
            return send("Test Email", "From TeamMentor");
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
            try
            {
                if (Smtp_Password.notValid())
                {
                    "Can't sent email because the Smtp_Password is not set".error();
                    return false;
                }
                "Sending email:\n  to: {0}\n  from: {0}\n  subject: {0} ".info(to, subject, message);
                var mailMsg = new MailMessage();

                message += "Send by TeamMentor. ".format().lineBefore().lineBefore();
                // To
                mailMsg.To.Add(new MailAddress(to));
                // From
                mailMsg.From = new MailAddress(From);
                // Subject and multipart/alternative Body
                mailMsg.Subject = subject;                
                if (htmlMessage)
                {
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html));
                }
                else
                {
                    mailMsg.Body = message;
                }
                // Init SmtpClient and send
                var smtpClient = new SmtpClient(Smtp_Server, 587);
                var credentials = new System.Net.NetworkCredential(Smtp_UserName, Smtp_Password);
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);                                
                return true;
            }
            catch (Exception ex)
            {
                ex.log();
                return false;
            }
        }

        public static void SendEmailToTM(string subject, TMUser tmUser)
        {
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
            if (tmUser.EMail.valid())
            {

                var userMessage =
@"Hi {0} {1} Welcome to TeamMentor.

You can login with your {2} account at http://www.teammentor.net

TeamMentor Team.
".format(tmUser.FirstName, tmUser.LastName, tmUser.UserName);
                SendEmailToEmail(tmUser.EMail, "Welcome to TeamMentor", userMessage);
                userMessage = "(sent to: {0})\n\n{1}".format(tmUser.EMail, userMessage);
                SendEmailToTM("(user email) Welcome to TeamMentor", userMessage);
            }

        }

        public static void SendEmailToTM(string subject, string message)
        {
            O2Thread.mtaThread(
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
        }
        public static void SendEmailToEmail(string to, string subject, string message)
        {
            O2Thread.mtaThread(
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
        }
    }
}
