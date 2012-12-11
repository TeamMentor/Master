using System;
using System.ServiceModel.Web;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib.WebServices
{
	public class Admin_REST_Host
	{
		public static string	Service_Protocol	{ get; set; }
		public static int		Service_Port		{ get; set; }
		public static string	Service_IP			{ get; set; }
				
		public WebServiceHost	Host				{ get; set; }

		static Admin_REST_Host()
		{
			Service_Protocol = "http";
			Service_Port	 = 20000; 
			Service_IP		 = "local";
		}

		public Uri BaseAddress
		{
			get { return  "{0}://{1}:{2}".format(Service_Protocol, Service_IP, Service_Port).uri(); }
		}

		public Admin_REST_Host StartHost()
		{						
			Host = new WebServiceHost(typeof (Admin_REST), BaseAddress);						
			Host.Open();						
			return this;
		}

		public IAdmin_REST GetProxy()
		{
			var webChannelFactory = new WebChannelFactory<IAdmin_REST>(BaseAddress);			
			return webChannelFactory.CreateChannel();
		}

		public Admin_REST_Host StoptHost()
		{
			Host.Close();
			return this;
		}
	}
}