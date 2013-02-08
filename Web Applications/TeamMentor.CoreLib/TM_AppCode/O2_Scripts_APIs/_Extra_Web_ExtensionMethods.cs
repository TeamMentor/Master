using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

//From FluentSharp.BCL WebEncoding.cs file

namespace O2.DotNetWrappers.ExtensionMethods
{
    public static class Extra_Cookies
    {
        public static string value(this HttpCookie httpCookie)
        {
            return httpCookie.notNull()
                       ? httpCookie.Value
                       : null;
        }
    }
}