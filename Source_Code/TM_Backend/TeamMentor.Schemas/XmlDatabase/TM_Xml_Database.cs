using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{	
    public class TM_Xml_Database 
    {		    
        public static TM_Xml_Database   Current               { get; set; }         
        public static bool              SkipServerOnlineCheck { get; set; }                       
        public bool                     ServerOnline          { get; set; }         
        //public TM_UserData              UserData              { get; set; }         //users and tracking                                     
//        public Thread                   SetupThread           { get; set; } 

        public Events_TM_Xml_Database                   Events                      { get; set; }    
        public Dictionary<Guid, guidanceExplorer>	    GuidanceExplorers_XmlFormat { get; set; }	 //Xml Library and Articles   
        public Dictionary<Guid, TeamMentor_Article>	    Cached_GuidanceItems		{ get; set; }
        public Dictionary<Guid, VirtualArticleAction>   VirtualArticles			    { get; set; }
        
        public TM_Xml_Database()
        {            
            Current = this;                        
            Events                      = new Events_TM_Xml_Database(this);
            GuidanceExplorers_XmlFormat = new Dictionary<Guid,guidanceExplorer>();
            Cached_GuidanceItems        = new Dictionary<Guid,TeamMentor_Article>();
            VirtualArticles             = new Dictionary<Guid,VirtualArticleAction>();
        }                     
    }
}
