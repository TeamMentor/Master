using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FluentSharp.CoreLib
{
    public static class Extra_Web_Encoding_ExtensionMethods
    {
        public static string urlEncode(this String stringToEncode)
        {            
            return HttpUtility.UrlEncode(stringToEncode);
        }
        public static string urlDecode(this String stringToDecode)
        {
            return HttpUtility.UrlDecode(stringToDecode);            
        }
        public static string htmlEncode(this String stringToEncode)
        {
            return HttpUtility.HtmlEncode(stringToEncode);
        }
        public static string htmlDecode(this String stringToDecode)
        {
            return HttpUtility.HtmlDecode(stringToDecode);
        } 
    }
}
