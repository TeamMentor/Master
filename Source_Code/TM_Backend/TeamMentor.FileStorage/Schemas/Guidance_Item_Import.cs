using System;
using System.Xml.Serialization;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
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
            if (phase == null)
            {
                phase	    = Rule_Type;
                Rule_Type   = Type;
            }
            var teamMentor_Article = new TeamMentor_Article
            {
                Metadata = new TeamMentor_Article_Metadata
                {
                    Id          = id.guid(),
                    Id_History  = id_Original,
                    Library_Id  = libraryId.guid(),
                    Title       = title,
                    Category    = Category,
                    Phase       = phase,
                    Technology  = Technology,
                    Type        = Rule_Type,
                    Author      = Author,
                    Priority    = Priority,
                    Status      = Status,
                    Source      = Source,

                    DirectLink = "",
                    Tag = "",
                    Security_Demand = "",
                },
                Content = new TeamMentor_Article_Content
                {
                    Sanitized = true,
                    DataType = "Html",
                    Data = {Value = content}
                }
            };

            teamMentor_Article.setHashes();
            //teamMentor_Article.htmlEncode();            //encode contents
            return teamMentor_Article;
        }
    }
}