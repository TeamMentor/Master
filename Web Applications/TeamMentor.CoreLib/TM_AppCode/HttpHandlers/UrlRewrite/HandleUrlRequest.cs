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

		public void handleCassini404(string path)
		{
            if (path.starts("/article/"))
            {
                handleRequest("article", path.remove("/article/"));                
            }
            else if (path.starts("/raw/"))
            {
                handleRequest("raw", path.remove("/raw/"));                
            }
            else if (path.starts("/xsl/"))
            {
                handleRequest("xsl", path.remove("/xsl/"));                
            }
            else if (path.starts("/image/"))
            {
                handleRequest("image", path.remove("/image/"));                
            }            
		}

        
		public void handleRequest(string action , string path)
		{
            try
            {

                action = Encoder.HtmlEncode(action);
                path = Encoder.HtmlEncode(path);
                switch (action.lower())
                {
                    case "raw":
                        {
                            var guid = tmWebServices.getGuidForMapping(path);
                            if (guid != Guid.Empty)
                            {
                                context.Response.ContentType = "application/xml";
                                var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);                                
                                context.Response.Write(xmlContent);                                
                            }                            
                            break;
                        }
                    case "xsl":
                        {
                            var guid = tmWebServices.getGuidForMapping(path);
                            if (guid != Guid.Empty)
                            {
                                context.Response.ContentType = "application/xml";
                                var xmlContent = tmWebServices.XmlDatabase_GetGuidanceItemXml(guid);
                                var xmlSignature = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                                var xsltText = "<?xml-stylesheet type=\"text/xsl\" href=\"/xslt/TeamMentor_Article.xslt\"?>";
                                xmlContent = xmlContent.replace(xmlSignature, xmlSignature.line().append(xsltText));                
                                context.Response.Write(xmlContent);                                
                            }                            
                            break;
                        }
                    case "article":
                        {
                            redirectToArticleViewer();                            
                            break;
                        }
                    case "image":
                        {
                            context.Response.ContentType = "image/jpeg";

                            var imagePath = TM_Xml_Database.Path_XmlLibraries.pathCombine("Images").pathCombine(path);
                            //var bytes = imagePath.fileContents_AsByteArray();
                            context.Response.WriteFile(imagePath);
                            //context.Response.Write("<h1> showing image: {0}</h1>".info(imagePath));                             
                            break;
                        }
                    default:
                        {
                            context.Response.Write("<h2>Could not find requested item</h2>");
                            break;
                        }
                     //   context.Response.Write("<h1> default action</h1>");                                                
                      //  break;
                }
                
                
            }
            catch (Exception ex)
            {
                context.Response.Write("<h2>Error: {0} </h2>".format(ex.Message));
            }            
//			context.Response.Write("<h2>in handleRequest for : {0}</h2>".format(path));
			
		}

        /*public void flushRequest()
        /{ 
            context.Response.Flush();
            //context.Response.End();
        }*/

		public void redirectToArticleViewer()
		{
			var articleViewer = "/html_pages/GuidanceItemViewer/GuidanceItemViewer.html?";			
			context.Server.Transfer(articleViewer);			
		}

	}
}