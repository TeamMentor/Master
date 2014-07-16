using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_NUnitTests_Cassini_TeamMentor
    {
        [Test] public void NUnitTests_Cassini_TeamMentor_Ctor()
        {
            // Checks that the Ctor doesn't start the server
            var nUnitTests_TeamMentor = new NUnitTests_Cassini_TeamMentor();
            nUnitTests_TeamMentor.apiCassini.assert_Null();
            nUnitTests_TeamMentor.webRoot   .assert_Folder_Not_Exists();
            nUnitTests_TeamMentor.port      .assert_Default();
        }

        [Test] public void start()         
        {
            var nUnitTests_TeamMentor = new NUnitTests_Cassini_TeamMentor();            
            
            nUnitTests_TeamMentor.start();
            
            nUnitTests_TeamMentor.port      .tcpClient().assert_Not_Null();
            var homePage_Html= nUnitTests_TeamMentor.apiCassini.url().GET();
            homePage_Html.assert_Contains("<html>","<head>","</head>","</html>")
                         .assert_Equal_To(nUnitTests_TeamMentor.webRoot.pathCombine("default.htm").fileContents());

//            nUnitTests_TeamMentor.script_Me().waitForClose();

            nUnitTests_TeamMentor.stop();
            nUnitTests_TeamMentor.port      .tcpClient().assert_Null();  
            nUnitTests_TeamMentor.webRoot.assert_Folder_Exists();
        }

    }
    public class Test_NUnitTests_Cassini_TeamMentor_ExtensionMethods
    {
        [Test] public void teamMentor_Root_OnDisk()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor();
            var teamMentor_Root_OnDisk = nUnitTests_Cassini.teamMentor_Root_OnDisk();
            teamMentor_Root_OnDisk.assert_Folder_Exists()
                .assert_Folder_Has_Files("web.config", "default.htm" ,
                    "javascript/TM/settings.js" ,
                    "javascript/gAnalytics/ga.js");
        }

        
    }
}