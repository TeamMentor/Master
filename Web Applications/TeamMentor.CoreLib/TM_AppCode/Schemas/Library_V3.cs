using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
	public class Library_V3	
	{
		public Guid libraryId 				{ get; set; }
		public String name		 			{ get; set; }
		public List<Folder_V3> subFolders 	{ get; set; }
		public List<View_V3> views 			{ get; set; }
		public List<Guid> guidanceItems		{ get; set; }
		
		public Library_V3()
		{
			subFolders = new List<Folder_V3> ();
			views = new List<View_V3>();
			guidanceItems = new List<Guid>();
		}
		
		public override string ToString()
		{
			return "library: {0}".format(name);
		}
	}
}