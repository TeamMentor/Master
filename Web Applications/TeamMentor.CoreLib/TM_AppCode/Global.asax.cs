using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SecurityInnovation.TeamMentor.WebClient.WebServices;

namespace SecurityInnovation.TeamMentor.Website
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Error(object sender, EventArgs e)
		{			
			var lastError = Server.GetLastError();
			if (lastError is HttpException && (lastError as HttpException).GetHttpCode() == 404)
			{				
				new HandleUrlRequest().handleCassini404(Request.Url.PathAndQuery);								
			}						
		}

		 
		protected void Application_Start				(object sender, EventArgs e)		{ }
		protected void Session_Start					(object sender, EventArgs e)		{ }	
		protected void Application_BeginRequest			(object sender, EventArgs e)		
        {
            new HandleUrlRequest().handleCassini404(Request.Url.PathAndQuery);
            
            //HttpContext.Current.Response.Write(Request.Url);
            //HttpContext.Current.Response.End();
        }
		protected void Application_AuthenticateRequest	(object sender, EventArgs e)		{ }
		protected void Session_End						(object sender, EventArgs e)		{ }
		protected void Application_End					(object sender, EventArgs e)		{ }
	}
}