using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using FluentSharp.CoreLib;


namespace FluentSharp.CoreLib
{
    public static class Extra_ExtensionMethods_StringBuilder
    {
        public static StringBuilder appendLine(this StringBuilder stringBuilder, string line)
        {
            if (stringBuilder.notNull() && line.notNull())
                stringBuilder.AppendLine(line);
            return stringBuilder;
        }
        public static StringBuilder appendLines(this StringBuilder stringBuilder, params string[] lines)
        {
            if (stringBuilder.notNull() && lines.notNull())
                foreach(var line in lines)
                    stringBuilder.appendLine(line);
            return stringBuilder;
        }
    }

    public static class Extra_ExtensionMethods_DateTime
    {
        public static string  jsDate(this DateTime date)
        {
            return date.toJsDate();
        }
        public static string  toJsDate(this DateTime date)
    	{
            if (date == default(DateTime))
                return String.Empty;
		    var dateTime_1970       = new DateTime(1970, 1, 1);
    		var dateTime_Universal  = date.ToUniversalTime();
    		var timeSpan            = new TimeSpan(dateTime_Universal.Ticks - dateTime_1970.Ticks);
    		return timeSpan.TotalMilliseconds.ToString("#");
    	}
        public static DateTime fromJsDate(this string date_Milliseconds_After_1970)
        {
            if (date_Milliseconds_After_1970.valid() && date_Milliseconds_After_1970.isDouble())
            {
                var dateTime = new DateTime(1970, 1, 1);
                return dateTime.AddMilliseconds(date_Milliseconds_After_1970.toDouble());                
            }
            return default(DateTime);
        }
    }
    public static class Extra_ExtensionMethods_Double
    {
        public static bool isDouble(this string value)
        {
            double dummyDouble;
            return Double.TryParse(value, out dummyDouble);
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
        
        //httpRequestBase

        public static bool hasCookie(this HttpRequestBase response, string cookieName)
        {
            return response.notNull() &&
                   response.Cookies.notNull() &&
                   response.Cookies[cookieName].notNull();
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
        public static string cookie(this HttpRequestBase request, string cookieName)
        {
            if (request.hasCookie(cookieName))
                return request.Cookies[cookieName].value();
            return null;                       
        }
        public static bool hasHeader(this HttpRequestBase response, string headerName)
        {
            return response.notNull() &&
                   response.Headers.notNull() &&
                   response.Headers[headerName].notNull();
        }

        public static string header(this HttpRequestBase request, string headerName)
        {
            if (request.hasHeader(headerName))
                return request.Headers[headerName];
            return null;                       
        }        
    }
}