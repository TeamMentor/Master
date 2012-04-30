using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;

using urn.microsoft.guidanceexplorer.guidanceItem;
using System.Web.Script.Serialization;
using Microsoft.Security.Application;
//O2File:C:\_WorkDir\O2\O2 Install\_TeamMentor\TeamMentor-Dinis-Dev-Fork\Web Applications\TeamMentor.CoreLib\TM_AppCode\Schemas\GuidanceItem.cs

namespace SecurityInnovation.TeamMentor.WebClient
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
                        Data.Value = value.fixXmlDoubleEncodingIssue();
                    }
                get {
                        return Data.Value.fixXmlDoubleEncodingIssue();                        
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
            this.Data = new XmlDocument().CreateCDataSection("");
            this.DataType = "Html";
        }

	}


	//this is the class that can used to import/transform the previous version of guidanceItem files into the new 

	[Serializable, XmlRoot(ElementName = "guidanceItem", Namespace="urn:microsoft:guidanceexplorer:guidanceItem")]	
	public class Guidance_Item_Import
	{
        [XmlAttribute] public string Author			{ get; set; }	
        [XmlAttribute] public string Category		{ get; set; }		
		[XmlAttribute] public string id				{ get; set; }	
		[XmlAttribute] public string id_Original	{ get; set; }	
        [XmlAttribute] public string libraryId	    { get; set; }	
		[XmlAttribute] public string phase			{ get; set; }
	    [XmlAttribute] public string Priority		{ get; set; }	
		[XmlAttribute] public string Rule_Type		{ get; set; }
        [XmlAttribute] public string Status			{ get; set; }
        [XmlAttribute] public string Source			{ get; set; }
        [XmlAttribute] public string Technology		{ get; set; }
	    [XmlAttribute] public string Type			{ get; set; }	
		[XmlAttribute] public string type			{ get; set; }	
		[XmlAttribute] public string Type1			{ get; set; }	
		[XmlAttribute] public string title			{ get; set; }	

        [XmlElement]   public string content		{ get; set; }

		public TeamMentor_Article transform()
		{
			// fix the issue with older SI Library Articles
			if (this.phase == null)
			{
				this.phase		= this.Rule_Type;
				this.Rule_Type  = this.Type;
			}
            var teamMentor_Article = new TeamMentor_Article();
            teamMentor_Article.Metadata = new TeamMentor_Article_Metadata()
				                                {
					                                Id			 = this.id.guid(),
					                                Id_History   = this.id_Original,
                                                    Library_Id   = this.libraryId.guid(),
					                                Title		 = this.title,
					                                Category	 = this.Category,
					                                Phase		 = this.phase,
					                                Technology   = this.Technology,
					                                Type		 = this.Rule_Type,					                                
					                                Author		 = this.Author,					
					                                Priority	 = this.Priority,
					                                Status		 = this.Status,
					                                Source		 = this.Source,

				                                    DirectLink      = "",
                                                    Tag             = "",
                                                    Security_Demand = "",
				                                };

            teamMentor_Article.Content = new TeamMentor_Article_Content()
                                                {
                                                    Sanitized    = true,
                                                    DataType     = "Html"                                                                                                                
                                                };
            teamMentor_Article.Content.Data.Value = this.content;
            teamMentor_Article.setHashes();
            return teamMentor_Article;
		}
	}

    public static class TeamMentor_Article_ExtensionMethods
    {
        public static TeamMentor_Article setHashes(this TeamMentor_Article article)
        { 
            article.Metadata_Hash = article.Metadata.serialize(false).hash();
            article.Content_Hash  = article.Content.serialize(false).hash();
            return article;
        }


        public static guidanceItem transform_into_guidanceItem(this TeamMentor_Article article)
        {
            if (article.isNull())
                return null;
            return new guidanceItem()
                    {
                        id          =   article.Metadata.Id.str(),
                        id_original =   article.Metadata.Id_History,
                        title       =   article.Metadata.Title,

                        Technology  =   article.Metadata.Technology,
                        phase       =   article.Metadata.Phase,
                        Category    =   article.Metadata.Category,
                        Rule_Type   =   article.Metadata.Type,

                        content     =   article.Content.Data.Value
                    };
        }

        public static TeamMentor_Article teamMentor_Article(this string pathToXmlFile)
        { 
            var article = pathToXmlFile.load<TeamMentor_Article>()
                                       .htmlEncode();
            return article;
        }

        public static TeamMentor_Article htmlEncode(this TeamMentor_Article article)
        {          
            var metaData = article.Metadata;
            foreach(var prop in metaData.type().properties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string)metaData.prop(prop.Name);
                    metaData.prop(prop.Name, Encoder.HtmlEncode(value)); 
                }
            }
            if (TMConfig.Current.SanitizeHtmlContent)
            {
                article.Content.Data.Value = Sanitizer.GetSafeHtmlFragment(article.Content.Data.Value);
                article.Content.Sanitized = true;
            }

            return article;
        }

        //fix double encoding caused by JSON?CDATA/XML transfer of XML data
        public static string fixXmlDoubleEncodingIssue(this string htmlContent)
        { 
            return htmlContent.replace("&amp;", "&"); 
        }
    }

}

