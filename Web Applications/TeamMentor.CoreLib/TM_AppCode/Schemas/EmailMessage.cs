using System;

namespace TeamMentor.CoreLib
{
    public class EmailMessage
    {
        public string To            { get; set; }
        public string From          { get; set; }
        public string Subject       { get; set; }
        public string Message       { get; set; }
        public bool   HtmlMessage   { get; set; }        
        public bool   Sent          { get; set; }        
        public long   Created_Date  { get; set; }
        public long   Sent_Date     { get; set; }

        public EmailMessage()
        {
            Created_Date = DateTime.Now.ToFileTimeUtc();
        }
    }
}
