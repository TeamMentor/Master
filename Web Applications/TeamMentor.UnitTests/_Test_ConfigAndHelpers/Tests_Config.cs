using NUnit.Framework;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    [SetUpFixture]
    public class Tests_Config
    {
        public static bool offline = MiscUtils.online().isFalse();

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
}
