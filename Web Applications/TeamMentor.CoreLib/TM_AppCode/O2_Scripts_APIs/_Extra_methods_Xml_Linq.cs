// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using System.Xml.Linq;
using System.IO;
 
namespace O2.DotNetWrappers.ExtensionMethods
{
    //current in Linq_ExtensionMethods from O2_External_Sharpdevelop.dll

    public static class XML_Linq_ExtensionMethods
    {
        public static XDocument xDocument(this string xml)
        {
            var xmlToLoad = xml.fileExists() ? xml.fileContents() : xml;
            if (xmlToLoad.valid())
            {
                if (xmlToLoad.starts("\n"))       // checks for the cases where there the text starts with \n (which will prevent the document to be loaded
                    xmlToLoad = xmlToLoad.trim();
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.XmlResolver = null;
                xmlReaderSettings.ProhibitDtd = false;
                using (StringReader stringReader = new StringReader(xmlToLoad))
                using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
                    return XDocument.Load(xmlReader);
            }
            return null;

        }
        public static XElement xRoot(this string xml)
        {
            if (xml.valid())    // checks if the string is not empty
            {
                var xDocument = xml.xDocument();
                if (xDocument != null)
                    return xDocument.Root;
            }
            return null;
        }

        public static string innerXml(this XElement xElement)
        {
            if (xElement == null)
                return "";
            var reader = xElement.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }
    }
}