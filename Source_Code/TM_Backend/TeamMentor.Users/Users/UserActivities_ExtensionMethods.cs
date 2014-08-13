using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.Web;

namespace TeamMentor.CoreLib
{
    public static class UserActivities_ExtensionMethods
    {
        public static List<UserActivity> add_Activity(this List<UserActivity> userActivities, UserActivity userActivity)
        {
            try
            {
                userActivities.Add(userActivity);
            }
            catch(Exception ex)
            {
                ex.log("[List<UserActivity>][add_UserActivity]");
            }
            return userActivities;
        }
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
                if(userActivities.logging_Enabled())
                    { 
                    userActivities.ActivitiesLog.add_Activity(userActivity);	
                
                    userActivity.firebase_Log();
                    if (tmUser.notNull() && tmUser.ID != Guid.Empty)
                    {                
                        tmUser.UserActivities.add_Activity(userActivity);
                    
                        tmUser.event_User_Updated(); //tmUser.saveTmUser();
                    } 
                }
            }
            return userActivity;
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

        public static bool logging_Disabled(this UserActivities userActivities)
        {
            return TM_Server.Current.userActivities_Disable_Logging();
        }
        public static bool logging_Enabled(this UserActivities userActivities)
        {
            return userActivities.logging_Disabled().isFalse();
        }
    }
}