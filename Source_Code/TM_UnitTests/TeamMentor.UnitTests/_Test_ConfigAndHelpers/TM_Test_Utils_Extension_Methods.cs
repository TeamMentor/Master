using NUnit.Framework;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests
{
    public static class TM_Test_Utils_Extension_Methods
    {
        public static string WebSite_Root_OnDisk(this object objectInUnitTestDll)
        {
            
            var assembly        = objectInUnitTestDll.type().Assembly;
            var dllLocation     = assembly.CodeBase.subString(8);
            var webApplications = dllLocation.parentFolder()
                                             .pathCombine(@"\..\..\..\..");
            var tmWebsite       = webApplications.pathCombine(@"TM_Websites\Website_3.4");

            Assert.That(tmWebsite.dirExists()       , "tmWebsite dir not found: {0}".format(tmWebsite));
            Assert.That(dllLocation.fileExists()    , "dllLocation file not found: {0}".format(dllLocation));
            Assert.That(webApplications.dirExists() , "webApplications dir not found: {0}".format(webApplications));

            return tmWebsite;            
        }
    }
}