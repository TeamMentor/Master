using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class TBot_V2_MainPages : API_IE_TBot
    {
        [Test]
        public void Page_Tbot_View_Bug()
        {                             
            var urlTemplate  = "{0}./tbot_v2/{1}";
            var expectedHtml = "This is a test";

            Action<string,bool> runTest = 
                    (pageUrl, hasExpectedHtml) =>
                        {
                            var url         = urlTemplate.format(TargetServer, pageUrl);                            
                            ie.open(url);                                                        
                            "Current Url: {0}".info(ie.url());
                            Assert.AreEqual(hasExpectedHtml, html().contains(expectedHtml));
                        };
            
            
            //runTest("default.htm#test" , false);        // default html has the ng-view inside a directive       
            //runTest("users.htm#test"   , true);         // default html has the ng-view on this page

            runTest("default.htm#test" , true);        
            runTest("users.htm#test"   , true);         
        }        
        [Test]
        public void Page_main()
        {
            this.login_As_Admin();            
            ie.open(TargetServer + "/TBot_v2");                                                
            //waitForClose();
            Assert.IsTrue(html().contains("TBot v2.0"));
        }
    }
}
