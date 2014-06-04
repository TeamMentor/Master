using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
/*    [TestFixture]
    public class Test_Setup : TM_XmlDatabase_InMemory
    {
        [Test]
        public void Copy_FilesIntoWebRoot()
        {
            TM_Server.WebRoot            = "TM_BaseFolder".tempDir(true);                      // Temp webroot folder
            userData.Path_UserData      = TM_Server.WebRoot.pathCombine("..//UserData");       // set temp location
            userData.useFileStorage(true);

            var targetFolder = TM_Server.WebRoot;
            var userDataPath = userData.Path_UserData;
            var webRootFiles = userData.Path_WebRootFiles;
            var sourceFolder   = userDataPath.pathCombine(webRootFiles);

            Assert.IsNotNull(userData       , "userData");
            Assert.IsNotNull(sourceFolder   , "sourceFolder");
            Assert.IsNotNull(webRootFiles   , "webRootFiles");
            Assert.IsNotNull(userDataPath   , "userDataPath");
            Assert.IsNotNull(targetFolder   , "targetFolder");            
            Assert.IsTrue   (targetFolder.dirExists(), "targetFolder should exist");
            //Assert.IsFalse  (sourceFolder.dirExists(), "sourceFolder shouldn't exists (yet)"); // in 3.3. this happens (need to find a way to reset the changes)

            //Create temp data
            sourceFolder.createDir();
            "so that it exists".saveAs(targetFolder.pathCombine("web.config"));     // copy_FilesIntoWebRoot will check that this file exists
            var fileToCopy_Text = "Some content";
            var fileToCopy_Name = "file.txt";
            var fileToCopy_InSource = sourceFolder.pathCombine(fileToCopy_Name);
            var fileToCopy_InTarget = targetFolder.pathCombine(fileToCopy_Name);
            fileToCopy_Text.saveAs(fileToCopy_InSource);
            
            var result = userData.copy_FilesIntoWebRoot();

            //check if copy went ok
            Assert.IsTrue(result, "copy_FilesIntoWebRoot result");
            Assert.IsTrue(fileToCopy_InSource.fileExists(), "fileToCopy_InSource didn't exist");
            Assert.IsTrue(fileToCopy_InTarget.fileExists(), "fileToCopy_InTarget didn't exist");            

            //delete temp folders
            Assert.IsTrue     (TM_Server.WebRoot.dirExists());
            Assert.IsTrue     (Files.deleteFolder(TM_Server.WebRoot,true));
            Assert.IsFalse    (TM_Server.WebRoot.dirExists());

            Assert.IsTrue     (userData.Path_UserData.dirExists());
            Assert.IsTrue     (Files.deleteFolder(userData.Path_UserData,true));
            Assert.IsFalse    (userData.Path_UserData.dirExists());                        
        }
    }
 */
}
