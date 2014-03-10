using System;
using System.Web;
using NUnit.Framework;
using FluentSharp.CoreLib;
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
            Assert.IsNotNull(HttpContextFactory.Context					     , "Context");
            Assert.IsNotNull(HttpContextFactory.Context.Request			     , "Request");
            Assert.IsNotNull(HttpContextFactory.Context.Request.Headers      , "Request.Headers");
            Assert.IsNotNull(HttpContextFactory.Context.Response		     , "Response");
            Assert.IsNotNull(HttpContextFactory.Context.Response.Headers     , "Response.Headers");
            Assert.IsNotNull(HttpContextFactory.Context.Server		     	 , "Server");
            Assert.IsNotNull(HttpContextFactory.Context.Session			     , "Session");
            Assert.IsTrue   (MiscUtils.runningOnLocalHost()                  , "runningOnLocalHost");
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
            Assert.AreEqual         (string.Empty, context.Response.RedirectLocation, "before Transfer");
            Assert.Throws<Exception>(()=>context.Server.Transfer(transferTarget));                          //Server.Transfer throws exception after configuring the transfer
            Assert.IsNotNull        (context.Response.RedirectLocation, "after Transfer");
            Assert.AreEqual         (transferTarget, context.Response.RedirectLocation);
        }
        [Test] public void Test_Session()
        {            
            var session       = context.Session;
            var sessionId     = session.SessionID;
            var sessionKey1   = "SessionID";
            var sessionKey2   = "SessionID".add_RandomLetters(10);
            var sessionValue1 = Guid.NewGuid();
            var sessionValue2 = "A value".add_RandomLetters(10);

            Assert.IsNotNull  (session             , "session was null");
            Assert.AreEqual   (session.size(),0    , "session size");
            Assert.IsNull     (session[sessionKey1], "sessionKey1 value before set");
            Assert.IsNull     (session[sessionKey2], "sessionKey2 value before set");

            session[sessionKey1] = sessionValue1;
            session[sessionKey2] = sessionValue2;
            var realValue1       = session[sessionKey1]; 
            var realValue2       = session[sessionKey2];

            Assert.AreEqual   (session.size(), 2        , "session size after set");
            Assert.AreEqual   (sessionValue1, realValue1, "sessionKey1 value after set");
            Assert.AreEqual   (sessionValue2, realValue2, "sessionKey2 value after set");
            Assert.AreNotEqual(realValue1   , realValue2, "sessionKey2 value after set");
            Assert.AreNotEqual(realValue1   , sessionId);
            Assert.AreNotEqual(realValue2   , sessionId);

            Assert.IsInstanceOf<Guid>   (realValue1, "realValue1 should be a GUID");
            Assert.IsInstanceOf<string> (realValue2, "realValue2 should be a string");




        }
    }
}
