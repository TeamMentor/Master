using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    [Serializable] 
    public class UserActivity
    {
        [XmlAttribute] public string	Action		{ get; set; }
        [XmlAttribute] public string	Detail		{ get; set; }
        [XmlAttribute] public string	Who		    { get; set; }
        [XmlAttribute] public string	IPAddress	{ get; set; }
        [XmlAttribute] public long      When		{ get; set; }        
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

        [LogTo_GoogleAnalytics]
        public UserActivity LogUserActivity(TMUser tmUser , UserActivity userActivity)
        {
            if (tmUser.notNull() && tmUser.ID != Guid.Empty)
            {                
                tmUser.UserActivities.Add(userActivity);
                tmUser.saveTmUser();
            }  
            ActivitiesLog.Add(userActivity);			
            return userActivity;
        }


    }

    public static class UserActivities_ExtensionMethods
    {
        public static UserActivity LogUserActivity(this TM_WebServices tm_WebServices, string name, string detail)
        {
            var currentUser = tm_WebServices.Current_User();
            if (currentUser.notNull())
            {
                currentUser.UserName.tmUser().logUserActivity(name, detail);
            }
            return null;

        }

        public static UserActivity logUserActivity(this TMUser tmUser , string action, string detail)
        {
            try
            {            
                var userActivites = UserActivities.Current;
                if (userActivites.notNull())
                {
                    var userActivity = new UserActivity
                        {
                            Action    = action, 
                            Detail    = detail, 
                            Who       = tmUser.notNull() ? tmUser.UserName :"[NoUser]",
                            When      = DateTime.Now.ToFileTimeUtc(),
                            IPAddress = HttpContextFactory.Context.ipAddress()
                        };
                    return userActivites.LogUserActivity(tmUser , userActivity);
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
