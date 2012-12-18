using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Routing;
using SecurityInnovation.TeamMentor.WebClient;

namespace TeamMentor.CoreLib.WebServices
{
	public class Admin_REST_Config
	{
		public static string urlPath		= "REST";
		public static string urlPath_Tests  = "test";

		public static void SetRouteTable()
		{
			setCrossDomainAccess();
			RouteTable.Routes.Add(new ServiceRoute(urlPath		, new WebServiceHostFactory(), typeof(REST_Admin)));			
			//RouteTable.Routes.Add(new ServiceRoute(UrlPath, new WSHttpBinding(), typeof(REST_Admin)));			
		}

		public static void setCrossDomainAccess()
		{
			if (TMConfig.Current.REST.AllowCrossDomainAccess)
			{
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Origin", "*");
			}
		}
	}
}
