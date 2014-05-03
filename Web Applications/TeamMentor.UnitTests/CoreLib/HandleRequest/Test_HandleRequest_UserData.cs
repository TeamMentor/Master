using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    [TestFixture]
    public class Test_HandleRequest_UserData : TM_WebServices_InMemory
    {
        public HttpContextBase      context;
        //public HttpResponseBase     response;
        public HandleUrlRequest     handleRequest;
        
        [SetUp]
        public void Setup()
        {            
            context       = HttpContextFactory.Context.mock();
            //response         = context.Response;
            handleRequest = new HandleUrlRequest();
            handleRequest.tmWebServices = tmWebServices;
        }

        [Test]
        public void whoAmI()
        {   
            var whoAmI      = tmWebServices.tmAuthentication.currentUser.whoAmI();
            var whoAmI_Json = whoAmI.json();

            Assert.NotNull(whoAmI_Json);
            Assert.AreEqual(context.response_Read(), "");
            handleRequest.showWhoAmI();

            var responseOut = context.response_Read();
            var whoAmI_ReponseOut = responseOut.json_Deserialize<WhoAmI>();
            Assert.AreEqual( responseOut, whoAmI_Json);
            Assert.AreEqual( whoAmI.UserName, whoAmI_ReponseOut.UserName);
            Assert.AreEqual( whoAmI.UserId  , whoAmI_ReponseOut.UserId);
            Assert.AreEqual( whoAmI.GroupId , whoAmI_ReponseOut.GroupId);
        }

        //workflows
        [Test]
        public void Test_Ability_to_Read_Response_Writes()
        {
            var value1 = 10.randomLetters();
            var value2 = 10.randomLetters();
            Assert.AreEqual(context.response_Read(), "");
            context.response_Write(value1);
            Assert.AreEqual(context.response_Read(), value1);
            context.Response.Write(value2);
            Assert.AreEqual(context.response_Read(), value1 + value2);
        }
    }
}
