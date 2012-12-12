using System.ServiceModel;
using System.ServiceModel.Activation;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib.WebServices
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall),
	 AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

	public class REST_Tests : IREST_Tests
	{
		public string Test()
		{
			var response = HttpContextFactory.Response;
			response.Write("<h1>Before</h1>".lineBeforeAndAfter());
			return "REST_Tests working ok".line() + "ASd";
		}
	}
}
