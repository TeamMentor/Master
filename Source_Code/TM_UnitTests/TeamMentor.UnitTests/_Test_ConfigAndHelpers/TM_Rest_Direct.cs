using FluentSharp.CoreLib;
using FluentSharp.Moq;
using FluentSharp.Web;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.REST

{	
    public class TM_Rest_Direct : TM_Config_InMemory
    {
        public TM_Xml_Database      tmXmlDatabase;
        public TM_UserData          tmUserData;

        public API_Moq_HttpContext  moq_HttpContext;
        public TM_REST              TmRest				{ get; set; }
                
        public TM_Rest_Direct()
        {            
            SendEmails.Disable_EmailEngine = true;
            //TMConfig.Current.TMSetup.UseAppDataFolder = true;									// set the TM XMl Database folder to be 
            moq_HttpContext              = new API_Moq_HttpContext();
            HttpContextFactory.Context   = moq_HttpContext.httpContext();
            setDatabase();
            TmRest = new TM_REST();
            
        }

        [Assert_Admin]
        public void setDatabase()
        {       
            UserGroup.Admin.assert();

            tmXmlDatabase = new TM_Xml_Database();
            tmXmlDatabase.setup();

            tmUserData = new TM_UserData();
            tmUserData.createDefaultAdminUser();

            UserGroup.None.assert(); 
        }
    }
}