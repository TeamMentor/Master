using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    [DataContract] 
    public class UserActivity
    {
        [DataMember] [XmlAttribute] public string	Action		{ get; set; }
        [DataMember] [XmlAttribute]  public string	Detail		{ get; set; }
        [DataMember] [XmlAttribute]  public string	Who		    { get; set; }
        [DataMember] [XmlAttribute]  public string	IPAddress	{ get; set; }
        [DataMember] [XmlAttribute]  public long    When		{ get; set; }        
        [DataMember] [XmlAttribute]  public string  When_JS		{ get; set; }
    }

    public class UserActivities
    {
        public static UserActivities Current { get; set; }

        public List<UserActivity> ActivitiesLog { get; set; }



        static UserActivities()
        {
            Current = new UserActivities();
        }
        public UserActivities()
        {
            ActivitiesLog = new List<UserActivity>();
        }
    }

    public static class UserActivities_ExtensionMethods
    {
        public static UserActivity newUserActivity  (this UserActivities userActivities, string who, string action, string detail)
        {             
            if (userActivities.notNull())
            {
                return new UserActivity
                    {
                        Action    = action, 
                        Detail    = detail, 
                        Who       = who,
                        When      = DateTime.Now.ToFileTimeUtc(),
                        When_JS   = DateTime.Now.jsDate(),
                        IPAddress = HttpContextFactory.Request.ipAddress()
                    };
            }
            return null;
        }
        /*public static UserActivity logUserActivity  (this UserActivities userActivities, string who, string action, string detail)
        {
            var userActivity = userActivities.newUserActivity(who,action,detail);
            return userActivities.logUserActivity(userActivity, null);                                
        } */       
        public static UserActivity logUserActivity  (this UserActivities userActivities, UserActivity userActivity, TMUser tmUser)
        {
            if (userActivities.notNull() && userActivity.notNull())
            {
                userActivities.ActivitiesLog.Add(userActivity);	
                userActivity.firebase_Log();
                if (tmUser.notNull() && tmUser.ID != Guid.Empty)
                {                
                    tmUser.UserActivities.Add(userActivity);
                    tmUser.saveTmUser();
                }                  	
	        }
            return userActivity;
        }        
        public static UserActivity logUserActivity  (this TM_WebServices tmWebServices, string action, string detail)
        {
            if (tmWebServices.notNull())
            {
                var currentUser = tmWebServices.Current_User();
                if (currentUser.notNull())
                {
                    return currentUser.UserName.tmUser().logUserActivity(action, detail);
                }
                return tmWebServices.logTBotActivity(action, detail);
            }
            return null;
        }
        public static UserActivity logTBotActivity  (this TM_WebServices tmWebServices, string action, string detail)
        {
            if (tmWebServices.notNull())
                return tmWebServices.tmXmlDatabase.logTBotActivity(action,detail);
            return null;
        }
        public static UserActivity logTBotActivity  (this TM_Xml_Database tmXmlDatabase, string action, string detail)
        {            
            return tmXmlDatabase.userData().logTBotActivity(action, detail);   
        }
        public static UserActivity logTBotActivity  (this TM_UserData userData, string action, string detail)
        {
            var userActivities = UserActivities.Current;
            if (userActivities.notNull())
            {
                   var userActivity = userActivities.newUserActivity("TBot",action,detail);
                return userActivities.logUserActivity(userActivity, null);                                                
            }
            return null;
        }
        public static UserActivity logUserActivity  (this TMUser         tmUser        , string action, string detail)
        {
            try
            {            
                var userActivites = UserActivities.Current;
                if (userActivites.notNull())
                {
                    var who           = tmUser.notNull() ? tmUser.UserName :"[NoUser]";
                    var userActivity  = userActivites.newUserActivity(who, action,detail);                    
                    return userActivites.logUserActivity(userActivity, tmUser);
                }
            }
            catch (Exception ex)
            {
                ex.log("[logUserActivity]");
            }
            return null;
        }
        
        public static UserActivities reset(this UserActivities userActivites)
        {
            userActivites.ActivitiesLog.clear();
            return userActivites;
        }
    }
}
