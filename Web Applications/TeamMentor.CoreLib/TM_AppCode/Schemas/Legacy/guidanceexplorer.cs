using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using O2.DotNetWrappers.ExtensionMethods;

namespace urn.microsoft.guidanceexplorer 
{
    [Serializable]
    //[XmlRoot(Namespace="urn:microsoft:guidanceexplorer")]
    public class guidanceExplorer
    {
        [XmlAttribute] public string           name                { get; set; }
        [XmlElement]   public Library          library             { get; set; }

        public guidanceExplorer()
        {
            library = new Library();
        }

        public static guidanceExplorer Load(string xmlFile)
        {
            return xmlFile.load<guidanceExplorer>();            
        }

        public void SaveLibraryTo(string xmlFile)
        {
            this.saveAs(xmlFile);
        }
    }
    public class Library
    {
        [XmlAttribute] public string           name                { get; set; }
        [XmlAttribute] public string           caption             { get; set; }        
        [XmlElement  ] public Items            items               { get; set; }        
        [XmlElement  ] public LibraryStructure libraryStructure    { get; set; }

        public Library()
        {
            libraryStructure = new LibraryStructure();
        }
    }

    public class LibraryStructure
    {
        [XmlElement  ] public List<View>       view                 { get; set; }
        [XmlElement  ] public List<Folder>     folder               { get; set; }

        public LibraryStructure()
        {
            view    = new List<View>();
            folder  = new List<Folder>();
        }
    }

    public class Folder
    {
        [XmlAttribute] public string           folderId            { get; set; }
        [XmlAttribute] public string           caption             { get; set; }
        [XmlElement  ] public List<View>       view                { get; set; }         
        [XmlElement  ] public List<Folder>     folder1             { get; set; }        

        public Folder()
        {
            view     = new List<View>();
            folder1  = new List<Folder>();            
        }
    }
    public class View
    {   
        [XmlAttribute] public string           id                  { get; set; }
        [XmlAttribute] public string           caption             { get; set; }
        [XmlAttribute] public string           author              { get; set; }
        [XmlAttribute] public DateTime         creationDate        { get; set; }
        [XmlElement  ] public Items            items               { get; set; }                

        public View()
        {
            items    = new Items();                        
        }
    }
    public class Items 
    {
        [XmlElement(ElementName = "item")] public List<string>     item  { get; set; }

        public Items()
        {
            item = new List<string>();
        }
    }
}
