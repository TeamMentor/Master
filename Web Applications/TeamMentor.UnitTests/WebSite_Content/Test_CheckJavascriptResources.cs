using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.WebSite_Content
{
    [TestFixture]
    public class Test_CheckJavascriptResources 
    {
        [SetUp]
        public void setup()
        {
            //PublicDI.log.writeToDebug(true);						
        }

        [Test]
        public void Check_GoogleAnalitics()
        {			
            var assembly		 = this.type().Assembly;

            var dllLocation		 = assembly.CodeBase.subString(8);
            var webApplications  = dllLocation.parentFolder()
                                              .pathCombine(@"\..\..\..");
            var tmWebsite 		 = webApplications.pathCombine("TM_Website");
            var gAnalyticsFolder = tmWebsite.pathCombine(@"Javascript\gAnalytics");
            var gaFile 			 = gAnalyticsFolder.pathCombine("ga.js");
            
            Assert.That(dllLocation		.fileExists(), "dllLocation file");
            Assert.That(webApplications .dirExists() , "webApplications dir");
            Assert.That(tmWebsite		.dirExists() , "tmWebsite dir");
            Assert.That(gAnalyticsFolder.dirExists() , "gAnalyticsFolder dir");
            Assert.That(gaFile			.fileExists(), "gaFile file");

            var tmVersion	  = gaFile.fileContents().fixCRLF();

            //if(TM_Xml_Database.SkipServerOnlineCheck.isFalse()
                if(new O2.Kernel.CodeUtils.O2Kernel_Web().online())
            {
                Assert.That(tmVersion.valid()	, "ga.js tmVersion not valid");
                var googleVersion = "http://www.google-analytics.com/ga.js".GET().fixCRLF();
                Assert.That(googleVersion.valid(), "ga.js googleVersion not valid");
                Assert.AreEqual(tmVersion, googleVersion, "ga.js files didn't match");
            }
            else
            {
             Assert.Ignore("Ignoring Test because we are offline");   
            }
        }		
    }
}
