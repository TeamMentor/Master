// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Collections.Specialized;
using System.Web;
using FluentSharp.CoreLib;
using Moq;


//O2Ref:Moq.dll
//O2Ref:System.Web.Abstractions.dll

namespace O2.FluentSharp
{
    public class API_Moq_HttpContext 
    {        	
        public Mock<HttpContextBase>        MockContext  { get; set; }
        public Mock<HttpRequestBase>        MockRequest  { get; set; }
        public Mock<HttpResponseBase>       MockResponse { get; set; }
        public MockHttpSession              MockSession  { get; set; }
        public Mock<HttpServerUtilityBase>  MockServer   { get; set; }		
        
        public HttpContextBase HttpContextBase  	{ get; set; }
        public HttpRequestBase HttpRequestBase  	{ get; set; }
        public HttpResponseBase HttpResponseBase  	{ get; set; }  
        
        public String BaseDir						{ get; set; }
        public Uri    RequestUrl                    { get; set; }   

        public API_Moq_HttpContext() : this(null)
        {			
        }
        
        public API_Moq_HttpContext(string baseDir)
        {
            BaseDir = baseDir;
            RequestUrl = "http://localhost".uri();
            createBaseMocks();
            setupNormalRequestValues();
        }
        
        public API_Moq_HttpContext createBaseMocks()
        {
            MockContext = new Mock<HttpContextBase>();
            MockRequest = new Mock<HttpRequestBase>();
            MockResponse = new Mock<HttpResponseBase>();
            MockSession = new MockHttpSession();
            MockServer = new Mock<HttpServerUtilityBase>();
            
    
            MockContext.Setup(ctx => ctx.Request).Returns(MockRequest.Object);
            MockContext.Setup(ctx => ctx.Response).Returns(MockResponse.Object);
            MockContext.Setup(ctx => ctx.Session).Returns(MockSession);
            MockContext.Setup(ctx => ctx.Server).Returns(MockServer.Object);
            
            
            HttpContextBase = MockContext.Object; 
            HttpRequestBase = MockRequest.Object;
            HttpResponseBase = MockResponse.Object;
                        
            return this;
        }
        
        public Func<string,string> context_Server_MapPath {get;set;} 
        
        public API_Moq_HttpContext setupNormalRequestValues()		
        {							        
            var genericIdentity = new GenericIdentity("genericIdentity");
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] {});
            MockContext.Setup(context => context.User).Returns(genericPrincipal);	     	
            MockContext.Setup(context => context.Cache).Returns(HttpRuntime.Cache);            
            
            //Request
            MockRequest.Setup(request =>request.InputStream	      ).Returns(new MemoryStream()); 
            MockRequest.Setup(request =>request.Cookies		      ).Returns(new HttpCookieCollection()); 
            MockRequest.Setup(request =>request.Headers		      ).Returns(new NameValueCollection()); 
            MockRequest.Setup(request =>request.QueryString	      ).Returns(new NameValueCollection()); 
            MockRequest.Setup(request =>request.Form		      ).Returns(new NameValueCollection()); 
            MockRequest.Setup(request =>request.ServerVariables   ).Returns(new NameValueCollection()); 
            
            MockRequest.Setup(request =>request.ContentType	      ).Returns(""); 
            MockRequest.Setup(request =>request.Url	              ).Returns(()=> RequestUrl); 
            MockRequest.Setup(request =>request.IsLocal	          ).Returns(()=> RequestUrl.Host == "localhost" || RequestUrl.Host=="127.0.0.1"); 
            MockRequest.Setup(request =>request.IsSecureConnection).Returns(()=> RequestUrl.Scheme.lower() == "https");
                
            
            //Response
            var outputStream = new MemoryStream();
            var redirectTarget = "";
            MockResponse.SetupGet(response => response.Cache        ).Returns(new Mock<HttpCachePolicyBase>().Object);
            MockResponse.Setup   (response => response.Cookies      ).Returns(new HttpCookieCollection()); 	     	
            MockResponse.Setup   (response => response.Headers      ).Returns(new NameValueCollection());
            MockResponse.Setup   (response => response.OutputStream ).Returns(outputStream);
            MockResponse.Setup   (response => response.Write        (It.IsAny<string>())                    ).Callback((string code)              => outputStream.Write(code.asciiBytes(), 0, code.size()));
            MockResponse.Setup   (response => response.AddHeader    (It.IsAny<string>(), It.IsAny<string>())).Callback((string name,string value) => MockResponse.Object.Headers.Add(name,value));
            MockResponse.Setup   (response => response.Redirect     (It.IsAny<string>())                    ).Callback((string target)            =>{ redirectTarget = target; throw new Exception("Thread was being aborted.");});            
            
