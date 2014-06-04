using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using NUnit.Framework;
using TeamMentor.CoreLib;
using MiscUtils = TeamMentor.CoreLib.MiscUtils;

namespace TeamMentor.UnitTests
{
    [SetUpFixture]
    public class Tests_Config
    {
        [SetUp]
        public void RunBeforeAllTests()
        {
            O2ConfigSettings.CheckForTempDirMaxSizeCheck = false;
            PublicDI        .log.writeToDebug(true);                     // redirect log messages to debug (so that it shows up in unit tests results)            
            TM_Xml_Database .SkipServerOnlineCheck = true;       
            TMConfig        .Current               = new TMConfig();     // set to default values                        
        }

        [TearDown]
        public void RunAfterAllTests()
        {            
        }
    }

    public class Tests_Consts
    {        
        public static bool      offline                     = MiscUtils.online().isFalse();        
        public static string    TM_REST_Service_Protocol    = "http";
        public static int       TM_REST_Service_Port        =  8732;
        public static string    TM_REST_Service_IP          = "localhost";
        public static string    TM_REST_Url_Template        = "{0}://{1}:{2}/Design_Time_Addresses/REST";
    }

    [TestFixture]
    public class Test_Tests_Config
    {
        [Test]
        public void SetUp_TearDown()
        {
            var testsConfig = new Tests_Config();
            testsConfig.RunBeforeAllTests();
            
            Assert.IsFalse  (O2ConfigSettings.CheckForTempDirMaxSizeCheck);
            Assert.IsNotNull(PublicDI.log.LogRedirectionTarget);
            Assert.IsTrue   (PublicDI.log.LogRedirectionTarget.alsoShowInConsole);
            Assert.IsTrue   (TM_Xml_Database.SkipServerOnlineCheck);
            Assert.IsNotNull(TMConfig.Current);     
            
            testsConfig.RunAfterAllTests();
            //nothing to test since there is nothing here
        }

        [Test]
        public void Tests_Consts_Static_Value()
        {
            Assert.AreEqual(Tests_Consts.offline , MiscUtils.online().isFalse());
        }
    }
}
