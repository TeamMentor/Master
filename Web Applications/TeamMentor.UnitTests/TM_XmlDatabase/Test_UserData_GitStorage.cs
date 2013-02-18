using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_UserData_GitStorage
    {

        [Test][Assert_Admin] public void CheckNonGitRepoDoesntCommit()
        {            
            var userData = new TM_UserData(true)
                                 {
                                    Path_UserData = "nonGitRepo".tempDir(),
                                    AutoGitCommit = false
                                 };                        

            Assert.IsTrue   (userData.UsingFileStorage);
            Assert.IsTrue   (userData.Path_UserData.dirExists());            
            Assert.IsEmpty  (userData.Path_UserData.files());

            userData.SetUp()
                    .newUser();

            var users = userData.tmUsers();
            Assert.IsNotEmpty(users, "There should be at least one user (the admin)");
            Assert.IsNotEmpty(userData.Path_UserData.files());                        
            Assert.AreEqual  (2,userData.Path_UserData.files().size());
            Assert.IsFalse   (userData.Path_UserData.isGitRepository());            
        }
        [Test][Assert_Admin] public void ManualyGitCommitNewUsers()
        {
            var userData    = new TM_UserData(true)
                                    {
                                        Path_UserData = "nonGitRepo".tempDir(),
                                        AutoGitCommit = false                       // so that the .newUser() call doesn't trigger a Git Commit
                                    };                        
        
            var nGit        = userData.Path_UserData.git_Init();

            Assert.IsNotNull(nGit);
            Assert.IsTrue   (userData.Path_UserData.isGitRepository());
            Assert.IsTrue   (nGit.head().isNull());
            
            var tmUser      = userData.newUser().tmUser();
            
            var userXmlFile = tmUser.getTmUserXmlFile().fileName();
            var untracked   = nGit.status_Untracked();
            
            Assert.AreEqual (1,untracked.size());
            Assert.AreEqual (userXmlFile, untracked.first());

            nGit.add_and_Commit_using_Status();
            
            untracked       = nGit.status_Untracked();
            Assert.AreEqual (0,untracked.size());
            Assert.IsFalse  (nGit.head().isNull());
                        
            "Head is now: {0}".info(nGit.head());
        }
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnNewUser()
        {
            var userData        = new TM_UserData(true)
                                        {
                                            Path_UserData = "nonGitRepo".tempDir()
                                        }                                    
                                        .SetUp();
            
            Assert.IsTrue        (userData.AutoGitCommit);

            var nGit            = userData.NGit;
            Assert.IsNotNull     (nGit.head());

            var headBeforeUser  = nGit.head();
            userData             .newUser();
            var headAfterUser   = nGit.head();

            Assert.IsFalse       (nGit.head().isNull());
            Assert.AreNotEqual   (headBeforeUser, headAfterUser , "Git Head value should be different after a TMUser create");
        }
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnUserSave()
        {
            var userData        = new TM_UserData(true)
                                        {
                                            Path_UserData = "nonGitRepo".tempDir()
                                        }                                    
                                        .SetUp();

            Assert.IsTrue       (userData.AutoGitCommit);
                        
            var nGit            = userData.NGit;           
            var tmUser          = userData.newUser().tmUser();
            var headBeforeSave  = nGit.head();
            
            tmUser.FirstName    = "New Name";
            tmUser.saveTmUser   ();
            
            var headAfterSave   = nGit.head();

            Assert.AreNotEqual  (headBeforeSave, headAfterSave , "Git Head value should be different after a TMUser save");
        }
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnUserAddAndDelete()
        {
            var userData        = new TM_UserData(true)
                                        {
                                            Path_UserData = "nonGitRepo".tempDir()                                            
                                        }                                    
                                        .SetUp(false);                        
                        
            var nGit                    = userData.NGit;           
            var tmUser                  = userData.newUser().tmUser();
            var commitsAfterNewUser     = nGit.commits().size();
            tmUser                      .deleteTmUser();
            var commitsAfterDeleteUser  = nGit.commits().size();

            nGit.refLogs().toString().info();

            Assert.AreEqual(1, commitsAfterNewUser    , "There should be 1 commits after user create");
            Assert.AreEqual(2, commitsAfterDeleteUser , "There should be 2 commits after user delete");
            
            Assert.IsEmpty(nGit.status());
            
        }
    }
}
