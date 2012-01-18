using System;
using System.Web;
using System.IO;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.WebClient;
using XssEncoder = Microsoft.Security.Application.Encoder;
//O2Ref:AntiXSSLibrary.dll

public class FileUpload : IHttpHandler
{

	public void ProcessRequest(HttpContext context)
	{				
		if(context.Request["qqfile"].inValid())
			return;

		//var file = context.Request.Files["Filedata"];	
		//context.Response.Write(file);		
		
		var message = "...";
		var result = true;
		var targetFolder = TMConfig.Current.LibrariesZipsFolder;
		if (targetFolder.valid())
		{
			targetFolder.createDir();
			var fileName = XssEncoder.UrlEncode(context.Request["qqfile"]).Replace("%20"," ");
			var filePath = targetFolder.pathCombine(fileName);
			saveFile(context,filePath);
			message = "file saved to: " + filePath;
		}
		else
		{
			result = false;
			message = "Could find target directory";
		}
		var reply = 
		@"{ 
			success		: " + (result ? "true" : "false")  + @" ,
			message	: '" + message + @"'
		}";		
		context.Response.Write(reply);		
	}

	public bool saveFile(HttpContext context, string targetFile)
	{
		var request = context.Request;
		byte[] buffer = new byte[request.ContentLength];
		using (BinaryReader br = new BinaryReader(request.InputStream))
			br.Read(buffer, 0, buffer.Length);

		
		FileStream fs = new FileStream(targetFile, FileMode.Create, 
			FileAccess.ReadWrite);
		BinaryWriter bw = new BinaryWriter(fs);
		bw.Write(buffer);
		bw.Close();
		return true;
	}
	public bool IsReusable
	{
		get { return false;	}
	}
}

