
namespace TeamMentor.CoreLib
{
	public class ResponseHeaders
	{

		//RESPONSE
		public static void addDefaultResponseHeaders()
		{
			//add clickjacking protection			
			HttpContextFactory.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
			
			//IE AntiXSS projecttion
			HttpContextFactory.Response.AddHeader("X-XSS-Protection", "1; mode=block");
			
			//Allow Cross Domain Requests
			if (TMConfig.Current.REST.AllowCrossDomainAccess)
			{
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Origin" , "*");
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS");
				HttpContextFactory.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");				
			}
		}
		
	}
}