using System;
using System.Web;
using FluentSharp;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class HttpContextFactory
    {
        private static HttpContextBase _context; 		

        public static HttpContextBase       Current
        {
            get
            {
                if (_context.notNull())                                 // context has been set
                    return _context;
                if (HttpContext.Current.isNull())                       // context has not been set and we are not inside ASP.NET
                    return null;
                return new HttpContextWrapper(HttpContext.Current);     // return current asp.net Context			    
            }
        }
        public static HttpContextBase       Context     { 	get { return Current;           } set { _context = value; }	}
        public static HttpRequestBase       Request		{	get { return Current.Request;   } }
        public static HttpResponseBase      Response	{	get { return Current.Response;  } }
        public static HttpServerUtilityBase Server      {	get { return Current.Server;    } }
        public static HttpSessionStateBase  Session		{   get { return Current.Session;   } }
    }


    public static class HttpContextFactory_ExtensionMethods
    {
        public static HttpContextBase addCookieFromResponseToRequest(this HttpContextBase    httpContext, string cookieName)
        {
            if (httpContext.Response.hasCookie(cookieName))
            {
                var cookieValue = httpContext.Response.cookie(cookieName);
                if (httpContext.Request.hasCookie(cookieName))
                    httpContext.Request.Cookies[cookieName].value(cookieValue);
                else
                {
                    var newCookie = new HttpCookie(cookieName, cookieValue);
                    httpContext.Request.Cookies.Add(newCookie);
                }
            }
            return httpContext;
        }

        public static string ipAddress(this HttpContextBase httpContext)
        {            
            try
            {
                return HttpContextFactory.Request.UserHostAddress ?? ""; // todo:change to available method in 3.4                
            }
            catch (Exception ex)
            {
                ex.log("[HttpContextBase][ipAddress]");
                return "";
            }            
        }
    }
}