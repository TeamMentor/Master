using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace TeamMentor.CoreLib
{
	public class JsTree
	{
		public List<JsTreeNode> data;
		
		public JsTree()
		{
			data = new List<JsTreeNode>();
		}
	}
	
	public class JsTreeNode
	{
		public Attributes attr { get; set; }       
	    public Data data  { get; set; }           
	    public string state { get; set; }       	    
	    public List<JsTreeNode> children { get; set; }           
	    
	    public JsTreeNode()
	    {
	    	children = new  List<JsTreeNode>();
	    	data = new Data();
	    	attr = new Attributes();
	    }    
		
	    public JsTreeNode(string title) : this()
	    {
	    	data.title = title;
	    }
		
		public JsTreeNode(string title, string id) : this(title)
		{
			attr.id = id;
		}
		
		public override string ToString()
		{
			return data.title;
		}
	}
	public class Attributes
	{
	    public string id { get; set; }   
	    public string rel { get; set; }           
	    public string mdata { get; set; }       
	}
	
	public class Data
	{
	    public string title { get; set; }       
	    public string icon { get; set; }       
	}
	
	public static class JsTree_ExtensionMethods
	{
		/*public static string jsonString(this object _object)
		{
			return new JavaScriptSerializer().Serialize(_object);
		}*/
		
		public static JsTreeNode add_Node(this JsTree jsTree, string title)
		{
			return jsTree.add_Node(title, "");
		}
		
		public static JsTreeNode add_Node(this JsTree jsTree, string title, string id)
		{
			var newJsTreeNode = new JsTreeNode(title,id);
			jsTree.data.Add(newJsTreeNode);
			return newJsTreeNode;
		}
		
		public static List<JsTreeNode> add_Nodes(this JsTree jsTree, params string[] titles)
		{
		    return titles.Select(title => jsTree.add_Node(title)).ToList();
		}

	    public static JsTreeNode add_Node(this JsTreeNode jsTreeNode, string title)
		{
			return jsTreeNode.add_Node(title, "");
		}
		
		public static JsTreeNode add_Node(this JsTreeNode jsTreeNode, string title, string id)
		{
			var newJsTreeNode = new JsTreeNode(title, id);
			jsTreeNode.children.Add(newJsTreeNode);
			return newJsTreeNode;
		}
		
		public static List<JsTreeNode> add_Nodes(this JsTreeNode jsTreeNode, params string[] titles)
		{
		    return titles.Select(title => jsTreeNode.add_Node(title)).ToList();
		}
	}
}
