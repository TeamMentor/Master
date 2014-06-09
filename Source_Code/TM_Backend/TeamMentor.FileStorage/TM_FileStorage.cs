using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.FileStorage
{
    public class TM_FileStorage
    {
        public static TM_FileStorage Current        { get;set;}        
        public TM_Server             Server         { get;set;}
        public TM_UserData           UserData       { get;set;}
        public TM_Xml_Database       TMXmlDatabase  { get;set;}
        public string WebRoot                       { get; set; }        
        public string Path_XmlDatabase              { get; set; }
        public string Path_UserData 	            { get; set; }	                        
        public string Path_XmlLibraries 	        { get; set; }   
        public Dictionary<guidanceExplorer, string>	    GuidanceExplorers_Paths     { get; set; }	 
        public Dictionary<Guid, string>				    GuidanceItems_FileMappings	{ get; set; }			

        //public bool   UseFileStorage                 { get; set; }

        public TM_FileStorage() : this(true)
        {
            
        }
        public TM_FileStorage(bool loadData)
        {
            Server                      = new TM_Server();
            TMXmlDatabase               = new TM_Xml_Database();
            UserData                    = new TM_UserData();
            GuidanceExplorers_Paths     = new Dictionary<guidanceExplorer,string>();
            GuidanceItems_FileMappings  = new Dictionary<Guid,string>();
            if (loadData)
            {  
                Current = this;
                
                this.set_WebRoot();                
                    this.set_Path_XmlDatabase();                
                    this.set_Path_UserData();
                    this.set_Path_XmlLibraries();                
                    this.tmServer_Load();                         
                    this.load_UserData();                
                    this.load_Libraries();

                this.hook_Events_TM_UserData();
                this.hook_Events_TM_Xml_Database();
            }            
        }
    }
}
