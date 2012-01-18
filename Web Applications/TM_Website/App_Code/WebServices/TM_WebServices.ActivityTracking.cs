using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Permissions;	
using System.Text;
using System.Web.Services;
using Microsoft.Practices.Unity;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
//O2File:../IJavascriptProxy.cs
//O2File:../UtilMethods.cs
//O2File:../WebServices/TM_WebServices.asmx.cs
//O2File:../TrackingDB/ActivityTracking.cs
//O2File:../TrackingDB/PagesHistory.cs
//O2Ref:System.Web.Services.dll 
//O2Ref:Microsoft.Practices.Unity.dll
//O2Ref:System.Xml.Linq.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{ 					
	//WebServices related to: user Activity Tracking
    public partial class TM_WebServices 
    {							
		[WebMethod(EnableSession= true)] 
		[Admin(SecurityAction.Demand)]	
		public string GetPostData()
		{
			return HttpContextFactory.Current.GetPostDataAsString();
		}	
		
		[WebMethod(EnableSession= true)] 
		[Admin(SecurityAction.Demand)]	
		public RequestData GetRequestData()
		{
			return activityTracking.GetRequestData(sessionID, currentUser.UserName);
		}
		 
		[WebMethod(EnableSession= true)] 
		[Admin(SecurityAction.Demand)]	
		public List<RequestData> GetPastRequests()
		{			
			return activityTracking.PastRequests;
		}
		  
		[WebMethod(EnableSession= true)] 
		[Admin(SecurityAction.Demand)]	
		public RequestData GetLastRequest()
		{
			return activityTracking.GetLastRequest();			
		}
		
		[WebMethod(EnableSession = true)] 
		[Admin(SecurityAction.Demand)]	
		public JsDataTable JsDataTableWithPastRequests()
		{			
			var jsDataTable =  new JsDataTable();
			jsDataTable.add_Columns("Url", "Date", "UserName", "SessionID", "PostData");
			foreach(var pastRequest in activityTracking.PastRequests)
				jsDataTable.add_Row(pastRequest.Url, pastRequest.Date, pastRequest.UserName, pastRequest.SessionID, pastRequest.PostData);
			return jsDataTable;
		}
		
		[WebMethod(EnableSession = true)]
		[Admin(SecurityAction.Demand)]			
		public List<ChangedPage> GetPagesHistory()
		{
			return new PagesHistory().getPages();
		}
		
    }	
}
