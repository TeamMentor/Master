using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
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
        [Test]public void StartAndStop()
        {
            //trackingApplication.start();

//            trackingApplication.stop();

            //var trackingFiles = trackingApplication.LogFilesLocation.files();

//            Assert.IsNotEmpty(trackingApplication.LogFilePath);            
        }

        [Test] public void realTime_LogFilePath()
        {
            var tmpFolder = "logFolder".tempDir();  
            
            TM_StartUp.Current.trackingApplication().realTime_LogFilePath().assert_Null();
            new TM_Server().RealTime_Logs = false;
            new Tracking_Application(tmpFolder).RealTime_LogFilePath.assert_Null();
            PublicDI.log.LogRedirectionTarget.assert_Instance_Of<Logger_DiagnosticsDebug>();
            
            new TM_Server().RealTime_Logs = true;

            var tracking = new Tracking_Application(tmpFolder);
            tracking.RealTime_LogFilePath.assert_Not_Null();
            PublicDI.log.LogRedirectionTarget.assert_Instance_Of<Logger_File_Append>();     
            
            var realTime_LogFilePath= tracking.realTime_LogFilePath();
            
            realTime_LogFilePath.assert_File_Exists();
            
            var testMessage = "Info Test".add_5_RandomLetters().info();

            realTime_LogFilePath.fileContents().assert_Contains(testMessage);


            Files.delete_Folder_Recursively(tmpFolder).assert_True();            

            new TM_Server().RealTime_Logs = false;
            TM_StartUp.Current.trackingApplication().realTime_LogFilePath().assert_Null();
        }
    }
}
