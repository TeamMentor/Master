using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    public class TestFixture_WebServices 
    {
        public TM_QA_Config     QAConfig      { get; set; }
    	public Uri 				WebSite_Url   { get; set; }


        public TM_WebServices_Configured 	webServices;

        public TestFixture_WebServices()
        {
            QAConfig = new TM_QA_Config_Loader().load();
            this.set_TM_Server();   
            QAConfig.assertIgnore_If_Offine(webServices.TargetServer);            
        }

    }

    public static class TestFixture_WebServices_ExtensionMethods
    {
        public static Uri                       default_TM_Server    (this TestFixture_WebServices tfWebServices)
        {
            return tfWebServices.QAConfig.Url_Target_TM_Site.uri();
        }        
    	public static TestFixture_WebServices   set_TM_Server        (this TestFixture_WebServices tfWebServices, string websiteUrl = null)
    	{            
            tfWebServices.WebSite_Url = websiteUrl.valid() 
                                                ? websiteUrl.uri() 
                                                : tfWebServices.default_TM_Server();         
    		tfWebServices.webServices  	 = new TM_WebServices_Configured(tfWebServices.WebSite_Url);
    		return tfWebServices;
    	}
                    
        public static Test_User                 login_As_Admin       (this TestFixture_WebServices tfWebServices)                   
        {
            var adminUser = tfWebServices.QAConfig.testUser("qa-admin");
            var authToken = adminUser.AuthToken;
            if (tfWebServices.webServices.login_with_AuthToken(authToken))
                return adminUser;
            return null;
        }
        public static Test_User                 login_As_Editor      (this TestFixture_WebServices tfWebServices)                   
        {
            var editorUser = tfWebServices.QAConfig.testUser("qa-editor");
            var authToken = editorUser.AuthToken;
            if (tfWebServices.webServices.login_with_AuthToken(authToken))
                return editorUser;
            return null;
        }
        public static Test_User                 login_As_Reader      (this TestFixture_WebServices tfWebServices)                   
        {
            var readerUser = tfWebServices.QAConfig.testUser("qa-reader");
            var authToken = readerUser.AuthToken;
            if (tfWebServices.webServices.login_with_AuthToken(authToken))
                return readerUser;
            return null;
        }        
        public static string                    http_GET             (this TestFixture_WebServices tfWebServices, string virtualPath)
        {
            if (tfWebServices.notNull() && virtualPath.valid())
            {
                var webResponse = tfWebServices.http_GET_WebResponse(virtualPath);                
                var stream = webResponse.GetResponseStream();    
                return stream.readToEnd();                                                    
            }
            return "";
        }
        public static HttpWebResponse           http_GET_WebResponse (this TestFixture_WebServices tfWebServices, string virtualPath)
        {               
            var url = tfWebServices.WebSite_Url.append(virtualPath);
            var webRequest = tfWebServices.webServices.getWebRequest_Configured(url);
            webRequest.AllowAutoRedirect = true;
                
            return tfWebServices.webServices.getWebResponse();            

        }
        public static string                    http_POST            (this TestFixture_WebServices tfWebServices, string virtualPath, string postData)
        {
            return tfWebServices.http_METHOD("POST", "x-www-form-urlencoded", virtualPath, postData);
        }
        public static string                    http_POST_JSON       (this TestFixture_WebServices tfWebServices, string virtualPath, string postData)
        {
            return tfWebServices.http_METHOD("POST", "application/json", virtualPath, postData);
        }
        public static string                    http_PUT             (this TestFixture_WebServices tfWebServices, string virtualPath, string postData)
        {
            return tfWebServices.http_METHOD("PUT", "x-www-form-urlencoded", virtualPath, postData);
        }
        public static string                    http_PUT_JSON        (this TestFixture_WebServices tfWebServices, string virtualPath, string postData)
        {
            return tfWebServices.http_METHOD("PUT", "application/json", virtualPath, postData);
        }
        public static string                    http_METHOD          (this TestFixture_WebServices tfWebServices, string method, string contentType, string virtualPath, string postData)
        {
            if (tfWebServices.notNull() && virtualPath.valid() && postData.valid())
            {
                var webResponse = tfWebServices.http_METHOD_WebResponse(method, contentType, virtualPath,postData);
                
                var stream = webResponse.GetResponseStream();    
                return stream.readToEnd();                                                    
            }
            return "";
        }
        public static HttpWebResponse           http_METHOD_WebResponse(this TestFixture_WebServices tfWebServices, string method, string contentType, string virtualPath, string postData)
        {
            
            var url = tfWebServices.WebSite_Url.append(virtualPath);
            var webRequest = tfWebServices.webServices.getWebRequest_Configured(url);
            
            webRequest.AllowAutoRedirect = true;
            
            webRequest.Method = method;            
            webRequest.ContentType = contentType;

            var data = postData.asciiBytes();
            webRequest.ContentLength = data.Length;
            Stream myStream = webRequest.GetRequestStream();
            myStream.Write(data,0,data.Length);
            myStream.Close();                        
            
            return tfWebServices.webServices.getWebResponse();                                   
        }
    }
}
