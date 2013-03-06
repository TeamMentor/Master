using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class TM_User
    {
        [DataMember]	public int		UserId		    { get; set; }
        [DataMember]	public string	UserName	    { get; set; }
        [DataMember]	public string	FirstName	    { get; set; }
        [DataMember]	public string	LastName	    { get; set; }
        [DataMember]	public string	Email		    { get; set; }
        [DataMember]	public string	Company		    { get; set; }
        [DataMember]	public string	Title		    { get; set; }
        [DataMember]	public string	Country		    { get; set; }
        [DataMember]	public string	State		    { get; set; }
        [DataMember]	public Int64	CreatedDate	    { get; set; }
        [DataMember]    public string   CSRF_Token      { get; set; }         
        [DataMember]    public DateTime ExpirationDate  { get; set; } 
        [DataMember]    public bool     PasswordExpired { get; set; } 
        [DataMember]    public bool     UserEnabled     { get; set; } 
    }


    public static class TMUser_ExtensionMethod
    {
        public static List<TM_User> users(this List<TMUser> tmUsers)
        {
            return (from tmUser in tmUsers select tmUser.user()).toList();
        }
        public static TM_User user(this TMUser tmUser)
        {
            if (tmUser.isNull())
                return null;
            return new TM_User
                {
                    CreatedDate = tmUser.Stats.CreationDate.ToFileTimeUtc(),
                    Company		= tmUser.Company,
                    Email		= tmUser.EMail,
                    FirstName	= tmUser.FirstName,
                    LastName	= tmUser.LastName,                    
                    Title		= tmUser.Title,
                    Country		= tmUser.Country,
                    State		= tmUser.State,
                    UserId		= tmUser.UserID,
                    UserName	= tmUser.UserName,
                    CSRF_Token  = tmUser.SecretData.CSRF_Token,
                    ExpirationDate  = tmUser.AccountStatus.ExpirationDate,
                    PasswordExpired = tmUser.AccountStatus.PasswordExpired,
                    UserEnabled     = tmUser.AccountStatus.UserEnabled
                };
        }
        public static NewUser newUser(this TM_User user)
        {
            return new NewUser
                {
                    company = user.Company,                    
                    email = user.Email,
                    firstname = user.FirstName,
                    lastname = user.LastName,
                    title = user.Title,                    
                    username = user.UserName,                    
                    country = user.Country,
                    state = user.State
                };
        }
    }
}