using System.Collections.Generic;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace TeamMentor.CoreLib.WebServices
{
	[ServiceContract]//(SessionMode = SessionMode.Required)]
	public interface IAdmin_REST
	{
		[OperationContract][WebGet(UriTemplate = "/Version"		)]	string Version();
		[OperationContract][WebGet(UriTemplate = "/SessionId"	)]	string SessionId();

		[OperationContract][WebGet(UriTemplate = "/RBAC_CurrentIdentity_Name"			)]	string			RBAC_CurrentIdentity_Name();
		[OperationContract][WebGet(UriTemplate = "/RBAC_CurrentIdentity_IsAuthenticated")]	bool			RBAC_CurrentIdentity_IsAuthenticated();
		[OperationContract][WebGet(UriTemplate = "/RBAC_CurrentPrincipal_Roles"			)]	List<string>	RBAC_CurrentPrincipal_Roles();
		[OperationContract][WebGet(UriTemplate = "/RBAC_HasRole/{role}"					)]	bool			RBAC_HasRole(string role);
		[OperationContract][WebGet(UriTemplate = "/RBAC_IsAdmin"						)]	bool			RBAC_IsAdmin();
		[OperationContract][WebGet(UriTemplate = "/RBAC_SessionCookie"					)]	string			RBAC_SessionCookie();		 
	}
}
