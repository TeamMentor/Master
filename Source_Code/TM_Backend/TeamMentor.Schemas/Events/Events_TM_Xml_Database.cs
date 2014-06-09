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
                

        public TM_Event_TM_Xml_Database Articles_Cache_Updated        { get; set;} 

        public TM_Event_TM_Article          Article_Saved                 { get; set;} 
        public TM_Event_TM_Article          Article_Deleted               { get; set;} 
        
        public TM_Event_TM_Library          Library_Deleted               { get; set;} 
        public TM_Event_GuidanceExplorer    GuidanceExplorer_Save         { get; set;} 
        
        public TM_Event_TM_Xml_Database     VirtualArticles_Loaded        { get; set;} 
        public TM_Event_TM_Xml_Database     VirtualArticles_Saved        { get; set;} 
        
        
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
            
            Articles_Cache_Updated      = new TM_Event_TM_Xml_Database (tmXmlDatabase);
            Article_Saved                = new TM_Event_TM_Article      (tmXmlDatabase);
            Article_Deleted             = new TM_Event_TM_Article      (tmXmlDatabase);

            Library_Deleted             = new TM_Event_TM_Library      (tmXmlDatabase);
            GuidanceExplorer_Save       = new TM_Event_GuidanceExplorer(tmXmlDatabase);

            VirtualArticles_Loaded      = new TM_Event_TM_Xml_Database (tmXmlDatabase);
            VirtualArticles_Saved       = new TM_Event_TM_Xml_Database (tmXmlDatabase);                        
        }        
    }
}
