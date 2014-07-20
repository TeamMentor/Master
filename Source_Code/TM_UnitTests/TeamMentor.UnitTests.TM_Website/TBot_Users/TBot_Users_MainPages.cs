using System;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.REPL;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class TBot_Users_MainPages : TestFixture_TBot
    {        
        [SetUp]
        public void setUp()
        {
            ie.parentForm().show();
            tbot.login_As_Admin();  
            ie.close();
            ie = tbot.ie = ie.parentForm().clear().add_IE();    // need to re-add the IE object since there is a bug 
                                                                // with its state after the call with tbot.login_As_Admin();  
        }

        [Test]
        public void Check_Root_Level_Pages()
        {                       
            var urlTemplate  = "{0}/Tbot_Monitor/{1}";            

            Action<string,string> runTest = 
                    (pageUrl, expectedHtml) =>
                        {                            
                            var url         = urlTemplate.format(tbot.TargetServer, pageUrl);                            
                            ie.open(url);                                                     
                            "Current Url: {0}".info(ie.url());
                            tbot.html().assert_Contains(expectedHtml);                            
                        };
            
            
            runTest("default.htm?".add_RandomLetters(5) , "TBot V2");             
            runTest("monitor.htm?".add_RandomLetters(5)   , "Welcome to the Monitor (in realtime) page");

        }        

        [Test]
        public void Page_main()
        {                                      

            ie.open(tbot.TargetServer + "/Tbot_Monitor/default.htm?".add_RandomLetters());

            "Url: {0}".info(ie.url());
            "Html: {0}".info(tbot.html());
            
            
            tbot.html().assert_Contains("TBot V2");
        }

        [Test]
        public void Check_Top_Links()
        {                         
            var mainUrl = tbot.TargetServer + "/Tbot_Monitor/monitor.htm";

            ie.open("about:blank");            
            Assert.AreNotEqual(ie.url(), mainUrl);

            ie.open(mainUrl);
            ie.url()   .assert_Equal(mainUrl);
            tbot.html().assert_Contains("TBot V2");
            
            var links = ie.links();            
            Assert.IsNotEmpty(links);
            
            Action<string, bool,string> checkLink = 
                (linkText, shouldExist, expectedUrl)=>
                    {
                        Assert.AreEqual(shouldExist, ie.hasLink(linkText));
                        if (shouldExist)
                        {
                            var link = ie.link(linkText);
                            Assert.IsNotNull(link);                            
                            Assert.AreEqual(expectedUrl, link.url());
                        }
                    };

            //these should exist            
            checkLink("TM Monitor"            , true, mainUrl      + "#/monitor/activities");                        
            checkLink("Tbot"                  , true, tbot.TargetServer + "/tbot");            
            checkLink("Control Panel (Legacy)", true, tbot.TargetServer + "/admin");            
            checkLink("Logout"                , true, tbot.TargetServer + "/logout");

            //these shouldn't
            checkLink("tbot1234"            , false, null);
            checkLink("AAAA"                , false, null);            
            checkLink("AAAA/BBB"            , false, null);            
        }

        [Test]
        public void Check_Monitor_Directive()
        {
            var usersPage = tbot.TargetServer + "/Tbot_Monitor/monitor.htm";

            ie.open(usersPage);
            Assert.AreEqual(usersPage, ie.url());

            Action<string,string> checkLink = 
                (linkText, target)=>
                    {
                        Assert.IsTrue     (ie.hasLink(linkText), 
                                          "Couldn't find link '{0}' in page {1}".format(linkText, ie.url()));
                        var expectedHref = usersPage + target;
                        var linkHref     = ie.link(linkText).url();
                        Assert.AreEqual   (expectedHref, linkHref);
                    };

            checkLink("User Activities" , "#/monitor/activities");
            checkLink("Debug Logs"      , "#/monitor/logs");
            checkLink("Url Requests"    , "#/monitor/urlRequests");
        }
    }
}
