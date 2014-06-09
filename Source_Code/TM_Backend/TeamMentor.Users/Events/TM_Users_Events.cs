using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Events
    {
        public static Events_TM_UserData events(this TM_UserData userData)
        {
            return (userData.notNull())
                ? userData.Events
                : null;            ;
        }
        public static bool event_TM_Config_Changed(this TM_UserData userData)
        {
            if (userData.events().notNull())
            {
                return userData.events().After_TM_Config_Changed.raise()
                                .Last_Exception.isNull();
            }
            return true;
        }        
    }         
    public static class TM_User_Events
    {
        public static TMUser event_User_Updated(this TMUser tmUser)
        {
            var tmUserData = TM_UserData.Current;
            if (tmUserData.notNull())
                tmUserData.Events.User_Updated.raise(tmUser);
            return tmUser;
        }
    }
}
