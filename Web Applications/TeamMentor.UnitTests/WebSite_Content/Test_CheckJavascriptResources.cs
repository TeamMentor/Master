using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.WebSite_Content
{
    [TestFixture]
    public class Test_CheckJavascriptResources 
    {
        [SetUp]
        public void setup()
        {
            if (Tests_Consts.offline)
                Assert.Ignore("Ignoring Test because we are offline");   
        }

        [Test]
        public void Check_GoogleAnalitics()
        {
            var tmWebsite = this.WebSite_Root_OnDisk();
            var gAnalyticsFolder = tmWebsite.pathCombine(@"Javascript\gAnalytics");
            var gaFile 			 = gAnalyticsFolder.pathCombine("ga.js");
                        
            Assert.That(tmWebsite		.dirExists() , "tmWebsite dir");
            Assert.That(gAnalyticsFolder.dirExists() , "gAnalyticsFolder dir");
            Assert.That(gaFile			.fileExists(), "gaFile file");

            var tmVersion	  = gaFile.fileContents().fix_CRLF();
            
            Assert.That(tmVersion.valid()	, "ga.js tmVersion not valid");
            var googleVersion = "http://www.google-analytics.com/ga.js".GET().fix_CRLF();
            Assert.That(googleVersion.valid(), "ga.js googleVersion not valid");
            Assert.AreEqual(tmVersion, googleVersion, "ga.js files didn't match");            
        }        
    }
}
