using System.ComponentModel;
using System.Web.Script.Services;
using System.Web.Services;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{		
    [WebService(Namespace = "http://teammentor.net/")]	 
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public partial class TM_WebServices : WebService
    {
        public TM_Authentication    tmAuthentication;
        public TM_Xml_Database      tmXmlDatabase;
        public TM_FileStorage       tmFileStorage;
        public TM_UserData          userData;

        //[LogUrl("WebService")]
        public TM_WebServices() : this(false)
        {            
        }      
        public TM_WebServices(bool disable_Csrf_Check)
        {
			tmXmlDatabase       = TM_Xml_Database.Current;			
            tmFileStorage       = TM_FileStorage .Current;
            userData            = TM_UserData.Current;
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
