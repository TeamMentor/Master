<%@ WebHandler Language="C#" Class="GetDataHandler" %>

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

public class UserRequest
{
	public string securityToken {get;set;}	
	public string command {get;set;}
	public string file {get;set;}
	public string contents {get;set;}

	public UserRequest()
	{				
	}
	
	public bool isAuthorized()
	{
		var webEditorSecretDataFile = AppDomain.CurrentDomain.BaseDirectory.pathCombine("webEditorSecretData.config");
		if (webEditorSecretDataFile.fileExists())		
			return securityToken == webEditorSecretDataFile.load<string>();
		return false;
				
		//this (below) doesn't work because the webeditor is an *.ashx and doesn't have access to the HttpContext Session object
        /*
	    var webEditorSecretData = System.Web.HttpContext.Current.Session["webEditorSecretData"];
		if (webEditorSecretData is Guid)
			return securityToken == webEditorSecretData.str();		
		return false;
		*/
	}	
}

public class UserRequestExecution
{
	public Action<object> writeJson {get;set;}	
	public List<string> dirsToIgnore {get;set;}
  
  	public UserRequestExecution()
	{
		dirsToIgnore = new List<string> { "bin", "obj" };
	}
  
	public UserRequestExecution(Action<object> _writeJson) : this()
	{
		writeJson = _writeJson;
	}
	
	public UserRequestExecution executeCommand(UserRequest userRequest)
	{
		if (userRequest.isAuthorized().isFalse())
		{
			writeJson("NOT Authorized(please set the securityToken value)");
			return this;
		}
		
		object result = null;
        var targetFolder = AppDomain.CurrentDomain.BaseDirectory;
        //var targetFolder = AppDomain.CurrentDomain.BaseDirectory.pathCombine("..");
		switch(userRequest.command)
		{
			case "getFiles":
				
				var jsTree = new JsTree();
				var rootNode = jsTree.add_Node("\\");
				mapFolder(rootNode, targetFolder, targetFolder);
				rootNode.state="open";
				result  = jsTree;
				break;	
			case "saveFile":							
				var fullPath = HttpContext.Current.Server.MapPath(userRequest.file);
                //var fullPath = targetFolder.pathCombine("." + userRequest.file);
				userRequest.contents.saveAs(fullPath);				
				result = "file '{0}' saved ok at {1}".format(userRequest.file, DateTime.Now);			
				break;
			case "getFile":							
				//var fileToGet = HttpContext.Current.Server.MapPath(userRequest.file);	
                var fileToGet = targetFolder.pathCombine("." + userRequest.file);
				if (fileToGet.fileExists())
					result = fileToGet.fileContents();
				else
					result = "ERROR: in getFile, could not find file: {0}".format(fileToGet);
				break;
			case "deleteFile":							
				var fileToDelete = HttpContext.Current.Server.MapPath(userRequest.file);							
				if (fileToDelete.fileExists() && Files.deleteFile(fileToDelete))
				{					
					result = "Deleted file: {0}".format(userRequest.file);
				}
				else
					result = "ERROR: in getFile, could not find or delete file: {0}".format(fileToDelete);
				break;	
			default:
				result = "unsuported command: {0}".format(userRequest.command);
				break;
		}
		
		
		//	executeCommand(userRequest);
			//writeJson(userRequest);
		//else
		writeJson(result);
		return this;
			
	}
	
	public JsTreeNode mapFolder(JsTreeNode targetNode, string path, string rootPath)
	{        		
		foreach(var folder in path.dirs().sort())						
			if (dirsToIgnore.contains(folder.remove(rootPath)).isFalse())
			{
				var folderNode = targetNode.add_Node(folder.fileName(), "\\" + folder.remove(rootPath) );
				mapFolder(folderNode, folder,  rootPath);			
			}
		addFiles(targetNode, path, rootPath);
		return targetNode;
	}
	
	public JsTreeNode addFiles(JsTreeNode targetNode, string path, string rootPath)
	{
		var files  = path.files().sort().where((file)=> file.extension()!=".dll");
		if (files.size()>0)
		{
			//var filesNode = targetNode.add_Node("_files");
			foreach(var file in files)			
				targetNode.add_Node(file.fileName(), "\\" + file.remove(rootPath))
						 .data.icon = "images/ViewIcon.png";
		}
		return targetNode;	
	}
}

public class GetDataHandler : IHttpHandler
{
	public HttpContext context;
	public HttpRequest request;
	public HttpResponse response;
	
	public void handleRequest()
	{
		var userRequest = new UserRequest() { 
								command= request["command"] , 
								file = request["file"] , 
								contents = request["contents"] , 
								securityToken = request["securityToken"]
							  };
							  
		new UserRequestExecution(writeJson).executeCommand(userRequest);
	}
	
	
    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest (HttpContext _context)
    {
		context = _context;
		request = _context.Request;
		response = _context.Response;
        context.Response.ContentType = "application/json";
		//context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = Encoding.UTF8;		
		handleRequest();		              
    }
			
	public void writeJson(object _object)
	{
		JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        string jsondata = javaScriptSerializer.Serialize(_object);        
        writeRaw(jsondata);
	}

	
	public void writeRaw(string text)
	{
		context.Response.Write(text);
	}
}