using System;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class TBot_Users_MainPages : TestFixture_TBot
    {
        [Test]
        public void Check_Root_Level_Pages()
        {                                         
            var urlTemplate  = "{0}/tbot_users/{1}";            

            Action<string,string> runTest = 
                    (pageUrl, expectedHtml) =>
                        {                            
                            var url         = urlTemplate.format(tbot.TargetServer, pageUrl);                            
                            ie.open(url);                                                     
                            "Current Url: {0}".info(ie.url());
                            //"Current HTML {0}".info(ie.html()); 
                          Assert.IsTrue( tbot.html().contains(expectedHtml));                            
                        };
            
            
            runTest("default.htm?".add_RandomLetters(5) , "TBot v2.0");             
            runTest("users.htm?".add_RandomLetters(5)   , "Welcome to the Users page");
        }        

        [Test]
        public void Page_main()
        {            
            tbot.login_As_Admin()
                .close_IE()
                .open_IE();

            ie.open(tbot.TargetServer + "/tbot_users/default.htm?".add_RandomLetters());

            "Url: {0}".info(ie.url());
            "Html: {0}".info(tbot.html());
            
            
            Assert.IsTrue(tbot.html().contains("TBot v2.0"));
        }

        [Test][Ignore]
        public void Check_Top_Links()
        {             
            var mainUrl = tbot.TargetServer + "/tbot_users/";

            ie.open("about:blank");
            Assert.AreNotEqual(ie.url(), mainUrl);

            ie.open(mainUrl);
            Assert.AreEqual(ie.url(), mainUrl);
            Assert.IsTrue (tbot.html().contains("TBot v2.0"));
            
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
            
            checkLink("users"               , true, mainUrl      + "users.htm#/users/main");                        

            checkLink("Admin"               , true, tbot.TargetServer + "/admin");            
            checkLink("tbot"                , true, tbot.TargetServer + "/tbot");
            checkLink("login"               , true, tbot.TargetServer + "/login?LoginReferer=/tbot_users");
            checkLink("Logout"              , true, tbot.TargetServer + "/logout");
            checkLink("Legacy Control Panel", true, tbot.TargetServer + "/admin");
            checkLink("Main TeamMentor site", true, tbot.TargetServer + "/TeamMentor");
            
            //these shouldn't
            checkLink("tbot1234"            , false, null);
            checkLink("AAAA"                , false, null);            
            checkLink("AAAA/BBB"            , false, null);            
        }

        [Test]
        public void Check_UsersMenu_Directive()
        {
            var usersPage = tbot.TargetServer + "/tbot_users/users.htm";

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

            checkLink("Home"            , "#/users/main");
            checkLink("Users List"      , "#/users/list");
            checkLink("Create SSO token", "#/users/sso");
        }
    }
}
