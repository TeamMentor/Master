using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{ 					
    //WebServices related to: Config Methods
    public partial class TM_WebServices 
    {		
        
        [WebMethod(EnableSession = true)] [None]      public string GetTime() 						{   return "...Via Proxy:" + DateTime.Now.str();                        }  	         
        [WebMethod(EnableSession = true)] [None]      public string Ping(string message)  			{   return "received ping: {0}".format(message);                        }        

        [WebMethod(EnableSession = true)] public string         RBAC_CurrentIdentity_Name()				                {	return new UserRoleBaseSecurity().currentIdentity_Name(); }
        [WebMethod(EnableSession = true)] public bool           RBAC_CurrentIdentity_IsAuthenticated()	                {	return new UserRoleBaseSecurity().currentIdentity_IsAuthenticated(); }
        [WebMethod(EnableSession = true)] public List<string>   RBAC_CurrentPrincipal_Roles()		                    {	return new UserRoleBaseSecurity().currentPrincipal_Roles().toList(); }
        [WebMethod(EnableSession = true)] public bool           RBAC_HasRole(string role)					            {	return RBAC_CurrentPrincipal_Roles().contains(role); }
        [WebMethod(EnableSession = true)] public bool           RBAC_IsAdmin()											{	return RBAC_CurrentPrincipal_Roles().contains("Admin"); }        
        
        // test security demands
        [WebMethod(EnableSession = true)] [Admin]	            public bool      RBAC_Demand_Admin             ()	    {	admin             .demand(); return true; }        
        [WebMethod(EnableSession = true)] [ManageUsers]	        public bool      RBAC_Demand_ManageUsers       ()		{	manageUsers       .demand(); return true; }
        [WebMethod(EnableSession = true)] [EditArticles]	    public bool      RBAC_Demand_EditArticles      ()		{	editArticles      .demand(); return true; }
        [WebMethod(EnableSession = true)] [ReadArticles]	    public bool      RBAC_Demand_ReadArticles      ()		{	readArticles      .demand(); return true; }
        [WebMethod(EnableSession = true)] [ReadArticlesTitles]	public bool      RBAC_Demand_ReadArticlesTitles()		{	readArticlesTitles.demand(); return true; }
        [WebMethod(EnableSession = true)] [ViewLibrary]	        public bool      RBAC_Demand_ViewLibrary       ()		{	viewLibrary       .demand(); return true; }
        [WebMethod(EnableSession = true)] [None]	            public bool      RBAC_Demand_None              ()		{	none              .demand(); return true; }
        

        // admin methods
        [WebMethod(EnableSession = true)] [Admin]	    public string     XmlDatabase_GetDatabasePath()		{	admin.demand(); return tmFileStorage.path_XmlDatabase();	                            }
        [WebMethod(EnableSession = true)] [Admin]	    public string     XmlDatabase_GetLibraryPath()		{	admin.demand(); return tmFileStorage.path_XmlLibraries();	                            }		
        [WebMethod(EnableSession = true)] [Admin]	    public string     XmlDatabase_GetUserDataPath()		{	admin.demand(); return tmFileStorage.Path_UserData;	                                }		
        [WebMethod(EnableSession = true)] [Admin]	    public string     XmlDatabase_ReloadData()			{	admin.demand(); guiObjectsCacheOk = false; return  tmFileStorage.reloadData();        }
        [WebMethod(EnableSession = true)] [Admin]	    public bool       XmlDatabase_IsUsingFileStorage()	{	admin.demand();  return tmFileStorage != null;                                         }               
                                                                                                             						
        [WebMethod(EnableSession = true)] [Admin]	    public string	  TMConfigFileLocation()			     {	admin.demand(); return tmFileStorage.tmConfig_Location();             }		
        [WebMethod(EnableSession = true)] [Admin]	    public TMConfig	  TMConfigFile()                         {	admin.demand(); return TMConfig.Current;              }																					        
        [WebMethod(EnableSession = true)] [Admin]	    public bool		  SetTMConfigFile(TMConfig tmConfig)     {  admin.demand(); return userData.tmConfig_SetCurrent(tmConfig); }                                                                                            
        [WebMethod(EnableSession = true)] [Admin] 	    public Firebase_ClientConfig Get_Firebase_ClientConfig() {  admin.demand(); return userData.firebase_ClientConfig();  }
        

        // Install libraries from ZIP
        [WebMethod(EnableSession = true)] [Admin]	    public bool         XmlDatabase_ImportLibrary_fromZipFile(string pathToZipFile, string unzipPassword) { admin.demand(); return tmFileStorage.xmlDB_Libraries_ImportFromZip(pathToZipFile, unzipPassword); } 
        [WebMethod(EnableSession = true)] [Admin]	    public string		TMServerFileLocation()			{	admin.demand(); return tmFileStorage.tmServer_Location();  }		
        [WebMethod(EnableSession = true)] [Admin]	    public TM_Server	TMServerFile()
                                                        {	
                                                            admin.demand(); 
                                                            return tmFileStorage.Server;  
                                                        }
        [WebMethod(EnableSession = true)] [Admin]	    public bool		    SetTMServerFile(TM_Server tmServer)
                                                        {
                                                            admin.demand(); 
                                                            tmFileStorage.Server = tmServer;                                                                                         
                                                            return tmFileStorage.tmServer_Save(tmServer);                                                                                        
                                                        }

        [WebMethod(EnableSession = true)] [Admin]	    public string		Get_Libraries_Zip_Folder()
                                                        {
                                                            admin.demand(); 
                                                            var librariesZipsFolder = TMConfig.Current.TMSetup.LibrariesUploadedFiles;
                                                            return tmFileStorage.path_XmlDatabase().fullPath().pathCombine(librariesZipsFolder).fullPath();
                                                        }		
        [WebMethod(EnableSession = true)] [Admin]	    public List<string> Get_Libraries_Zip_Folder_Files()
                                                        {
                                                            admin.demand(); 
                                                            return Get_Libraries_Zip_Folder().files().fileNames();
                                                        }																							
        [WebMethod(EnableSession = true)] [Admin]	    public string		Set_Libraries_Zip_Folder(string folder)
                                                        {
                                                            admin.demand(); 
                                                            if (folder.valid())
                                                            {
                                                                var tmConfig = TMConfig.Current;
                                                                tmConfig.TMSetup.LibrariesUploadedFiles = folder;
                                                                //folder.createDir();
                                                                if (tmFileStorage.tmConfig_Save())																																										
                                                                    return "Path set to '{0}' which currently has {1} files".format(folder.fullPath(), folder.files().size());
                                                            }
                                                            return null;
                                                        }

        [WebMethod(EnableSession = true)] [Admin]	    public Guid			GetUploadToken()
                                                        {
                                                            admin.demand(); 
                                                            var uploadToken = Guid.NewGuid();
                                                            FileUpload.UploadTokens.Add(uploadToken);
                                                            return uploadToken;
                                                        }

        [WebMethod(EnableSession = true)] [Admin]	    public string		GetLogs()                           { admin.demand(); return PublicDI.log.LogRedirectionTarget.prop("LogData").str() ; }        
        [WebMethod(EnableSession = true)] [Admin]	    public string		ResetLogs()                         { admin.demand(); (PublicDI.log.LogRedirectionTarget.prop("LogData") as StringBuilder).Clear() ; return "done"; }                
        
        [WebMethod(EnableSession = true)] [Admin]	    public string		REPL_ExecuteSnippet(string snippet) { admin.demand(); return REPL.executeSnippet(snippet);}
        


        //Virtual Articles
        [WebMethod(EnableSession = true)] [Admin]	    public List<VirtualArticleAction>	VirtualArticle_GetCurrentMappings()        
                                                        {
                                                            admin.demand();
                                                            return TM_Xml_Database.Current.getVirtualArticles().Values.toList();
                                                        }				
        [WebMethod(EnableSession = true)] [Admin]	    public VirtualArticleAction			VirtualArticle_Add_Mapping_VirtualId( Guid id, Guid virtualId)
                                                        {
                                                            admin.demand();
                                                            return TM_Xml_Database.Current.add_Mapping_VirtualId(id, virtualId);																						
                                                        }
        [WebMethod(EnableSession = true)] [Admin]	    public VirtualArticleAction			VirtualArticle_Add_Mapping_Redirect (Guid id, string redirectUri)
                                                        {
                                                            admin.demand();
                                                            return TM_Xml_Database.Current.add_Mapping_Redirect(id, redirectUri.uri());																						
                                                        }
        [WebMethod(EnableSession = true)] [Admin]	    public VirtualArticleAction			VirtualArticle_Add_Mapping_ExternalArticle(Guid id, string tmServer, Guid externalId)
                                                        {
                                                            admin.demand();
                                                            return TM_Xml_Database.Current.add_Mapping_ExternalArticle(id, tmServer, externalId);																						
                                                        }			
        [WebMethod(EnableSession = true)] [Admin]	    public VirtualArticleAction			VirtualArticle_Add_Mapping_ExternalService(Guid id, string service, string data)
                                                        {
                                                            admin.demand();
                                                            return TM_Xml_Database.Current.add_Mapping_ExternalService(id, service, data);																						
                                                        }			
        [WebMethod(EnableSession = true)] [Admin]	    public bool		                    VirtualArticle_Remove_Mapping( Guid id)
                                                        {
                                                            admin.demand();
                                                            return TM_Xml_Database.Current.remove_Mapping_VirtualId(id);																						
                                                        }

        [WebMethod(EnableSession = true)] [ReadArticles]        public string					    VirtualArticle_Get_GuidRedirect(Guid id)
                                                                                    {
                                                                                        readArticles.demand();
                                                                                        return TM_Xml_Database.Current.get_GuidRedirect(id);																						
                                                                                    }				
        [WebMethod(EnableSession = true)] [ReadArticles]        public TeamMentor_Article		    VirtualArticle_CreateArticle_from_ExternalServiceData(string service, string serviceData)
                                                                                    {
                                                                                        readArticles.demand();
                                                                                        return service.createArticle_from_ExternalServiceData(serviceData);																						
                                                                                    }

    }	
}
