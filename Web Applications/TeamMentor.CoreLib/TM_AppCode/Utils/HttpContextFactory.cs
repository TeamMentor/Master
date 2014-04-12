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
        public static HttpContextBase       Context     { 	get { return Current;          } set { _context = value;  }	}
        public static HttpRequestBase       Request		{	get { return Current.notNull() ? Current.Request : null;  } }
        public static HttpResponseBase      Response	{	get { return Current.notNull() ? Current.Response: null;  } }
        public static HttpServerUtilityBase Server      {	get { return Current.notNull() ? Current.Server  : null;  } }
        public static HttpSessionStateBase  Session		{   get { return Current.notNull() ? Current.Session : null;  } }
        
        public static DateTime              LastModified_HeaderDate	{ get; set; } //used to calculate if we will send a '302 Not Modified' to the user (this could be better if the value used was the 'TM Startup Date')

        static HttpContextFactory()
        {
            LastModified_HeaderDate = DateTime.Now;			        
        }
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

        public static string sessionId(this HttpSessionStateBase sessionState)
        {
            return sessionState.notNull() 
                        ? sessionState.SessionID 
                        : "";
        }

        public static string ipAddress(this HttpRequestBase request)
        {            
            return request.notNull() 
                        ? request.UserHostAddress 
                        : "";
            /*try
            {
                return HttpContextFactory.Request.UserHostAddress ?? ""; 
            }
            catch (Exception ex)
            {
                ex.log("[HttpContextBase] [ipAddress]");
                return "";
            }*/            
        }
        public static bool isLocal(this HttpRequestBase request)
        {
            return  request.isNull() || request.IsLocal;
        }
        public static string  referer(this HttpRequestBase httpRequest)
        {
            if (httpRequest.notNull() && httpRequest.UrlReferrer.notNull())
                return  httpRequest.UrlReferrer.str();
            return "";
        }
        public static string  url(this HttpRequestBase request)
        {
            if (request.notNull() && request.Url.notNull())
                return  request.Url.str();
            return "";
        } 
       
        //cache
                

        public static bool sent304Redirect(this HttpContextBase context)
        {
            try
            {
                if (TMConfig.Current.TMSetup.Enable304Redirects && context.send304Redirect())
			    {
				    context.Response.StatusCode = 304;
				    context.Response.StatusDescription = "Not Modified";
				    return true;
			    }
			    context.setCacheHeaders();		    
            }
            catch(Exception ex)
            {
                ex.log("[HttpContextBase] [sent304Redirect]");
            }
            return false;
        }
        
		public static bool  send304Redirect(this HttpContextBase context)
		{            
			var ifModifiedSinceHeader = context.Request.Headers["If-Modified-Since"];
			if (ifModifiedSinceHeader.valid() && ifModifiedSinceHeader.isDate())
			{
				var ifModifiedSinceDate = DateTime.Parse(ifModifiedSinceHeader);
				if (HttpContextFactory.LastModified_HeaderDate.str() == ifModifiedSinceDate.str())
					return true;
			}
			return false;
		}

		public static HttpContextBase setCacheHeaders(this HttpContextBase context)
		{	
			context.Response.Cache.SetLastModified(HttpContextFactory.LastModified_HeaderDate);
            return context;
		}


    }
}