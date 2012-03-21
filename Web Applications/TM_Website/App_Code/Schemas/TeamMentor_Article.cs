using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.Kernel.ExtensionMethods;

namespace SecurityInnovation.TeamMentor.WebClient
{		
	public class TeamMentor_Article
	{
		//Article ID(s)
		[XmlAttribute] public string Id				 { get; set; }		// Article GUID
		[XmlAttribute] public string Id_History		 { get; set; }		// Previous GUIDs (populated when copying Articles) - [Comma Delimited]

		//Article Title
		[XmlAttribute] public string Title			 { get; set; }		// Article Title

		//4 main MetaData tags
		[XmlAttribute] public string Category		 { get; set; }		// [Comma Delimited]
		[XmlAttribute] public string Phase			 { get; set; }		// [Comma Delimited]
		[XmlAttribute] public string Technology		 { get; set; }		// [Comma Delimited]
		[XmlAttribute] public string Type			 { get; set; }		// [Comma Delimited]

		
		//Main HTML Content
		[XmlElement]   public string Html_Content	 { get; set; }		// HTML code (using AntiXSSLibrary)
		
		//MetaData - Used by TM GUI
		[XmlAttribute] public string DirectLink		 { get; set; }		// To enabled direct URL links to this Article
		[XmlAttribute] public string Tag			 { get; set; }		// To allow articles to have extra Metadata Items [Comma Delimited]
		[XmlAttribute] public string Security_Demand { get; set; }		// To allow articles to make special Security Demands [Comma Delimited]

		//MetaData - NOT Used by TM GUI		
		[XmlAttribute] public string Author			 { get; set; }		// Not Used in GUI
		[XmlAttribute] public string Priority		 { get; set; }		// Not Used in GUI
		[XmlAttribute] public string Status			 { get; set; }		// Not Used in GUI
		[XmlAttribute] public string Source			 { get; set; }		// Not Used in GUI
		
	}


	//this is the class that can used to import/transform the previous version of guidanceItem files into the new 

	[Serializable, XmlRoot(ElementName = "guidanceItem", Namespace="urn:microsoft:guidanceexplorer:guidanceItem")]	
	public class Guidance_Item_Import : TeamMentor_Article
	{
		[XmlElement]   public string content		{ get; set; }
		[XmlAttribute] public string id				{ get; set; }	
		[XmlAttribute] public string id_Original	{ get; set; }	
		[XmlAttribute] public string phase			{ get; set; }	
		[XmlAttribute] public string Rule_Type		{ get; set; }	
		[XmlAttribute] public string type			{ get; set; }	
		[XmlAttribute] public string Type1			{ get; set; }	
		[XmlAttribute] public string title			{ get; set; }	

		public TeamMentor_Article transform()
		{
			// fix the issue with older SI Library Articles
			if (this.phase == null)
			{
				this.phase		= this.Rule_Type;
				this.Rule_Type  = this.Type;
			}

			return new TeamMentor_Article()
				{
					Id			 = this.id,
					Id_History   = this.id_Original,
					Title		 = this.title,
					Category	 = this.Category,
					Phase		 = this.phase,
					Technology   = this.Technology,
					Type		 = this.Rule_Type,
					Html_Content = this.content,
					Author		 = this.Author,					
					Priority	 = this.Priority,
					Status		 = this.Status,
					Source		 = this.Source					
				};
		}
	}

}