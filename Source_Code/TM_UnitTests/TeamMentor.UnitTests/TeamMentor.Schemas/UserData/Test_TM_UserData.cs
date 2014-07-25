using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_UserData
    {
        [Test] public void TM_UserData_Ctor()   
        {
            TM_FileStorage.Current = null;
            var userData = new TM_UserData();
            
            Assert.AreEqual (TM_UserData.Current, userData);            
            Assert.IsEmpty  (userData.TMUsers);
            Assert.NotNull  (userData.SecretData);        
            Assert.NotNull  (userData.Events);
            Assert.IsNull   (TM_UserData.GitPushThread);
                        
            Assert.IsNull   (new TMUser().user_XmlFile_Name());            
        }
        
 
        //public void InvokeUserDataScriptFile()
        /*[Test]
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
        */

    }
}
