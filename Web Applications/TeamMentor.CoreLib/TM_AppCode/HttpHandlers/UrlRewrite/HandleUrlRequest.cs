using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel.ExtensionMethods;
using O2.XRules.Database.Utils;

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public class HandleUrlRequest
	{
		public HttpContext context = HttpContext.Current;

		public void handleCassini404(string path)
		{			
			if (path.starts("/article/"))
			{
				handleRequest_Article(path.remove("/article/"));
				context.Response.End();
			}
			
		}

		public void handleRequest_Article(string path)
		{
//			context.Response.Write("<h2>in handleRequest for : {0}</h2>".format(path));
			redirectToArticleViewer();
		}

		public void redirectToArticleViewer()
		{
			var articleViewer = "/html_pages/GuidanceItemViewer/GuidanceItemViewer.html?";			
			context.Server.Transfer(articleViewer);			
		}


		public Guid resolveMappingToArticleGuid(string mapping)
		{
			if (mapping.contains("XSS"))
				return "b1094acb-5448-446c-9b12-136e883fb28d".guid();

			return "c33d392e-5421-44cc-9db4-9563c72f7080".guid();
		}

	}
}