// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Collections.Specialized;
using System.Web;
using Moq;
using TeamMentor.CoreLib;


//O2Ref:Moq.dll
//O2Ref:System.Web.Abstractions.dll

namespace FluentSharp.CoreLib
{
    public class API_Moq_HttpContext 
    {        	
        public Mock<HttpContextBase>        MockContext  { get; set; }
        public MockHttpRequest              MockRequest  { get; set; }
        public Mock<HttpResponseBase>       MockResponse { get; set; }
        public MockHttpSession              MockSession  { get; set; }
        public Mock<HttpServerUtilityBase>  MockServer   { get; set; }		
        
        public HttpContextBase HttpContextBase  	{ get; set; }
        public HttpRequestBase HttpRequestBase  	{ get; set; }
        public HttpResponseBase HttpResponseBase  	{ get; set; }  
        
        public String BaseDir						{ get; set; }
        //public Uri    RequestUrl                    { get; set; }   

        public API_Moq_HttpContext() : this(null)
        {			
        }
        
        public API_Moq_HttpContext(string baseDir)
        {
            BaseDir = baseDir;
            //RequestUrl = "http://localhost".uri();
            createBaseMocks();
            setupNormalRequestValues();
        }
        
        public API_Moq_HttpContext createBaseMocks()
        {
            MockContext  = new Mock<HttpContextBase>();
            MockRequest  = new MockHttpRequest(); // new Mock<HttpRequestBase>();
            MockResponse = new Mock<HttpResponseBase>();
            MockSession  = new MockHttpSession();
            MockServer   = new Mock<HttpServerUtilityBase>();
            
    
            MockContext.Setup(ctx => ctx.Request).Returns(MockRequest);
            MockContext.Setup(ctx => ctx.Response).Returns(MockResponse.Object);
            MockContext.Setup(ctx => ctx.Session).Returns(MockSession);
            MockContext.Setup(ctx => ctx.Server).Returns(MockServer.Object);
            
            
            HttpContextBase  = MockContext.Object; 
            HttpRequestBase  = MockRequest;
            HttpResponseBase = MockResponse.Object;
                        
            return this;
        }
        
        public Func<string,string> context_Server_MapPath {get;set;} 
         
        public API_Moq_HttpContext setupNormalRequestValues()		
        {							        
            var genericIdentity = new GenericIdentity("genericIdentity");
            IPrincipal genericPrincipal = new GenericPrincipal(genericIdentity, new string[] {});
            MockContext.Setup(context => context.User).Returns(()=>
                    {
                        return genericPrincipal;
                    });	     	            
            MockContext.SetupSet(context => context.User).Callback((IPrincipal principal)=>
                    {
                        genericPrincipal = principal;
                    });
            MockContext.Setup(context => context.Cache).Returns(HttpRuntime.Cache);            
                 
            //Response
            var outputStream = new MemoryStream();
            var redirectTarget = "";
            var contentType = "";
            MockResponse.SetupGet(response => response.ContentType  ).Returns(()=>
            {
                return contentType;
            });
            MockResponse.SetupSet(response => response.ContentType  ).Callback((string value)=>
            {
                contentType = value;
            });
            MockResponse.SetupGet(response => response.Cache        ).Returns(new Mock<HttpCachePolicyBase>().Object);
            MockResponse.Setup   (response => response.Cookies      ).Returns(new HttpCookieCollection()); 	     	
            MockResponse.Setup   (response => response.Headers      ).Returns(new NameValueCollection());
            MockResponse.Setup   (response => response.OutputStream ).Returns(outputStream);
            MockResponse.Setup   (response => response.Write        (It.IsAny<string>())                    ).Callback((string code)
                =>
                {
                    HttpContextBase.response_Write(code);
                    //outputStream.Write(code.asciiBytes(), 0, code.size());
                });
            MockResponse.Setup   (response => response.AddHeader    (It.IsAny<string>(), It.IsAny<string>())).Callback((string name,string value) => MockResponse.Object.Headers.Add(name,value));
            MockResponse.Setup   (response => response.Redirect     (It.IsAny<string>())                    ).Callback((string target)            =>{ redirectTarget = target; throw new Exception("Thread was being aborted.");});            
            
            MockResponse.Setup   (response => response.IsRequestBeingRedirected ).Returns(() => redirectTarget.valid());
            MockResponse.Setup   (response => response.RedirectLocation         ).Returns(() => redirectTarget);
            
            //Server
            MockServer.Setup(server => server.MapPath (It.IsAny<string>()))                 .Returns ((string path)                      =>  BaseDir.pathCombine(path));
            MockServer.Setup(server => server.Transfer(It.IsAny<string>()))                 .Callback((string target)                    =>  { redirectTarget = target; throw new Exception("Thread was being aborted.");}  );   // use the redirectTarget to hold this value
            MockServer.Setup(server => server.Transfer(It.IsAny<string>(),It.IsAny<bool>())).Callback((string target, bool preserveForm) =>  { redirectTarget = target; throw new Exception("Thread was being aborted.");}  );   // use the redirectTarget to hold this value            
            return this;
        }
    }
    public class MockHttpRequest : HttpRequestBase
    {
        public string _userHostAddress = "127.0.0.1";        
        public string _contentType     = "";        
        public Uri    _url             = "http://localhost".uri();
        public Stream _inputStream     =  new MemoryStream();  
        public string _physicalPath    = null;

