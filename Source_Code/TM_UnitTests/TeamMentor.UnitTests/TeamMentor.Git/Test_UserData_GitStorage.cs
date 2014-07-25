using System;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;
using FluentSharp.Git.APIs;
using FluentSharp.NUnit;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture] 
    public class TM_UserData_Git_ExtensionMethods
    {
        [Test] [Admin] public void setup_UserData_Git_Support()
        {
            admin.assert();
            TM_UserData_Git.Current = null;

            var tmFileStorage = new TM_FileStorage(false);

            // check reflection based invocation of setup_UserData_Git_Support
            "TeamMentor.Git".assembly()                                               .assert_Not_Null()
                            .type("TM_UserData_Git_ExtensionMethods")                 .assert_Not_Null()
                            .invokeStatic("setup_UserData_Git_Support", tmFileStorage).assert_Not_Null()
                                                                                      .assert_Instance_Of<TM_UserData_Git>()
                                                                                      .assert_Is(TM_UserData_Git.Current)
                                                                                      .FileStorage.assert_Not_Null()
                                                                                                  .assert_Is(tmFileStorage);
            
            //check that tmFileStorage.load_UserData  also sets TM_UserData_Git.Current
            TM_UserData_Git.Current = null;
            var temp_UserData = "temp_UserData".temp_Dir();
            
            temp_UserData.isGitRepository().assert_False();

            tmFileStorage = new TM_FileStorage(false);
                            
            tmFileStorage.set_Path_UserData(temp_UserData)
                         .load_UserData();

            TM_UserData_Git.Current    .assert_Not_Null()
                           .FileStorage.assert_Not_Null()
                                       .assert_Is(tmFileStorage);

            temp_UserData.isGitRepository().assert_True();
            
            Files.delete_Folder_Recursively(temp_UserData)
                 .assert_True();
            
        }



    }
    [TestFixture]
    public class Test_UserData_GitStorage 
    {           
        public TM_FileStorage   tmFileStorage;
        public TM_Xml_Database  tmXmlDatabase;
        public TM_Server        tmServer;
        public TM_UserData      userData;
        public TM_UserData_Git  userDataGit;
        public API_NGit         nGit;

        [SetUp]    [Admin] public void setUp()  
        {
            UserGroup.Admin.assert();

            //create TM_FileStorage on temp Custom_WebRoot for this TestFixture
            TM_FileStorage.Custom_WebRoot = "custom_WebRoot".tempDir();                        
            
            tmFileStorage = new TM_FileStorage(false);
            tmFileStorage.set_WebRoot()
                         .set_Path_XmlDatabase()
                         .tmServer_Load()
                         .set_Path_UserData()                         
                         .load_UserData();
            tmFileStorage.hook_Events_TM_UserData();

            tmXmlDatabase = tmFileStorage.TMXmlDatabase   .assert_Not_Null();
            userData      = tmFileStorage.UserData        .assert_Not_Null();
            tmServer      = tmFileStorage.Server          .assert_Not_Null();

            tmFileStorage.Path_XmlDatabase.assert_Folder_Exists();
            tmFileStorage.Path_UserData   .assert_Folder_Exists();

            userDataGit = tmFileStorage   .setup_UserData_Git_Support();        // adds Git Support for UserData repos

            tmFileStorage.Path_UserData.isGitRepository().assert_True();

            Assert.NotNull(tmFileStorage.Server.userData_Config());
                                           
            userData .createDefaultAdminUser();
            
            userDataGit.triggerGitCommit();

            nGit     = userDataGit.NGit;     
            
            nGit.commits().assert_Size_Is(2, "there should be two commits here");            

            UserGroup.None.assert();
        }
        [TearDown] public void tearDown()       
        {            
            admin.assert(()=>tmFileStorage.delete_Database());
            Files.delete_Folder_Recursively(TM_FileStorage.Custom_WebRoot);
            TM_FileStorage.Custom_WebRoot.folder_Wait_For_Deleted();
            TM_FileStorage.Custom_WebRoot.assert_Folder_Doesnt_Exist();
            TM_FileStorage.Custom_WebRoot = null;            
        }
        
        //workflows
        [Test] public void Check_Non_Git_Repo_Doesnt_Commit()                          
        {            
            var temp_Path_UserData      = "nonGitRepo".tempDir(); 
            tmFileStorage.Path_UserData = temp_Path_UserData;
            
            tmFileStorage.path_UserData().assert_Folder_Exists()                                         
                                         .assert_Folder_Empty()
                                         .assert_Is(temp_Path_UserData);

            admin.assert(()=>tmFileStorage.load_UserData());

            tmFileStorage.users_XmlFile_Location().assert_Folder_Exists()        // before we create the default admin user (below)
                                                  .assert_Folder_Empty();        // there should be no files in the userData's user's folder
            
            userData.tmUsers().assert_Empty();
            
            manageUsers.assert(()=>userData.createDefaultAdminUser());

            tmFileStorage.users_XmlFile_Location().assert_Folder_Has_Files()     // now there should be files in the userData's user's folder
                                                  .files().assert_Size_Is(1)
                                                  .first().fileName().assert_Starts("admin");

            userData.tmUsers().assert_Size_Is(1);

            userData.tmUser("admin").assert_Not_Null();

            tmFileStorage.path_UserData().assert_False(path=>path.isGitRepository())
                                         .files().assert_Not_Empty()
                                         .first().parentFolder().assert_Folder_File_Count_Is(2)
                                         .files(true).assert_Size_Is(3);
           
            Files.delete_Folder_Recursively(tmFileStorage.path_UserData())
                 .assert_True("UserData Folder failed to delete");
            
            tmFileStorage.path_UserData().assert_Folder_Doesnt_Exist();     
        }
        [Test] public void Manualy_Git_Commit_NewUsers()                        
        {
            var head1              = nGit.head();

            nGit.assert_Is_Not_Null();
            tmFileStorage.path_UserData().isGitRepository()
                                         .assert_Is_True();
            head1.assert_Not_Null();
            
            var tmUser      = userData.newUser().tmUser();            
            var userXmlFile = tmFileStorage.user_XmlFile_Location(tmUser).assert_File_Exists();
            var untracked   = nGit.status_Raw().untracked();

            untracked.assert_Size_Is(1)
                     .first().assert_Equal_To(@"Users/{0}".format(userXmlFile.fileName()));

            nGit.add_and_Commit_using_Status();            

            nGit.status_Raw().untracked().assert_Size_Is(0);
            nGit.head().assert_Not_Null()
                       .assert_Is_Not(head1);            
        }
        [Test] public void CheckGitRepoDoesNotCommit_OnNewUser()                
        {          
            Assert.NotNull(tmServer);
            Assert.IsTrue(tmServer.Git.UserData_Git_Enabled);
            
            nGit.status().assert_Empty();
            
            userData            .newUser();            // adding a user
            Assert.IsNotNull    (nGit.head());
            userData            .newUser();            // adding another user

            nGit.status().assert_Not_Empty();

            var headBeforeUser  = nGit.head();
            
            userData.newUser();            

            nGit.head().assert_Not_Null()
                       .assert_Equal(headBeforeUser, "Git Head value are still the same here");
                        
            nGit.commits().assert_Size_Is(2);

            userDataGit.triggerGitCommit();                 // manually trigger the commit
            nGit.status().assert_Empty();
            nGit.commits().assert_Size_Is(3);

            UserGroup.None.assert(); 
        }
        [Test] public void CheckGitRepo_DoesNotCommit_OnUserSave()              
        {            
            Assert.NotNull(tmServer);
            Assert.IsTrue(tmServer.Git.UserData_Git_Enabled);
             
            var tmUser          = userData.newUser().tmUser();
            var headBeforeSave  = nGit.head();
            
            tmUser.FirstName    = "New Name";
            tmFileStorage.saveTmUser   (tmUser);
            
            var headAfterSave   = nGit.head();

            Assert.AreEqual   (headBeforeSave, headAfterSave, "Git Head value should be the same after a TMUser save");
            Assert.IsNotEmpty (nGit.status());
            
            userDataGit.triggerGitCommit();                   //manuall trigger the git commit 
            var headAfterCommit = nGit.head();
            Assert.AreNotEqual(headAfterCommit, headAfterSave, "Git Head value should be different after triggerGitCommit");
            Assert.IsEmpty    (nGit.status());
        }
        [Test] public void CheckGitRepo_DoesNot_Commit_OnUserAddAndDelete()     
        {
            var commitsBeforeNewUser    = nGit.commits().size();         
            var tmUser                  = userData.newUser().tmUser();
            var commitsAfterNewUser     = nGit.commits().size();
            tmUser                      .deleteTmUser();
            var commitsAfterDeleteUser  = nGit.commits().size();

            Assert.AreEqual(2, commitsBeforeNewUser,    "There should be 2 commits before user create");
            Assert.AreEqual(2, commitsAfterNewUser    , "There should be 2 commits after user create");
            Assert.AreEqual(2, commitsAfterDeleteUser , "There should be 2 commits after user delete");                                  
            Assert.IsEmpty(nGit.status());

            UserGroup.None.assert(); 
            
        }
        [Test] public void CheckGitRepo_DoesNotCommit_OnActivites()             
        {
            nGit.commits().assert_Size_Is(2);
            nGit.status ().assert_Empty();
            var tmUser = userData.newUser().tmUser();   

            nGit.commits().assert_Size_Is(2);
            var status1 = nGit.status ().assert_Not_Empty();

            tmUser.logUserActivity("testAction", "testDetail");

            nGit.commits().assert_Size_Is(2);
            nGit.status ().assert_Not_Empty()
                          .assert_Equal_To(status1);

            userData.tmUser("admin").assert_Not_Null()
                                    .logUserActivity("an admin","activity");
            
            nGit.commits().assert_Size_Is(2);
            nGit.status ().assert_Not_Empty()
                          .assert_Not_Equal_To(status1);                                            
        }
        
        
        [Ignore("BUG - To fix")]
        [Test] public void BUG_Check_That_Git_Commit_Happens_On_UserData_Load()     
        {
            tmFileStorage.path_UserData().isGitRepository().assert_True();            

            nGit.status ().assert_Empty    ( );
            nGit.commits().assert_Size_Is  (2);            
            
            userData.newUser();                         // this should not trigger an git commit

            nGit.status ().assert_Not_Empty( );
            nGit.commits().assert_Size_Is  (2);
            admin.assert(()=>tmFileStorage.load_UserData());              // this should trigger an git commit
            
            nGit.status ().assert_Empty    ( );
            nGit.commits().assert_Size_Is  (2);            
        }
    }
}