using System;
using System.IO;
using System.Security;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.REST;

namespace TeamMentor.UnitTests.REST_Direct
{
    [TestFixture]
    public class Test_TBot  : TM_Rest_Direct
    {        
        public Test_TBot()
        {
            var assembly		    = this.type().Assembly;
            var dllLocation		    = assembly.CodeBase.subString(8);
            var webApplications     = dllLocation.parentFolder().pathCombine(@"\..\..\..");
            var tmWebsite 		    = webApplications.pathCombine("TM_Website");
            var tmConfig            = tmWebsite.pathCombine("TmConfig.config");
            moq_HttpContext.BaseDir = tmWebsite;
            Assert.IsTrue(tmConfig.fileExists(), "couldn't find tmconfig file at: {0}".format(tmConfig));
        }

        [Test]
        public void RedirectToLoginOnNoAdmin()
        {
            var response = HttpContextFactory.Response;
            Assert.IsFalse  (response.IsRequestBeingRedirected);
            Assert.AreEqual ("", response.RedirectLocation);            
            Assert.Throws<Exception>(()=>TmRest.TBot_Show());
            Assert.IsTrue   (response.IsRequestBeingRedirected);
            Assert.AreEqual ("/Login?LoginReferer=/tbot", response.RedirectLocation);            
        }

        [Test][Assert_Admin]
        public void TbotMainPage()
        {
            var showTbotHtml     = TmRest.TBot_Show().cast<MemoryStream>().ascii();            
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBot_Brain.TBOT_MAIN_HTML_PAGE);
            Assert.IsNotNull(tbotMainHtmlFile             , "tbotMainHtmlFile was null");
            Assert.IsTrue   (tbotMainHtmlFile.fileExists(), "tbotMainHtmlFile didn't exist");
            Assert.IsNotNull(showTbotHtml);
            Assert.IsTrue   (showTbotHtml.contains("bootstrap.min.css"), "Couldn't find bootstrap.min.css");            
        }

        [Test]
        [Assert_Admin]
        public void EmailMessages()
        {
            var tbotBrain = new TBot_Brain(TmRest);
            var html = tbotBrain.ExecuteRazorPage("View_Emails_Sent");
            Assert.IsNotNull(html);
            html.info();
            //"test webBrowser".popupWindow().add_WebBrowser().set_Html(html).waitForClose();
        }
    }
}
