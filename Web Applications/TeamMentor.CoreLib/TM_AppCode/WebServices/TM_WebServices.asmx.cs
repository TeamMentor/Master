using System.Web.Services;

namespace TeamMentor.CoreLib
{		
    [WebService(Namespace = "http://teammentor.net/")]	 
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public partial class TM_WebServices : WebService
    {
        public TM_Authentication    tmAuthentication;
        public TM_Xml_Database      tmXmlDatabase;
        public TM_UserData          userData;

        [LogUrl("WebService")]
        public TM_WebServices() : this(false)
        {
            tmXmlDatabase = TM_Xml_Database.Current;			
            userData      = tmXmlDatabase  .UserData;
        }

        
        public TM_WebServices(bool disable_Csrf_Check)
        {			
            //javascriptProxy		= new TM_Xml_Database_JavaScriptProxy(); 
            tmAuthentication	= new TM_Authentication(this).mapUserRoles(disable_Csrf_Check);
            GZip.setGZipCompression_forAjaxRequests();						
        }                             
    }
}
