using System;
using System.Web;
using System.IO;
using System.Security;
using System.Collections.Generic;
using XssEncoder = Microsoft.Security.Application.Encoder;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class FileUpload : IHttpHandler
    {        
        public static List<Guid> UploadTokens { get; set; }

        static FileUpload()
        {
            UploadTokens = new List<Guid>();
        }

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

            string message;
            var result = true;

            //calculate target folder
            var librariesZipsFolder = TMConfig.Current.LibrariesUploadedFiles;            

            var targetFolder = TM_Xml_Database.Current.Path_XmlDatabase.fullPath().pathCombine(librariesZipsFolder).fullPath();
            
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
            var buffer = new byte[request.ContentLength];
            using (var binaryReader = new BinaryReader(request.InputStream))
                binaryReader.Read(buffer, 0, buffer.Length);


            var fileStream = new FileStream(targetFile, FileMode.Create,
                FileAccess.ReadWrite);
            var binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(buffer);
            binaryWriter.Close();
            return true;
        }
        public bool IsReusable
        {
            get { return false; }
        }
    }
}
