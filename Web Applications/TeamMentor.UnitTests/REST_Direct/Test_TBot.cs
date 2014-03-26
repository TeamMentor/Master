using System;
using System.IO;
using FluentSharp.Git.APIs;
using FluentSharp.REPL;
using NUnit.Framework;
using FluentSharp.CoreLib;
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

        [Test][Assert_Admin] public void TBot_Run()
        {
            var commandsHtml     = TmRest.TBot_Run("commands").cast<MemoryStream>().ascii();            
            Assert.NotNull(commandsHtml);
        }
        [Test][Assert_Admin] public void TBot_Render()
        {
            var commandsHtml   = TmRest.TBot_Run("commands").cast<MemoryStream>().ascii().lower();            
            var commandsRender = TmRest.TBot_Render("commands").cast<MemoryStream>().ascii().lower();                        
            Assert.NotNull    (commandsHtml);
            Assert.NotNull    (commandsRender);            

            Assert.AreNotEqual(commandsHtml, commandsRender);
            Assert.IsTrue     (commandsHtml  .contains("<body>")    , "commandsHtml should have <body>");
            Assert.IsFalse    (commandsRender.contains("<body>")    , "commandsRender should not have <body>");
            Assert.IsTrue     (commandsHtml.contains(commandsRender));
            Assert.IsFalse    (commandsRender.contains(commandsHtml));

            //test for empty and nulls
            var noRender   = TmRest.TBot_Render("abcdef12345").cast<MemoryStream>().ascii().lower();            
            var emptyRender   = TmRest.TBot_Render("").cast<MemoryStream>().ascii().lower();            
            var nullRender    = TmRest.TBot_Render(null).cast<MemoryStream>().ascii().lower();                                    
            Assert.AreEqual   ("", noRender);
            Assert.AreEqual   ("", emptyRender);
            Assert.AreEqual   ("", nullRender);
        }
        [Test][Assert_Admin] public void TBot_Json()
        {
            var statusJson  = TmRest.TBot_Json("json_Stats").cast<MemoryStream>().ascii().lower();
            Assert.NotNull    (statusJson);                         
            Assert.IsTrue     (statusJson.contains("'status'"));
            var jsonData = statusJson.json_Deserialize();
            Assert.NotNull    (jsonData);            

            //test for empty and nulls
            var noJson    = TmRest.TBot_Json("abcdef12345").cast<MemoryStream>().ascii().lower();
            var emptyJson    = TmRest.TBot_Json("").cast<MemoryStream>().ascii().lower();
            var nullJson    = TmRest.TBot_Json(null).cast<MemoryStream>().ascii().lower();
            Assert.AreEqual   ("{}", noJson    , "noJson");
            Assert.AreEqual   ("{}", emptyJson , "emptyJson");
            Assert.AreEqual   ("{}", nullJson  , "nullJson");
        }

        [Test]
        public void RedirectToLoginOnNoAdmin()
        {
            var response = HttpContextFactory.Response;
            Assert.IsFalse  (response.IsRequestBeingRedirected);
            Assert.AreEqual ("", response.RedirectLocation);            
            Assert.Throws<Exception>(()=>TmRest.TBot_Run("abc"));
            Assert.IsTrue   (response.IsRequestBeingRedirected);
            Assert.AreEqual ("/Login?LoginReferer=/tbot", response.RedirectLocation);            
        }

        [Test][Assert_Admin]
        public void TbotMainPage()
        {
            var showTbotHtml     = TmRest.TBot_Run("commands").cast<MemoryStream>().ascii();            
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBot_Brain.TBOT_MAIN_HTML_PAGE);
            Assert.IsNotNull(tbotMainHtmlFile             , "tbotMainHtmlFile was null");
            Assert.IsTrue   (tbotMainHtmlFile.fileExists(), "tbotMainHtmlFile didn't exist");
            Assert.IsNotNull(showTbotHtml);
            Assert.IsTrue   (showTbotHtml.contains("bootstrap.min.css"), "Couldn't find bootstrap.min.css");            
        }

        [Test][Assert_Admin]        
        public void Script_ViewEmailsSent()
        {
            //tests one script to make sure core engine is working
            // (run CheckThatAllTBotPagesLoad to test all scripts)
            var tbotBrain = new TBot_Brain(TmRest);
            var html = tbotBrain.ExecuteRazorPage("View_Emails_Sent");    
            Assert.IsNotNull(html, "html was null");
        }
        [Test][Assert_Admin]       
        public void Script_GitStatus()
        {
            SetUpNGit();            
            var responseStream = (MemoryStream)TmRest.TBot_Run("GitStatus");// trigger unpack of NGit and Sharpen dlls            
            var html = responseStream.ascii();
            Assert.NotNull(html);
            //html.info();
            Assert.IsFalse(html.contains("Unable to compile template"), "Compilation error");
            Assert.IsFalse(html.contains("<hr /><b>Exception:</b> "), "Execution Exception");
        }        
        [Test]
        [Assert_Admin]
        [Ignore("trigger manually since it takes a while to compile all Tbot scripts")]
        public void Script_Run_AllScripts()
        {            
            SetUpNGit();            
            var tbotBrain = new TBot_Brain(TmRest);
            foreach (var scriptName in tbotBrain.scriptsNames())
            {
                "================= Executing TBot script: {0}".info(scriptName);
                var html = tbotBrain.ExecuteRazorPage(scriptName); //"View_Emails_Sent");    
                Assert.IsNotNull(html, "for :{0}".format(scriptName));
                var compileError = html.contains("Unable to compile template");
                if (compileError)
                {
                    html.info();
                    Assert.Fail("Failed to compile: {0}".format(scriptName));
                }
                var executionError = html.contains("Opps: Something went wrong:");
                if (executionError)
                {
                    html.info();
                    Assert.Fail("Execution error: on  {0}".format(scriptName));
                }
                
            }

            //"test webBrowser".popupWindow().add_WebBrowser().set_Html(html).waitForClose();
        }

        //Helper methods
        public void SetUpNGit()
        {
            TmRest.TBot_Run("Git");// trigger unpack of NGit and Sharpen dlls
            var fluentSharpGit = new API_NGit();
            Assert.NotNull(fluentSharpGit, "fluentSharpGit was null");
        }
    }
}
