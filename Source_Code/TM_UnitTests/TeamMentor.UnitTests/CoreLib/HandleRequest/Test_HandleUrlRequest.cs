using System;
using System.Web;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_HandleUrlRequest
    {
        public HttpContextBase      context;
        public HandleUrlRequest     handleUrlRequest;
         [SetUp]
        public void Setup()
        {            
            context       = HttpContextFactory.Context.mock();
            handleUrlRequest = new HandleUrlRequest();
        }
        //Extension Methods
        [Test]
        public  void handleUrlRewrite__String()
        {                        
            Assert.IsTrue   (handleUrlRequest.handleUrlRewrite( "login"));            
            Assert.IsFalse  (handleUrlRequest.handleUrlRewrite( "AAABBCC"));            
            Assert.IsFalse  (handleUrlRequest.handleUrlRewrite(null as string));                                    
        }
        //Extension Methods
        [Test]
        public  void handleUrlRewrite__Uri()
        {                                         
            Assert.IsTrue   (handleUrlRequest.handleUrlRewrite( "http://aaa/login".uri()));            
            Assert.IsFalse  (handleUrlRequest.handleUrlRewrite( "http://aaa/AABBCC".uri()));    
            Assert.IsFalse  (handleUrlRequest.handleUrlRewrite( "login".uri()));   
            Assert.IsFalse  (handleUrlRequest.handleUrlRewrite(null as Uri));   

            // check that the request.Url.AbsolutePath can also be used to define the absolutePath value
            handleUrlRequest.request.field("_url", "http://aaa/login".uri());
            Assert.IsTrue  (handleUrlRequest.handleUrlRewrite(null as Uri));   
        }
        [Test]
        public void shouldSkipCurrentRequest()
        {            
            Assert.IsFalse  (handleUrlRequest.shouldSkipCurrentRequest());  
            
            Action<string, bool> testPath = (path,result)=>
                {
                            
                    handleUrlRequest.request.field("_physicalPath", path);                      // shouldSkipCurrentRequest uses the _physicalPath value
                    Assert.AreEqual(handleUrlRequest.request.PhysicalPath, path);               // check if value was set correctly
                    Assert.AreEqual(handleUrlRequest.shouldSkipCurrentRequest(), result);       // check result
                };

            testPath(@"c:\a.js"  , false);
            testPath(@"c:\a.asmx", true);
            testPath(@"c:\a.ashx", true);
            testPath(@"c:\a.aspx", true);

        }

    }
}
