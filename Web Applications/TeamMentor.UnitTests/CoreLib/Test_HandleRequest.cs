using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_HandleRequest
    {
        public API_Moq_HttpContext  moqHttpContext;
        public HttpContextBase      context;
        public HandleUrlRequest     handleUrlRequest;
        

        [SetUp]
        public void Setup()
        {
            moqHttpContext   = new API_Moq_HttpContext();
            context          = HttpContextFactory.Context = moqHttpContext.httpContext();		//clean http request for each test            
            handleUrlRequest = new HandleUrlRequest();
        }

        [Test]
        public void TestMultipleActionInvokes()
        {
            
            // ReSharper disable InconsistentNaming 
            var result_TeamMentor = handleUrlRequest.handleRequest("TeamMentor", "");
            var result_Teaamentor = handleUrlRequest.handleRequest("Teammentor", "");            
            var result_teammentor = handleUrlRequest.handleRequest("TeamMentor", "");
            var result_AAABBBCCC  = handleUrlRequest.handleRequest("AAABBBCCC", "");
            
            Assert.IsFalse(result_TeamMentor, "result_TeamMentor");
            Assert.IsFalse(result_Teaamentor, "result_Teaamentor");
            Assert.IsFalse(result_teammentor, "result_teammentor");
            Assert.IsFalse(result_AAABBBCCC , "result_AAABBBCCC");
            // ReSharper restore InconsistentNaming

            //handleUrlRequest.handleRequest("TeamMentor", "");            
        }

        [Test]
        public void TestRedirectToLoginPage()
        {
            handleUrlRequest.redirectTo_Login();
            var redirecting = context.Response.IsRequestBeingRedirected;
            Assert.IsTrue   (redirecting, "redirecting");
            Assert.AreEqual ("/Login",context.Response.RedirectLocation,"Login redirect location");

            Setup();
            redirecting       = context.Response.IsRequestBeingRedirected;
            Assert.IsFalse    (redirecting, "redirecting after Setup");
            Assert.AreNotEqual("/Login",context.Response.RedirectLocation,"Login redirect location, after Setup");                        
        }

        [Test]
        public void CheckRedirectionOnAdminFunction()
        {
            var targetUrl = "https://localhost/virtualarticles".uri();
            var expectedRedirect = "/Html_Pages/Gui/Pages/login.html?LoginReferer=/virtualarticles";

            moqHttpContext.RequestUrl = targetUrl;
            handleUrlRequest.routeRequestUrl();
            var redirecting     = context.Response.IsRequestBeingRedirected;
            Assert.IsTrue   (redirecting                                        , "redirecting after call to Admin method");
            Assert.AreEqual (expectedRedirect,context.Response.RedirectLocation ,"Login redirect location,  after call to Admin");
        }

        [Test]
        public void CheckServerTransferOnPasswordReset()
        {
            var resetToken = Guid.NewGuid().str();
            handleUrlRequest.handleRequest("passwordreset", resetToken);
            var expectedRedirect = "/Html_Pages/Gui/Pages/passwordReset.html";
            Assert.AreEqual (expectedRedirect,context.Response.RedirectLocation ,"Password redirect location");
        }
        [Test]
        public void Check_ServerTransfers()
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

        [Test]
        public void Check_ResponseRedirects()
        {
            var responseRedirects = HandleUrlRequest.Response_Redirects;            
            Assert.IsNotEmpty(responseRedirects);
            foreach (var mapping in responseRedirects)
            {
                context.Response.Redirect("");
                Assert.IsFalse(context.Response.IsRequestBeingRedirected);                
                handleUrlRequest.handleRequest(mapping.Key, "");
                "{0} -> {1} : {2}".info(mapping.Key, mapping.Value, context.Response.RedirectLocation);
                Assert.IsTrue(context.Response.IsRequestBeingRedirected , "IsRequestBeingRedirected");                
                Assert.AreEqual(mapping.Value, context.Response.RedirectLocation);                
            }
        }
    }
}
