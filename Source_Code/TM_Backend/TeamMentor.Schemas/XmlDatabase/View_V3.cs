using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
	public class View_V3 : MarshalByRefObject
	{
		public Guid         libraryId { get; set; }
		public Guid         folderId { get; set; }
		public Guid         viewId { get; set; }
		public string       caption { get; set; }
		public string       author { get; set; }
		public string       guidanceItems_Indexes { get; set; }
		public List<Guid>   guidanceItems {get;set;}
		
		public View_V3()
		{
			guidanceItems= new List<Guid>();
		}
		
		public override string ToString()
		{
			return "view: {0}".format(caption);
		}
	}
}