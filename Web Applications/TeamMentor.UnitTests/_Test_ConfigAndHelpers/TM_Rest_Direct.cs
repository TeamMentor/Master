using TeamMentor.CoreLib;
using O2.FluentSharp;

namespace TeamMentor.UnitTests.REST

{	
    public class TM_Rest_Direct
    {
        public TM_Xml_Database      tmXmlDatabase;
        public API_Moq_HttpContext  moq_HttpContext;
        public ITM_REST             TmRest				{ get; set; }
        

        public TM_Rest_Direct()
        {
            UserGroup.Admin.setThreadPrincipalWithRoles();                
            tmXmlDatabase = new TM_Xml_Database(false);	
            UserGroup.Anonymous.setThreadPrincipalWithRoles();

            TMConfig.Current.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
            moq_HttpContext                   = new API_Moq_HttpContext();
            HttpContextFactory.Context        = moq_HttpContext.httpContext();			
            TmRest = new TM_REST();
        }
    }
}