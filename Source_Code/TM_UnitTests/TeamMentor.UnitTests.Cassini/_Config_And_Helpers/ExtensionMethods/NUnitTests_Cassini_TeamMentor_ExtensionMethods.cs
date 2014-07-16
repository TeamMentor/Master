using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    public static class NUnitTests_Cassini_TeamMentor_ExtensionMethods
    {
        
        public static string teamMentor_Root_OnDisk(this NUnitTests_Cassini_TeamMentor nUnitTests_Cassini)
        {            
            //equivalent to TeamMentor.UnitTests WebSite_Root_OnDisk
            var assembly        = nUnitTests_Cassini.type().Assembly;
            var dllLocation     = assembly.CodeBase.subString(8);
            var webApplications = dllLocation.parentFolder().pathCombine(@"\..\..\..\..");
            var tmWebsite       = webApplications.pathCombine(@"TM_Websites\Website_3.5");

            tmWebsite.assert_Folder_Exists("tmWebsite dir not found: {0}".format(tmWebsite));
            Assert.That(dllLocation.fileExists()    , "dllLocation file not found: {0}".format(dllLocation));
            Assert.That(webApplications.dirExists() , "webApplications dir not found: {0}".format(webApplications));

            return tmWebsite;            
        }
    }
}