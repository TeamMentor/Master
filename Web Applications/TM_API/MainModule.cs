using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Routing;

namespace TM_API
{
	public class MainModule : NancyModule
	{
		public MainModule(IRouteCacheProvider routeCacheProvider)
		{
			Get["/"]		= x => View["routes", routeCacheProvider.GetCache()];
			Get["/test"]	= x => "Test";

			Get["/razor-simple"] = x =>
			{
				var model = new  { FirstName = "Fraaaaaank" };
				return View["razor-simple.cshtml", model];
			};
		}
	}
}