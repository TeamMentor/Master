using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Tracking
{
    [TestFixture]
    public class Test_Tracking_Application
    {
        public Tracking_Application tracking;
        public string               tmpFolder;
        public string               logFolder;

        [SetUp]
        public void setup()
        {            
            tmpFolder = "logFolder".tempDir(false);             
            logFolder = tmpFolder.pathCombine(TMConsts.APPLICATION_LOGS_FOLDER_NAME);
            
            Assert.IsTrue (tmpFolder.dirExists());
            Assert.IsFalse(logFolder.dirExists());
            
            tracking = new Tracking_Application(tmpFolder);
            
            Assert.IsTrue(logFolder.dirExists());
        }
        [TearDown]
        public void tearDown()
        {            
            Files.deleteFolder(tmpFolder,true);
            Assert.IsFalse(tmpFolder.dirExists());
            Assert.IsFalse(logFolder.dirExists());
        }

        [Test]
        public void Tracking_Application_Ctor()
        {
            Assert.IsNotNull(tracking);
            Assert.IsNotNull(tracking.LogFilesLocation);
            Assert.IsNotNull(tracking.LogFilePath);
            Assert.AreEqual (tracking.LogFilesLocation          , logFolder);
            Assert.AreEqual (tracking.LogFilePath.parentFolder(), logFolder);
            Assert.IsTrue   (tracking.LogFilesLocation.dirExists());
            Assert.IsFalse  (tracking.LogFilePath.fileExists());
            Assert.IsEmpty  (tracking.LogFilesLocation.files());
        }

        [Test]
        public void saveLog()
        {   
            tracking.clearLog(false);
            var expected1 = "INFO: an info\r\nINFO: an debug\r\nINFO: an error\r\n";
            var expected2 = "INFO: an message\r\n";
            "an info".info();
            "an debug".info();
            "an error".info();
            
            Assert.IsFalse  (tracking.LogFilePath.fileExists());            
            tracking.saveLog();                      

            Assert.IsTrue  (tracking.LogFilePath.fileExists());
            Assert.AreEqual(tracking.LogFilePath.fileContents(), expected1);
                        
            tracking.LogFilePath.delete_File();

            Assert.IsFalse  (tracking.LogFilePath.fileExists());
            tracking.clearLog(false);                      
            "an message".info();
            tracking.saveLog();                      

            Assert.IsTrue  (tracking.LogFilePath.fileExists());
            Assert.AreEqual(tracking.LogFilePath.fileContents(), expected2);
        }

        [Test]
        public void clearLog()
        {
            var expected1 = "INFO: an info\r\n";
            var expected2 = "DEBUG: an debug";

            var logData = PublicDI.log.LogRedirectionTarget.prop("LogData");
               
            tracking.clearLog(false);
            "an info".info();
            
            Assert.AreEqual(logData.str(), expected1);
            Assert.AreEqual(logData.str(), tracking.logData());

            tracking.clearLog();
            "an debug".debug();
            var logLines = logData.str().lines();
            Assert.AreEqual(logLines.size()  , 2);
            Assert.AreEqual(logLines.second(), expected2);
            Assert.AreEqual(logData.str()    , tracking.logData());
        }

        [Test]
        public void start()
        {
            Assert.AreEqual(tracking, tracking.start(""));
            Assert.AreEqual(tracking.LogFilesLocation, logFolder);           // this value shouldn't change with tracking.start("")
            Assert.AreEqual(tracking, tracking.start(null));
            Assert.AreEqual(tracking.LogFilesLocation, logFolder);           // this value shouldn't change with tracking.start(null)
            
            //testing nulls
            Assert.IsNull((null as Tracking_Application).start(tmpFolder));
            Assert.IsNull((null as Tracking_Application).start(null));
            Assert.IsNull((null as Tracking_Application).start(""));            
        }
        [Test]
        public void stop()
        {                        
            //testing nulls
            Assert.IsNull((null as Tracking_Application).stop());            
        }
    }
}
