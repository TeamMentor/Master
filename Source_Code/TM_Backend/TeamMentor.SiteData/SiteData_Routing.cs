using System.Web;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.SiteData
{
    public static class SiteData_Routing
    {
        public static bool siteData_Handle_VirtualPath(this string virtualPath)
        {
            return TM_FileStorage.Current.siteData_Handle_VirtualPath(virtualPath);        
        }
        public static bool siteData_Handle_VirtualPath(this TM_FileStorage tmFileStorage, string virtualPath)
        {
            if (tmFileStorage.isNull())
                return false;

            var pathSiteData  = tmFileStorage.path_SiteData();
            if (pathSiteData.isNull())
                return false;
                        
            var fullPath = pathSiteData.pathCombine(virtualPath);
            
            if(fullPath.fileExists().isFalse())
                fullPath = pathSiteData.pathCombine(virtualPath.fileName());            // see if the file is in the root

            if (fullPath.contains(pathSiteData).isFalse())          // prevent file transversal
                return false;       
            if(fullPath.fileExists())
                return fullPath.siteData_WriteFileToResponseStream();

            if (virtualPath == "siteData")
                return true;
            return false;            
        }

        public static bool siteData_WriteFileToResponseStream(this string fullPath)
        {
            if (fullPath.fileExists().isFalse())
                return false;
            var response = HttpContextFactory.Response;
            if (response.isNull())            
                return false;
            fullPath.siteData_SetContentType_For_File(response);
            if (response.ContentType.contains("image"))
            //response.WriteFile(fullPath);
                response.WriteFile(fullPath);
            else
                response.Write(fullPath.fileContents());
            response.End();
            return true;
        }
        public static string siteData_SetContentType_For_File(this string fullPath, HttpResponseBase response)
        {
            var extension  = fullPath.extension();
            
            if (extension.valid() && response.notNull())
            {
                switch (extension)
                {
                    case ".js"  : response.ContentType = "application/javascript";  break;
                    case ".aspx": response.ContentType = "text/html"             ;  break;
                    case ".rz"  : response.ContentType = "text/html"             ;  break;    // for Razor pages
                    case ".html": response.ContentType = "text/html"             ;  break;
                    case ".htm" : response.ContentType = "text/html"             ;  break;
                    case ".jpg" : response.ContentType = "image/jpeg"            ;  break;
                    case ".jpeg": response.ContentType = "image/jpeg"            ;  break;                        
                    case ".gif" : response.ContentType = "image/gif"             ;  break;
                    case ".png" : response.ContentType = "image/png"             ;  break;
                    case ".css" : response.ContentType = "text/css"              ;  break;
                    case ".xml" : response.ContentType = "application/xml"       ;  break;
                    case ".xslt": response.ContentType = "application/xslt+xml"  ;  break;
                    default    : response.ContentType  = "text/plain"            ;  break;    // will also capture .txt                       
                }
            }
            return fullPath;
        }
    }
}
