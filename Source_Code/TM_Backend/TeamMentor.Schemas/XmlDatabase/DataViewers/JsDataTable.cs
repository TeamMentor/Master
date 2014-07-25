using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamMentor.CoreLib
{
    [Serializable]
	public class JsDataTable : MarshalByRefObject
	{
		public List<List<object>> aaData;
		public List<JsDataColumn> aoColumns;		
		
		public JsDataTable()
		{
			 aaData = new List<List<object>>() ;
			 aoColumns = new List<JsDataColumn>();
		}
		
        [Serializable]
		public class JsDataColumn : MarshalByRefObject
		{
			public string sTitle {get; set; }
			public string sClass {get; set; }
		}
	}
	
	
	public static class JsDataTable_ExtensionMethods
	{		
		
		public static JsDataTable add_Row(this JsDataTable jsDataTable, params object[] cells)
		{
			return jsDataTable.add_Row(cells.ToList());
		}
		
		public static JsDataTable add_Row(this JsDataTable jsDataTable, List<object> cells)
		{
			jsDataTable.aaData.Add(cells);
			return jsDataTable;
		}
				
		public static JsDataTable add_Column(this JsDataTable jsDataTable, string title)
		{
			return jsDataTable.add_Column(title, null);
		}
		
		public static JsDataTable add_Column(this JsDataTable jsDataTable, string title, string _class)
		{
			jsDataTable.aoColumns.Add(new JsDataTable.JsDataColumn { sTitle = title, sClass = _class });
			return jsDataTable;
		}
		
		public static JsDataTable add_Columns(this JsDataTable jsDataTable, params string[] titles)
		{
			foreach(var title in titles)
				jsDataTable.add_Column(title);
			return jsDataTable;
		}
	}
}
