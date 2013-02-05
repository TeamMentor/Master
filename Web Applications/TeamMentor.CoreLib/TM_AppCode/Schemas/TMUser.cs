using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib
{    
    [Serializable]            
    public class TMUser
    {
        [XmlAttribute] public Guid		ID { get; set; }
        [XmlAttribute] public int		UserID { get; set; }
        [XmlAttribute] public string	UserName { get; set; }
        [XmlAttribute] public string	FirstName { get; set; }
        [XmlAttribute] public string	LastName { get; set; }
        [XmlAttribute] public string	Title { get; set; }
        [XmlAttribute] public string	Company { get; set; }
        [XmlAttribute] public string	EMail { get; set; }
        [XmlAttribute] public int		GroupID { get; set; }
        [XmlAttribute] public bool		IsActive { get; set; }
        [XmlAttribute] public string	CSRF_Token { get; set; }
        
        [XmlAttribute] public string    SSOKey               { get; set; }
        [XmlAttribute] public string    PasswordHash         { get; set; }
        [XmlAttribute] public string    PostLoginView        { get; set; }
        [XmlAttribute] public string    PostLoginScript      { get; set; }
        

        [XmlElement]   public UserStats             Stats	        { get; set; }
        [XmlElement]   public List<UserActivity>    UserActivities  { get; set; }

        public TMUser()
        {
            ID = Guid.NewGuid();
            UserActivities = new List<UserActivity>();

            Stats = new UserStats
                {
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(TMConfig.Current.Eval_Accounts.Days)
                };
        }
    }

    public class UserStats
    {
        [XmlAttribute]	public DateTime ExpirationDate		{ get; set; }
        [XmlAttribute]	public DateTime CreationDate		{ get; set; }		
        [XmlAttribute]	public DateTime LastLogin			{ get; set; }
        [XmlAttribute] public int       Stats_LoginOk		{ get; set; }
        [XmlAttribute] public int       Stats_LoginFail     { get; set; }
    }

}

