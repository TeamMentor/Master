using System;
using System.ServiceModel.Web;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
	public class TM_REST_Host
	{
		public static string	Service_Protocol	{ get; set; }
		public static int		Service_Port		{ get; set; }
		public static string	Service_IP			{ get; set; }
				
		public WebServiceHost	Host				{ get; set; }

		static TM_REST_Host()
		{
			Service_Protocol = "http";
			Service_Port	 = 20000; 
			Service_IP		 = "localhost";
		}

		public Uri BaseAddress
		{
			get { return  "{0}://{1}:{2}".format(Service_Protocol, Service_IP, Service_Port).uri(); }
		}

		public TM_REST_Host StartHost()
		{						
			Host = new WebServiceHost(typeof (TM_REST), BaseAddress);						
			Host.Open();						            
			return this;
		}

		public ITM_REST GetProxy()
		{
			var webChannelFactory = new WebChannelFactory<ITM_REST>(BaseAddress);			
			return webChannelFactory.CreateChannel();
		}

		public TM_REST_Host StoptHost()
		{
			Host.Close();
			return this;
		}
	}
}