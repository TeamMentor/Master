using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace O2.DotNetWrappers.ExtensionMethods
{
    public static class Extra_ExtensionMethods_Collections
    {
        public static List<string>  toStringList(this List<Guid> guids)
        {
            return (from guid in guids
                    select guid.str()).toList();
        }

        public static bool notContains(this List<string> list, string stringToNotFind)
        {
            return list.contains(stringToNotFind).isFalse();
        }

    }

    public static class Extra_ExtensioMethods_Cookies
    {        
        public static string value(this HttpCookie httpCookie)
        {
            return httpCookie.notNull()
                       ? httpCookie.Value
                       : null;
        }

        public static bool hasCookie(this HttpResponseBase response, string cookieName)
        {
            return response.notNull() &&
                   response.Cookies.notNull() &&
                   response.Cookies[cookieName].notNull();
        }

        public static string cookie(this HttpResponseBase response, string cookieName)
        {
            if (response.hasCookie(cookieName))
                return response.Cookies[cookieName].value();
            return null;                       
        }
    }
}