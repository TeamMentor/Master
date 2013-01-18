using TeamMentor.CoreLib;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{	
	public class RestClass_Direct
	{
		public TM_Xml_Database tmXmlDatabase;
		public IREST_Admin IRESTAdmin				{ get; set; }
		public API_Moq_HttpContext	moq_HttpContext	{ get; set; }

		public RestClass_Direct()
		{
			tmXmlDatabase = new TM_Xml_Database(false);	

			TMConfig.Current.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
			moq_HttpContext = new API_Moq_HttpContext();
			HttpContextFactory.Context = moq_HttpContext.httpContext();			
			IRESTAdmin = new REST_Admin();
		}
	}
}