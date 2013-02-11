using System;
using System.Collections.Generic;
using System.Web.Services;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{ 					
    //WebServices related to: Config Methods
    public partial class TM_WebServices 
    {		
        
        [WebMethod(EnableSession = true)] public string GetTime() 						{   return "...Via Proxy:" + DateTime.Now.str(); }  	         
        [WebMethod(EnableSession = true)] public string Ping(string message)  			{   return "received ping: {0}".format(message); }
        
        //Xml Database Specific
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_GetDatabasePath()		{	return TM_Xml_Database.Current.Path_XmlDatabase;	}
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_GetLibraryPath()		{	return TM_Xml_Database.Current.Path_XmlLibraries;	}		
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_ReloadData()			{	
                                                                                                                                        guiObjectsCacheOk = false; 
                                                                                                                                        return  TM_Xml_Database.Current.ReloadData(null); 
                                                                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public bool XmlDatabase_ImportLibrary_fromZipFile(string pathToZipFile, string unzipPassword)
                                                                                                                                    {
                                                                                                                                        if (TM_Xml_Database.Current.xmlDB_Libraries_ImportFromZip(pathToZipFile, unzipPassword))
                                                                                                                                        {
                                                                                                                                            XmlDatabase_ReloadData();
                                                                                                                                            return true;
                                                                                                                                        }
                                                                                                                                        return false;																																		
                                                                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_SetLibraryPath(string libraryPath)		
                                                                                                                                    {	guiObjectsCacheOk = false; 
                                                                                                                                        return  TM_Xml_Database.Current.ReloadData(libraryPath); 
                                                                                                                                    }

        [WebMethod(EnableSession = true)] public List<Guid>     XmlDatabase_GuidanceItems_SearchTitleAndHtml(List<Guid> guidanceItemsIds, string searchText)		{	 return  TM_Xml_Database.Current.guidanceItems_SearchTitleAndHtml(guidanceItemsIds,searchText); }																																		
        [WebMethod(EnableSession = true)] public string         XmlDatabase_GetGuidanceItemXml(Guid guidanceItemId)	    {	return  TM_Xml_Database.Current.xmlDB_guidanceItemXml(guidanceItemId); }        
        [WebMethod(EnableSession = true)] public string         XmlDatabase_GetGuidanceItemPath(Guid guidanceItemId)	{	return  TM_Xml_Database.Current.xmlDB_guidanceItemPath(guidanceItemId); }                
                                                                    
        [WebMethod(EnableSession = true)] public string         RBAC_CurrentIdentity_Name()				                {	return new UserRoleBaseSecurity().currentIdentity_Name(); }
        [WebMethod(EnableSession = true)] public bool           RBAC_CurrentIdentity_IsAuthenticated()	                {	return new UserRoleBaseSecurity().currentIdentity_IsAuthenticated(); }
        [WebMethod(EnableSession = true)] public List<string>   RBAC_CurrentPrincipal_Roles()		                    {	return new UserRoleBaseSecurity().currentPrincipal_Roles().toList(); }
        [WebMethod(EnableSession = true)] public bool           RBAC_HasRole(string role)					            {	return RBAC_CurrentPrincipal_Roles().contains(role); }
        [WebMethod(EnableSession = true)] public bool           RBAC_IsAdmin()											{	return RBAC_CurrentPrincipal_Roles().contains("Admin"); }        

        [WebMethod(EnableSession = true)]		                public Guid		SSO_AuthenticateUser(string ssoToken)            {   return new SingleSignOn().authenticateUserBasedOn_SSOToken(ssoToken); }
        [WebMethod(EnableSession = true)] [Admin]			    public string	SSO_GetSSOTokenForUser(string userName)          {   return new SingleSignOn().getSSOTokenForUser(userName); }
        [WebMethod(EnableSession = true)] [Admin]			    public TMUser	SSO_GetUserFromSSOToken(string ssoToken)         {   return new SingleSignOn().getUserFromSSOToken(ssoToken); }                
                                                                                                                
        [WebMethod(EnableSession = true)] [Admin]		        public string		GitHub_Pull_Origin()	            {	return Git.syncWithGitHub_Pull_Origin();  }
        [WebMethod(EnableSession = true)] [Admin]		        public string		GitHub_Push_Origin()	            {	return Git.syncWithGitHub_Push_Origin();  }
        [WebMethod(EnableSession = true)] [Admin]	            public string		GitHub_Push_Commit()	            {	return Git.syncWithGitHub_Commit();  }
        [WebMethod(EnableSession = true)] [Admin]	            public string		Git_Execute(string gitCommand)		{	return Git.executeGitCommand(gitCommand);  }
        
        [WebMethod(EnableSession = true)] [Admin]	            public string		CreateWebEditorSecret()	
                                                                                                    {
                                                                                                        var webEditorSecretDataFile = AppDomain.CurrentDomain.BaseDirectory.pathCombine("webEditorSecretData.config");
                                                                                                        Guid.NewGuid().str().serialize(webEditorSecretDataFile);
                                                                                                        return webEditorSecretDataFile.load<string>();
                                                                                                        //this (below) doesn't work because the webeditor is an *.ashx and doesn't have access to the HttpContext Session object
                                                                                                        /*var session = HttpContextFactory.Current.Session;
                                                                                                        if (session["webEditorSecretData"].isNull())
                                                                                                            session["webEditorSecretData"] = Guid.NewGuid().str();
                                                                                                        return (string)session["webEditorSecretData"];
                                                                                                        */
                                                                                                    }						
        [WebMethod(EnableSession = true)] [Admin]	            public string		TMConfigFileLocation()			{	return TMConfig.Location;  }		
        [WebMethod(EnableSession = true)] [Admin]	            public TMConfig		TMConfigFile()
                                                                                    {	
                                                                                        return TMConfig.Current;  
                                                                                    }																					
        
        [WebMethod(EnableSession = true)] [Admin]	            public string		Get_Libraries_Zip_Folder()
                                                                                    {
                                                                                        var librariesZipsFolder = TMConfig.Current.LibrariesUploadedFiles;
                                                                                        return TM_Xml_Database.Current.Path_XmlDatabase.fullPath().pathCombine(librariesZipsFolder).fullPath();
                                                                                    }		
        [WebMethod(EnableSession = true)] [Admin]	            public List<string> Get_Libraries_Zip_Folder_Files()
                                                                                    {
                                                                                        return Get_Libraries_Zip_Folder().files().fileNames();
                                                                                    }																							
        [WebMethod(EnableSession = true)] [Admin]	            public string		Set_Libraries_Zip_Folder(string folder)
                                                                                    {
                                                                                        var tmConfig = TMConfig.Current;
                                                                                        tmConfig.LibrariesUploadedFiles = folder;
                                                                                        //folder.createDir();
                                                                                        if (tmConfig.SaveTMConfig())																																										
                                                                                            return "Path set to '{0}' which currently has {1} files".format(folder.fullPath(), folder.files().size());
                                                                                        return null;
                                                                                    }

        [WebMethod(EnableSession = true)] [Admin]	           public Guid			GetUploadToken()
                                                                                    {
                                                                                        var uploadToken = Guid.NewGuid();
                                                                                        FileUpload.UploadTokens.Add(uploadToken);
                                                                                        return uploadToken;
                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	           public string		GetLogs()        
                                                                                    {
                                                                                        var logData = O2.Kernel.PublicDI.log.LogRedirectionTarget.prop("LogData").str() ;
                                                                                        return logData;
                                                                                    }        
/*        [WebMethod(EnableSession = true)] [Admin]	           public List<KeyValue<Guid, string>>				Data_GuidanceItems_FileMappings()        
                                                                                    {
                                                                                        return TM_Xml_Database.Current.GuidanceItems_FileMappings.ConvertDictionary();
                                                                                    }		
        [WebMethod(EnableSession = true)] [Admin]	           public List<KeyValue<Guid, TeamMentor_Article>> Data_GuidanceItems_Cached_GuidanceItems()        
                                                                                    {
                                                                                        return TM_Xml_Database.Current.Cached_GuidanceItems.ConvertDictionary();
                                                                                    }*/
        
        [WebMethod(EnableSession = true)] [Admin]	           public string		REPL_ExecuteSnippet(string snippet)        
                                                                                    {
                                                                                        return REPL.executeSnippet(snippet);
                                                                                    }
        
        //Virtual Articles
        [WebMethod(EnableSession = true)] [Admin]	            public List<VirtualArticleAction>	VirtualArticle_GetCurrentMappings()        
                                                                                    {
                                                                                        return TM_Xml_Database.Current.getVirtualArticles().Values.toList();
                                                                                    }				
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_VirtualId( Guid id, Guid virtualId)
                                                                                    {
                                                                                        return TM_Xml_Database.Current.add_Mapping_VirtualId(id, virtualId);																						
                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_Redirect (Guid id, string redirectUri)
                                                                                    {
                                                                                        return TM_Xml_Database.Current.add_Mapping_Redirect(id, redirectUri.uri());																						
                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_ExternalArticle(Guid id, string tmServer, Guid externalId)
                                                                                    {
                                                                                        return TM_Xml_Database.Current.add_Mapping_ExternalArticle(id, tmServer, externalId);																						
                                                                                    }			
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_ExternalService(Guid id, string service, string data)
                                                                                    {
                                                                                        return TM_Xml_Database.Current.add_Mapping_ExternalService(id, service, data);																						
                                                                                    }			
        [WebMethod(EnableSession = true)] [Admin]	            public bool							VirtualArticle_Remove_Mapping( Guid id)
                                                                                    {
                                                                                        return TM_Xml_Database.Current.remove_Mapping_VirtualId(id);																						
                                                                                    }
        [WebMethod(EnableSession = true)] [ReadArticles]        public string					VirtualArticle_Get_GuidRedirect(Guid id)
                                                                                    {
                                                                                        return TM_Xml_Database.Current.get_GuidRedirect(id);																						
                                                                                    }				
        [WebMethod(EnableSession = true)] [ReadArticles]                        public TeamMentor_Article		VirtualArticle_CreateArticle_from_ExternalServiceData(string service, string serviceData)
                                                                                    {
                                                                                        return service.createArticle_from_ExternalServiceData(serviceData);																						
                                                                                    }
        
        //Article Guid Mappings
        [WebMethod(EnableSession = true)]		                public Guid getGuidForMapping(string mapping)
        {
            return TM_Xml_Database.Current.xmlBD_resolveMappingToArticleGuid(mapping);
        }
        [WebMethod(EnableSession = true)]		                public bool IsGuidMappedInThisServer(Guid guid)
                                                {
                                                    if (GetGuidanceItemById(guid).notNull())
                                                        return true;
                                                    if (TM_Xml_Database.Current.get_GuidRedirect(guid).valid())
                                                        return true;
                                                    return false;
                                                }
    }	
}
