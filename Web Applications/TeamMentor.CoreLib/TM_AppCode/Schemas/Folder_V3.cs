using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
	public class Folder_V3	
	{
		public Guid libraryId { get; set; }
		public Guid folderId  { get; set; }
		public string name { get; set; }
		public List<View_V3> views { get; set; }
		public List<Folder_V3> subFolders { get; set; }
		//public List<Guid> guidanceItems		{ get; set; }
		
		public Folder_V3()
		{
			views = new List<View_V3>();
			subFolders = new List<Folder_V3>();
			//guidanceItems = new List<Guid>();
		}
		
		public override string ToString()
		{
			return "folder: {0}".format(name);
		}
	}
}