using System;
using System.Collections.Generic;
using System.Web;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using FluentSharp.Web35;
using Microsoft.Security.Application;
using System.IO;
using System.Security;
using TeamMentor.FileStorage;
using TeamMentor.SiteData;

namespace TeamMentor.CoreLib
{
    public class HandleUrlRequest
    {
        public static Dictionary<string, string> Server_Transfers           { get; set; }
        public static Dictionary<string, string> Response_Redirects         { get; set; }
        public static bool                       SSL_Redirection_Disabled   { get; set; }

        public HttpContextBase context = HttpContextFactory.Current;
        public HttpRequestBase request = HttpContextFactory.Request;
        public TM_WebServices  tmWebServices;

        static HandleUrlRequest()
        {
            Server_Transfers = new Dictionary<string, string>();
            Response_Redirects = new Dictionary<string, string>();
            setDefault_Redirects_and_Transfers();
        }

        public static void setDefault_Redirects_and_Transfers()
        {
            Server_Transfers.add("teammentor", "/html_pages/Gui/TeamMentor.html")
                            .add("articleviewer", "/html_pages/GuidanceItemViewer/GuidanceItemViewer.html")
                            .add("articleeditor", "/html_pages/GuidanceItemEditor/GuidanceItemEditor.html")
                            .add("passwordreset", "/Html_Pages/Gui/Pages/passwordReset.html")
                            .add("passwordforgot", "/Html_Pages/Gui/Pages/passwordForgot.html")
                            .add("passwordexpired", "/Html_Pages/Gui/Pages/passwordReset.html")                            
                            .add("error", "/Html_Pages/Gui/Pages/errorPage.html");

            Response_Redirects.add("csharprepl" , "/html_pages/ControlPanel/CSharp_REPL/Repl.html")
                              .add("tbot"       , "/rest/tbot/run/Commands")
                              .add("asmx"       , "/aspx_pages/TM_WebServices.asmx")
                              .add("wcf"        , "/rest/help")
                              .add("home"       , "/")
                              .add("debug"      , "/Aspx_Pages/Debug.aspx")
                              .add("admin"      , "/html_pages/ControlPanel/controlpanel.html")
                              .add("admin_extra", "/html_pages/ControlPanel/controlpanel.html?extra");
                
        }
        public void transfer_Request(string action)
        {          
            action = action.lower();
            if (Server_Transfers.hasKey(action))
            {        
                if (action == "teammentor")
                    tmWebServices.logUserActivity("Open TeamMentor", "The HomePage (ie. http://.../teamMentor)");
                context.Response.ContentType = "text/html";
                Server_Transfers[action].siteData_Handle_VirtualPath();
                context.Server.Transfer(Server_Transfers[action],true);   // will throw "Thread was being aborted exception                
            }            
        }

        public void response_Redirect(string action)
        {            
            if (Response_Redirects.hasKey(action))
                context.Response.Redirect(Response_Redirects[action]);
                
        }

        /*public bool transfer_TeamMentorGui()
        {
            context.Server.Transfer("/html_pages/Gui/TeamMentor.html");            
            return false;
        }*/        

