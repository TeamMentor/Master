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
using O2.Kernel;
//O2Ref:AntiXSSLibrary.dll

namespace SecurityInnovation.TeamMentor.WebClient
{
    public class LibraryDownload : IHttpHandler
    {                

        public void ProcessRequest(HttpContext context)
        {
            var uploadTokenString = context.Request["uploadToken"];
/*            if (uploadTokenString.isGuid().isFalse())
            {
                context.Response.Write("No Upload Token provided");
                return;
            }

            var uploadToken = uploadTokenString.guid();
            if (FileUpload.UploadTokens.contains(uploadToken).isFalse())
                throw new SecurityException("Invalid Upload Token");
            */
            var libraryName = context.Request["library"] ?? "";            
            var library = TM_Xml_Database.Current.tmLibrary(libraryName);
            if (library.isNull())
            {                
                context.Response.Write("Library not found");
                return;
            }
            context.Response.Write("Processing Library: {0}".format(libraryName));
            context.Response.Write("...O2 Tempdir: {0}".format(PublicDI.config.O2TempDir));
            return;            
            
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

            //FileUpload.UploadTokens.remove(uploadToken);
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
