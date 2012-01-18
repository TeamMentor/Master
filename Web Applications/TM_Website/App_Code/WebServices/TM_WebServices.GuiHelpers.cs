using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Security.Permissions;	
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using Microsoft.Practices.Unity;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

//O2File:TM_WebServices.asmx.cs
//O2File:../ExtensionMethods/TM_Xml_Database_ExtensionMethods_GuiHelpers.cs
//_O2Ref:System.Web.Services.dll 
//_O2Ref:Microsoft.Practices.Unity.dll
//O2Ref:System.Xml.Linq.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{ 					
	//WebServices related to: Data Viewers
    public partial class TM_WebServices 
    {
		
		[WebMethod(EnableSession= true)]
		public List<Library_V3> GetFolderStructure_Libraries()
		{
			return javascriptProxy.getFolderStructure_Libraries(GetGUIObjects());
		}
		
		[WebMethod(EnableSession= true)]
		public Library_V3 GetFolderStructure_Library(Guid libraryId)
		{
			return javascriptProxy.getFolderStructure_Library(libraryId, GetGUIObjects());
		}
		
		//********  Gui Objects
		
		//******** New Data Transfer mode based on String indexes
		
		public static TM_GUI_Objects last_GUI_Objects = null;		// this will need to be updated when changes are made
		public static bool guiObjectsCacheOK = false;
		
		public void resetCache() {  	guiObjectsCacheOK = false;  }
		
		[WebMethod(EnableSession= true)]
		public bool ClearGUIObjects()
		{
			last_GUI_Objects = null;
			guiObjectsCacheOK = false;
			return last_GUI_Objects.isNull();
		}
		
		[WebMethod(EnableSession= true)]
		public TM_GUI_Objects GetGUIObjects()
		{
			if (guiObjectsCacheOK &&  last_GUI_Objects.notNull())		// returns cached version on next calls
				return last_GUI_Objects; 
				
			var guiObjects = new TM_GUI_Objects(); 						
			var allGuidanceItems = javascriptProxy.GetAllGuidanceItems_XmlDB();
			foreach(var row in allGuidanceItems)      
			{  
				var guidanceItemMappings = "{0},{1},{2},{3},{4},{5},{6}".format(
												//guiObjects.add_UniqueString(row.guidanceItemId.str().hash().str()),	// this will shave off another 80k from the request
												guiObjects.add_UniqueString(row.guidanceItemId.str()),
												guiObjects.add_UniqueString(row.libraryId.str()),
												guiObjects.add_UniqueString(row.title),
												guiObjects.add_UniqueString(row.technology),
												guiObjects.add_UniqueString(row.phase),
													guiObjects.add_UniqueString(row.rule_Type),
												guiObjects.add_UniqueString(row.category));
				
				guiObjects.GuidanceItemsMappings.Add(guidanceItemMappings);												
			};
			last_GUI_Objects = guiObjects;
			//for the big library this is now a 360k string
			guiObjectsCacheOK = true;
			return guiObjects;
		}
		
	
		[WebMethod(EnableSession= true)]
		public List<string> GetStringIndexes()
		{
			var guiObjects = GetGUIObjects();
			return guiObjects.UniqueStrings;
		}
		
		[WebMethod(EnableSession= true)]
		public List<string> GetGuidanceItemsMappings()
		{
			var guiObjects = GetGUIObjects();
			return guiObjects.GuidanceItemsMappings;
		}
	}
}
