using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{	
	public class TM_GUI_Objects
	{		
		public List<string> GuidanceItemsMappings 	{ get; set;}
		public List<string> UniqueStrings			{ get; set;}
		
		public TM_GUI_Objects()
		{
			GuidanceItemsMappings = new List<string>();						
			UniqueStrings = new List<string>();
		}				
		
		public int      add_UniqueString(string value)
		{				
			if (UniqueStrings.Contains(value).isFalse())
				UniqueStrings.Add(value);
				
			return get_UniqueString(value);
						
		}						
		public int      get_StringIndex(string value)
		{
			return UniqueStrings.IndexOf(value);
		}		
		public string   get_UniqueString(int index)
		{
			return UniqueStrings[index];
		}		
		public int      get_UniqueString(string value)
		{
			return get_StringIndex(value);
		}						
		public int      get_Index(string value)
		{
			return get_StringIndex(value);
		}		
		public string   get_String(int index)
		{
			return get_UniqueString(index);
		}		
	}

}