        public void routeRequestUrl_for404()
        {
            if (request.Url != null)
            {
                var fixedPath = request.Url.AbsolutePath.replace("/html_pages/Gui/", "/article/");
                    //deal with the cases where there is an relative link inside the html_pages/Gui viewer page

                 if (fixedPath.siteData_Handle_VirtualPath().isFalse())
                    handleUrlRewrite(fixedPath);
            }
        }
        public void routeRequestUrl()
        {
            if (redirectedToSSL().isFalse())
                handleUrlRewrite(context.Request.Url);
        }
        public bool redirectedToSSL()
        {
            try
            {           
                if (SSL_Redirection_Disabled)               // so that we only do this check once per server
                    return false;
                if (TMConfig.Current.TMSecurity.SSL_RedirectHttpToHttps && 
                    context.Request.IsLocal.isFalse() && 
                    context.Request.IsSecureConnection.isFalse())
                {
                    if (request.Url != null)
                    {
                        if (sslPageIsAvailable())           // addresses issue that happened when an SSL redirection was set for a server without SSL configured
                        {
                            var originalRequest = request.Url.str();
                            var redirectUrl     = originalRequest.Replace("http://", "https://");
                            "[redirectedToSSL] Redirecting original request '{0}' to {1}".info(originalRequest,redirectUrl);
                            context.Response.Redirect(redirectUrl);
                            return true;
                        }
                        "[redirectedToSSL] since sslPageIsAvailable was failed, setting SSL_Redirection_Disabled to true".debug();
                        SSL_Redirection_Disabled = true;                // prevents multiple attemps to check for redirections (was causing lots of error messages on live servers)                       
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("[in redirectedToSLL]");
            }            
            return false;
        }

        public bool sslPageIsAvailable()
        {
            if (request.Url != null)
            {
                var currentServer = (request.Url.PathAndQuery != "/") 
                                        ? request.Url.str().remove(request.Url.PathAndQuery)
                                        : request.Url.str();
                var httpLink = currentServer.replace("http://", "https://");                
                if (httpLink.contains("https://"))
                { 
                    if (httpLink.uri().HEAD())
                    {
                        "[sslPageIsAvailable] got valid HEAD request from: {0}".debug(httpLink);
                        return true;
                    }                    
                    "[sslPageIsAvailable]HEAD request to server failed!: {0}".error(httpLink);
                    return false;
                }
                "[sslPageIsAvailable] no https:// on '{0}' original request is '{1}".info(currentServer,request.Url.str());                
            }
            return false;
        }

        public bool handleUrlRewrite(Uri uri)
        {
            try
            {                       
                var absolutePath = uri.notNull() ? uri.AbsolutePath 
                                                 : request.Url.notNull() 
                                                    ? request.Url.AbsolutePath
                                                    : null;

                if (absolutePath.isNull())
                    return false;
                if (absolutePath.siteData_Handle_VirtualPath())
                    return true;
                if (shouldSkipCurrentRequest())
                    return true;                
                               

                return handleUrlRewrite(absolutePath);
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                { 
                    ex.logWithStackTrace("[at handleUrlRewrite]");
                    return true;
                }
                return false;
            }
        }

        
        public bool handleUrlRewrite(string path)
        {
            if (path.isNull())
                return false;
            if(path.starts("/"))
                path = path.removeFirstChar();
            var splitedPath = path.split("/");
            if (splitedPath.size() > 0)
            { 
                var action = splitedPath.shift();   // extract first element                               
                var data = String.Join(",", splitedPath.ToArray());
                if (action.valid())                
                    return handleRequest(action, data);                
            }     
            return false;       
        }
        public bool shouldSkipCurrentRequest()
        {
            if (request.Url == null)
                return false;
            var path = context.Request.PhysicalPath;
            var extension = path.extension();
            switch (extension)
            {                 
                //case ".htm":
                //case ".js":
                //case ".css":
                //case ".html":
                case ".asmx":
                case ".ashx":
                case ".aspx":
                //case ".ico":
                //case ".woff":
                    return true;
                //default:
                //    return false;
            }
            var absolutePath = request.Url.AbsolutePath;
            if (absolutePath.lower().contains("/images/","/javascript/","/html_pages","/css/") || 
                extension.contains(".ico"))
            {
                if(TMConfig.Current.enable304Redirects() && context.sent304Redirect())
                    context.Response.End();
                return true;    
            }
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
                {
                    redirectTo_Article(action);
                    endResponse();
                }
                
                transfer_Request(action.lower());       // throw "Thread was being aborted." exception if worked
                response_Redirect(action.lower());      // throw "Thread was being aborted." exception if worked
                
                //content viewer
                switch (action.lower())
                {
                    case "raw":
                        handleAction_Raw(data);
                        break;
                    case "html":
                        handleAction_Html(data);
                        break;
                    case "content":
                        handleAction_Content(data);
                        break;
                    case "xml":
                        handleAction_Xml(data);
                        break;
                    case "xsl":
                        handleAction_Xsl(data, "TeamMentor_Article.xslt");
                        break;
                    case "creole":
                        handleAction_Xsl(data, "JsCreole_Article.xslt");
                        break;
                    
                    case "image":
                        handleAction_Image(data);
                        break;
                    case "jsonp":
                        handleAction_JsonP(data);
                        break;
                    case "viewer":
                    case "article":
                    case "a":
                        handle_ArticleViewRequest(data);
                        break;
                    case "edit":
                    case "editor":
                         handle_ArticleEditRequest(data);
                        break;
                    case "notepad":
                        handleAction_Xsl(data, "Notepad_Edit.xslt");
                        break;
                    case "create":
                        handleAction_Create(data);
                        break;
                }
                //user actions
                switch (action.lower())
                {
                    case "login":
                        redirect_Login();
                        break;
                    case "login_ok":
                        handle_LoginOK();
                        break;
                    case "logout":
                        redirectTo_Logout();
                        break;
                    
                    case "library":
                        redirectTo_SetLibrary(data);
                        break;
                }
                //admin actions
                switch (action.lower())
                {
//                    case "reload":
//                        reloadCache_and_RedirectToHomePage();
//                        break;                                       
//                    case "reload_config":
//                        reload_Config();                    
//                        break;
                    //case "reload_userdata":
                    //    reload_UserData();
                        //break;
                    case "library_download":
                    case "download_library":
                        redirectTo_DownloadLibrary(data);
                        break;
                        //case "sso":
                        //    return handleAction_SSO();                                                            
                }
                
                tmWebServices.tmAuthentication.mapUserRoles();			 // enable  CSRF protection
                switch (action.lower())
                {
                    case "external":
                        showVirtualArticleExternal(data);
                        break;
                    case "virtualarticles":
                        showVirtualArticles();
                        break;
                    case "addvirtualarticle":
                        addVirtualArticleMapping(data);
                        break;
                    case "removevirtualarticle":
                        removeVirtualArticleMapping(data);
                        break;                    
                    case "whoami":
                        showWhoAmI(data);
                        break;
                }
            }                
            catch (Exception ex)
            {
                if (ex is SecurityException)
                {
                    var originalUrl = "/{0}/{1}".format(action,data.replace(",","/"));
                    redirect_Login_AccessDenied(originalUrl);              
                }
                if (ex.Message != "Thread was being aborted.")                
                    ex.logWithStackTrace("at handleRequest");    
                else
                    return true;                                                                  
            }   
            return false;                                 		
        }
                
        //Virtual Articles
        public void showVirtualArticles()
        {
            var virtualArticles = tmWebServices.VirtualArticle_GetCurrentMappings();
            var xmlContent = virtualArticles.serialize(false);
            context.Response.ContentType = "text/xml";
            context.Response.Write(xmlContent);
            endResponse();
        }
        public void addVirtualArticleMapping(string data)
        {
            tmWebServices.VirtualArticle_GetCurrentMappings();  // will trigger an authorization check if needed
            try
            {
                Action<VirtualArticleAction> outputVirtualArticleActionAsXml = 
                        (virtualArticleAction)=> {
                                                    context.Response.ContentType = "text/xml";
                                                    var xml = virtualArticleAction.serialize(false);
                                                    context.Response.Write(xml);
                                                    endResponse();
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
                        }
                    }
                    else								 // EXTERNAL SERVICE
                    {
                        var service = mappings[1];
                        var serviceData = mappings[2];
                        var virtualArticleAction = tmWebServices.VirtualArticle_Add_Mapping_ExternalService(guid1, service, serviceData);
                        outputVirtualArticleActionAsXml(virtualArticleAction);						                        
                    }					
                }
            }
            catch (Exception ex)
            {
                ex.log();
            }			            
        }
        public void removeVirtualArticleMapping(string data)
        {
            if (data.isGuid())
            {
                if (tmWebServices.VirtualArticle_Remove_Mapping(data.guid()))
                {
                    context.Response.Write("Mapping removed");
                    endResponse(); 
                }
            }
            context.Response.Write("Provided mapping data could not be parsed");
            endResponse(); 
        }
        public void showVirtualArticleExternal(string data)
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
                    endResponse();
                }
            }            
        }
        
        
        //handlers
        public void handleAction_JsonP(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {
                var article = tmWebServices.GetGuidanceItemById(guid);

                var serializedData = article.javascript_Serialize();
                var callbackRaw = context.Request["callback"];
                if (callbackRaw.valid())
                {
                    var callbackFunction = Encoder.JavaScriptEncode(callbackRaw, false);
                    context.Response.Write("{0}({1})".format(callbackFunction, serializedData));
                }
                else
                    context.Response.Write(serializedData);
                endResponse();
            }            
        }
        public void handleAction_Image(string data)
        {            
            
            var imagePath = TM_FileStorage.Current.Get_Path_To_File(data);            
            if (imagePath.fileExists())
            {
                context.Response.ContentType = "image/{0}".format(data.extension().removeFirstChar());
                context.Response.WriteFile(imagePath);
                endResponse();
            }                        
        }
        public void handleAction_Xsl(string data, string xsltToUse)
        {
            //if (this.TmWebServices.tmAuthentication.sessionID. UserRole.ReadArticles
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

                        var article = tmWebServices.GetGuidanceItemById(guid);
                        switch(xsltToUse)
                        {
                            case "Notepad_Edit.xslt":
                                tmWebServices.RBAC_Demand_EditArticles();           // will trigger an Security exception if the user if not authorized
                                tmWebServices.logUserActivity("Edit Article (Notepad)", "{0} ({1})".format(article.Metadata.Title, guid));
                                break;
                            case "TeamMentor_Article.xslt":
                                tmWebServices.logUserActivity("View Article (xslt)", "{0} ({1})".format(article.Metadata.Title, guid));
                                break;
                            case "JsCreole_Article.xslt":
                                tmWebServices.logUserActivity("View Article (wiki)", "{0} ({1})".format(article.Metadata.Title, guid));
                                break;
                            default:
                                tmWebServices.logUserActivity("View Article ({0})", "{1} ({2})".format(xsltToUse, data,xsltToUse));
                                break;
                        }                        

                        endResponse();                        
                    }                    
                }
                else
                    transfer_Request("articleViewer");              // will trigger exception
            }            
        }
        public void handleAction_Create(string data)
        {
            var article = new TeamMentor_Article {Metadata = {Title = data.urlDecode()}};
            var xmlContent = article.serialize(false)
                                    .add_Xslt("Article_Edit.xslt"); 
            context.Response.ContentType = "application/xml";
            context.Response.Write(xmlContent);  

            tmWebServices.logUserActivity("Create Article (Notepad)", "{0} ({1})".info(article.Metadata.Title, data));

            endResponse();            
        }        
        public void handleAction_Raw(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {                
                var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);
                context.Response.ContentType = "application/xml";
                context.Response.Write(xmlContent);
                
                tmWebServices.logUserActivity("View Article (raw)", data);

                endResponse(); 
            }
            else
                transfer_Request("articleViewer");              // will trigger exception
        }
        public void handleAction_Xml(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {                
                var xmlContent = tmWebServices.GetGuidanceItemHtml(guid);
                context.Response.ContentType = "application/xml";
                context.Response.Write(xmlContent);
                
                tmWebServices.logUserActivity("View Article (XML)", data);

                endResponse(); 
            }
            else
               transfer_Request("articleViewer");              // will trigger exception            
        }
        public void handleAction_Html(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {
                context.Response.ContentType = "text/html";
                var article = tmWebServices.GetGuidanceItemById(guid);
                if (article.notNull())
                { 
                    var articleHtml = tmWebServices.GetGuidanceItemHtml(guid);
                
                    var htmlTemplateFile = @"\Html_Pages\Gui\Pages\article_Html.html";
                    var htmlTemplate = context.Server.MapPath(htmlTemplateFile).fileContents();
                    
                    var htmlContent = htmlTemplate.replace("#ARTICLE_TITLE", article.Metadata.Title)
                                                  .replace("#ARTICLE_HTML", articleHtml);
                    context.Response.Write(htmlContent);      
                
                    tmWebServices.logUserActivity("View Article (HTML)", "{0} ({1})".info(article.Metadata.Title, data));

                    endResponse(); 
                }
            }
            else
                transfer_Request("articleViewer");              // will trigger exception     
        }
        public void handle_ArticleViewRequest(string data)
        {			
            if ( data.isGuid())
            {				
                var guid = data.guid();
                var guidanceItem = tmWebServices.GetGuidanceItemById(guid);
                if (guidanceItem.isNull())
                {
                    var redirectTarget = tmWebServices.VirtualArticle_Get_GuidRedirect(guid);
                    if (redirectTarget.valid())
                    {
                        context.Response.Redirect(redirectTarget); // ends request                        
                    }
                }
                else
                    tmWebServices.logUserActivity("View Article (direct)", "{0} ({1})".info(guidanceItem.Metadata.Title, data));
            }
            else
                tmWebServices.logUserActivity("View Article (direct)", data);
            
            transfer_Request("articleViewer");                      // will trigger exception    
        }	
	    public void handle_ArticleEditRequest(string data)
	    {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid == Guid.Empty)
                transfer_Request("articleViewer");                  // will trigger exception
            else
            {
                tmWebServices.RBAC_Demand_EditArticles();           // will trigger an Security exception if the user if not authorized
                
                var article = tmWebServices.GetGuidanceItemById(guid); 
                        
                if (article.Content.DataType.lower() == "markdown")
                {
                   context.Response.Redirect("/Markdown/Editor?articleId={0}".format(guid));
                }
                else
                { 
	                transfer_ArticleEditor(article);
                }
            }
	    }

        public void handleAction_Content(string data)
        { 
            var guid = tmWebServices.getGuidForMapping(data);
            tmWebServices.logUserActivity("View Article (content)", "{0} ".info(data));
            if (guid != Guid.Empty)
            {                
                context.Response.ContentType = "text/html";
                var htmlContent = tmWebServices.GetGuidanceItemHtml(guid);
                if(htmlContent.notValid())
                    "[handleAction_Content] there was no GuidanceItemHtml for id: {0}".error(guid);
                context.Response.Write(htmlContent);
                endResponse();
            }
            else
                transfer_Request("articleViewer");              // will trigger exception           

        }

        //User related
        public void showWhoAmI(string data = "")
        {
            if (tmWebServices.notNull() && tmWebServices.tmAuthentication.notNull())
            {
                var currentUser = tmWebServices.tmAuthentication.currentUser;
                if(data=="json")
                    context.Response.ContentType = "application/json";                
                context.Response.Write(currentUser.whoAmI().json());
                endResponse();             
            }
        }
        //utils
        public void endResponse()
        { 
            context.Response.Flush();
            context.Response.End();
        }           
        public void transfer_ArticleEditor(TeamMentor_Article article)
        {                     
            tmWebServices.logUserActivity("Edit Article (WYSIWYG)", "{0} ({1})".format(article.Metadata.Title, article.Metadata.Id));                                
            transfer_Request("articleEditor");    
        }
        
        public void redirect_Login_AccessDenied(string urlRequested)
        {
            if(tmWebServices.tmAuthentication.currentUser.notNull())
                tmWebServices.tmAuthentication.currentUser.logUserActivity("Access Denied", urlRequested);
            else
                tmWebServices.logUserActivity("Access Denied", urlRequested);
            redirect_Login();
        }
        public void redirect_Login()
        {
            var loginPage = "/Html_Pages/Gui/Pages/login.html";
            if (context.Request.Url.notNull())
            { 
                var loginReferer = context.Request.QueryString["LoginReferer"];
                var redirectTarget =  (loginReferer.notNull() && loginReferer.StartsWith("/"))
                                            ? loginReferer
                                            : context.Request.Url.AbsolutePath;
                if (redirectTarget.lower() == "/login")
                    redirectTarget = "/";

                //tmWebServices.logUserActivity("Login Page", "Redirect: {0}".format(redirectTarget));

                context.Response.Redirect("{0}?LoginReferer={1}".format(loginPage, redirectTarget));                
            }
            context.Response.Redirect(loginPage);
        }
        public void handle_LoginOK()
        {            
            if (context.Request.notNull() && context.Request.QueryString.notNull())
            {
                var loginReferer = context.Request.QueryString["LoginReferer"]   // get user provided redirect                                                      
                                                  .replace("//","/");            // prevent urls that start with  //
                if (loginReferer.htmlEncode() != loginReferer )                  // prevent html tags
                    return;
                var referTarget = (loginReferer.notNull() && loginReferer.StartsWith("/"))
                                    ? loginReferer                               // only allow paths that start with /
                                    : "/";                                       // default to redirect to /                
                context.Response.Redirect(referTarget);        
                // Response.Redirect will throw an exception so the current request ends here                
            }            
        }
   
        public void redirectTo_Logout()
        {	
            tmWebServices.Logout();
            response_Redirect("home");                        	            
        }
        public void redirectTo_DownloadLibrary(string data)
        {
            // UserGroup.Admin.setThreadPrincipalWithRoles();      // to test for this (for now allow normal users to download libraries)
            //var currentUserRoles = tmWebServices.RBAC_CurrentPrincipal_Roles();

            var uploadToken = new TM_WebServices().GetUploadToken();            
            context.Response.Redirect("/Aspx_Pages/Library_Download.ashx?library={0}&uploadToken={1}".format(data, uploadToken));            
        }                                 
        public void redirectTo_Article(string article)
        {			
            context.Response.Redirect("/article/{0}".format(article));                
        }          
        public void redirectTo_SetLibrary(string libraryIdOrName)
        {			
            context.Response.Redirect("/aspx_pages/SetLibrary.aspx?Library={0}".format(libraryIdOrName));             
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