using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Security.Application;

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	/// <summary>
	/// Summary description for UrlRewriter
	/// </summary>
	public class UrlRewriter : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			var value = Encoder.HtmlEncode(context.Request["value"]);
			new HandleUrlRequest().handleRequest_Article(value);			
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}