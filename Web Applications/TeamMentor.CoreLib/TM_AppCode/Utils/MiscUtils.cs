using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class MiscUtils
    {
        public static Uri googleIpAddress = "http://173.194.66.99".uri();
        public static bool online()
        {
            return googleIpAddress.HEAD();
        }

        public static bool runningOnLocalHost()
        {
            try
            {
                if (HttpContextFactory.Context.notNull() && HttpContextFactory.Request.notNull())                
                    return HttpContextFactory.Request.IsLocal;
            }
            catch (Exception ex)
            {
                ex.log("[runningOnLocalHost");                
            }
            return true;        //default to true
        }
    }
}
