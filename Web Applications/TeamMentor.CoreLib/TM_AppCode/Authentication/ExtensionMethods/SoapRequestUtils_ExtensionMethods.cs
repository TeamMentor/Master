using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace SecurityInnovation.TeamMentor.Authentication.ExtensionMethods
{
    public static class SoapRequestUtils
    {
/*        public static bool RequestHasSoapAction(this HttpContextBase httpContext)
        {
            return httpContext.Request.ServerVariables["HTTP_SOAPACTION"] != null;
        }
*/
 /*       public static XmlDocument GetPostDataAsXmlDocument(this HttpContextBase httpContext)
        {
            try
            {
                var HttpStream = httpContext.Request.InputStream;
                long posStream = HttpStream.Position;

                XmlDocument xmlDocument = new XmlDocument();                
                xmlDocument.Load(HttpStream);
                HttpStream.Position = posStream;
                return xmlDocument;
            }
            catch
            {
                return null;
            }            
        }

        public static string GetPostDataElementValue(this HttpContextBase httpContext, string elementName)
        {
            try
            {
                var xmlDocument = httpContext.GetPostDataAsXmlDocument();
                if (xmlDocument != null && elementName != null)
                {
                    var element = xmlDocument.GetElementsByTagName(elementName);                    
                    if (element != null && element.Count > 0)
                        return element[0].InnerXml;
                }
            }
            catch
            {                
            }
            return null;
        }
*/
 
    }
}
