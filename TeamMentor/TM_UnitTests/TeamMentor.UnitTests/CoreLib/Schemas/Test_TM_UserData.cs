using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_UserData
    {
        [Test] public void TM_UserData_Ctor()   
        {
            var userData = new TM_UserData();

            Assert.IsFalse  (userData.UsingFileStorage);
            Assert.AreEqual (TM_UserData.Current, userData);
            Assert.IsNull   (TM_UserData.GitPushThread);
            
            Assert.IsNull   (userData.Path_UserData);
            Assert.IsNull   (userData.tmConfig_Location());
            Assert.IsNull   (userData.secretData_Location());            
            Assert.IsNull   (userData.users_XmlFile_Location());
            Assert.IsNull   (new TMUser().user_XmlFile_Location());
            Assert.IsNull   (new TMUser().user_XmlFile_Name());

            var userData2 = new TM_UserData_Git(true);
            
            Assert.IsNull   (userData2.Path_UserData);
            Assert.IsNull   (userData2.NGit);
            
            //set by ResetData
            Assert.IsTrue   (userData2.UsingFileStorage);
            //Assert.AreEqual (userData2.FirstScriptToInvoke, TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE);
            Assert.AreEqual (userData2.Path_WebRootFiles  , TMConsts.USERDATA_PATH_WEB_ROOT_FILES);            
            Assert.AreEqual (TM_UserData.Current, userData2);            
            Assert.IsEmpty  (userData2.TMUsers);
            Assert.IsNotNull(userData2.SecretData);                        
        
            // was in resetData()          
        
            var userData3 = new TM_UserData_Git();

            Assert.AreEqual(userData3.NGit_Author_Name, TMConsts.NGIT_DEFAULT_AUTHOR_NAME);
            Assert.AreEqual(userData3.NGit_Author_Email, TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL);
            //Assert.AreEqual(userData.FirstScriptToInvoke, TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE);
            Assert.AreEqual(userData3.Path_WebRootFiles, TMConsts.USERDATA_PATH_WEB_ROOT_FILES);
            Assert.IsEmpty(userData3.TMUsers);
            Assert.NotNull(userData3.SecretData);        // see Test_TM_SecretData for the Ctor checks
        }

        [Test] public void loadUsers()
        {
            var userData = new TM_UserData();
            
            userData.loadUsers();
            Assert.IsEmpty(userData.TMUsers);
            Assert.IsNull(userData.Path_UserData);

            var userData2 = new TM_UserData(true);   // when UsingFileStorage = true

            userData2.loadUsers();                   // but userData.Path_UserData =null            
            Assert.IsEmpty(userData2.TMUsers);       // we should get no users
        }
        
        
        [Test] public void user_XmlFile_Location()
        {
            
   
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
