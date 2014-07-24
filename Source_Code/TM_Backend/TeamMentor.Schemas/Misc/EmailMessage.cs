using System;

namespace TeamMentor.CoreLib
{
    //[Serializable]   // was throwing REST deserialization error
    public class EmailMessage_Post //: MarshalByRefObject
    {
        public string To            { get; set; }
        public string Subject       { get; set; }
        public string Message       { get; set; }
    }
    //[Serializable]
    public class EmailMessage : EmailMessage_Post
    {
        public string     From          { get; set; }                
        public bool       HtmlMessage   { get; set; }                
        public long       Created_Date  { get; set; }
        public long       Sent_Date     { get; set; }
        public SentStatus SentStatus    { get; set; }        

        public EmailMessage()
        {
            Created_Date = DateTime.Now.ToFileTimeUtc();
            SentStatus   = SentStatus.New;
        }

        public EmailMessage(EmailMessage_Post emailMessage_Post) : this()
        {
            To      = emailMessage_Post.To;
            Subject = emailMessage_Post.Subject;
            Message = emailMessage_Post.Message;
        }
    }

    [Serializable]
    public enum SentStatus
    {
        New,
        Offline,
        Sending,
        NoConfig,
        Error,
        Sent,
        NotSent
    }

}
