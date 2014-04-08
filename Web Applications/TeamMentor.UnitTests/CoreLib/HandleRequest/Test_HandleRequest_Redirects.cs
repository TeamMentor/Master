using System;
using System.Collections.Generic;
using System.Security;
using System.Web;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_HandleRequest_Redirects
    {        
        public HttpContextBase      context;
        public HandleUrlRequest     handleUrlRequest;
        
        [SetUp]
        public void Setup()
        {
            //moqHttpContext   = new API_Moq_HttpContext();
            //context          = HttpContextFactory.Context = moqHttpContext.httpContext();		//clean http request for each test            
            context            = HttpContextFactory.Context.mock();
            handleUrlRequest = new HandleUrlRequest();
        }

        [Test] public void transfer_Request()
        {
            Assert.IsFalse(context.Response.IsRequestBeingRedirected);
            handleUrlRequest.transfer_Request(null);
            Assert.IsFalse(context.Response.IsRequestBeingRedirected);
            handleUrlRequest.transfer_Request("abbcccddd");
            Assert.IsFalse(context.Response.IsRequestBeingRedirected);

            // test rediret to teammentor
    //        Assert.Throws<Exception>(()=> handleUrlRequest.transfer_Request("teammentor"));
    //        Assert.AreEqual(HandleUrlRequest.Server_Transfers["teammentor"], context.Response.RedirectLocation);                
    //        Assert.IsTrue(context.Response.IsRequestBeingRedirected);

            // test rediret to a null mapping
            var nullTransferKey = "nullTransfer".add_RandomLetters(10).lower();
            HandleUrlRequest.Server_Transfers.add   (nullTransferKey,null);
            Assert.IsTrue                           (HandleUrlRequest.Server_Transfers.hasKey(nullTransferKey));
            Assert.Throws<Exception>                (()=> handleUrlRequest.transfer_Request(nullTransferKey));
            Assert.AreEqual                         (null, context.Response.RedirectLocation);                
            HandleUrlRequest.Server_Transfers.remove(nullTransferKey);
            Assert.IsFalse                          (HandleUrlRequest.Server_Transfers.hasKey(nullTransferKey));

        }
        //workflows
        [Test] public void Check_RedirectToLoginPage()              
        {            
            handleUrlRequest.handleRequest("login","");            
            Assert.IsTrue   (context.Response.IsRequestBeingRedirected, "redirecting");
            Assert.AreEqual ("/Html_Pages/Gui/Pages/login.html?LoginReferer=/",context.Response.RedirectLocation,"Login redirect location");

            Setup();        // run setup again and ensure that values have been reset
            
            Assert.IsFalse    (context.Response.IsRequestBeingRedirected, "redirecting after Setup");
            Assert.AreNotEqual("/Login",context.Response.RedirectLocation,"Login redirect location, after Setup");                        
        }
        [Test] public void Check_RedirectionOnAdminFunction()       
        {
            var targetUrl = "https://localhost/virtualarticles".uri();
            var expectedRedirect = "/Html_Pages/Gui/Pages/login.html?LoginReferer=/virtualarticles";

            context.Request.field("_url", targetUrl);
            Assert.AreEqual(context.Request.Url, targetUrl);
            handleUrlRequest.routeRequestUrl();
            var redirecting  = context.Response.IsRequestBeingRedirected;
            Assert.IsTrue   (redirecting                                        , "redirecting after call to Admin method");
            Assert.AreEqual (expectedRedirect,context.Response.RedirectLocation ,"Login redirect location,  after call to Admin");
        }
        [Test] public void Check_ServerTransferOnPasswordReset()    
        {
            var resetToken = Guid.NewGuid().str();
            handleUrlRequest.handleRequest("passwordreset", resetToken);
            var expectedRedirect = "/Html_Pages/Gui/Pages/passwordReset.html";
            Assert.AreEqual (expectedRedirect,context.Response.RedirectLocation ,"Password redirect location");
        }
        [Test] public void Check_ServerTransfers()                  
        {
            var serverTransfers = HandleUrlRequest.Server_Transfers;
            
            Assert.IsNotEmpty(serverTransfers);
            foreach (var mapping in serverTransfers)
            {
                handleUrlRequest.handleRequest(mapping.Key, "");
                "{0} -> {1} : {2}".info(mapping.Key, mapping.Value, context.Response.RedirectLocation);
                Assert.AreEqual(mapping.Value, context.Response.RedirectLocation);                
            }            
        }
        [Test] public void Check_ResponseRedirects()                
        {
            var responseRedirects = HandleUrlRequest.Response_Redirects;            
            Assert.IsNotEmpty(responseRedirects);
            foreach (var mapping in responseRedirects)
            {
                Assert.Throws<Exception>(()=> context.Response.Redirect(""));   // (Redirect throws exception after redirection is set)
                Assert.IsFalse(context.Response.IsRequestBeingRedirected);                
                handleUrlRequest.handleRequest(mapping.Key, "");
                "{0} -> {1} : {2}".info(mapping.Key, mapping.Value, context.Response.RedirectLocation);
                Assert.IsTrue(context.Response.IsRequestBeingRedirected , "IsRequestBeingRedirected");                
                Assert.AreEqual(mapping.Value, context.Response.RedirectLocation);                
            }
        }
        [Test] public void Check_Redirect_Security_OkRedirects()    
        {            
            var targetServer    = "http://x.y.z";            
            var okRedirects = new Dictionary<string, string>()      
                                         //   requestUrl                   expected redirect
                                        .add("/test"                    ,"/test")
                                        .add("/test/123"                ,"/test/123")
                                        .add("/test?param=12"           ,"/test?param=12")
                                        .add("//test"                   ,"/test")                                                                                
                                        .add("/newline_\x0a_\n char0_\0","/newline_\n_\n char0_\0")
                                        .add("/newline_".line()         ,"/newline_\r\n")
                                        .add("aaa/bbb"                  ,"/")
                                        .add("http://www.google.com"    ,"/")
                                        .add("//www.google.com"         ,"/www.google.com");
            
            var request  = context.Request;
            var response = context.Response;
            
            request.field("_url", targetServer.append("/some/path").uri());                      
            
            foreach (var item in okRedirects)
            {         
                Assert.Throws<Exception>(()=> response.Redirect(""));               // reset redirection (Redirect throws exception after redirection is set)
                Assert.IsFalse (response.IsRequestBeingRedirected);
                request.QueryString["LoginReferer"] = item.Key;                     // set redirection target
                Assert.Throws<Exception>(()=>handleUrlRequest.handle_LoginOK());    // trigger redirect  (Redirect throws exception after redirection is set)                             
                Assert.IsTrue  (response.IsRequestBeingRedirected);       
                Assert.AreEqual(targetServer + item.Value,response.RedirectLocation, 
                                "response.RedirectLocation for: {0}".format(item.Value));                                
            }
        }
        [Test] public void Check_Redirect_Security_FailedRedirects()
        {
            var targetServer    = "http://x.y.z";            
            var failedRedirects = new List<string>
                                        {
                                            "/test<h1>xss</h1>",                                            
                                            "<",">", "&", "'", "\"" 
                                        };
                                                                                               
            var request               = context.Request;
            var response              = context.Response;
            request.field("_url", targetServer.append("/some/path").uri());                   

            Assert.IsFalse  (response.IsRequestBeingRedirected);  
             
            foreach (var item in failedRedirects)
            {
                request.QueryString["LoginReferer"] = item;                  // set redirection target
                Assert.DoesNotThrow(()=>handleUrlRequest.handle_LoginOK(), "didn't fail for: " + item);  // trigger redirect     
                Assert.IsFalse  (response.IsRequestBeingRedirected       , "redirected for: " + item);                       
            }
        }
    }
}
