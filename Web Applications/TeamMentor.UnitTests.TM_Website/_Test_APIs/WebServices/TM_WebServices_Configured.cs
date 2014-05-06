using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Web.Services.Protocols;
using System.Xml;
using FluentSharp.CoreLib;
using TeamMentor.UnitTests.TM_Website.WebServices;

namespace TeamMentor.UnitTests.TM_Website
{
    public class TM_WebServices_Configured : TM_WebServices
    {            
        public        string            Namespace            { get; set; }
        public        Uri               TargetServer         { get; set; }         
        public        string            SOAP_Request         { get; set; }
        public 		  TM_User 	        Cached_CurrentUser	 { get; set; }		        
        public        bool              Disable_Gzip         { get; set; }
        public        bool              Capture_ResponseData { get; set; }
        public        bool              Capture_CurrentUser  { get; set; }

        public        HttpWebRequest    webRequest       ;
        public        SoapClientMessage soapClientMessage;
        public        WebResponse       webResponse      ;        
        public        XmlReader         responseXmlReader;
        public        object[]          responseData     ;            
        

        public TM_WebServices_Configured(Uri targetServer)
        {			
            Disable_Gzip         = true;
            Capture_ResponseData = true;
            Capture_CurrentUser  = true;
            Namespace            = "http://teammentor.net/";
            TargetServer         = targetServer;
            Url                  = targetServer.append(Tests_Consts.WEBSERVICES_PATH).str();			
            CookieContainer      =  new CookieContainer();
        }
		
        protected override WebRequest GetWebRequest(Uri Uri)
        {
            return getWebRequest_Configured(Uri);
        }                
        public HttpWebRequest getWebRequest_Configured(Uri Uri)
        {
            webRequest = (HttpWebRequest)base.GetWebRequest(Uri);            
            webRequest.AutomaticDecompression = (Disable_Gzip) ? DecompressionMethods.None : DecompressionMethods.GZip;

            //set session Cookies			
            var cookies = CookieContainer.GetCookies(Uri);	    	
            if (cookies.notNull() && cookies.size() > 0 && cookies["Session"].notNull())
            {
                var sessionValue = cookies["Session"].Value;	    		
                var crsf_Token = (Cached_CurrentUser.notNull()) 
                                    ? Cached_CurrentUser.CSRF_Token 
                                    : "";
                webRequest.Headers.Add("CSRF-Token",crsf_Token);
                webRequest.Headers.Add("Session",sessionValue);	    			    		
            }
            return webRequest;
        }  
        
        protected override WebResponse GetWebResponse(WebRequest request)
        {            
            return getWebResponse_Configured(request);
        }        

        public HttpWebResponse  getWebResponse_Configured(WebRequest request)
        {
            webResponse = base.GetWebResponse(request);      
            captureResponseData(); 
            captureCurrentUser();
            return (HttpWebResponse)webResponse;
        }
        public HttpWebResponse getWebResponse()
        {
            return (HttpWebResponse)base.GetWebResponse(webRequest);
        }
        
        protected override XmlReader GetReaderForMessage(SoapClientMessage message, int bufferSize)
        {
            soapClientMessage = message;
            responseXmlReader = base.GetReaderForMessage(soapClientMessage, bufferSize);                        
            return responseXmlReader;   
        }
        
        public string currentSoapAction()
        {
            if (webRequest.notNull())
            {
                var soapAction = webRequest.Headers["SOAPAction"];
                if(soapAction.valid())
                    return soapAction.remove(Namespace,"\"");       
            }
            return null;
        }

        public object[] captureResponseData()
        {       
            //this code replicates what happens inside  System.Web.Services.Protocols.SoapHttpClientProtocol.Invoke

            responseData = null;   
            if (Capture_ResponseData)
            {
                var soapMethod_Name = currentSoapAction();
                if (soapMethod_Name.valid())
                {                               
                    var soapMethod_Name_Parameters  = new object[] {};
                    var methodInfo_ReadResponse     = typeof(SoapHttpClientProtocol).method("ReadResponse");
                    var methodInfo_BeforeSerialize  = typeof(SoapHttpClientProtocol).method("BeforeSerialize");
                    var current_SoapClientMessage = (SoapClientMessage)this.invoke(methodInfo_BeforeSerialize, webRequest, soapMethod_Name, soapMethod_Name_Parameters);                                

                
                    var stream = webResponse.GetResponseStream();                                   
                    if(stream.notNull())
                    {
                        //replace the original System.Net.ConnectStream object with an System.IO.MemoryStream
                        var streamReader = new StreamReader(stream);
                        var memoryStream = new MemoryStream(streamReader.ReadToEnd().bytes_Ascii());                    // this might have some side effects if there are binary data involve (but I don't think it is)
                        webResponse.field("m_ConnectStream", memoryStream);
                    
                        // get the current response data
                        responseData = (object[])this.invoke(methodInfo_ReadResponse, 
                                                             current_SoapClientMessage, 
                                                             webResponse, 
                                                             memoryStream, 
                                                             false);
                        // reset the stream position so that the normal Soap workflow also gets the data and returns it to the caller
                        memoryStream.Position = 0;
                    }
                }
            }
            return responseData;
        }
        public T getResponseData<T>()
        {
            if (responseData.notNull() && responseData.size() ==1)
            {
                if (responseData.first().type() == typeof(T))
                    return (T)responseData.first();
            }
            return default(T);
        }
        public TM_User captureCurrentUser()
        {
            if(Capture_CurrentUser)
            {
                switch(currentSoapAction())
                {
                    case "Current_User":                        
                        Cached_CurrentUser =  getResponseData<TM_User>();               
                        break;
                    case "Logout":
                        Cached_CurrentUser =  null;
                        break;
                }
            }            
            return Cached_CurrentUser;
        }
    }
}