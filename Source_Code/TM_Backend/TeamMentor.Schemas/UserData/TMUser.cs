using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{                
    [Serializable]
    public class TMUser : MarshalByRefObject
    {
        [XmlAttribute] public Guid		ID          { get; set; }
        [XmlAttribute] public int		UserID      { get; set; }
        [XmlAttribute] public string	UserName    { get; set; }
        [XmlAttribute] public string	FirstName   { get; set; }
        [XmlAttribute] public string	LastName    { get; set; }
        [XmlAttribute] public string	Title       { get; set; }
        [XmlAttribute] public string	Company     { get; set; }
        [XmlAttribute] public string	EMail       { get; set; }        
        [XmlAttribute] public string	Country     { get; set; }
        [XmlAttribute] public string	State       { get; set; }
        [XmlAttribute] public int		GroupID     { get; set; }                
        
        [XmlElement]   public List<UserTag>         UserTags	    { get; set; }
        [XmlElement]   public UserSecretData        SecretData	    { get; set; }        
        [XmlElement]   public List<UserSession>     Sessions        { get; set; }
        [XmlElement]   public List<AuthToken>       AuthTokens      { get; set; }
        [XmlElement]   public UserAccountStatus     AccountStatus	{ get; set; }
        [XmlElement]   public UserStats             Stats	        { get; set; }
        [XmlElement]   public List<UserActivity>    UserActivities  { get; set; }
          

        public TMUser()
        {
            ID = Guid.NewGuid();
            UserTags        = new List<UserTag>();
            SecretData      = new UserSecretData
                                    {                                        
                                        PasswordResetToken  = null,                  // default to Null
                                        EnableUserToken = Guid.Empty
                                    };
            Sessions        = new List<UserSession>();
            AuthTokens      = new List<AuthToken>();
            UserActivities  = new List<UserActivity>();
            AccountStatus   = TMConfig.Current.isNull() 
                                    ? new UserAccountStatus()
                                    : new UserAccountStatus
                                            {
                                                ExpirationDate      = default(DateTime), //TMConfig.Current.currentExpirationDate(), 
                                                PasswordExpired     = false,
                                                AccountNeverExpires = TMConfig.Current.TMSecurity.NewAccounts_DontExpire,
                                                UserEnabled         = TMConfig.Current.TMSecurity.NewAccounts_Enabled //TMConfig.Current.newAccountsEnabled()                                        
                                            };
            Stats           = new UserStats
                                    {
                                        CreationDate = DateTime.Now                                       
                                    };                
        }
    }
    
    [DataContract]  
    public class UserTag : MarshalByRefObject
    {
        [XmlAttribute][DataMember][Required][StringLength(255)] public string   Key		        { get; set; }
        [XmlAttribute][DataMember][Required][StringLength(255)] public string   Value		        { get; set; }        
    }
    
    public class UserSecretData : MarshalByRefObject
    {
        [XmlAttribute]	public string   PasswordHash		{ get; set; }        
        [XmlAttribute]	public string   PasswordResetToken	{ get; set; }
        [XmlAttribute]	public string   DecryptionKey       { get; set; }                       
        [XmlAttribute]  public string	CSRF_Token          { get; set; }        
        [XmlAttribute]  public string   PostLoginView       { get; set; }
        [XmlAttribute]  public string   PostLoginScript     { get; set; }
        [XmlAttribute]	public Guid     EnableUserToken		{ get; set; }
    }
    
    public class UserSession : MarshalByRefObject
    {
        [XmlAttribute]	public Guid     SessionID		    { get; set; }
        [XmlAttribute]	public DateTime CreationDate		{ get; set; }
        [XmlAttribute]	public string   IpAddress		    { get; set; }
        [XmlAttribute]	public string   LoginMethod		    { get; set; }
    }
    
    public class AuthToken : MarshalByRefObject
    {
        [XmlAttribute]	public Guid      Token		        { get; set; }
        [XmlAttribute]	public DateTime  CreationDate		{ get; set; }        
    }
    
    public class UserAccountStatus : MarshalByRefObject
    {
        [XmlAttribute]	public bool     AccountNeverExpires	{ get; set; }
        [XmlAttribute]	public DateTime ExpirationDate		{ get; set; }
        [XmlAttribute]	public bool     PasswordExpired		{ get; set; }
        [XmlAttribute]	public bool     UserEnabled		    { get; set; }        
        
    }
    
    public class UserStats : MarshalByRefObject
    {
        [XmlAttribute]	public DateTime CreationDate		{ get; set; }		        
    }
}

