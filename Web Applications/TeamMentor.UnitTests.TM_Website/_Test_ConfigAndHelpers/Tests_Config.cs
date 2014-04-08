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

    [TestFixture]
    public class Test_Tests_Config
    {
        [Test]
        public void RunBeforeAllTests()
        {
            Assert.IsFalse(O2ConfigSettings.CheckForTempDirMaxSizeCheck);
            Assert.IsTrue(PublicDI.log.LogRedirectionTarget.alsoShowInConsole);
            
            O2ConfigSettings.CheckForTempDirMaxSizeCheck        = true;
            PublicDI.log.LogRedirectionTarget.alsoShowInConsole = false;                 

            Assert.IsTrue(O2ConfigSettings.CheckForTempDirMaxSizeCheck);
            Assert.IsFalse(PublicDI.log.LogRedirectionTarget.alsoShowInConsole);

            var testsConfig = new Tests_Config();
            testsConfig.RunBeforeAllTests();
            testsConfig.RunAfterAllTests();

            Assert.IsFalse(O2ConfigSettings.CheckForTempDirMaxSizeCheck);
            Assert.IsTrue(PublicDI.log.LogRedirectionTarget.alsoShowInConsole);
        }
    }

}
