using System;
using System.Web;
using System.IO;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using SecurityInnovation.TeamMentor.WebClient;
using XssEncoder = Microsoft.Security.Application.Encoder;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.APIs;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
//O2Ref:AntiXSSLibrary.dll

namespace SecurityInnovation.TeamMentor.WebClient
{
    public class FileUpload : IHttpHandler
    {        
        public static List<Guid> UploadTokens = new List<Guid>();

        public void ProcessRequest(HttpContext context)
        {
            var uploadTokenString = context.Request["uploadToken"];
            if (uploadTokenString.isGuid().isFalse())
                return;

            var uploadToken = uploadTokenString.guid();
            if (UploadTokens.contains(uploadToken).isFalse())
                throw new SecurityException("Upload Token must be provided");


            if (context.Request["qqfile"].inValid())
                return;

            //var file = context.Request.Files["Filedata"];	
            //context.Response.Write(file);		

            var message = "...";
            var result = true;

            //calculate target folder
            var librariesZipsFolder = TMConfig.Current.LibrariesUploadedFiles;            

            var targetFolder = TM_Xml_Database.Path_XmlDatabase.fullPath().pathCombine(librariesZipsFolder).fullPath();
            
            if (targetFolder.valid())
            {
                targetFolder.createDir();
                var fileName = XssEncoder.UrlEncode(context.Request["qqfile"]).Replace("%20", " ");
                var filePath = targetFolder.pathCombine(fileName);
                saveFile(context, filePath);
                message = "file saved to: " + filePath;
            }
            else
            {
                result = false;
                message = "Could find target directory";
            }
            var reply =@"{ 
			                success		: " + (result ? "true" : "false") + @" ,
			                message	: '" + message + @"'
		                }";
            context.Response.Write(reply);

            UploadTokens.remove(uploadToken);
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
            get { return false; }
        }
    }
}
