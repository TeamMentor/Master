using System;
using System.Collections.Generic;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel;
using O2.Kernel.InterfacesBaseImpl;

namespace TeamMentor.CoreLib
{
	public class TMEvents
	{
		public static List<Action> OnApplication_Error			{ get; set; }
		public static List<Action> OnApplication_Start			{ get; set; }
		public static List<Action> OnApplication_BeginRequest	{ get; set; }
		
		static TMEvents()
		{
			OnApplication_Error			= new List<Action>();
			OnApplication_Start			= new List<Action>();
			OnApplication_BeginRequest	= new List<Action>();			
		}
	}

	public class Global : HttpApplication
	{
		protected void Application_Error(object sender, EventArgs e)
		{
			TMEvents.OnApplication_Error.invoke();
			var lastError = Server.GetLastError();
			if (lastError is HttpException && (lastError as HttpException).GetHttpCode() == 404)
			{				
				new HandleUrlRequest().routeRequestUrl_for404();                                          
			}						
		}
		 
		protected void Application_Start				(object sender, EventArgs e)
		{
			TMEvents.OnApplication_Start.invoke();
			PublicDI.log.LogRedirectionTarget = new MemoryLogger();			
			Admin_REST_Config.SetRouteTable();			
        }
		
		protected void Application_BeginRequest			(object sender, EventArgs e)		
        {
			TMEvents.OnApplication_BeginRequest.invoke();
			ResponseHeaders.addDefaultResponseHeaders();
            new HandleUrlRequest().routeRequestUrl();                                  
        }

		protected void Session_Start					(object sender, EventArgs e) { }	
        protected void Application_AcquireRequestState  (object sender, EventArgs e)		{}        
		protected void Application_AuthenticateRequest	(object sender, EventArgs e)		{ }
		protected void Session_End						(object sender, EventArgs e)		{ }
		protected void Application_End					(object sender, EventArgs e)		{ }
	}
}