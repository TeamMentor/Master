using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using Microsoft.Security.Application;
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
            if (splitedPath.size() > 1)
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
                data = Encoder.HtmlEncode(data);
                switch (action.lower())
                {
                    case "xml":
                    case "raw":                        
                        return handleAction_Raw(data);                                                      
                    case "xsl":
                        return handleAction_Xsl(data);
                    case "article":
                        return redirectToArticleViewer();                                                
                    case "images":                        
                    case "image":
                        return handleAction_Image(data);
                    case "jsonp":
                        return handleAction_JsonP(data);
                    default:                        
                        return false;                                          
                }                                           
            }
            catch //(Exception ex)
            {
                //context.Response.Write("<h2>Error: {0} </h2>".format(ex.Message));
            }            
            return false;			
		}

        private bool handleAction_JsonP(string data)
        {            
            var guid = tmWebServices.getGuidForMapping(data);
            var article = tmWebServices.GetGuidanceItemById(guid.str());               
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var serializedData = serializer.Serialize(article);                       
            var callbackRaw = context.Request["callback"];
            if (callbackRaw.valid())
            {
                var callbackFunction = Encoder.JavaScriptEncode(callbackRaw,false);
                context.Response.Write("{0}({1})".format(callbackFunction, serializedData));
            }
            else            
                context.Response.Write(serializedData);
            return true;
        }


        //handlers
        private bool handleAction_Image(string data)
        {            
            var imagePath = TM_Xml_Database.Path_XmlLibraries.pathCombine("Images").pathCombine(data);
            if (imagePath.fileExists())
            {
                context.Response.ContentType = "image/{0}".format(data.extension().removeFirstChar());
                context.Response.WriteFile(imagePath);
                return true;
            }            
            return false;
        }

        private bool handleAction_Xsl(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {
                context.Response.ContentType = "application/xml";
                var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);
                var xmlSignature = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                var xsltText = "<?xml-stylesheet type=\"text/xsl\" href=\"/xslt/TeamMentor_Article.xslt\"?>";
                xmlContent = xmlContent.replace(xmlSignature, xmlSignature.line().append(xsltText));
                context.Response.Write(xmlContent);                
            }
            return true;
        }

        private bool handleAction_Raw(string data)
        {
            var guid = tmWebServices.getGuidForMapping(data);
            if (guid != Guid.Empty)
            {
                context.Response.ContentType = "application/xml";
                var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);
                context.Response.Write(xmlContent);
            }
            return true;
        }


        //utils
        public void endResponse()
        { 
            context.Response.Flush();
            context.Response.End();
        }

		public bool redirectToArticleViewer()
		{
			var articleViewer = "/html_pages/GuidanceItemViewer/GuidanceItemViewer.html?";			
			context.Server.Transfer(articleViewer);
            return false;    
		}

	}
}