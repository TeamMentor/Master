using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_WebServices_Logging_ExtensionMethods
    {
        public static UserActivity logUserActivity(this TM_WebServices tmWebServices, string action, string detail)
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

        public static UserActivity logTBotActivity(this TM_WebServices tmWebServices, string action, string detail)
        {
            if (tmWebServices.notNull())
                return tmWebServices.tmXmlDatabase.logTBotActivity(action, detail);
            return null;
        }
    }
}
