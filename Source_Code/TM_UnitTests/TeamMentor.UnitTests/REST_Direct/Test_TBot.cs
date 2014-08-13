using System;
using System.Xml.Linq;
using System.IO;
using FluentSharp.Git.APIs;
using FluentSharp.NUnit;
using FluentSharp.Web;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
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
            var webApplications     = dllLocation.parentFolder().pathCombine(@"\..\..\..\..");
            var tmWebsite 		    = webApplications.pathCombine(@"TM_Websites\Website_3.5");
            var webConfig            = tmWebsite.pathCombine("Web.config");
            moq_HttpContext.BaseDir = tmWebsite;
            Assert.IsTrue(webConfig.fileExists(), "couldn't find webConfig file at: {0}".format(webConfig));            
        }

        [SetUp] public void setup()
        {
            UserGroup.Admin.assert();
        }
        [Test] public void TBot_Run()
        {
            var commandsHtml     = TmRest.TBot_Run("commands").cast<MemoryStream>().ascii();            
            Assert.NotNull(commandsHtml);
        }
        [Test][Assert_Admin] public void TBot_Render()
        {
            UserGroup.Admin.assert(); 
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
            UserGroup.None.assert(); 
        }
/*        [Test][Assert_Admin] public void TBot_Json()
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
        }*/ 

        [Test] public void RedirectToLoginOnNoAdmin()
        {
            UserGroup.None.assert();                                    // remove all privileges
            
            TmRest.TBot_Run("abc").cast<MemoryStream>().ascii()           
                                 .assert_Equals("Redirecting to Login Page...\n\n");
        }

        [Test] public void TbotMainPage()
        {
            var showTbotHtml     = TmRest.TBot_Run("commands").cast<MemoryStream>().ascii();            
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBot_Brain.TBOT_MAIN_HTML_PAGE);
            Assert.IsNotNull(tbotMainHtmlFile             , "tbotMainHtmlFile was null");
            Assert.IsTrue   (tbotMainHtmlFile.fileExists(), "tbotMainHtmlFile didn't exist");
            Assert.IsNotNull(showTbotHtml);
            Assert.IsTrue   (showTbotHtml.contains("bootstrap.min.css"), "Couldn't find bootstrap.min.css");            
        }

        [Test] public void Script_ViewEmailsSent()
        {            
            TBot_Brain.AvailableScripts = TBot_Brain.SetAvailableScripts();
            //tests one script to make sure core engine is working
            // (run CheckThatAllTBotPagesLoad to test all scripts)
            var tbotBrain = new TBot_Brain(TmRest);
            var html = tbotBrain.ExecuteRazorPage("Current_Users");    
            Assert.IsNotNull(html, "html was null");
        }
        [Test] public void Script_GitStatus()
        {
            SetUpNGit();            
            var responseStream = (MemoryStream)TmRest.TBot_Run("GitStatus");// trigger unpack of NGit and Sharpen dlls            
            var html = responseStream.ascii();
            Assert.NotNull(html);
            html.info();
            Assert.IsFalse(html.contains("Unable to compile template"), "Compilation error");
            Assert.IsFalse(html.contains("<hr /><b>Exception:</b> "), "Execution Exception");
        }                

        //Helper methods
        public void SetUpNGit()
        {
            TmRest.TBot_Run("Git");                 // trigger unpack of NGit and Sharpen dlls
            var fluentSharpGit = new API_NGit();
            Assert.NotNull(fluentSharpGit, "fluentSharpGit was null");
        }
    }
}
