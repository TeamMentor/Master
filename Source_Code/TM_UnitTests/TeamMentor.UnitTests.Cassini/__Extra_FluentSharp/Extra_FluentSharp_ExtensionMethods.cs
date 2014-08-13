using System;
using System.IO;
using System.Net;
using FluentSharp.CoreLib;

namespace FluentSharp.CoreLib
{
    public static class Extra_FluentSharp_Web_ExtensionMethods
    {
        public static HttpWebRequest httpWebRequest(this string url)
        {
            return url.uri().httpWebRequest();
        }
        public static HttpWebRequest httpWebRequest(this Uri uri)
        {
            if(uri.notNull())
                return (HttpWebRequest)WebRequest.Create(uri);
            return null;
        }
        public static HttpWebRequest json_Request(this HttpWebRequest httpWebRequest)
        {
            if (httpWebRequest.notNull())
                httpWebRequest.Accept = "application/json";
            return httpWebRequest;
        }

        public static HttpWebResponse get_Response(this HttpWebRequest httpWebRequest)
        {
            return (httpWebRequest.notNull())
                        ? (HttpWebResponse) httpWebRequest.GetResponse()
                        : null;
        }

        public static string readToEnd(this HttpWebRequest httpWebRequest)
        {
            return httpWebRequest.get_Response()
                                 .readToEnd();
        }
        public static string readToEnd(this HttpWebResponse httpWebResponse)
        {
            if (httpWebResponse.notNull())
            {                
                var responseStream = httpWebResponse.GetResponseStream();
                if (responseStream.notNull())
                    using (var sr = new StreamReader(responseStream))
                    {
                        return sr.ReadToEnd();
                    }
            }
            return null;
        }

        public static string GET_Json(this HttpWebRequest httpWebRequest)
        {
            return httpWebRequest.json_Request()
                                 .readToEnd();
        }

        public static string GET_Json(this string url)
        {
            return url.uri().GET_Json();
        }
        public static string GET_Json(this Uri uri)
        {
            return uri.httpWebRequest().GET_Json();
        }
    }
    public static class Extra_FluentSharp_ExtensionMethods
    {
        public static int inc(this int value)
        {
            return value + 1;
        }
        public static int dec(this int value)
        {
            return value - 1;
        }
        public static string folder_Create_Folder(this string targetFolder, string subFolderName)
        {
            if(targetFolder.folderExists())
                return targetFolder.mapPath(subFolderName).createDir();
            return null;
        }
        public static string append_FileName_To(this string fullPath, string target)
        {
            return target.append(fullPath.fileName());
        }
        public static Uri append_FileName_To(this string fullPath, Uri uri)
        {
            return uri.append(fullPath.fileName());
        }
        public static string append_To(this string value, string target)
        {
            return target.append(value);
        }
        public static Uri append_To(this string value, Uri target)
        {
            return target.append(value);
        }

        public static Uri mapPath(this Uri target, string value)
        {
            return target.append(value);
        }
    }
}