using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UserData
{
    public static class FileStorage_Utils
    {
        public static TM_UserData                    useFileStorage(this TM_UserData userData, bool value)
        {
            if (userData.tmServer().notNull())
                //userData.tmServer().UseFileStorage = value;
                TM_Server.UseFileStorage = value;
            return userData;
        }
        public static TM_UserData                    useFileStorage(this TM_UserData userData)
        {
            return userData.useFileStorage(true);
        }
        public static bool                           usingFileStorage(this TM_UserData userData)
        {
            if (userData.tmServer().notNull())
                //return userData.tmServer().UseFileStorage;
                return TM_Server.UseFileStorage;
            return false;
        }
    }
}
