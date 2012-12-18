using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.CoreLib.WebServices
{
	public partial class REST_Admin
	{
		//REST_User_Management
		public Stream users_html()
		{
			//var xml = users().toXml();
			//var html = xml.xsl_Transform();
			var html = "test";
			this.response_ContentType_Html();
			return html.stream_UFT8();
		}
	}
}
