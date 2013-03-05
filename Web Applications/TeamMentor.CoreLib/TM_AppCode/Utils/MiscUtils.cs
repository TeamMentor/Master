using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class MiscUtils
    {
        public static Uri googleIpAddress = "http://173.194.66.99".uri();
        public static bool online()
        {
            return googleIpAddress.HEAD();
        }
    }
}
