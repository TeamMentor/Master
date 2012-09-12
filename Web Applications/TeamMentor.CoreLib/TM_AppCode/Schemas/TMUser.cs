using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    [System.SerializableAttribute()]        
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://TeamMentor.securityinnovation.com:13415/")]
    public partial class TMUser
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string EMail { get; set; }
        public int    GroupID { get; set; }
        public bool   IsActive { get; set; }
        public string CSRF_Token { get; set; }

        //new ones
        [XmlAttribute] public System.DateTime ExpirationDate { get; set; }
        
        [XmlAttribute] public DateTime  Stats_CreationDate   { get; set; }
        //[XmlAttribute] public int      Stats_LoginOk        { get; set; }
        //[XmlAttribute] public int      Stats_LoginFail      { get; set; }
        [XmlAttribute] public DateTime  Stats_LastLogin      { get; set; }
        [XmlAttribute] public string    SSOKey               { get; set; }
        [XmlAttribute] public string    PostLoginView        { get; set; }
        [XmlAttribute] public string    PostLoginScript      { get; set; }
    }

}
