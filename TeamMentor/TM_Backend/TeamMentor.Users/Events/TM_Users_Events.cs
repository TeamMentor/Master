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
    public static class TM_Users_Events
    {
        public static Events_TMUser    events(this TMUser tmUser)
        {
            return (tmUser.notNull())
                ? tmUser.Events
                : null;            ;
        }    
        public static TMUser event_TmUser_Changed(this TMUser tmUser)
        {
            if (tmUser.events().notNull())
                    tmUser.events().After_User_Changed.raise();
            return tmUser;
        }

        public static TMUser event_TmUser_Deleted(this TMUser tmUser)
        {
            if (tmUser.events().notNull())
                    tmUser.events().After_User_Deleted.raise();
            return tmUser;
        }
        
    }
}
