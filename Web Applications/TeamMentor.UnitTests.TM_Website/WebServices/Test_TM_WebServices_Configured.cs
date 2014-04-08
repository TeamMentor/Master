using System;
using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.WebServices
{
    [TestFixture]
    public class Test_TM_WebServices_Configured
    {
        TM_WebServices_Configured webServices;
        TestFixture_WebServices   tfWebServices;

        [SetUp] public void setup()
        {
            tfWebServices = new TestFixture_WebServices();          // this will check if the current test server is online
            webServices = tfWebServices.webServices;
            
            Assert.IsNotNull(webServices);
        }            

        [Test] public void TM_WebServices_Configured_Ctor() 
        {
            Assert.IsNull(webServices.webRequest);
            Assert.IsNull(webServices.soapClientMessage);
            Assert.IsNull(webServices.webResponse);
            Assert.IsNull(webServices.responseXmlReader);
            Assert.IsNull(webServices.responseData);
        }
        [Test] public void captureResponseData  ()          
        {
            Assert.IsTrue     (webServices.Capture_ResponseData);
            Assert.AreEqual   (webServices.Ping(10.randomLetters()), webServices.responseData.first());
            
            webServices.Capture_ResponseData = false;

            Assert.AreNotEqual(webServices.Ping(10.randomLetters()), webServices.responseData.first());
            Assert.IsNull     (webServices.responseData.first());
        }
        [Test] public void getResponseData()                
        {
            var pong            = webServices.Ping(10.randomLetters());
            var pong_Response   = webServices.getResponseData<string>();
            Assert.IsNotNull           (pong       );
            Assert.IsNotNull           (pong_Response);
            Assert.IsInstanceOf<String>(pong );
            Assert.IsInstanceOf<String>(pong_Response);
            Assert.AreEqual            (pong_Response, pong);

            var sessionId          = webServices.Current_SessionID();
            var sessionId_Response = webServices.getResponseData<Guid>();
            
            Assert.IsNotNull(sessionId);
            Assert.IsNotNull(sessionId_Response       );
            Assert.IsInstanceOf<Guid>(sessionId );
            Assert.IsInstanceOf<Guid>(sessionId_Response);
            Assert.AreEqual          (sessionId, sessionId_Response);
            Assert.AreEqual          (sessionId, Guid.Empty);

            Assert.IsNull(webServices.getResponseData<TM_User>());
        }
        [Test] public void captureCurrentUser()             
        {
            Assert.IsTrue (webServices.Capture_CurrentUser);
            
            webServices.Ping(10.randomLetters());

            Assert.IsNull (webServices.captureCurrentUser());

            //login
            Assert.NotNull(tfWebServices.login_As_Admin());

            var currentUser        = webServices.Current_User();
            var userName           = currentUser.UserName;
            var userId             = currentUser.UserId;                        
            var currentUser_as_Xml = currentUser.toXml();

            Assert.IsNotNull(currentUser);
            Assert.AreEqual (currentUser_as_Xml, webServices.getResponseData<TM_User>().toXml());

            Assert.IsNotNull(webServices.Cached_CurrentUser);
            Assert.AreEqual (currentUser_as_Xml, webServices.Cached_CurrentUser.toXml());

            //if the currentUserCapture is enabled these methods should work                        
            Assert.IsTrue    (webServices.RBAC_IsAdmin()                                      );
            Assert.AreEqual  (webServices.Current_User()          .toXml(), currentUser_as_Xml);
            Assert.AreEqual  (webServices.GetUser_byName(userName).toXml(), currentUser_as_Xml);
            Assert.AreEqual  (webServices.GetUser_byID(userId)    .toXml(), currentUser_as_Xml);
            Assert.IsNotEmpty(webServices.GetUser_AuthTokens(userId));          
  
            //logout
            Assert.IsTrue(webServices.logout()          );
            Assert.IsNull(webServices.Cached_CurrentUser);
            Assert.IsNull(webServices.Current_User()    );
            Assert.IsNull(webServices.Cached_CurrentUser);

            //after logout these should throw an exception (or not work)                        
            Assert.Throws<SoapException>(()=>webServices.GetUser_byName(userName).toXml());
            Assert.Throws<SoapException>(()=>webServices.GetUser_byID(userId).toXml()    );
            Assert.Throws<SoapException>(()=>webServices.GetUser_AuthTokens(userId)      );            
        }
        [Test] public void currentSoapAction()              
        {
            Action<string, Action> checkSoapAction = (expectedSoapAction,callWebMethod) =>
                {
                    if (callWebMethod.notNull())
                        callWebMethod();
                    Assert.AreEqual(expectedSoapAction, webServices.currentSoapAction());
                };

            checkSoapAction(null, null);
            checkSoapAction("Ping"              , ()=> webServices.Ping("")           );
            checkSoapAction("Current_User"      , ()=> webServices.Current_User()     );
            checkSoapAction("Current_SessionID" , ()=> webServices.Current_SessionID());
            
            webServices.webRequest.Headers["SOAPAction"] = null;

            Assert.IsNull   (webServices.currentSoapAction());
        }

        //workflows
        [Test] public void Make_Request_Ping()              
        {
            var message         = 10.randomLetters();
            var pong            = webServices.Ping(message);
            var expectedMessage = "received ping: {0}".format(message);
            Assert.AreEqual(expectedMessage, pong);

            Assert.IsNotNull(webServices.webRequest);
            Assert.IsNotNull(webServices.soapClientMessage);
            Assert.IsNotNull(webServices.webResponse);            
            Assert.IsNotNull(webServices.responseXmlReader);
            Assert.IsNotNull(webServices.responseData);
        }        
    }
}
