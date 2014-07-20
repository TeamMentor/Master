using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.REST;

namespace TeamMentor.UnitTests.QA
{
    //[Ignore("Integration Test")]
    [TestFixture]
    public class Test_TBot_Scripts : TM_Rest_Direct
    {
        public Test_TBot_Scripts()          // same sa code found in Test_TBot
        {             
            var assembly		    = this.type().Assembly;
            var dllLocation		    = assembly.CodeBase.subString(8);
            var webApplications     = dllLocation.parentFolder().pathCombine(@"\..\..\..\..");
            var tmWebsite 		    = webApplications.pathCombine(@"TM_Websites\Website_3.5");
            var webConfig            = tmWebsite.pathCombine("Web.config");
            moq_HttpContext.BaseDir = tmWebsite;
            Assert.IsTrue(webConfig.fileExists(), "couldn't find webConfig file at: {0}".format(webConfig)); 
        }
        [Test]        
        //[Ignore("trigger manually since it takes a while to compile all Tbot scripts")]
        public void Script_Run_AllScripts()
        {            
            UserGroup.Admin.assert();

            TmRest.TBot_Run("Git");                 // trigger unpack of NGit and Sharpen dlls
            var fluentSharpGit = new API_NGit();
            Assert.NotNull(fluentSharpGit, "fluentSharpGit was null");
     

            // Load dlls as required by some TBot pages:  
            "System.Xml.Linq".assembly().assert_Not_Null(); // Import Legacy Users
            "FluentSharp.Xml".assembly().assert_Not_Null(); // Import Legacy Users
            //"System.Web"     .assembly().assert_Not_Null(); // IIS Sessions

            var tbotBrain = new TBot_Brain(TmRest);
            foreach (var scriptName in tbotBrain.scriptsNames())
            {
                if(scriptName.equals("GitDiff_UserData_Commit", "GitDiff_UserData_File"))        // there is a prob is this Tbot page (currently not exposed to users)
                    continue;
                //if (scriptName != "Json_UserTags") continue;                    
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
    }
}
