using System.Web;

namespace TeamMentor.CoreLib
{
	public class HttpContextFactory
	{
		public static HttpContextBase Context { get; set;}

		public static HttpContextBase Current
		{
			get
			{
				return Context ?? (HttpContext.Current == null
					                   ? null
					                   : new HttpContextWrapper(HttpContext.Current));
			}
		}
		public static HttpRequestBase Request
		{
			get { return HttpContextFactory.Current.Request; }
		}
		public static HttpResponseBase Response
		{
			get { return HttpContextFactory.Current.Response; }
		}
		public static HttpServerUtilityBase Server
		{
			get { return HttpContextFactory.Current.Server; }
		}
		public static HttpSessionStateBase Session
		{
			get { return HttpContextFactory.Current.Session; }
		}
	}
}