using System;
using System.Net.Mail;
using System.Net.Mime;

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
        }

        public SendEmails(string smtpServer, string smtpUserName, string smtpPassword ) : this()
        {
            Smtp_Server     = smtpServer;
            Smtp_UserName   = smtpUserName;
            Smtp_Password   = smtpPassword;
        }

        public bool send_TestEmail()
        {
            return send(From, "Test Email", "From TeamMentor");
        }

        public bool send(string to, string subject, string message)
        {
            return send(to, subject, message, false);
        }

        public bool send(string to, string subject, string message, bool htmlMessage)
        {
            try
            {                
                var mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress(to));
                // From
                mailMsg.From = new MailAddress(From);
                // Subject and multipart/alternative Body
                mailMsg.Subject = subject;                
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));
                if (htmlMessage)                                
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null,MediaTypeNames.Text.Html));                
                // Init SmtpClient and send
                var smtpClient = new SmtpClient(Smtp_Server, 587);
                var credentials = new System.Net.NetworkCredential(Smtp_UserName, Smtp_Password);
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);                                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
