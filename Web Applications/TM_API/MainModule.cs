using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Json;
using Nancy.Responses.Negotiation;
using Nancy.Routing;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using TeamMentor.CoreLib.WebServices;
using O2.DotNetWrappers.Network;

namespace TM_API
{
	public class MainModule : NancyModule
	{
		public IREST_Admin rest_Admin { get; set; }		

		public MainModule() 
		{
			
		}

		public MainModule(IRouteCacheProvider routeCacheProvider)
		{
			rest_Admin = new REST_Admin();
			UserGroup.Admin.setThreadPrincipalWithRoles(); //el	evate privileges during dev

			Get["/"]					= (x) => View["routes", routeCacheProvider.GetCache()];
			Get["/api"]					= (x) => View["routes", routeCacheProvider.GetCache()];
			Get["/api/test"]			= (x) => "Test";

			Get["/api/users"]			= (x) => restAdminView("users.cshtml");
			Get["/api/reload"]			= (x) => restAdminView("reload.cshtml");
			Get["/api/users/new"]		= (x) => restAdminView("user_new.cshtml");
			Get["/api/user/{idOrname}"] = (x) => user(x.idOrName);

			Get["/api/hubspot/users/new"]   = x=> hubspot_NewUser(x);
			Get["/api/hubspot/users"] = x => showHubspotUsers();
		}


		public Negotiator restAdminView(string viewName)
		{			
			return View[viewName, rest_Admin];
		}

		public Negotiator user(string idOrName)
		{
			UserGroup.Admin.setThreadPrincipalWithRoles(); //elevate privileges during dev
			//var userId = this.Context.Request.Query["userId"].Value.ToString();
			var user = rest_Admin.user(idOrName);
			if (user.isNull())
				user = new TM_User();
			return View["user.cshtml", user];
		}

		// HUB SPOT Stuff
		/// <returns></returns>.
		public Negotiator showHubspotUsers()
		{
			//var jsonFile = @"E:\O2_V4\O2\_TempDir_v4.5.3.0\12_19_2012\tmp27F9.tmp.json";
			//var json = jsonFile.fileContents();
			Web.Https.ignoreServerSslErrors();
			var jsonUrl = "https://api.hubapi.com/contacts/v1/lists/recently_updated/contacts/recent?hapikey=demo";
			var json = jsonUrl.GET();
			//var json = jsonFile.fileContents();
			var serializer = new JavaScriptSerializer();
			var users = new List<TM_User>();

			if (json.valid())
			{
				dynamic data = serializer.DeserializeObject(json);
				var index = 0;
				foreach (var contact in data["contacts"])
				{
					try
					{
						var properties = contact["properties"];
						if (properties.Count > 0)
						{
							users.add(new TM_User()
								{
									UserId = contact["vid"],
									UserName = properties["firstname"]["value"],
									LastName = properties["lastname"]["value"],
									CreatedDate = contact["addedAt"],									
									//Company = properties["lastname"]["value"]
								});
						}
					}
					catch
					{
					}
				}
				;
			}

			//var model = new REST_Admin().users().Take(1);
			return View["hubspot_users.cshtml", users];

		}

		public Negotiator hubspot_NewUser(dynamic x)
		{
			Web.Https.ignoreServerSslErrors();
			var userName = this.Context.Request.Query["userName"].Value ?? "userID";
			var firstName = (this.Context.Request.Query["firstName"].Value as string) ?? "TM";
			var lastName = (this.Context.Request.Query["lastName"].Value as string)?? "User #" + 3.randomNumbers();
			var email = (this.Context.Request.Query["email"].Value as string) ?? "testUser".add_RandomLetters(5) + "@hubspot.com";
				
			var json_template = @"E:\O2_V4\O2\_TempDir_v4.5.3.0\12_19_2012\tmpCD67.tmp";
			var url = "https://api.hubapi.com/contacts/v1/contact/?hapikey=demo";
			var postData = json_template.fileContents().replace("_New_testingapis@hubspot.com", email)
													   .replace("New_Adrian", firstName)
													   .replace("New_Mott", lastName); ;
			var response = url.POST(postData);
			return View["hubspot_NewUser.cshtml", response];
		}
	}
}