            MockResponse.Setup   (response => response.IsRequestBeingRedirected ).Returns(() => redirectTarget.valid());
            MockResponse.Setup   (response => response.RedirectLocation         ).Returns(() => redirectTarget);
            
            //Server
            MockServer.Setup(server => server.MapPath (It.IsAny<string>()))                 .Returns ((string path)                      =>  BaseDir.pathCombine(path));
            MockServer.Setup(server => server.Transfer(It.IsAny<string>()))                 .Callback((string target)                    =>  { redirectTarget = target; throw new Exception("Thread was being aborted.");}  );   // use the redirectTarget to hold this value
            MockServer.Setup(server => server.Transfer(It.IsAny<string>(),It.IsAny<bool>())).Callback((string target, bool preserveForm) =>  { redirectTarget = target; throw new Exception("Thread was being aborted.");}  );   // use the redirectTarget to hold this value            
            //var writer = new StringWriter();
            //context.Expect(ctx => ctx.Response.Output).Returns(writer);	     		     	
            return this;
        }
    }
    public class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string,object> sessionData = new Dictionary<string,object> ();
        public override string SessionID
        {
            get { return "".add_RandomLetters(15); }
        }
        public override object this[string key]
        {
            get
            {                
                return (sessionData.hasKey(key)) ? sessionData[key] :  null;
            }
            set { sessionData[key] = value; }
        }
        public override int Count
        {
            get { return sessionData.Count; }
        }
    }
    public static class API_Moq_HttpContext_ExtensionMethods
    {
        public static HttpContextBase httpContext(this API_Moq_HttpContext moqHttpContext)
        {
            return moqHttpContext.HttpContextBase;
        }
        
        public static HttpContextBase request_Write_Clear(this API_Moq_HttpContext moqHttpContext)
        {
            moqHttpContext.MockRequest.Setup(request =>request.InputStream).Returns(new MemoryStream()); 
            
            return moqHttpContext.httpContext();
        }
        
        public static HttpContextBase request_Write(this HttpContextBase httpContextBase,string text)
        {														
            httpContextBase.stream_Write(httpContextBase.Request.InputStream, text);			
            return httpContextBase;
        }
                
        public static string request_Read(this HttpContextBase httpContextBase)
        {					
            return httpContextBase.stream_Read(httpContextBase.Request.InputStream);
        }
        
        public static HttpContextBase response_Write(this HttpContextBase httpContextBase,string text)
        {														
            httpContextBase.stream_Write(httpContextBase.Response.OutputStream, text);			
            return httpContextBase;
        }
        
        public static string response_Read(this HttpContextBase httpContextBase)
        {					
            return httpContextBase.stream_Read(httpContextBase.Response.OutputStream);						
        }
        
        public static string response_Read_All(this HttpContextBase httpContextBase)
        {
            httpContextBase.Response.OutputStream.Flush();
            httpContextBase.Response.OutputStream.Position = 0;
            return httpContextBase.response_Read();
        }
        
        public static HttpContextBase stream_Write(this HttpContextBase httpContextBase, Stream inputStream, string text)
        {														
            var streamWriter = new StreamWriter(inputStream);
            
            inputStream.Position = inputStream.property("Length").str().toInt();  
            //inputStream.Position = (int)inputStream.Length; // the line above can also be this
            streamWriter.Write(text);    
            streamWriter.Flush(); 			
            inputStream.Position = 0; 			
            
            return httpContextBase;
        }
        
        public static string stream_Read(this HttpContextBase httpContextBase, Stream inputStream)
        {								
            var originalPosition = inputStream.Position;
            var streamReader = new StreamReader(inputStream);
            var requestData = streamReader.ReadToEnd();	
            inputStream.Position = originalPosition;
            return requestData;
        }
    }
}        
