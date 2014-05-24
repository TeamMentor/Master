using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class GZip
    {
        public static void setGZipCompression_forAjaxRequests()
        {
            setGZipCompression_forAjaxRequests(HttpContextFactory.Request, HttpContextFactory.Response);
        }

        public static void setGZipCompression_forAjaxRequests(HttpRequestBase request, HttpResponseBase response)			
        {
            //based on code from http://geekswithblogs.net/rashid/archive/2007/09/15/Compress-Asp.net-Ajax-Web-Service-Response---Save-Bandwidth.aspx
            if (TMConfig.Current.TMSetup.EnableGZipForWebServices.isFalse())
                return;
            if (request.Url.isNull() || request.Url.AbsolutePath.starts("/rest")) //disabled it for rest requests
                return;
            try
            {
                if (request.ContentType.lower().starts(new List<string> {"text/xml", "application/json"}))
                {
                    string acceptEncoding = request.Headers["Accept-Encoding"];

                    if (!string.IsNullOrEmpty(acceptEncoding))
                    {
                        acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);

                        if (acceptEncoding.Contains("gzip"))
                        {
                            response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                            response.AddHeader("Content-encoding", "gzip");
                        }
                        else if (acceptEncoding.Contains("deflate"))
                        {
                            response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                            response.AddHeader("Content-encoding", "deflate");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.log("in enableGZipCompression_forAjaxRequests");
            }
        }

    }
}