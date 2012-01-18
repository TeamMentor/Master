using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Security.Permissions;	
using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using Microsoft.Practices.Unity;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;
//O2File:../IJavascriptProxy.cs
//O2File:../Authentication/UserRoleBaseSecurity.cs
//O2File:TM_WebServices.asmx.cs
//O2Ref:System.Web.Services.dll 
//O2Ref:Microsoft.Practices.Unity.dll
//O2Ref:System.Xml.Linq.dll


namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{ 					
	//WebServices related to: Config Methods
    public partial class TM_WebServices 
    {		
		
		[WebMethod(EnableSession = true)] public string GetTime() 						{   return "...Via Proxy:" + javascriptProxy.GetTime(); }
		[WebMethod(EnableSession = true)] public string Ping(string message)  			{   return "received ping: {0}".format(message); }
		
        [WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string UseEnvironment_Moq()      		{   UnityInjection.useEnvironment_Moq(); 		return "using Moq Environment"; }
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string UseEnvironment_XmlDatabase()   	{   UnityInjection.useEnvironment_XmlDatabase();return "using XmlDatabase Environment"; }
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string CurrentProxyType()        		{ 	return javascriptProxy.ProxyType; }
		[WebMethod(EnableSession = true)] 											public string GetPasswordHash(string username, string password)		
																																	{	return username.createPasswordHash(password); }
		//Xml Database Specific
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string XmlDatabase_GetDatabasePath()		{	return TM_Xml_Database.Path_XmlDatabase;	}
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string XmlDatabase_GetLibraryPath()		{	return TM_Xml_Database.Path_XmlLibraries;	}		
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string XmlDatabase_ReloadData()			{	
																																		guiObjectsCacheOK = false; 
																																		return new TM_Xml_Database().reloadData(null); 
																																	}
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public bool XmlDatabase_ImportLibrary_fromZipFile(string pathToZipFile)
																																	{
																																		if (new TM_Xml_Database().xmlDB_Libraries_ImportFromZip(pathToZipFile))
																																		{
																																			this.XmlDatabase_ReloadData();
																																			return true;
																																		}
																																		return false;																																		
																																	}
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string XmlDatabase_SetLibraryPath(string libraryPath)		
																																	{	guiObjectsCacheOK = false; 
																																		return new TM_Xml_Database().reloadData(libraryPath); 
																																	}

		[WebMethod(EnableSession = true)] public List<Guid> XmlDatabase_GuidanceItems_SearchTitleAndHtml(List<Guid> guidanceItemsIds, string searchText)		
																	{	 return new TM_Xml_Database().guidanceItems_SearchTitleAndHtml(guidanceItemsIds,searchText); }																	
																	
		[WebMethod(EnableSession = true)] public string XmlDatabase_GetGuidanceItemXml(Guid guidanceItemId)	
																	{	return new TM_Xml_Database().xmlDB_guidanceItemXml(guidanceItemId); }
																	
		[WebMethod(EnableSession = true)] public string RBAC_CurrentIdentity_Name()				{	return new UserRoleBaseSecurity().currentIdentity_Name(); }
		[WebMethod(EnableSession = true)] public bool   RBAC_CurrentIdentity_IsAuthenticated()	{	return new UserRoleBaseSecurity().currentIdentity_IsAuthenticated(); }
		[WebMethod(EnableSession = true)] public List<string>  RBAC_CurrentPrincipal_Roles()		{	return new UserRoleBaseSecurity().currentPrincipal_Roles().toList(); }
		[WebMethod(EnableSession = true)] public bool  RBAC_HasRole(string role)					{	return RBAC_CurrentPrincipal_Roles().contains(role); }
		[WebMethod(EnableSession = true)] public string  RBAC_SessionCookie()						{	return HttpContext.Current.Request.Cookies["Session"].notNull() 
																												? HttpContext.Current.Request.Cookies["Session"].Value : ""; }
																												
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string SyncWithGitHub(string username, string password)	{	return UtilMethods.syncWithGitHub(username,password);  }
		
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string CreateWebEditorSecret()	
																									{
																										var webEditorSecretDataFile = AppDomain.CurrentDomain.BaseDirectory.pathCombine("webEditorSecretData.config");
																										Guid.NewGuid().str().serialize(webEditorSecretDataFile);
																										return webEditorSecretDataFile.load<string>();
																										//this (below) doesn't work because the webeditor is an *.ashx and doesn't have access to the HttpContext Session object
																										/*var session = System.Web.HttpContext.Current.Session;
																										if (session["webEditorSecretData"].isNull())
																											session["webEditorSecretData"] = Guid.NewGuid().str();
																										return (string)session["webEditorSecretData"];
																										*/
																									}		
		
		//TMConfigFileLocation
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string TMConfigFileLocation()
																					{	
																						return TMConfig.Location;  
																					}
		//TMConfigFile
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public TMConfig TMConfigFile()
																					{	
																						return TMConfig.Current;  
																					}
		//GetDisabledLibraries																			
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public List<string> GetDisabledLibraries()
																					{	
																						return TMConfig.Current.Libraries_Disabled;  
																					}
		//SetDisabledLibraries
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public List<string> SetDisabledLibraries( List<string> disabledLibraries)
																					{
																						var config = TMConfig.Current;
																						config.Libraries_Disabled = disabledLibraries;
																						if (config.SaveTMConfig())
																							return TMConfig.Current.Libraries_Disabled;
																						return 
																							null;
																					}
		
		//Get_Libraries_Zip_Folder
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string Get_Libraries_Zip_Folder()
																					{
																						return TMConfig.Current.LibrariesZipsFolder;
																					}
		//Get_Libraries_Zip_Folder_Files
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public List<string> Get_Libraries_Zip_Folder_Files()
																					{
																						return TMConfig.Current.LibrariesZipsFolder.files().fileNames();
																					}																					
		//Set_Libraries_Zip_Folder
		[WebMethod(EnableSession = true)] [Admin(SecurityAction.Demand)]			public string Set_Libraries_Zip_Folder(string folder)
																					{
																						var tmConfig = TMConfig.Current;
																						tmConfig.LibrariesZipsFolder = folder;
																						folder.createDir();
																						if (tmConfig.SaveTMConfig())																																										
																							return "Path set to '{0}' which currently has {1} files".format(folder.fullPath(), folder.files().size());
																						return null;
																					}
																					
    }
}
