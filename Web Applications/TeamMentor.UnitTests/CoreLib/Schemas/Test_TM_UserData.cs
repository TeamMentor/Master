using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_UserData : TM_XmlDatabase_InMemory
    {
        [Test]
        public void TM_UserData_Ctor()
        {
            Assert.IsFalse  (userData.UsingFileStorage);
            Assert.AreEqual (TM_UserData.Current, userData);
            Assert.IsNull   (TM_UserData.GitPushThread);

            var userData2 = new TM_UserData(true);
            
            Assert.IsNull   (userData2.Path_UserData);
            Assert.IsNull   (userData2.Path_UserData_Base);            
//            Assert.IsNull   (userData2.Git_UserData);
            Assert.IsNull   (userData2.NGit);
//            Assert.IsFalse  (userData2.AutoGitCommit);            

            //set by ResetData
            Assert.IsTrue   (userData2.UsingFileStorage);
            Assert.AreEqual (userData2.FirstScriptToInvoke, TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE);
            Assert.AreEqual (userData2.Path_WebRootFiles  , TMConsts.USERDATA_PATH_WEB_ROOT_FILES);
//            Assert.AreEqual (userData2.AutoGitCommit      , TMConfig.Current.Git.AutoCommit_UserData);
            Assert.IsEmpty  (userData2.TMUsers);
            Assert.IsNotNull(userData2.SecretData);                        
            
            userData = new TM_UserData();                   // restore userData to the version that doesn't use the FileStorage
            Assert.IsFalse    (userData.UsingFileStorage);
            Assert.AreEqual   (TM_UserData.Current, userData);
            Assert.AreNotEqual(TM_UserData.Current, userData2);
        }

        [Test]
        public void resetData()
        {
            userData.resetData();
            Assert.AreEqual(userData.NGit_Author_Name, TMConsts.NGIT_DEFAULT_AUTHOR_NAME);
            Assert.AreEqual(userData.NGit_Author_Email, TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL);
            Assert.AreEqual(userData.FirstScriptToInvoke, TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE);
            Assert.AreEqual(userData.Path_WebRootFiles, TMConsts.USERDATA_PATH_WEB_ROOT_FILES);
            Assert.IsEmpty(userData.TMUsers);
            Assert.NotNull(userData.SecretData);        // see Test_TM_SecretData for the Ctor checks
        }
        

        [Test]
        public void InvokeUserDataScriptFile()
        {
            userData.Path_UserData  = "userData".tempDir();
            var h2ScriptFile        = userData.firstScript_FileLocation();
            
            Assert.IsNotNull(h2ScriptFile);
            Assert.IsFalse  (h2ScriptFile.fileExists());
            
            h2ScriptFile.parentFolder().createDir();
            "return 42;".saveAs (h2ScriptFile);

            var fileExists          = h2ScriptFile.fileExists();
            var scriptContents      = h2ScriptFile.h2_SourceCode();
            var assembly            = scriptContents.compileCodeSnippet();
            var result_DirectInvoke = assembly.firstMethod().invoke();          // invoke directly
            var result_TmInvoke     = userData.firstScript_Invoke();         // invoke via TM UserData            
            var fileDeleted         = h2ScriptFile.file_Delete() && h2ScriptFile.fileExists().isFalse();

            Assert.IsTrue   (fileExists);
            Assert.IsNotNull(scriptContents);
            Assert.IsNotNull(assembly);            
            Assert.AreEqual (result_DirectInvoke,  42);    
            Assert.AreEqual (result_TmInvoke    , "42");
            Assert.IsTrue   (fileDeleted);

            //Remove temp UserData folder
            Assert.IsTrue   (userData.Path_UserData.dirExists());
            Assert.IsTrue   (Files.deleteFolder(userData.Path_UserData,true));
            Assert.IsFalse  (userData.Path_UserData.dirExists());
        }

    }
}
