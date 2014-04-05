using System.Web.Services.Protocols;
using FluentSharp.CoreLib;
using NUnit.Framework;
using FluentSharp.CSharpAST;
namespace TeamMentor.UnitTests.TM_Website.WebServices
{    
    [TestFixture]
    public class Test_WSDL_Generated_Reference : TestFixture_WebServices
    {

        //Check Security
        [Test] public void Methods_AvailableTo_Anonymous()
        {            
            //check privileges for Anonymous
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ReadArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_EditArticles());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_ManageUsers ());
            Assert.Throws<SoapException>(() => webServices.RBAC_Demand_Admin       ());
            
            //check webMethods
            Assert.Throws<SoapException>(() => webServices.VirtualArticle_CreateArticle_from_ExternalServiceData(null,null));            
            Assert.DoesNotThrow         (() => webServices.getGuidForMapping(null));
            
            Assert.DoesNotThrow         (() =>webServices.Ping(10.randomLetters()));

            //Assert.Throws<SoapException>(()=>
            //Assert.DoesNotThrow(()=> );
        }
        //Check Security
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
            Assert.DoesNotThrow         (() => webServices.VirtualArticle_CreateArticle_from_ExternalServiceData(null,null));            
            Assert.DoesNotThrow         (() => webServices.getGuidForMapping(null));
            

            Assert.DoesNotThrow         (() =>webServices.Ping(10.randomLetters()));
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
            Assert.DoesNotThrow         (() => webServices.VirtualArticle_CreateArticle_from_ExternalServiceData(null,null));            
            Assert.DoesNotThrow         (() => webServices.getGuidForMapping(null));
            

            Assert.DoesNotThrow         (() =>webServices.Ping(10.randomLetters()));
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
            Assert.DoesNotThrow         (() => webServices.VirtualArticle_CreateArticle_from_ExternalServiceData(null,null));            
            Assert.DoesNotThrow         (() => webServices.getGuidForMapping(null));
            

            Assert.DoesNotThrow         (() =>webServices.Ping(10.randomLetters()));
        }
    }
}
