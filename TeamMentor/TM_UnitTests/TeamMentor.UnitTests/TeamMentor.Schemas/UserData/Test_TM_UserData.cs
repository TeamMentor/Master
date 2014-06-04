using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_UserData
    {
        [Test] public void TM_UserData_Ctor()   
        {
            var userData = new TM_UserData();

            Assert.IsFalse  (userData.usingFileStorage());
            Assert.AreEqual (TM_UserData.Current, userData);
            Assert.AreEqual (userData.Path_WebRootFiles, TMConsts.USERDATA_PATH_WEB_ROOT_FILES);
            Assert.IsEmpty  (userData.TMUsers);
            Assert.NotNull  (userData.SecretData);        
            Assert.NotNull  (userData.Events);
            Assert.IsNull   (TM_UserData.GitPushThread);
            
            Assert.IsNull   (userData.Path_UserData);
            Assert.IsNull   (userData.tmConfig_Location());
            Assert.IsNull   (userData.secretData_Location());            
            Assert.IsNull   (userData.users_XmlFile_Location());
            Assert.IsNull   (new TMUser().user_XmlFile_Location());
            Assert.IsNull   (new TMUser().user_XmlFile_Name());


            var userData2   = new TM_UserData().useFileStorage();
            var userDataGit = new TM_UserData_Git(userData2);
            
            Assert.NotNull  (userDataGit.userData());
            Assert.NotNull  (userDataGit.UserData);
            Assert.IsNull   (userDataGit.UserData.Path_UserData);
            Assert.IsNull   (userDataGit.NGit);
            
            //set by ResetData
            Assert.IsTrue   (userDataGit.UserData.usingFileStorage());
            //Assert.AreEqual (userData2.FirstScriptToInvoke, TMConsts.USERDATA_FIRST_SCRIPT_TO_INVOKE);
            Assert.AreEqual (userData2.Path_WebRootFiles  , TMConsts.USERDATA_PATH_WEB_ROOT_FILES);            
            Assert.AreEqual (TM_UserData.Current, userData2);            
            Assert.IsEmpty  (userData2.TMUsers);
            Assert.IsNotNull(userData2.SecretData);                        
        
            
            // test TM_UserData_Git NGit Default values
        
            var userData3 = new TM_UserData_Git(null);

            Assert.AreEqual(userData3.NGit_Author_Name , TMConsts.NGIT_DEFAULT_AUTHOR_NAME);
            Assert.AreEqual(userData3.NGit_Author_Email, TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL);            
        }

        [Test] public void loadUsers()
        {
            var userData = new TM_UserData();
            
            userData.users_Load();
            Assert.IsEmpty(userData.TMUsers);
            Assert.IsNull(userData.Path_UserData);

            var userData2 = new TM_UserData().useFileStorage();   // when UsingFileStorage = true

            userData2.users_Load();                   // but userData.Path_UserData =null            
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
