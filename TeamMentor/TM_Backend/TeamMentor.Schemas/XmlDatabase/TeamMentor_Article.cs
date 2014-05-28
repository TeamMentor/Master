using System;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
	public class TeamMentor_Article
	{ 
        [XmlAttribute] 
        public int  Metadata_Hash                   { get;set; }                 // Metadata Hash   
        [XmlAttribute] 
        public int  Content_Hash                    { get;set; }                 // Content Hash   

		public TeamMentor_Article_Metadata Metadata { get; set; }
        public TeamMentor_Article_Content  Content  { get; set; }

        public TeamMentor_Article()
        { 
            Metadata = new TeamMentor_Article_Metadata();
            Content = new TeamMentor_Article_Content();

            Metadata.Id = Guid.NewGuid();
        }
    }

	public class TeamMentor_Article_Metadata
	{        
		//Article ID(s)
		public Guid     Id                  { get; set; }		// Article GUID
		public string   Id_History          { get; set; }		// Previous GUIDs (populated when copying Articles) - [Comma Delimited]
        public Guid     Library_Id          { get; set; }		// Library GUID
		//Article Title
		public string   Title               { get; set; }		// Article Title

		//4 main MetaData tags
		public string   Category            { get; set; }		// [Comma Delimited]
		public string   Phase               { get; set; }		// [Comma Delimited]
		public string   Technology	        { get; set; }		// [Comma Delimited]
		public string   Type                { get; set; }		// [Comma Delimited]

				
		//MetaData - Used by TM GUI
		public string   DirectLink          { get; set; }		// To enabled direct URL links to this Article
		public string   Tag			        { get; set; }		// To allow articles to have extra Metadata Items [Comma Delimited]
		public string   Security_Demand     { get; set; }		// To allow articles to make special Security Demands [Comma Delimited]

		//MetaData - NOT Used by TM GUI		
		public string   Author              { get; set; }		// Not Used in GUI
		public string   Priority            { get; set; }		// Not Used in GUI
		public string   Status              { get; set; }		// Not Used in GUI
		public string   Source              { get; set; }		// Not Used in GUI
		public string   License             { get; set; }		// Not Used in GUI
    }

	public class TeamMentor_Article_Content
	{ 
           
                                         		            
		[XmlAttribute] public bool Sanitized            { get; set; }		            // Flag to indicate if the data (for example Html) was sanitized before saving
        [XmlAttribute] public string DataType           { get; set; }		            // Flag to indicate if the data (for example Html) was sanitized before saving

        [XmlElement]   public string Description	    { get; set; }		// Can be used to store a description about the Article Data        
                
        [XmlElement]
        public List<string> Files { get; set; }

        [XmlElement][ScriptIgnore]
        public XmlCDataSection Data { get; set;}

        [XmlIgnore]                                     // used to send receive data from JSON (since the serializer doen't support XmlCDataSection)
        public String Data_Json 
            {
                set {                        
                        Data.Value = value.replace("&amp;", "&");                       //.fixXmlDoubleEncodingIssue();
                    }
                get {
                        return Data.Value.replace("&amp;", "&").replace("&amp;", "&");  //.fixXmlDoubleEncodingIssue();                        
                    }
            }

        /*{
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(Data_Raw);
            }
            set { Data_Raw = (value != null) ? value.Data : null; }
        } */

        public TeamMentor_Article_Content()
        { 
            Data = new XmlDocument().CreateCDataSection("");
            DataType = "Html";
        }

	}


	//this is the class that can used to import/transform the previous version of guidanceItem files into the new 
}

