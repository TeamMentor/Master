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
                                             .pathCombine(@"\..\..\..");
            var tmWebsite       = webApplications.pathCombine("TM_Website");

            Assert.That(tmWebsite.dirExists()       , "tmWebsite dir");
            Assert.That(dllLocation.fileExists()    , "dllLocation file");
            Assert.That(webApplications.dirExists() , "webApplications dir");

            return tmWebsite;            
        }
    }
}