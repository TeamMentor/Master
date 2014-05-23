using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git.APIs;
using urn.microsoft.guidanceexplorer;
using System.Threading;

namespace TeamMentor.CoreLib
{	
    public class TM_Xml_Database 
    {		    
        public static TM_Xml_Database   Current               { get; set; }         
        public static bool              SkipServerOnlineCheck { get; set; }        
        
        //public object			        setupLock             = new object();
        public bool			            UsingFileStorage	  { get; set; }         //config                   
        public bool                     ServerOnline          { get; set; }         
        public TM_Server                Server                { get; set; }         
        public TM_UserData              UserData              { get; set; }         //users and tracking             
        public List<API_NGit>           NGits                 { get; set; }         // Git object, one per library that has git support
        public string 	                Path_XmlDatabase      { get; set; }					
        public string 	                Path_XmlLibraries 	  { get; set; }    
//        public Thread                   SetupThread           { get; set; } 

        public TM_Database_Events       Events                { get; set; }    
        public Dictionary<Guid, guidanceExplorer>	    GuidanceExplorers_XmlFormat { get; set; }	 //Xml Library and Articles   
        public Dictionary<guidanceExplorer, string>	    GuidanceExplorers_Paths     { get; set; }	 
        public Dictionary<Guid, string>				    GuidanceItems_FileMappings	{ get; set; }			
        public Dictionary<Guid, TeamMentor_Article>	    Cached_GuidanceItems		{ get; set; }
        public Dictionary<Guid, VirtualArticleAction>   VirtualArticles			    { get; set; }
        
        [Admin]                                            
        public TM_Xml_Database() : this (false)
        {
            
        }

        [Admin]
        public TM_Xml_Database(bool useFileStorage) 
        {
            Current = this;
            
            UsingFileStorage = useFileStorage;

            this.setDefaultValues()
                .set_Path_XmlDatabase()
                .tmServer_Load();
        }                     
    }
}
