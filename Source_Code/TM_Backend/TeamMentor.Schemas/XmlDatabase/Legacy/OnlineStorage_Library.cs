using System;
using System.Xml;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib
{
    /*
	public class GuidanceItemIndex : MarshalByRefObject
	{		
		[XmlAttribute]		public string id { get; set; }
		[XmlAttribute]		public DateTime lastUpdate { get; set; }
		[XmlAttribute]		public bool deleted { get; set; }
	}

	public class LibraryIndex : MarshalByRefObject
	{
		[XmlArrayItem("GuidanceItem", IsNullable = false)]	public GuidanceItemIndex[] GuidanceItems { get; set; }
		[XmlAttribute]										public string id { get; set; }
	}


	public class Field : MarshalByRefObject
	{
		[XmlAttribute]		public string    id { get; set; }
		[XmlAttribute]		public string    name { get; set; }
		[XmlAttribute]		public FieldType type { get; set; }
		[XmlAttribute]		public bool      distinct { get; set; }
	}
	

	public enum FieldType
	{
		Text,
		Combo,
	}

	public class Schema : MarshalByRefObject
	{
		[XmlArrayItem(IsNullable = false)]	public Field[] Fields { get; set; }
		[XmlAttribute]						public string id { get; set; }
	}

	public class GuidanceType : MarshalByRefObject
	{
		[XmlAttribute]		public string   id { get; set; }
		[XmlAttribute]		public string   name { get; set; }
		[XmlAttribute]		public string   caption { get; set; }
		[XmlAttribute]		public string   listCaption { get; set; }
		[XmlAttribute]		public string   newCaption { get; set; }
		[XmlAttribute]		public string   schemaId { get; set; }
		[XmlAttribute]		public string   exampleContent { get; set; }
		[XmlAttribute]		public string   templateSchemaContent { get; set; }
		[XmlAttribute]		public string   templateContent { get; set; }
		[XmlAttribute]		public DateTime lastUpdate { get; set; }
		[XmlIgnore]			public bool     lastUpdateSpecified { get; set; }
		[XmlAttribute]		public bool     delete { get; set; }
		[XmlIgnore]			public bool     deleteSpecified { get; set; }
	}
	
	public class UserViews : FolderStructure            { }
	
    [Serializable]
	public class NavigationTree : Identifiable
	{
		public UserViews UserViews { get; set; }
		public Libraries Libraries { get; set; }
	}

	public class Credentials : System.Web.Services.Protocols.SoapHeader
	{
		public string			User { get; set; }
		public string			Password { get; set; }
		public Guid				AdminSessionID { get; set; }
		[XmlAnyAttribute]	
		public XmlAttribute[]	AnyAttr { get; set; }
	}

	public class Content : MarshalByRefObject
	{
		[XmlElement("GuidanceItem")]	public GuidanceItem[] GuidanceItem { get; set; }
		[XmlAttribute]					public int @from { get; set; }
		[XmlAttribute]					public int count { get; set; }
	}
	*/

	public class Identifiable : MarshalByRefObject
	{
		[XmlAttribute]      public string id                    { get; set; }
		[XmlAttribute]      public bool delete                  { get; set; }
	}

	public class Libraries : Identifiable
	{
		[XmlElement("Library")] public Library[] Library        { get; set; }
	}


	public class Library : FolderStructure
	{
		[XmlAttribute]      public bool readProtection { get; set; }
	}
	
	public class FolderStructure : Identifiable
	{
		[XmlArrayItem(IsNullable = false)]	public View[] Views { get; set; }
		[XmlArrayItem(IsNullable = false)]	public Folder[] Folders { get; set; }
		[XmlAttribute]						public string caption { get; set; }
	}

	public class View : Identifiable
	{
		[XmlAttribute]	public string caption { get; set; }
		[XmlAttribute]	public string creator { get; set; }
		[XmlAttribute]	public string parentFolder { get; set; }
		[XmlAttribute]	public string library { get; set; }
		[XmlAttribute]	public string creatorCaption { get; set; }
		[XmlAttribute]	public string criteria { get; set; }
		[XmlAttribute]	public DateTime lastUpdate { get; set; }
		[XmlIgnore]		public bool lastUpdateSpecified { get; set; }
	}

	public class GuidanceItem
	{
		[XmlAttribute]		public string id_original { get; set; }
							public string content { get; set; }
		[XmlAttribute]		public string id { get; set; }
		[XmlAttribute]		public string title { get; set; }
		[XmlAttribute]		public bool delete { get; set; }
		[XmlAttribute]		public string guidanceType { get; set; }
		[XmlAttribute]		public DateTime lastUpdate { get; set; }
		[XmlAttribute]		public string guidanceTypeCaption { get; set; }
		[XmlAttribute]		public string library { get; set; }
		[XmlAttribute]		public string creator { get; set; }
		[XmlAttribute]		public string creatorCaption { get; set; }
		[XmlAttribute]		public string images { get; set; }
		[XmlAnyAttribute]	public XmlAttribute[] AnyAttr { get; set; }
	}

	public class Folder : FolderStructure               { }
	/*
	public class ListView
	{
		[XmlArrayItem(IsNullable = false)]	public ColumnDefinition[] ColumnDefinitions { get; set; }
											public Content Content { get; set; }
		[XmlAttribute]						public string id { get; set; }
	}
	
	public class ColumnDefinition
	{
		[XmlAttribute]		public string caption { get; set; }
		[XmlAttribute]		public bool sortEnabled { get; set; }
		[XmlAttribute]		public bool autoFilterEnabled { get; set; }
		[XmlAttribute]		public int schemaFieldId { get; set; }
		[XmlAttribute]		public int width { get; set; }
		[XmlAttribute]		public SortOrder sortOrder { get; set; }
		[XmlIgnore]			public bool sortOrderSpecified { get; set; }
		[XmlAttribute]		public string autoFilter { get; set; }
	}

	public enum SortOrder
	{
		None,
		Ascending,
		Descending,
	}*/
}
