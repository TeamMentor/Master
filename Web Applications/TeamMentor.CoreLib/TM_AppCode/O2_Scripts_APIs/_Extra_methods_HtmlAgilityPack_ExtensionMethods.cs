// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using FluentSharp.BCL;
using FluentSharp.CoreLib;

namespace FluentSharp
{	
	public static class _Extra_methods_HtmlAgilityPack_ExtensionMethods
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
}    	