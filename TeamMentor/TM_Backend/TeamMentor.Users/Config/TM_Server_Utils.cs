using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UserData
{
    public static class TM_Server_Utils
    {
        public static TM_Server tmServer(this TM_UserData userData)
        {
            if (userData.notNull())
                return userData.Server;
            return null;
        }
    }
}
