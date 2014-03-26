using System;
using System.Net;
using FluentSharp.CoreLib;
using TeamMentor.UnitTests.TM_Website.WebServices;

namespace TeamMentor.UnitTests.TM_Website
{
    public class TM_WebServices_Configured : TM_WebServices
    {   
        public static string	WEBSERVICES_PATH = "/aspx_pages/TM_WebServices.asmx";
        public 		  TM_User 	CurrentUser		{ get; set; }
		
        public TM_WebServices_Configured(Uri websiteUrl)
        {			  
            Url             = websiteUrl.append(WEBSERVICES_PATH).str();			
            CookieContainer =  new CookieContainer();
        }
		
        protected override WebRequest GetWebRequest(Uri Uri)
        {
            var req = (HttpWebRequest)base.GetWebRequest(Uri);
            //req.AutomaticDecompression = DecompressionMethods.GZip;			 // enable GZIP
            req.AutomaticDecompression = DecompressionMethods.None;			 // enable GZIP
            //set session Cookies			
            var cookies = this.CookieContainer.GetCookies(Uri);	    	
            if (cookies.notNull() && cookies.size() > 0 && cookies["Session"].notNull())
            {
                var sessionValue = cookies["Session"].Value;	    		
                var crsf_Token = (CurrentUser.notNull()) ? CurrentUser.CSRF_Token : "";
                req.Headers.Add("CSRF-Token",crsf_Token);
                req.Headers.Add("Session",sessionValue);	    			    		
            }
            return req;
        }

    }
}