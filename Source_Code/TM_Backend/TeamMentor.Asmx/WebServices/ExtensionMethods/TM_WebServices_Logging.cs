using FluentSharp.CoreLib;
using TeamMentor.FileStorage;

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
                return tmWebServices.tmFileStorage.logTBotActivity(action, detail);                
            return null;
        }
        public static UserActivity logTBotActivity(this TM_FileStorage tmFileStorage, string action, string detail)
        {
            if (tmFileStorage.notNull() )
                return tmFileStorage.UserData.logTBotActivity(action, detail);
            return null;
        }
        public static UserActivity logTBotActivity(this TM_Xml_Database tmXmlDatabase, string action, string detail)
        {
            if (tmXmlDatabase.notNull() && TM_FileStorage.Current.notNull())
                return TM_FileStorage.Current.logTBotActivity(action, detail);
            return null;
        }
    }
}
