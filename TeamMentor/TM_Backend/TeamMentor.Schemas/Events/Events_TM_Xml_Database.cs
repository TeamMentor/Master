using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMentor.CoreLib
{
    public class Events_TM_Xml_Database 
    {
        public TM_Xml_Database           Target                        { get; set;}
        
        public TM_Event<TM_Xml_Database> Before_Setup                  { get; set;}
        public TM_Event<TM_Xml_Database> After_Setup                   { get; set;}
        public TM_Event<TM_Xml_Database> After_Set_Default_Values      { get; set;}
        public TM_Event<TM_Xml_Database> After_Set_Path_XmlLibraries   { get; set;}
     //   public TM_Event<TM_Xml_Database> After_TmServer_Load           { get; set;}
        public TM_Event<TM_Xml_Database> After_Load_UserData           { get; set;}
        public TM_Event<TM_Xml_Database> After_Load_SiteData           { get; set;}
        public TM_Event<TM_Xml_Database> After_Load_Libraries          { get; set;} 
        
        public TM_Event<TM_Xml_Database> After_UserData_Ctor           { get; set;} 
             

        public Events_TM_Xml_Database(TM_Xml_Database tmXmlDatabase)
        {
            this.Target                 = tmXmlDatabase;
            Before_Setup                = new TM_Event<TM_Xml_Database>(tmXmlDatabase); 
            After_Setup                 = new TM_Event<TM_Xml_Database>(tmXmlDatabase); 
            After_Set_Default_Values    = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
            After_Set_Path_XmlLibraries = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
   //         After_TmServer_Load         = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
            After_Load_UserData         = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
            After_Load_SiteData         = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
            After_Load_Libraries        = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
            
            After_UserData_Ctor         = new TM_Event<TM_Xml_Database>(tmXmlDatabase);
        }        
    }
}
