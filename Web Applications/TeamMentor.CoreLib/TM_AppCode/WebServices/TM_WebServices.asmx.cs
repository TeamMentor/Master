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

        //[LogUrl("WebService")]
        public TM_WebServices() : this(false)
        {            
        }      
        public TM_WebServices(bool disable_Csrf_Check)
        {
			tmXmlDatabase       = TM_Xml_Database.Current;			
            userData            = tmXmlDatabase  .userData();            
            tmAuthentication	= new TM_Authentication(this).mapUserRoles(disable_Csrf_Check);
            GZip.setGZipCompression_forAjaxRequests();						
        }

        /* used this if adding KingAOP to replace PostSharp
         * http://www.codeproject.com/Tips/624586/Introducing-the-KingAOP-Framework-Part-1 
         * https://github.com/AntyaDev/KingAOP
        //, System.Dynamic.IDynamicMetaObjectProvider
        public System.Dynamic.DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return null;
        }*/
    }
}
