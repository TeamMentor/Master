using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TeamMentor.CoreLib.WebServices
{
	[ServiceContract]
	public interface IREST_Tests
	{
		[OperationContract][WebGet(UriTemplate = "/Test"		)]	string Test();				
	}
}
