using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using FluentSharp.CoreLib;


namespace FluentSharp
{

    public static class Extra_ExtensionMethods_Web
    {
       /* public static WebHeaderCollection HEAD_Headers(this Uri uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Timeout = 1000;
            request.AllowAutoRedirect = false;            
            request.Method = "HEAD";
            try
            {
                return request.GetResponse().Headers;                                
            }
            catch (WebException)
            {                
                return null;
            }
        }
        public static bool HEAD(this Uri uri)
        {
            return uri.HEAD_Headers().notNull();
        }*/
    }

    public static class Extra_ExtensionMethods_DateTime
    {
        /*public static DateTime fromFileTimeUtc(this long fileTimeUtc)
        {
            return DateTime.FromFileTimeUtc(fileTimeUtc); 
        }*/
    }

    public static class Extra_ExtensionMethods_Collections
    {
        /*public static List<string>  toStringList(this List<Guid> guids)
        {
            return (from guid in guids
                    select guid.str()).toList();
        }*/

       /* public static bool notContains(this List<string> list, string stringToNotFind)
        {
            return list.contains(stringToNotFind).isFalse();
        }*/

        public static List<T> reverse<T>(this List<T> list)
        {
            if (list.notNull())
                list.Reverse();
            return list;
        }
    }

    public static class Extra_ExtensioMethods_Cookies
    {        
        public static HttpCookie value(this HttpCookie httpCookie, string newValue)
        {
            if (httpCookie.notNull())
                httpCookie.Value = newValue;
            return httpCookie;
        }

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

        public static bool hasCookie(this HttpRequestBase response, string cookieName)
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

        public static HttpCookie set_Cookie(this HttpResponseBase response, string name, string value)
        {
            if (response.isNull())
                return null;
            var httpCookie = (response.hasCookie(name))
                                ? response.Cookies[name]
                                : new HttpCookie(name);
            if (httpCookie.isNull())
                return null;
            httpCookie.Value = value;

            if ((response.hasCookie(name).isFalse()))
                response.Cookies.Add(httpCookie);
            return httpCookie;
        }

        public static HttpCookie set_Cookie(this HttpRequestBase request, string name, string value)
        {
            if (request.isNull())
                return null;
            var httpCookie = (request.hasCookie(name))
                                ? request.Cookies[name]
                                : new HttpCookie(name);
            if (httpCookie.isNull())
                return null;
            httpCookie.Value = value;

            if ((request.hasCookie(name).isFalse()))
                request.Cookies.Add(httpCookie);
            return httpCookie;
        }

        public static HttpCookie httpOnly(this HttpCookie httpCookie)
        {
            return httpCookie.httpOnly(true);
        }

        public static HttpCookie httpOnly(this HttpCookie httpCookie, bool value)
        {
            if (httpCookie.notNull())
                httpCookie.HttpOnly = value;
            return httpCookie;
        }
        
    }
}