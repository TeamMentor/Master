using System;
using FluentSharp.CoreLib;
using FluentSharp.Moq;
using FluentSharp.Web;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
	
	public class TM_Rest_Hosted : TM_XmlDatabase_InMemory
	{		
		public static TM_REST_Host TmRestHost { get; set; }
		public static ITM_REST     TmRest	  { get; set; }
        
        public bool HostStarted    { get; set;}

		public TM_Rest_Hosted()
		{
			TMConfig.Current.TMSetup.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();		
			TmRest = new TM_REST();
		}

		public void WCFHost_Start()
		{
			"Starting WCFHost Host".info();
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
            TmRestHost = new TM_REST_Host();
            try
            {
			    TmRestHost.StartHost();
			    TmRest = TmRestHost.GetProxy();
                HostStarted = true;
            }
            catch(Exception ex)
            {
                ex.log();
            }
		}


		public void WCFHost_Stop()
		{
            if (HostStarted)
            {
			    "Stopping Host".info();
			    TmRestHost.StoptHost();
                HostStarted = false;
            }
		}
	}
}