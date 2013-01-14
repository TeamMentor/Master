using System.Web;

namespace TeamMentor.CoreLib
{
	/// <summary>
	/// Summary description for UrlRewriter
	/// </summary>
	public class UrlRewriter : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			var value = context.Request["value"];
            var action = context.Request["action"];
			new HandleUrlRequest().handleRequest(action, value);			
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