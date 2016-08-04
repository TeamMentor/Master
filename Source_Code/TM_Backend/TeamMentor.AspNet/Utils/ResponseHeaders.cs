
using System;
using FluentSharp.Web;
using FluentSharp.CoreLib;
namespace TeamMentor.CoreLib
{
	public class ResponseHeaders 
	{		
		public static void addDefaultResponseHeaders()
		{
		    //add clickjacking protection
		    if (!TMConfig.Current.TMSecurity.X_Frame_Options.isEmpty())
		        switch (TMConfig.Current.TMSecurity.X_Frame_Options.ToUpper())
		        {
		            case "SAMEORIGIN":
		                HttpContextFactory.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
		                break;
		            case "DENY":
		                HttpContextFactory.Response.AddHeader("X-Frame-Options", "DENY");
		                break;
		            case "NONE": /*In this case the HTTP header is not set at all.*/
		                break;
		            case "ALLOW-FROM":
		                var allowedUrl = TMConfig.Current.TMSecurity.Allow_From_URI;
		                /* Uri.IsWellFormedUriString: https://msdn.microsoft.com/en-us/library/system.uri.iswellformeduristring(v=vs.110).aspx
                         * Indicates whether the string is well-formed by attempting to construct a URI with the string and ensures that the string does not require further escaping.
                         * Unit Test : Assert.IsTrue(Uri.IsWellFormedUriString("https://www.example.com", UriKind.Absolute));
                         */
		                if (!allowedUrl.isEmpty() && Uri.IsWellFormedUriString(allowedUrl, UriKind.Absolute))
		                    HttpContextFactory.Response.AddHeader("X-Frame-Options", "ALLOW-FROM " + allowedUrl);
		                else
		                    HttpContextFactory.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
		                break;
		            default:
		                HttpContextFactory.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
		                break;
		        }
		    else
		        HttpContextFactory.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");

		    //IE AntiXSS projecttion
			HttpContextFactory.Response.AddHeader("X-XSS-Protection", "1; mode=block");
			
			//add HSTS protection (HTTP Strict Transport Security)
			HttpContextFactory.Response.AddHeader("Strict-Transport-Security", "max-age=31536000");

			//Allow Cross Domain Requests
			if (TMConfig.Current.TMSecurity.REST_AllowCrossDomainAccess)
			{
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Origin" , "*");
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS");
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");				
			}							            
		}
		
	}
}