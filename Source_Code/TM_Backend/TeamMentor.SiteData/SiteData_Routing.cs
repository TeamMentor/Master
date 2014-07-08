using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using FluentSharp.CoreLib;
using FluentSharp.Web;
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
            if (tmFileStorage.isNull() || virtualPath.isNull())
                return false;

            temp_SwapSiteDataUtil(virtualPath.removeFirstChar());

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

        //temp method to help debugging (move to REST API)
        private static void temp_SwapSiteDataUtil(string virtualPath)
        {
            try
            {
                var tmFileStorage = TM_FileStorage.Current;
                var tmServer      = tmFileStorage.tmServer();
                var siteConfig    = tmServer.siteData_Config(virtualPath);

                var pathSiteData_before = tmFileStorage.path_SiteData();

                if (siteConfig.isNull())
                    return;
                tmServer.active_SiteData(siteConfig);

                var pathSiteData_after = tmFileStorage.path_SiteData();
                if (pathSiteData_before == pathSiteData_after)
                    tmFileStorage.set_Path_SiteData();

                var pathSiteData_after2 = tmFileStorage.path_SiteData();

                HttpContextFactory.Response.Redirect("/");
            }
            catch(Exception ex)
            {
                ex.log();
            }
        }

        public static bool siteData_WriteFileToResponseStream(this string fullPath)
        {
            if (fullPath.fileExists().isFalse())
                return false;
            var response = HttpContextFactory.Response;

            if (response.isNull())            
                return false;

            response.handleAspxPage(fullPath);

            fullPath.siteData_SetContentType_For_File(response);
            if (response.ContentType.contains("image"))            
                response.WriteFile(fullPath);
            else
                response.Write(fullPath.fileContents());
            response.End();
            
            return true;  //we will never reach here
        }
        public static bool handleAspxPage(this HttpResponseBase response, string fullPath)
        {
            if (fullPath.extension(".aspx"))
            {
                var compiledPage =System.Web.UI.PageParser.GetCompiledPageInstance(fullPath.fileName(), fullPath, HttpContext.Current);
                compiledPage.ProcessRequest(HttpContext.Current);
                HttpContextFactory.Response.End();    
                //we will never reach here
            }    
            return false;
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
