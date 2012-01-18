using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:../UtilMethods.cs
//O2File:../IJavascriptProxy.cs
//O2File:../XmlDatabase/TM_Xml_Database_JavaScriptProxy.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public static class TM_Xml_Database_JavaScriptProxy_ExtensionMethods
	{
		public static TMUser user(this TM_Xml_Database_JavaScriptProxy xmlDbProxy, string name)
		{
			return xmlDbProxy.GetUser_byName(name);
		}
		
    }
}