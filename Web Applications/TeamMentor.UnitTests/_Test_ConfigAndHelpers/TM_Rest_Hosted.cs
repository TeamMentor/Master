using TeamMentor.CoreLib;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{
	
	public class TM_Rest_Hosted
	{		
		public static TM_REST_Host TmRestHost { get; set; }
		public static ITM_REST IrestAdmin	 { get; set; }

		public TM_Rest_Hosted()
		{
			TMConfig.Current.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();		
			IrestAdmin = new TM_REST();
		}

		public static void WCFHost_Start()
		{
			"Starting Host".writeLine_Trace();
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			TmRestHost = new TM_REST_Host().StartHost();
			IrestAdmin = TmRestHost.GetProxy();
		}


		public static void WCFHost_Stop()
		{
			"Stopping Host".writeLine_Trace();
			TmRestHost.StoptHost();
		}
	}
}