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
            tmWebServices  = new TM_WebServices();
            tmWebServices.tmAuthentication = new TM_Authentication(tmWebServices);
            tmWebServices.tmAuthentication.disable_CSRF_Check = true;
			tmWebServices.tmAuthentication.mapUserRoles();
        }

        public void routeRequestUrl_for404()
        {
            var fixedPath = context.Request.Url.AbsolutePath.replace("/html_pages/Gui/","/article/");
            handleUrlRewrite(fixedPath);
        }
        public void routeRequestUrl()
        {
            handleUrlRewrite(context.Request.Url.AbsolutePath);
        }
            
		public void handleUrlRewrite(string path)
		{
            if (path.contains(".htm", ".asmx", ".ashx", ".aspx")) // don't process if these values are in path
            { 
                return;
            }
            if(path.starts("/"))
                path = path.removeFirstChar();
            var splitedPath = path.split("/");
            if (splitedPath.size() > 0)
            { 
                var action = splitedPath.shift();   // extract first element               
                var data = splitedPath.join("/");   // rejoin the rest
                if (action.valid() && handleRequest(action, data))    //if we did process it , end the request here
                    endResponse();                    
            }                        
		}

        
		public bool handleRequest(string action , string data)
		{
            try
            {
                action = Encoder.HtmlEncode(action);
                data = Encoder.HtmlEncode(data).replace("%20"," ");
                if (action.isGuid() & data.inValid())                
                    return redirectTo_Article(action);                                    
                switch (action.lower())
                {
                    case "raw":                        
                        return handleAction_Raw(data);                                                      
                    case "html":
                        return handleAction_Html(data);
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
                        return transfer_ArticleViewer();
                    case "edit":
                    case "editor":
                        return transfer_ArticleEditor(data);
                    case "create":
                        return handleAction_Create(data);    
                    case "admin":
                        return redirectTo_ControlPanel(false);
                    case "admin_extra":
                        return redirectTo_ControlPanel(true);                        
                    case "login":
                        return transfer_Login();   
                    case "logout":
                        return redirectTo_Logout();
                    case "reload":
                        return reloadCache_and_RedirectToHomePage();
                    case "home":
                        return redirectTo_HomePage();
                    //case "images":                        
                    case "image":
                        return handleAction_Image(data);
                    case "jsonp":
                        return handleAction_JsonP(data);
                    default:                        
                        return false;                                          
                }                                           
            }                
            catch (Exception exception)
            {                 
                if (exception is SecurityException)
                    return redirectTo_Login();
                //context.Response.Write("<h2>Error: {0} </h2>".format(ex.Message));
            }                                    
            return false;			
		}

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


        //handlers
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
                        var xslTransform = new System.Xml.Xsl.XslTransform();
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
                var content = tmWebServices.GetGuidanceItemHtml(guid);
                context.Response.Write(content);                
            }
            else
                transfer_ArticleViewer();
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
	        context.Response.ContentType = "text/html";    
			context.Server.Transfer("/Html_Pages/Gui/Pages/login.html");            
            return false; 
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