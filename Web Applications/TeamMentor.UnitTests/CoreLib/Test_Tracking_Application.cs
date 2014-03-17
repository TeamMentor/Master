using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    class Test_Tracking_Application : TM_XmlDatabase_InMemory
    {
        public Tracking_Application trackingApplication;
        public string               testDir;
        [SetUp]
        public void setup()
        {
            testDir             = "_Test_Tracking_Application".tempDir(false);            
            trackingApplication = new Tracking_Application(testDir);

            Assert.IsTrue       (testDir.dirExists());
            Assert.IsNotNull    (trackingApplication.Name);
            Assert.IsNotNull    (trackingApplication.LogFilePath);
            Assert.AreNotEqual  (testDir, trackingApplication.LogFilesLocation);
            Assert.IsTrue       (trackingApplication.LogFilesLocation.dirExists());                        

            "Tracking Application TempDir: {0}".info(trackingApplication.LogFilesLocation);            
        }

        [TearDown]
        public void tearDown()
        {
            Assert.IsTrue       (testDir.dirExists());
            Files.deleteFolder  (testDir, true);
            Assert.IsFalse      (testDir.dirExists());
        }


        [Test]
        public void StartAndStop()
        {
            //trackingApplication.start();

//            trackingApplication.stop();

            //var trackingFiles = trackingApplication.LogFilesLocation.files();

//            Assert.IsNotEmpty(trackingApplication.LogFilePath);            
        }
    }
}
