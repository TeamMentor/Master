using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.FluentSharp
{
	public static class Extra_ExtensionMethods
	{
		public static bool canNotWriteToPath(this string path)
		{
			return path.canWriteToPath().isFalse();
		}

		public static bool canWriteToPath(this string path)
		{
			try
			{
				//var files = path.files();
				var tempFile = path.pathCombine("tempFile".add_RandomLetters());
				"test content".saveAs(tempFile);
				if (tempFile.fileExists())
				{
					File.Delete(tempFile);
					if (tempFile.fileExists().isFalse())
						return true;
				}
				"[in canWriteToPath] test failed for for path: {0}".error(path);
			}
			catch (Exception ex)
			{
				ex.log("[in canWriteToPath] for path: {0} : {1}", path, ex.Message);
			}
			return false;
		}
	}

	public static class Extra_ExtensionMethods_Serialization
	{
		public static string toXml(this object _object)
		{
			return _object.serialize(false);
		}
	}

	public static class Extra_ExtensionMethods_Stream
	{
		public static MemoryStream stream_UFT8(this string text)
		{
			var memoryStream = text.valid()
				       ? new MemoryStream(Encoding.UTF8.GetBytes(text))
				       : new MemoryStream();
			memoryStream.Flush();
			return memoryStream;
		}
	}
	public static class Extra_ExtensionMethods_XSL
	{
		public static string xsl_Transform(this string xmlContent, string xstlFile)
		{
			try
			{
				var xslTransform = new XslCompiledTransform();

				xslTransform.Load(xstlFile);

				var xmlReader = new XmlTextReader(new StringReader(xmlContent));
				var xpathNavigator = new XPathDocument(xmlReader);
				var stringWriter = new StringWriter();

				xslTransform.Transform(xpathNavigator, new XsltArgumentList(), stringWriter);
				return stringWriter.str();
			}
			catch (Exception ex)
			{
				ex.log("[in xsl_Transform]");
				return "";
			}
		}
	}
	
}
