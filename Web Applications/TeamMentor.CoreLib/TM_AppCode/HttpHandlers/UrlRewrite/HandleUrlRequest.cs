using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using Microsoft.Security.Application;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using System.IO;
using System.Security;
//using O2.XRules.Database.Utils;

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public class HandleUrlRequest
	{
		public HttpContext context = HttpContext.Current;
        public TM_WebServices tmWebServices;        

        public HandleUrlRequest()
        { 
            //tmWebServices  = new TM_WebServices();

            /*tmWebServices.tmAuthentication = new TM_Authentication(tmWebServices);
            tmWebServices.tmAuthentication.disable_CSRF_Check = true;
			tmWebServices.tmAuthentication.mapUserRoles();*/
        }

        public void routeRequestUrl_for404()
        {
            var fixedPath = context.Request.Url.AbsolutePath.replace("/html_pages/Gui/","/article/");   //deal with the cases where there is an relative link inside the html_pages/Gui viewer page
            handleUrlRewrite(fixedPath);
        }
        public void routeRequestUrl()
        {
            if (redirectedToSLL().isFalse())
                handleUrlRewrite(context.Request.Url);
        }

        private bool redirectedToSLL()
        {			            
            if (TMConfig.Current.SSL_RedirectHttpToHttps && !context.Request.IsLocal && !context.Request.IsSecureConnection)
            {
                string redirectUrl = context.Request.Url.ToString().Replace("http://", "https://");
				"Redirecting current request to https: {0}".info(context.Request.Url);
                context.Response.Redirect(redirectUrl);
                return true;
            }
            return false;
        }
        
        public void handleUrlRewrite(Uri uri)
        {
            try
            {
                if (shouldSkipCurrentRequest())
                    return;
                var absolutePath = uri.notNull() ? uri.AbsolutePath : context.Request.Url.AbsolutePath;
                
                //if (absolutePath.starts("/html_pages/Gui/")) //deal with the cases where there is an relative link inside the html_pages/Gui viewer page
                //    absolutePath = absolutePath.replace("/html_pages/Gui/", "/article/");   

                handleUrlRewrite(absolutePath);
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                    ex.log("[in handleUrlRewrite]");
            }
        }

        public void handleUrlRewrite(string path)
        {
            if(path.starts("/"))
                path = path.removeFirstChar();
            var splitedPath = path.split("/");
            if (splitedPath.size() > 0)
            { 
                var action = splitedPath.shift();   // extract first element               
                //var data = splitedPath.join("/");   // rejoin the rest
                var data = String.Join(",", splitedPath.ToArray());
                if (action.valid() && handleRequest(action, data))    //if we did process it , end the request here
                    endResponse();                    
            }                        
		}

        public bool shouldSkipCurrentRequest()
        {
            var path = context.Request.PhysicalPath;
            var extension = path.extension();
            switch (extension)
            {                 
                case ".htm":
                case ".js":
                case ".css":
                case ".html":
                case ".asmx":
                case ".ashx":
                case ".aspx":
                    return true;
                //default:
                //    return false;
            }
            var absolutePath = context.Request.Url.AbsolutePath;
            if (absolutePath.lower().contains("/images/"))
                return true;
            return false;
        }

        //All mappings are here
		public bool handleRequest(string action , string data)
		{
            try
            {
                tmWebServices = new TM_WebServices(true);       // enable webservices access (and security checks with CSRF disabled)
                action = Encoder.HtmlEncode(action);
                data = Encoder.HtmlEncode(data).replace("%20"," ");
                if (action.isGuid() & data.inValid())                
                    return redirectTo_Article(action);                                    
                switch (action.lower())
                {
                    case "gui":
                    case "teammentor":
                        return transfer_TeamMentorGui();
                    case "raw":                        
                        return handleAction_Raw(data);                                                      
                    case "html":
                        return handleAction_Html(data);
                    case "content":
                        return handleAction_Content(data);
                    case "xml":
                        return handleAction_Xml(data);
                    case "xsl":
                        return handleAction_Xsl(data,"TeamMentor_Article.xslt");
                    case "creole":
                        return handleAction_Xsl(data,"JsCreole_Article.xslt");          
                    case "notepad":
                        return handleAction_Xsl(data, "Notepad_Edit.xslt");                        
                    case "viewer":
                    case "article":
						return handle_ArticleViewRequest(data);                                     
                    case "edit":
                    case "editor":
                        return transfer_ArticleEditor(data);                    
                    case "create":
                        return handleAction_Create(data);    
                    case "admin":
                        return redirectTo_ControlPanel(false);
                    case "admin_extra":
                        return redirectTo_ControlPanel(true);
                    case "reload_config":
                        return reload_Config();
                    case "login":
                        return transfer_Login();
                    case "login_ok":
                        return handle_LoginOK();   
                    case "logout":
                        return redirectTo_Logout();
                    case "wsdl":
                        return redirectTo_Wsdl();
                    case "reload":
                        return reloadCache_and_RedirectToHomePage();
                    case "home":
                        return redirectTo_HomePage();
                    //case "images":                        
                    case "image":
                        return handleAction_Image(data);
                    case "jsonp":
                        return handleAction_JsonP(data);
                    case "debug":
                        return redirectTo_DebugPage();
                    case "library":
                        return redirectTo_SetLibrary(data);					
					case "library_download":
					case "download_library":
						return redirectTo_DownloadLibrary(data);
                    case "sso":
                        return handleAction_SSO(data);                                                            
                }
				
				tmWebServices.tmAuthentication.mapUserRoles(false);			 // enable  CSRF protection
				switch (action.lower())
				{
					case "external":
						return showVirtualArticleExternal(data);
					case "virtualarticles":
						return showVirtualArticles();
					case "addvirtualarticle":
						return addVirtualArticleMapping(data);
					case "removevirtualarticle":
						return removeVirtualArticleMapping(data);
					default:
						return false;  
				}
            }                
            catch (Exception ex)
            {
                if (ex is SecurityException)
                    return transfer_Login();
              //      return redirectTo_Login();
                if (ex.Message != "Thread was being aborted.")
                {
                    ex.log();
                    //context.Response.Write("<h2>Error: {0} </h2>".format(ex.Message));
                }
            }                                    
            return false;			
		}

        public bool reload_Config()
        {
            TMConfig.loadConfig();
            return redirectTo_HomePage();
        }

		//Virtual Articles
		public bool showVirtualArticles()
		{
			var virtualArticles = tmWebServices.VirtualArticle_GetCurrentMappings();
			var xmlContent = virtualArticles.serialize(false);
			context.Response.ContentType = "text/xml";
			context.Response.Write(xmlContent);
			return true;
		}
		public bool addVirtualArticleMapping(string data)
		{
			tmWebServices.VirtualArticle_GetCurrentMappings();  // will trigger an authorization check if needed
			try
			{
				Action<VirtualArticleAction> outputVirtualArticleActionAsXml = 
						(virtualArticleAction)=> {
													context.Response.ContentType = "text/xml";
													var xml = virtualArticleAction.serialize(false);
													context.Response.Write(xml);
												};
				var mappings = data.split(",");
				if (mappings.size() == 2)				
				{					
					var guid1 = mappings[0].guid();   //will throw exception if guid not valid
					if (guid1 != Guid.Empty)
					{
						if (mappings[1].isGuid())		  // VIRTUAL ID
						{
							var guid2 = mappings[1].guid();
							if (guid2 != Guid.Empty)
							{
								var virtualArticleAction = tmWebServices.VirtualArticle_Add_Mapping_VirtualId(guid1, guid2);
								outputVirtualArticleActionAsXml(virtualArticleAction);
								return true;
							}
						}
						else
						{
							var redirectUrl = "http://{0}/article/{1}".format(mappings[1], guid1);
							var virtualArticleAction = tmWebServices.VirtualArticle_Add_Mapping_Redirect(guid1, redirectUrl);
							outputVirtualArticleActionAsXml(virtualArticleAction);
						}
					}
				}
				else if (mappings.size() == 3)
				{
					var guid1 = mappings[0].guid();
					if (mappings[2].isGuid())				// EXTERNAL ARTICLE
					{
						var tmServer = mappings[1].uri();
						var guid2 = mappings[2].guid();
						if (guid1 != Guid.Empty && guid2 != Guid.Empty && tmServer.notNull())
						{
							var virtualArticleAction = tmWebServices.VirtualArticle_Add_Mapping_ExternalArticle(guid1, tmServer.str(), guid2);
							outputVirtualArticleActionAsXml(virtualArticleAction);
							return true;
						}
					}
					else								 // EXTERNAL SERVICE
					{
						var service = mappings[1];
						var serviceData = mappings[2];
						var virtualArticleAction = tmWebServices.VirtualArticle_Add_Mapping_ExternalService(guid1, service, serviceData);
						outputVirtualArticleActionAsXml(virtualArticleAction);						
						return true;
					}					
				}
			}
			catch (Exception ex)
			{
				ex.log();
			}			
			return true;
		}
		public bool removeVirtualArticleMapping(string data)
		{
			if (data.isGuid())
			{
				if (tmWebServices.VirtualArticle_Remove_Mapping(data.guid()))
				{
					context.Response.Write("Mapping removed");
					return true;
				}
			}
			context.Response.Write("Provided mapping data could not be parsed");
			return true;
		}
		public bool showVirtualArticleExternal(string data)
		{
			var mappings = data.split(",");
			{
				if (mappings.size() == 2)
				{
					var service = mappings[0];
					var serviceData = mappings[1];
					var article = tmWebServices.VirtualArticle_CreateArticle_from_ExternalServiceData(service, serviceData);
					if (article.isNull())
						context.Response.Write("There was a problem fetching the requested data");
					else 
					{
						context.Response.ContentType = "text/html";						
						var htmlTemplateFile = @"\Html_Pages\Gui\Pages\article_Html.html";
						var htmlTemplate = context.Server.MapPath(htmlTemplateFile).fileContents();

						var htmlContent = htmlTemplate.replace("#ARTICLE_TITLE", article.Metadata.Title)
													  .replace("#ARTICLE_HTML", article.Content.Data.Value);
						context.Response.Write(htmlContent);           
					}					
				}
			}
			return true;
		}
        //handlers
		private bool handleAction_JsonP(string data)
		{
			var guid = tmWebServices.getGuidForMapping(data);
			if (guid != Guid.Empty)
			{
				var article = tmWebServices.GetGuidanceItemById(guid.str());

				var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
				var serializedData = serializer.Serialize(article);
				var callbackRaw = context.Request["callback"];
				if (callbackRaw.valid())
				{
					var callbackFunction = Encoder.JavaScriptEncode(callbackRaw, false);
					context.Response.Write("{0}({1})".format(callbackFunction, serializedData));
				}
				else
					context.Response.Write(serializedData);
				return true;
			}
			return false;
		}
        private bool handleAction_Image(string data)
        {            
            
            var imagePath = tmWebServices.javascriptProxy.tmXmlDatabase.Get_Path_To_File(data);            
            if (imagePath.fileExists())
            {
                context.Response.ContentType = "image/{0}".format(data.extension().removeFirstChar());
                context.Response.WriteFile(imagePath);
                return true;
            }            
            return false;
        }

        private bool handleAction_Xsl(string data, string xsltToUse)
        {
            //if (this.tmWebServices.tmAuthentication.sessionID. UserRole.ReadArticles
            var xstlFile = context.Server.MapPath("\\xslt\\" + xsltToUse);
            if (xstlFile.fileExists())
            {
                var guid = tmWebServices.getGuidForMapping(data);
                if (guid != Guid.Empty)
                {
                    var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);
                                                  //.add_Xslt(xsltToUse);
                    if (xmlContent.valid())
                    {
                        //var xslTransform = new System.Xml.Xsl.XslTransform();
						var xslTransform = new System.Xml.Xsl.XslCompiledTransform();
						
                        xslTransform.Load(xstlFile);

                        var xmlReader = new System.Xml.XmlTextReader(new StringReader(xmlContent));
                        var xpathNavigator = new System.Xml.XPath.XPathDocument(xmlReader);
                        var stringWriter = new StringWriter();

                        xslTransform.Transform(xpathNavigator, new System.Xml.Xsl.XsltArgumentList(), stringWriter);

                        context.Response.ContentType = "text/html";
                        context.Response.Write(stringWriter.str());
                        return true;
                    }
                    return false;
                }
                return transfer_ArticleViewer();
                    
            }
            return false;
        }

        private bool handleAction_Create(string data)
        {
            var article = new TeamMentor_Article();
            article.Metadata.Title = data.urlDecode();
            var xmlContent = article.serialize(false)
                                    .add_Xslt("Article_Edit.xslt"); 
            context.Response.ContentType = "application/xml";
            context.Response.Write(xmlContent);  
            return true;
        }
        //data, );                    

        private bool handleAction_Raw(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {                
                var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);
                context.Response.ContentType = "application/xml";
                context.Response.Write(xmlContent);
            }
            else
                transfer_ArticleViewer();
            return true;
        }
        private bool handleAction_Xml(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {                
                var xmlContent = tmWebServices.GetGuidanceItemHtml(guid);
                context.Response.ContentType = "application/xml";
                context.Response.Write(xmlContent);
            }
            else
               transfer_ArticleViewer();
            return true;
        }
        private bool handleAction_Html(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {
                context.Response.ContentType = "text/html";
                var article = tmWebServices.GetGuidanceItemById(guid.str());
                var htmlTemplateFile = (article.Content.DataType.lower() == "wikitext") 
                                            ? @"\Html_Pages\Gui\Pages\article_wikiText.html" 
                                            : @"\Html_Pages\Gui\Pages\article_Html.html";
                var htmlTemplate = context.Server.MapPath(htmlTemplateFile).fileContents();
                    
                var htmlContent = htmlTemplate.replace("#ARTICLE_TITLE", article.Metadata.Title)
                                              .replace("#ARTICLE_HTML", article.Content.Data.Value);
                context.Response.Write(htmlContent);                
            }
            else
                transfer_ArticleViewer();
            return true;
        }
		private bool handle_ArticleViewRequest(string data)
		{			
			if ( data.isGuid())
			{				
				var guid = data.guid();
				if (tmWebServices.GetGuidanceItemById(guid.str()).isNull())
				{
					var redirectTarget = tmWebServices.VirtualArticle_Get_GuidRedirect(guid);
					if (redirectTarget.valid())
					{
						context.Response.Redirect(redirectTarget); ;
						return false;
					}
				}
			}
			return transfer_ArticleViewer();       
		}		
        private bool handleAction_Content(string data)
        { 
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {
                context.Response.ContentType = "text/html";
                var htmlContent = tmWebServices.GetGuidanceItemHtml(guid);
                context.Response.Write(htmlContent);
            }
            else
                transfer_ArticleViewer();
            return true;

        }
        private bool handleAction_SSO(string data)
        {
            new SecurityInnovation.TeamMentor.Authentication.SingleSignOn().authenticateUserBasedOn_SSOToken();
            return true;
        }
        
        //utils
        public void endResponse()
        { 
            context.Response.Flush();
            context.Response.End();
        }
        private bool reloadCache_and_RedirectToHomePage()
        {
            tmWebServices.XmlDatabase_ReloadData();
            return redirectTo_HomePage();
        }
        public bool transfer_TeamMentorGui()
        {
            context.Server.Transfer("/html_pages/Gui/TeamMentor.html");            
            return false;
        }
		public bool transfer_ArticleViewer()
		{			
			context.Server.Transfer("/html_pages/GuidanceItemViewer/GuidanceItemViewer.html");						
            return false;    
		}        
        public bool transfer_ArticleEditor(string data)
		{         
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid == Guid.Empty)
                return transfer_ArticleViewer();

            tmWebServices.XmlDatabase_GetGuidanceItemXml(guid); // will trigger an Security exception if the user if not authorized

            context.Server.Transfer("/html_pages/GuidanceItemEditor/GuidanceItemEditor.html");
            return false;    
		}
        public bool transfer_Login()	
		{
			var loginReferer = context.Request.QueryString["LoginReferer"];
			var redirectTarget =  (loginReferer.notNull() && loginReferer.StartsWith("/"))
										? loginReferer
										: context.Request.Url.AbsolutePath;
			if (redirectTarget.lower() == "/login")
				redirectTarget = "/";
			context.Response.Redirect("/Html_Pages/Gui/Pages/login.html?LoginReferer=" + redirectTarget);
	        //context.Response.ContentType = "text/html";    
			//context.Server.Transfer("/Html_Pages/Gui/Pages/login.html");
            //context.Session["LoginReferer"] = context.Request.Url.AbsolutePath;
            return true; 
		}
		


        public bool handle_LoginOK()
        {
            var loginReferer = context.Request.QueryString["LoginReferer"];
            if (loginReferer.notNull() && loginReferer.StartsWith("/"))
                context.Response.Redirect(loginReferer);
            else
                context.Response.Redirect("/");
            return true;
        }

        public bool redirectTo_HomePage()
		{	            
		    context.Response.Redirect("/");                        	
            return false; 
		}

        public bool redirectTo_Login()
		{	            
		    context.Response.Redirect("/Login");                        	
            return false; 
		}
        
        public bool redirectTo_Logout()
		{	
            tmWebServices.Logout();
		    context.Response.Redirect("/");                        	
            return false; 
		}

		public bool redirectTo_DownloadLibrary(string data)
		{
			var uploadToken = new TM_WebServices().GetUploadToken();
			context.Response.Redirect("/Aspx_Pages/Library_Download.ashx?library={0}&uploadToken={1}".format(data, uploadToken));
			return false;
		}

        public bool redirectTo_Wsdl()
        {
            context.Response.Redirect("/Aspx_Pages/TM_WebServices.asmx");
            return false;
        }

        public bool redirectTo_ControlPanel(bool includeExtraTag)
		{			
            var adminUrl = "/html_pages/ControlPanel/controlpanel.html" + ((includeExtraTag) ? "?extra" : "");
			context.Response.Redirect(adminUrl);
            return false;    
		}
                       
        public bool redirectTo_Article(string article)
		{			
			context.Response.Redirect("/article/{0}".format(article));
            return false;    
		}        

        public bool redirectTo_DebugPage()
		{			
			context.Response.Redirect("/Aspx_Pages/Debug.aspx");
            return false;    
		}   

        public bool redirectTo_SetLibrary(string libraryIdOrName)
		{			
			context.Response.Redirect("/aspx_pages/SetLibrary.aspx?Library={0}".format(libraryIdOrName));
            return false;    
		}           
        
	}

    public static class HelperExtensionMethods
    {
        public static string add_Xslt(this string xmlContent, string xsltToUse)
        { 
            //var xmlSignature = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            var signature = "<TeamMentor_Article Metadata_Hash=";
            var xsltText = "<?xml-stylesheet type=\"text/xsl\" href=\"/xslt/{0}\"?>".format(xsltToUse);
            xmlContent = xmlContent.replace(signature, xsltText.line().append(signature));
            return xmlContent;
        }
    }
}