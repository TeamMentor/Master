using System;
using System.ServiceModel.Web;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
	public class TM_REST_Host
	{
		public string	Service_Protocol	{ get; set; }
		public int		Service_Port		{ get; set; }
		public string	Service_IP			{ get; set; }
        public string	Url_Template	    { get; set; }
				
		public WebServiceHost	Host				{ get; set; }

		public TM_REST_Host()
		{
			Service_Protocol = Tests_Consts.TM_REST_Service_Protocol;
            Service_Port	 = Tests_Consts.TM_REST_Service_Port; 
			Service_IP		 = Tests_Consts.TM_REST_Service_IP;
            Url_Template     = Tests_Consts.TM_REST_Url_Template;
            // to understand the use of Design_Time_Addresses see http://stackoverflow.com/questions/885744/wcf-servicehost-access-rights/10171284#10171284
		}              

		public Uri BaseAddress
		{			
            get { return  Url_Template.format(Service_Protocol, Service_IP, Service_Port).uri(); }
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