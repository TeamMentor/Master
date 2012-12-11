using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Routing;

namespace TeamMentor.CoreLib.WebServices
{
	public class Admin_REST_Config
	{
		public static string UrlPath = "AdminREST";

		public static void SetRouteTable()
		{
			RouteTable.Routes.Add(new ServiceRoute(UrlPath, new WebServiceHostFactory(), typeof(Admin_REST)));			
			//RouteTable.Routes.Add(new ServiceRoute(UrlPath, new WSHttpBinding(), typeof(Admin_REST)));			
			
		}
	}
}
