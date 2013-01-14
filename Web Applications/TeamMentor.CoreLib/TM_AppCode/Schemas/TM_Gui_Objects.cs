using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:System.Web.Extensions.dll

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
		
		public int add_UniqueString(string value)
		{				
			if (UniqueStrings.Contains(value).isFalse())
				UniqueStrings.Add(value);
				
			return get_UniqueString(value);
						
		}						
		public int get_StringIndex(string value)
		{
			return UniqueStrings.IndexOf(value);
		}		
		public string get_UniqueString(int index)
		{
			return UniqueStrings[index];
		}		
		public int get_UniqueString(string value)
		{
			return get_StringIndex(value);
		}						
		public int get_Index(string value)
		{
			return get_StringIndex(value);
		}		
		public string get_String(int index)
		{
			return get_UniqueString(index);
		}		
	}


/*	public class TreeNodeItem
	{
		public string type { get; set; }
		public string libraryId { get; set; }		
		public string itemId { get; set; }
	}*/
}
