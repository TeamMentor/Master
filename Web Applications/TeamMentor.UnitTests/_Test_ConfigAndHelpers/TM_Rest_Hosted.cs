using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using O2.FluentSharp;

namespace TeamMentor.UnitTests.REST
{
	
	public class TM_Rest_Hosted : TM_XmlDatabase_InMemory
	{		
		public static TM_REST_Host TmRestHost { get; set; }
		public static ITM_REST     TmRest	  { get; set; }

		public TM_Rest_Hosted()
		{
			TMConfig.Current.TMSetup.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();		
			TmRest = new TM_REST();
		}

		public static void WCFHost_Start()
		{
			"Starting Host".info();
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
			TmRestHost = new TM_REST_Host().StartHost();
			TmRest = TmRestHost.GetProxy();
		}


		public static void WCFHost_Stop()
		{
			"Stopping Host".info();
			TmRestHost.StoptHost();
		}
	}
}