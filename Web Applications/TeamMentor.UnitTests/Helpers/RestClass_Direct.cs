using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.XRules.Database.APIs;
using SecurityInnovation.TeamMentor.WebClient;
using TeamMentor.CoreLib.WebServices;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{	
	public class RestClass_Direct
	{		
		public static IREST_Admin IrestAdmin { get; set; }

		public RestClass_Direct()
		{
			TMConfig.Current.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
			HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();		
			IrestAdmin = new REST_Admin();
		}
	}
}