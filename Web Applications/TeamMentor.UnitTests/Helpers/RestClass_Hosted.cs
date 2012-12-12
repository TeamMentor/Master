using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityInnovation.TeamMentor.WebClient;
using TeamMentor.CoreLib.WebServices;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{
	
	public class RestClass_Hosted
	{		
		public static Admin_REST_Host AdminRestHost { get; set; }
		public static IREST_Admin IrestAdmin	 { get; set; }

		public RestClass_Hosted()
		{
			TMConfig.Current.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();		
			IrestAdmin = new REST_Admin();
		}

		public static void WCFHost_Start(TestContext context)
		{
			"Starting Host".writeLine_Trace();
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			AdminRestHost = new Admin_REST_Host().StartHost();
			IrestAdmin = AdminRestHost.GetProxy();
		}


		public static void WCFHost_Stop()
		{
			"Stopping Host".writeLine_Trace();
			AdminRestHost.StoptHost();
		}
	}
}