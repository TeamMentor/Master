using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.FileStorage
{
    /// <summary>
    /// This is the TeamMentor interface with the file system. 
    /// 
    /// The main TM objects (ie its objects) are not aware of where (if at all) their data is saved into
    /// </summary>
    public class TM_FileStorage : MarshalByRefObject
    {
        //***********************************
        //***      Static  objects        ***
        //***********************************

        /// <summary>
        /// Static reference to the current version of TM_FileStorage (there should only be one TM_FileStorage active at a given time)
        /// </summary>
        public static TM_FileStorage Current                    { get; set;}     
        /// <summary>
        /// This can be used to control the location of the main TeamMentor config and data files. 
        /// Note that this path must exist on disk or TM_FileStorage setup will not use this
        /// </summary>
        public static string         Custom_WebRoot             { get; set;}     

        /// <summary>
        /// This can be used to control the location of the main TeamMentor Xml Database files (including Library files)
        /// Note that this path must exist on disk or TM_FileStorage setup will not use this
        /// </summary>
        public static string         Custom_Path_XmlDatabase    { get; set;}        
        
        
        //***********************************
        //***      Instance  objects      ***
        //***********************************

        public TM_Server             Server         { get; set;}
        public TM_UserData           UserData       { get; set;}
        public TM_Xml_Database       TMXmlDatabase  { get; set;}
        public string WebRoot                       { get; set; }        
        public string Path_XmlDatabase              { get; set; }
        public string Path_UserData 	            { get; set; }	   
        public string Path_SiteData 	            { get; set; }	                        
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
                
                this.set_WebRoot()              // defines where the web root will exist               
                    .set_Path_XmlDatabase()     // the WebRoot is used to calculate the root of the XML Database         
                    .tmServer_Load()            // the TM_Server is loaded from the root of the Path_XmlDatabase
                    .set_Path_UserData()        // the Path_UserData are Path_SiteData are both
                    .set_Path_SiteData()        //     set based on info from TM_Server and Path_XmlDatabase
                    .tmConfig_Load()            // tm TM_Config is loaded from the root of the UserData
                    .set_Path_XmlLibraries()    // the Path_XmlLibraries is set based on info from TM_Config and Path_XmlDatabase                                          
                    .load_UserData()            // after all is configured we can load the users
                    .load_Libraries();          // and libraries

                this.hook_Events_TM_UserData()          // hook events to FileStorage handlers  so that actions on TeamMentor.Database 
                    .hook_Events_TM_Xml_Database();     // and TeamMentor.UserData are saved or deleted from disk
            }            
        }
    }
}
