// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.DotNetWrappers.ExtensionMethods
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
				htmlDocument.OptionCheckSyntax = true;
				htmlDocument.OptionFixNestedTags = true;
				htmlDocument.OptionAutoCloseOnEnd = true;
				htmlDocument.OptionOutputAsXml = true;
				//htmlDocument.OptionDefaultStreamEncoding = Encoding.Default;
				var formatedCode = htmlDocument.DocumentNode.OuterHtml.xmlFormat().xRoot().innerXml().trim();
				return formatedCode;			
			}
        	catch(Exception ex)
        	{
        		ex.log("[string.tidyHtml]");
        		return null;
        	}
		}
	}	
}    	