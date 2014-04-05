using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using NUnit.Framework;
using FluentSharp.CSharpAST;
namespace TeamMentor.UnitTests.TM_Website.WebServices
{    
    public enum Allowed_User
    {
        Admin     = 4, 
        Editor    = 3, 
        Reader    = 2,
        Anonymous = 1
    }
    public class SecurityMapping
    {
        public string        MethodName        { get; set; }
        public object[]      MethodParameters  { get; set; }
        public Allowed_User  AllowedUser       { get; set; }        
    }

    public static class SecurityMapping_ExtensionMethods
    {
        public static List<SecurityMapping> add_Mapping(this List<SecurityMapping> securityMappings, Allowed_User allowedUser , string methodName, object[] methodParameters)
        {
            securityMappings.Add(new SecurityMapping
                {
                    MethodName          = methodName,
                    MethodParameters    = methodParameters,
                    AllowedUser         = allowedUser
                });
            return securityMappings;
        }
    }

    [TestFixture]
    public class Test_WSDL_Generated_Reference : TestFixture_WebServices
    {
        public List<SecurityMapping> securityMappings { get; set; }
        public Type                  WebServices_BaseType;

        public Test_WSDL_Generated_Reference()
        {
            WebServices_BaseType = webServices.type().BaseType;
            securityMappings = new  List<SecurityMapping>();
            securityMappings.add_Mapping(Allowed_User.Reader    , "VirtualArticle_CreateArticle_from_ExternalServiceData" , new object[] { null,null});
            securityMappings.add_Mapping(Allowed_User.Anonymous , "getGuidForMapping"                                     , new object[] { null     });

            securityMappings.add_Mapping(Allowed_User.Anonymous , "Ping"                                                  , new object[] { null     });
        }
        
        [SetUp]
        public void setup()
        {
            webServices.logout();
        }

        //Check Security
        [Test] public void Methods_AvailableTo_Anonymous()
        {            
            //check privileges for Anonymous
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ReadArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers ());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());
                       
            //check webMethods
            checkSecurityMappings(Allowed_User.Anonymous);
        }

        [Test] public void Methods_AvailableTo_Reader()
        {
            Assert.IsNotNull(this.login_As_Reader());

            //check privileges for Reader
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());

            //check webMethods             
            checkSecurityMappings(Allowed_User.Reader);
        }

        [Test] public void Methods_AvailableTo_Editor()
        {
            Assert.IsNotNull(this.login_As_Editor());
            
            //check privileges for Editor
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());
            
            //check webMethods
            checkSecurityMappings(Allowed_User.Editor);            
        }

        [Test] public void Methods_AvailableTo_Admin()
        {
            Assert.IsNotNull(this.login_As_Admin());

            //check privileges for Admin
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ReadArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_EditArticles());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_ManageUsers());
            Assert.DoesNotThrow         (() => webServices.RBAC_Demand_Admin       ());
            
            //check webMethods
            checkSecurityMappings(Allowed_User.Admin);
        }

        //helper methods
        public void checkSecurityMappings(Allowed_User allowedUser)
        {
            foreach(var securityMapping in securityMappings)
            {
                var targetMethod     = WebServices_BaseType.method(securityMapping.MethodName);
                var methodParameters = securityMapping.MethodParameters;   
                             

                if (securityMapping.AllowedUser <= allowedUser)
                    Assert.DoesNotThrow                     (()=> targetMethod.Invoke(webServices,methodParameters), "On method: '{0}' for user '{1}'".format(targetMethod, allowedUser));
                else
                    Assert.Throws<TargetInvocationException>(()=> targetMethod.Invoke(webServices,methodParameters), "On method: '{0}' for user '{1}'".format(targetMethod, allowedUser));
            }
        }
    }
}