        public HttpCookieCollection _cookies           = new HttpCookieCollection();
        public NameValueCollection  _queryString       = new NameValueCollection();
        public NameValueCollection  _form              = new NameValueCollection();
        public NameValueCollection  _headers           = new NameValueCollection();
        public NameValueCollection  _serverVariables   = new NameValueCollection();

        public override string  ContentType         {  get {   return _contentType;           } set { _contentType = value; } }
        public override bool    IsLocal             {  get {   return _url.Host == "localhost" || _url.Host=="127.0.0.1";   } }
        public override bool    IsSecureConnection  {  get {   return _url.Scheme.lower() == "https";                       } }
        public override Uri     Url                 {  get {   return _url;                                                 } }
        public override string  UserHostAddress     {  get {   return _userHostAddress;                                     } }
        public override string  PhysicalPath        {  get {   return _physicalPath;                                                 } }

        public override Stream                InputStream     {  get {   return _inputStream;       } }
        public override HttpCookieCollection  Cookies         {  get {   return _cookies;           } }
        public override NameValueCollection   QueryString     {  get {   return _queryString;       } }
        public override NameValueCollection   Form            {  get {   return _form;              } }
        public override NameValueCollection   Headers         {  get {   return _headers;           } }
        public override NameValueCollection   ServerVariables {  get {   return _serverVariables;   } }

        public override string this[string key]
        {
            get
            {          
                if (_form.Get(key).notNull())
                    return _form.Get(key);
                if (_queryString.Get(key).notNull())
                    return _queryString.Get(key);
                return null;                
            }            
        }             
    }
    public class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string,object> sessionData = new Dictionary<string,object> ();
        string                    sessionId   = "".add_RandomLetters(15);

        public override string SessionID
        {
            get { return sessionId; }
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
        public static HttpContextBase mock(this HttpContextBase contextBase)
        {
            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
            return HttpContextFactory.Context;
        }
        public static HttpContextBase httpContext(this API_Moq_HttpContext moqHttpContext)
        {
            return moqHttpContext.HttpContextBase;
        }
        
        public static HttpContextBase request_Write_Clear(this HttpContextBase httpContextBase)
        {
            httpContextBase.Request.field("_inputStream",new MemoryStream());             
            return httpContextBase;
            //moqHttpContext.MockRequest.Setup(request =>request.InputStream).Returns(new MemoryStream()); 
            //return moqHttpContext.httpContext();
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
            
            //inputStream.Position = inputStream.property("Length").str().toInt();  
            inputStream.Position = (int)inputStream.Length; // the line above can also be this
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
