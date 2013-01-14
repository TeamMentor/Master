using System;
using System.Runtime.Serialization;

namespace TeamMentor.CoreLib
{
	[DataContract]
	public class TM_Credentials
	{
		[DataMember] public string UserName  { get; set; }
		[DataMember] public string Password  { get; set; }
	}

	[DataContract]
	public class TM_User
	{
		[DataMember]	public int		UserId		{ get; set; }
		[DataMember]	public string	UserName	{ get; set; }
		[DataMember]	public string	FirstName	{ get; set; }
		[DataMember]	public string	LastName	{ get; set; }
		[DataMember]	public string	Email		{ get; set; }
		[DataMember]	public string	Company		{ get; set; }
		[DataMember]	public string	Title		{ get; set; }
		[DataMember]	public Int64	CreatedDate	{ get; set; }
	}
}