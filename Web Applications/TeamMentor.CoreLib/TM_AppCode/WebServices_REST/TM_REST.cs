using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	[ServiceBehavior				(InstanceContextMode = InstanceContextMode.PerCall			  ), 
     AspNetCompatibilityRequirements(RequirementsMode	 = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class TM_REST : ITM_REST
	{
        public static string urlPath		= "REST";
		public static string urlPath_Tests	= "REST_Tests";

		public HttpContextBase		Context			 { get; set; }	
		public HttpSessionStateBase Session			 { get; set; }	
		public TM_WebServices		TmWebServices	 { get; set; }	

		[LogUrl("REST")]
		public TM_REST()
		{
			Context       = HttpContextFactory.Current;
			Session       = HttpContextFactory.Session;									
			TmWebServices = new TM_WebServices(true);	//Disabling CSRF
			//UserGroup.Admin.setThreadPrincipalWithRoles();					
		}

        
		public static void SetRouteTable()
		{		
			RouteTable.Routes.Add(new ServiceRoute(urlPath		, new WebServiceHostFactory(), typeof(TM_REST)));
			//RouteTable.Routes.Add(new ServiceRoute(urlPath_Tests, new WebServiceHostFactory(), typeof(REST_Tests)));						
		}

						
	}


}
	