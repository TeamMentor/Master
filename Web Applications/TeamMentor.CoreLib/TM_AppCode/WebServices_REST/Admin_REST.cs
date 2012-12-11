using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.WebClient.WebServices;

namespace TeamMentor.CoreLib.WebServices
{
	[ServiceBehavior				(InstanceContextMode = InstanceContextMode.PerCall			  ), 
     AspNetCompatibilityRequirements(RequirementsMode	 = AspNetCompatibilityRequirementsMode.Allowed)]

	public class Admin_REST : IAdmin_REST
	{
		public HttpContextBase		Context			 { get; set; }	
		public HttpSessionStateBase Session			 { get; set; }	
		public TM_WebServices		TmWebServices	 { get; set; }	

		public Admin_REST()
		{
			Context = HttpContextFactory.Current;
			Session = HttpContextFactory.Session;
			TmWebServices = new TM_WebServices();
		}

		public string Version()
		{			
			return this.type().Assembly.version();
		}

		public string SessionId()
		{			
			return Session.SessionID;			
		}

		public string RBAC_CurrentIdentity_Name()
		{
			return TmWebServices.RBAC_CurrentIdentity_Name();
		}

		public bool RBAC_CurrentIdentity_IsAuthenticated()
		{
			return TmWebServices.RBAC_CurrentIdentity_IsAuthenticated();
		}

		public List<string> RBAC_CurrentPrincipal_Roles()
		{
			return TmWebServices.RBAC_CurrentPrincipal_Roles();
		}

		public bool RBAC_HasRole(string role)
		{
			return TmWebServices.RBAC_HasRole(role);
		}

		public bool RBAC_IsAdmin()
		{
			return TmWebServices.RBAC_IsAdmin();
		}

		public string RBAC_SessionCookie()
		{
			return TmWebServices.RBAC_SessionCookie();
		}
	}
}
	