using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Routing;
using TeamMentor.CoreLib.WebServices;

namespace TM_API
{
	public class MainModule : NancyModule
	{
		public MainModule(IRouteCacheProvider routeCacheProvider)
		{
			Get["/"]			= x => View["routes", routeCacheProvider.GetCache()];
			Get["/api"]			= x => View["routes", routeCacheProvider.GetCache()];
			Get["/api/test"]	= x => "Test";

			Get["/api/users"] = x =>
			{				
				var model = new REST_Admin();
				return View["users.cshtml", model];
			};

			Get["/api/reload"] = x =>
			{
				var model = new REST_Admin();
				return View["reload.cshtml", model];
			};
			Get["/api/users/new"] = x =>
			{
				var model = new REST_Admin();
				return View["user_new.cshtml", model];
			};
		}
	}
}