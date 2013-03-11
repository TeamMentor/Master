using System;
using System.Collections.Generic;


namespace TeamMentor.CoreLib
{
    public class ___TM_Xml_Database_JavaScriptProxy 
    {
        public TM_Xml_Database tmXmlDatabase {get;set;}		
        public string ProxyType  { get; set; }    	
        public Guid adminSessionID  { get; set; }
        
        
        public ___TM_Xml_Database_JavaScriptProxy()
        {			
            ProxyType = "TM Xml Database";
            adminSessionID = Guid.Empty;
            tmXmlDatabase =  TM_Xml_Database.Current;			
        }
        
        public bool UpdateGuidanceItem(GuidanceItem_V3 guidanceItem)						{ return tmXmlDatabase.createGuidanceItem(guidanceItem) != Guid.Empty; }
        //XmlDB specific
        public List<TeamMentor_Article> GetGuidanceItemsInViews_XmlDB(List<Guid> viewIds)	    { return tmXmlDatabase.getGuidanceItemsInViews(viewIds);}  			
        
        public List<TeamMentor_Article> GetGuidanceItemsInLibrary_XmlDB(Guid libraryId) 	    { return tmXmlDatabase.tmGuidanceItems(libraryId);}
        public List<TeamMentor_Article> GetGuidanceItemsInFolder_XmlDB(Guid folderId)   	    { return tmXmlDatabase.tmGuidanceItems_InFolder(folderId);}				                       
    }
    
}