using System;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib
{	
	public class TM_Library : MarshalByRefObject
	{		
		[XmlAttribute] public Guid Id		 { get; set; }
		[XmlAttribute] public string Caption { get; set; }
	}

/*	public class User
	{
		/// <remarks/>
		public int    UserId	{ get; set; }
		public string UserName	{ get; set; }
		public string Password	{ get; set; }		
		public string email		{ get; set; }
		public string FirstName { get; set; }
		public string LastName	{ get; set; }		
		public int	  Role		{ get; set; }		
		public bool	  Active	{ get; set; }		
		public string Title		{ get; set; }		
		public string Company	{ get; set; }
		[XmlElementAttribute(IsNullable = true)] public Nullable<DateTime> ExpirationDate { get; set; }		
	}*/

	public class TM_GuidanceItem : MarshalByRefObject
	{

		[XmlAttribute]	public Guid		Id				{ get; set; }
		[XmlAttribute]	public Guid		Id_Original		{ get; set; }
		[XmlAttribute]	public string	Title			{ get; set; }
		[XmlAttribute]	public string	Topic			{ get; set; }
		[XmlAttribute]	public string	Technology		{ get; set; }
		[XmlAttribute]	public string	Category		{ get; set; }
		[XmlAttribute]	public string	RuleType		{ get; set; }
		[XmlAttribute]	public string	Priority		{ get; set; }
		[XmlAttribute]	public string	Status			{ get; set; }
		[XmlAttribute]	public string	Author			{ get; set; }
		[XmlAttribute]	public DateTime LastUpdate		{ get; set; }
		[XmlAttribute]	public string	GuidanceType	{ get; set; }
		[XmlAttribute]	public Guid		Library			{ get; set; }	
	}

	public class TM_Folder : MarshalByRefObject
	{
	 
		[XmlAttribute]public Guid		Id		{ get; set; }
		[XmlAttribute] public string	Name	{ get; set; }
		[XmlAttribute] public string	Caption { get; set; }
		[XmlAttribute] public Guid		Library { get; set; }	
	}
}