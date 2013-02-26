using System.Web;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    [TestFixture]
    public class Test_API_Moq_HttpContext
    {
        public API_Moq_HttpContext  moqHttpContext;
        public HttpContextBase      context;

        [SetUp]
        public void Setup()
        {
            moqHttpContext = new API_Moq_HttpContext();
            context = HttpContextFactory.Context = moqHttpContext.httpContext();		
        }

        [Test] public void Test_HttpContext_CoreObjectsExist()
        {
            Assert.IsNotNull(HttpContextFactory.Context					, "Context");
            Assert.IsNotNull(HttpContextFactory.Context.Request			, "Request");
            Assert.IsNotNull(HttpContextFactory.Context.Request.Headers , "Request.Headers");
            Assert.IsNotNull(HttpContextFactory.Context.Response		, "Response");
            Assert.IsNotNull(HttpContextFactory.Context.Response.Headers, "Response.Headers");
            Assert.IsNotNull(HttpContextFactory.Context.Server			, "Server");
            Assert.IsNotNull(HttpContextFactory.Context.Session			, "Session");
        }
        [Test] public void Test_Http_Headers()
        {
            //var request  = HttpContextFactory.Context.Request;
            var response = HttpContextFactory.Context.Response;

            var request_Headers  = HttpContextFactory.Context.Request.Headers;
            var response_Headers = HttpContextFactory.Context.Response.Headers;
            Assert.AreEqual(0, request_Headers.size() , "request_Headers not empty");
            Assert.AreEqual(0, response_Headers.size(), "response_Headers not empty");
            var name  = "Name" .add_RandomLetters();
            var value = "Value".add_RandomLetters();
            response.AddHeader(name, value);			
            Assert.AreEqual(1, response_Headers.size() , "response_Headers should had 1 key");
            var firstKey = response_Headers.Keys.first().str();
            Assert.AreEqual(name, firstKey						, "first Name");			
            Assert.AreEqual(value, response_Headers[firstKey]	, "first Value");
        }
        [Test] public void Test_RequestVariables()
        {
            var request = context.Request;
            var testUri = "http://1.1.1.1/test".uri();

            moqHttpContext.RequestUrl = testUri;
            var isLocal               = request.IsLocal;
            var isSecureConnection    = request.IsSecureConnection;

            Assert.AreEqual(testUri, context.Request.Url);
            Assert.IsFalse(isLocal           , "isLocal");
            Assert.IsFalse(isSecureConnection, "isSecureConnection");

            moqHttpContext.RequestUrl = "https://localhost/test".uri();
            isLocal                   = request.IsLocal;
            isSecureConnection        = request.IsSecureConnection;

            Assert.IsTrue(isLocal           , "isLocal 2");
            Assert.IsTrue(isSecureConnection, "isSecureConnection 2");
        }

        [Test] public void Test_ServerTransfer()
        {
            var transferTarget = "/a/page.html";
            Assert.AreEqual(string.Empty, context.Response.RedirectLocation, "before Transfer");
            context.Server.Transfer(transferTarget);
            Assert.IsNotNull(context.Response.RedirectLocation, "after Transfer");
            Assert.AreEqual(transferTarget, context.Response.RedirectLocation);
        }
    }
}
