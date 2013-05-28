using System;
using System.Security;
using System.Web;
using XssEncoder = Microsoft.Security.Application.Encoder;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class LibraryDownload : IHttpHandler
    {                

        public void ProcessRequest(HttpContext context)
        {
			try
			{
                //Temporarily disable uploadToken request
				var uploadTokenString = context.Request["uploadToken"];
				if (uploadTokenString.isGuid().isFalse())
				{
					context.Response.Write("No Upload Token provided");
					return;
				}

				var uploadToken = uploadTokenString.guid();
				if (FileUpload.UploadTokens.contains(uploadToken).isFalse())
					throw new SecurityException("Invalid Upload Token");
				FileUpload.UploadTokens.remove(uploadToken);			
                
				var libraryName = context.Request["library"] ?? "";
				var library = TM_Xml_Database.Current.tmLibrary(libraryName);
				if (library.isNull())
				{
					context.Response.Write("Library not found");
					return;
				}
                var pathToLibraryXmlFile = TM_Xml_Database.Current.xmlDB_Path_Library_XmlFile(library);
                var pathToGuidanceItemsFolder = TM_Xml_Database.Current.xmlDB_Path_Library_RootFolder(library);
				
				var targetDir = libraryName.tempDir();
				var folderToZip = targetDir.pathCombine(libraryName).createDir();
				//var zipFile = targetDir.parentFolder().pathCombine(libraryName + ".zip");
				var zipFile = "{0}.zip".format(libraryName).tempFile();
				"Copying library files {0} into folder: {1}".info(libraryName, folderToZip);

				O2.DotNetWrappers.Windows.Files.copyFolder(pathToGuidanceItemsFolder, folderToZip, true, true, ".git");
				O2.DotNetWrappers.Windows.Files.copy(pathToLibraryXmlFile, folderToZip);
				
				new ICSharpCode.SharpZipLib.Zip.FastZip().CreateZip(zipFile, targetDir, true, "");
				context.Response.ContentType = "application/zip";
				context.Response.AddHeader("content-disposition", "filename={0}.zip".format(libraryName));
				context.Response.WriteFile(zipFile);
				//
				//context.Response.Write("Processing Library: {0}".format(libraryName));
				//context.Response.Write("...O2 Tempdir: {0}".format(PublicDI.config.O2TempDir));

				//context.Response.Write(reply);

				//FileUpload.UploadTokens.remove(uploadToken);
			}
			catch (Exception ex)
			{
				ex.log("in LibraryDownload");
				context.Response.Write("There was an error downloading the library");
			}

        }
        
        public bool IsReusable
        {
            get { return false; }
        }
    }
}
