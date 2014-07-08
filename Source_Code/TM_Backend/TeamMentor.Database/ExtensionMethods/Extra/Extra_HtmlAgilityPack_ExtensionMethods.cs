// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{	
	public static class Extra_HtmlAgilityPack_ExtensionMethods
	{	
        public static HtmlAgilityPack.HtmlDocument htmlDocument(this string htmlCode)
        {
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(htmlCode);
            return htmlDocument;
        }

		public static string tidyHtml(this string htmlCode)
		{			
			var htmlDocument = htmlCode.htmlDocument();
			var tidiedhtml = htmlDocument.tidyHtml();
			if (tidiedhtml.valid())
				return tidiedhtml;
			return htmlCode;
		}
		
		public static string tidyHtml(this HtmlAgilityPack.HtmlDocument htmlDocument)
		{
			try
			{	
				//htmlDocument.OptionCheckSyntax = true;
				htmlDocument.OptionFixNestedTags = true;
				htmlDocument.OptionAutoCloseOnEnd = true;
				htmlDocument.OptionOutputAsXml = true;				
				//htmlDocument.OptionDefaultStreamEncoding = Encoding.Default;
                var documentNode = htmlDocument.DocumentNode;
                                
                if (documentNode.InnerHtml == documentNode.InnerText)        //nothing to do since there are no Html tags
                    return documentNode.InnerHtml.fix_CRLF(); 
                                
				var formatedCode = documentNode.OuterHtml
                                               .xmlFormat()
											   .xRoot()
                                               .innerXml()
                                               .trim();
				return formatedCode.fix_CRLF();			
			}
        	catch(Exception ex)
        	{
        		ex.log("[string.tidyHtml]");
        		return null;
        	}
		}
	}
	
    // the methods below are used in tidyHtml and are included in FluentSharp.WinForms 
    // (and It didn't make sense to add a reference to that assembly just for these methods)  
    // code created by ILSPY    
    public static class Extra_Xml_Linq_ExtensionMethods
    {
        public static XElement xRoot(this string xml)
        {
	        if (xml.valid())
	        {
		        XDocument xDocument = xml.xDocument();
		        if (xDocument != null)
		        {
			        return xDocument.Root;
		        }
	        }
	        return null;
        }
        public static XDocument xDocument(this string xml)
        {
	        string text = xml.fileExists() ? xml.fileContents() : xml;
	        if (text.valid())
	        {
		        if (text.starts("\n"))
		        {
			        text = text.trim();
		        }
		        XmlReaderSettings settings = new XmlReaderSettings
		        {
			        XmlResolver = null,
			        ProhibitDtd = false
		        };
		        using (StringReader stringReader = new StringReader(text))
		        {
			        using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
			        {
				        return XDocument.Load(xmlReader);
			        }
		        }
	        }
	        return null;
        }
        public static string innerXml(this XElement xElement)
        {
	        if (xElement == null)
	        {
		        return "";
	        }
	        XmlReader xmlReader = xElement.CreateReader();
	        xmlReader.MoveToContent();
	        return xmlReader.ReadInnerXml();
        }

        //from FluentSharp.WinForms code base
         public static XElement element(this XElement xElement, string elementName)
        {
            if (xElement != null)
                return xElement.elements().FirstOrDefault(childElement => childElement.name() == elementName);
            return null;
        }
        public static List<XElement> elements(this XElement xElement)
        {
            return xElement.elements(false);
        }
        public static List<XElement> elements(this XElement xElement, bool includeSelf)
        {
            return xElement.elements("", includeSelf);
        }
        public static List<XElement> elements(this XElement xElement, string elementName, bool includeSelf)
        {
            var xElements = (elementName.valid())
                                ? xElement.Elements(elementName).ToList()
                                : xElement.Elements().ToList();
            if (includeSelf)
                xElements.Add(xElement);
            return xElements;
        }
        public static string name(this XElement xElement)
        {
            return xElement.Name.str();
        }
    }
}    	