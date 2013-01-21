using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

//From FluentSharp.BCL WebEncoding.cs file
namespace O2.DotNetWrappers.ExtensionMethods
{
    public class WebEncoding
    {
        public static string urlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        public static string urlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public static string htmlEncode(string html)
        {
            return HttpUtility.HtmlEncode(html);
        }

        public static string htmlDecode(string html)
        {
            return HttpUtility.HtmlDecode(html);
        }
    }

    public static class Web_ExtensionMethods
    {
        public static string urlEncode(this String stringToEncode)
        {
            return WebEncoding.urlEncode(stringToEncode);
        }

        public static string urlDecode(this String stringToEncode)
        {
            return WebEncoding.urlDecode(stringToEncode);
        }

        public static string htmlEncode(this String stringToEncode)
        {
            return WebEncoding.htmlEncode(stringToEncode);
        }

        public static string htmlDecode(this String stringToEncode)
        {
            return WebEncoding.htmlDecode(stringToEncode);
        }

        public static byte asciiByte(this char charToConvert)
        {
            try
            {
                return System.Text.ASCIIEncoding.ASCII.GetBytes(new char[] { charToConvert })[0];
            }
            catch
            {
                return default(byte);
            }
        }

/*        public static byte[] asciiBytes(this string stringToConvert)
        {
            return System.Text.ASCIIEncoding.ASCII.GetBytes(stringToConvert);
        }
        */

        public static string base64Encode(this string stringToEncode)
        {
            try
            {
                return System.Convert.ToBase64String(stringToEncode.asciiBytes());
            }
            catch (Exception ex)
            {
                ex.log("in base64Encode");
                return "";
            }
        }
        public static string base64Encode(this byte[] bytesToEncode)
        {
            try
            {
                return System.Convert.ToBase64String(bytesToEncode);
            }
            catch (Exception ex)
            {
                ex.log("in base64Encode");
                return "";
            }
        }

        public static string base64Decode(this string stringToDecode)
        {
            try
            {
                return System.Convert.FromBase64String(stringToDecode).ascii(); ;
            }
            catch (Exception ex)
            {
                ex.log("in base64Decode");
                return "";
            }
        }

        public static byte[] base64Decode_AsByteArray(this string stringToDecode)
        {
            try
            {
                return System.Convert.FromBase64String(stringToDecode);
            }
            catch (Exception ex)
            {
                ex.log("in base64Decode");
                return null;
            }
        }
    }
}