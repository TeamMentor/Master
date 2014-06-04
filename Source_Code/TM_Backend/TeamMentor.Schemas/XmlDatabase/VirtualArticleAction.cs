using System;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib
{
    [Serializable]
    public class VirtualArticleAction
    {
        [XmlAttribute] public Guid		Id				{ get; set; }
        [XmlAttribute] public String	Action			{ get; set; }		
        [XmlAttribute] public Guid		Target_Id		{ get; set; }						
        [XmlAttribute] public String	Redirect_Uri	{ get; set; }		// this should be an Uri but Uri's are not serializable
        [XmlAttribute] public string	TM_Server		{ get; set; }						
        [XmlAttribute] public string	Service			{ get; set; }
        [XmlAttribute] public string	Service_Data		{ get; set; }
    }
}