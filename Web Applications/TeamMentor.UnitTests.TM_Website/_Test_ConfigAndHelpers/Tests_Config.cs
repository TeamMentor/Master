using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [SetUpFixture]
    public class Tests_Config
    {        
        [SetUp]
        public void RunBeforeAllTests()
        {
            O2ConfigSettings.CheckForTempDirMaxSizeCheck = false;
            PublicDI        .log.writeToDebug(true);                     // redirect log messages to debug (so that it shows up in unit tests results)                 
        }

        [TearDown]
        public void RunAfterAllTests()
        {            
        }
    }
}
