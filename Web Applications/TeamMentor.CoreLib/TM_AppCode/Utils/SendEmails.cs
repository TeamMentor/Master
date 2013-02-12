using System;
using System.Net.Mail;
using System.Net.Mime;
using O2.DotNetWrappers.Network;

namespace TeamMentor.CoreLib
{
    public class SendEmails
    {        
        public static string SmtpServer            { get; set; }
        public static string SmtpServer_Username   { get; set; }
        public static string SmtpServer_Password   { get; set; }
        public static string From                  { get; set; }

        static SendEmails()
        {
            SmtpServer          = "smtp.sendgrid.net";
            SmtpServer_Username = "TeamMentor";
            SmtpServer_Password = "";
            From = "TM_Security_Dude@securityinnovation.com";
        }

        public bool send_TestEmail()
        {
            return send(From, "Test Email", "From TeamMentor");
        }

        public bool send(string to, string subject, string message, bool htmlMessage = false)
        {
            try
            {
                var stmpServerOnline = Mail.isMailServerOnline(SmtpServer);                
                
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
                var smtpClient = new SmtpClient(SmtpServer, 587);
                var credentials = new System.Net.NetworkCredential(SmtpServer_Username, SmtpServer_Password);
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
