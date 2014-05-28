using System;
using System.Security;
using System.Web;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_HandleRequest_LibraryData : TM_XmlDatabase_InMemory
    {
        public API_Moq_HttpContext  moqHttpContext;
        public HttpContextBase      context;
        public HandleUrlRequest     handleUrlRequest;
        public Guid                 userSessionId;

        public Test_HandleRequest_LibraryData()
        {
            var testUser  = "user".add_RandomLetters(10);                       
            var testPwd   = "!!Pwd".add_RandomLetters(10);
            userData.newUser(testUser, testPwd).tmUser();                           //create test user     
            userSessionId = userData.login(testUser, testPwd);
            Assert.AreNotEqual(userSessionId,Guid.Empty);
            "Current User SessionID: {0}".info(userSessionId);
        }

        [SetUp]
        public void Setup()
        {
            moqHttpContext   = new API_Moq_HttpContext();
            context          = HttpContextFactory.Context = moqHttpContext.httpContext();		//clean http request for each test                       
            Assert.NotNull(context.Session);
            context.Session["SessionID"] = userSessionId;
            handleUrlRequest = new HandleUrlRequest();
        }

        [Ignore("TO FIX (Refactor Side Effect")]
        [Test][Assert_Editor]
        public void GetContentForRandomGuid_CheckThrowOf_ThreadAborted()
        {
            UserGroup.Editor.assert();

            var guid = Guid.NewGuid();       
            // the throws should all be'Thread was being aborted
            Assert.Throws<Exception>(() => handleUrlRequest.handleRequest("content" , guid.str()), "content");            
            Assert.Throws<Exception>(() => handleUrlRequest.handleRequest("raw"     , guid.str()), "raw");            
            Assert.Throws<Exception>(() => handleUrlRequest.handleRequest("xml"     , guid.str()), "xml");
            Assert.Throws<Exception>(() => handleUrlRequest.handleRequest("html"    , guid.str()), "html");
            Assert.DoesNotThrow     (() => handleUrlRequest.handleRequest("xsl"     , guid.str()), "xsl");
            Assert.Throws<Exception>(() => handleUrlRequest.handleRequest("jsonp"   , guid.str()), "jsonp");
            Assert.Throws<Exception>(() => handleUrlRequest.handleRequest("article" , guid.str()), "article");            
        }
    }
}
