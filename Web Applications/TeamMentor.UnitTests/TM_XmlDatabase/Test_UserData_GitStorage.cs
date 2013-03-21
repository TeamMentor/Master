using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture][Ignore("Git User doesn't happen on LocalRequests")]
    public class Test_UserData_GitStorage
    {
        public TM_UserData  userData;
        public API_NGit     nGit;
        [SetUp]
        public void setUp()
        {
            //create temp repo with no Admin user
            userData = new TM_UserData(true)
                                {
                                    Path_UserData = "nonGitRepo".tempDir()
                                };                                    
            userData .SetUp(); 
            nGit     = userData.NGit;     

            Assert.AreEqual(1, nGit.commits().size() , "there should be one commit of the TMSecretData.config file");
        }

        [Test][Assert_Admin]
        [Ignore("Fix when Git support for libraries is fixed")]
        public void CheckNonGitRepoDoesntCommit()
        {
            userData.Path_UserData = "nonGitRepo".tempDir();
            userData.AutoGitCommit = false;

            Assert.IsTrue   (userData.UsingFileStorage);
            Assert.IsTrue   (userData.Path_UserData.dirExists());            
            Assert.IsEmpty  (userData.Path_UserData.files());

            userData.SetUp()
                    .newUser();

            var users = userData.tmUsers();
            Assert.IsNotEmpty(users, "There should be at least one user (the admin)");
            Assert.IsNotEmpty(userData.Path_UserData.files());                        
            Assert.AreEqual  (3,userData.Path_UserData.files().size());
            Assert.IsFalse   (userData.Path_UserData.isGitRepository());            
        }
        [Test][Ignore("Rewrite to take into account new Git Storage")]
        [Assert_Admin] public void ManualyGitCommitNewUsers()
        {
            userData.AutoGitCommit = false;
            var head1              = nGit.head();

            Assert.IsNotNull(nGit);
            Assert.IsTrue   (userData.Path_UserData.isGitRepository());
            Assert.IsFalse   (head1.isNull());
            
            var tmUser      = userData.newUser().tmUser();            
            var userXmlFile = tmUser.getTmUserXmlFile().fileName();
            var untracked   = nGit.status_Untracked();            
            
            Assert.AreEqual (1,untracked.size());
            Assert.AreEqual (userXmlFile, untracked.first());

            nGit.add_and_Commit_using_Status();            
            untracked         = nGit.status_Untracked();
            var head2         = nGit.head();
            Assert.AreEqual   (0,untracked.size());
            Assert.IsFalse    (nGit.head().isNull());
            Assert.AreNotEqual(head1, head2);
            "Head is now: {0}".info(nGit.head());
        }
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnNewUser()
        {                        
            Assert.IsTrue       (userData.AutoGitCommit);
            
            userData            .newUser();            // adding a user
            Assert.IsNotNull    (nGit.head());
            userData            .newUser();            // adding another user

            var headBeforeUser  = nGit.head();
            userData            .newUser();
            var headAfterUser   = nGit.head();

            Assert.IsFalse      (nGit.head().isNull());
            Assert.AreNotEqual  (headBeforeUser, headAfterUser , "Git Head value should be different after a TMUser create");
            Assert.AreNotEqual  (2, nGit.commits().size());
        }
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnUserSave()
        {
            Assert.IsTrue       (userData.AutoGitCommit);
             
            var tmUser          = userData.newUser().tmUser();
            var headBeforeSave  = nGit.head();
            
            tmUser.FirstName    = "New Name";
            tmUser.saveTmUser   ();
            
            var headAfterSave   = nGit.head();

            Assert.AreNotEqual  (headBeforeSave, headAfterSave , "Git Head value should be different after a TMUser save");
        }
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnUserAddAndDelete()
        {                        
            var tmUser                  = userData.newUser().tmUser();
            var commitsAfterNewUser     = nGit.commits().size();
            tmUser                      .deleteTmUser();
            var commitsAfterDeleteUser  = nGit.commits().size();

            nGit.refLogs().toString().info();

            Assert.AreEqual(2, commitsAfterNewUser    , "There should be 2 commits after user create");
            Assert.AreEqual(3, commitsAfterDeleteUser , "There should be 3 commits after user delete");
            
            Assert.IsEmpty(nGit.status());
            
        }
        [Test][Assert_Admin] public void CheckActivitiesLogging()
        {
            var tmUser = userData.newUser().tmUser();            
            Assert.AreEqual(2, nGit.commits().size());
            var sessionId = tmUser.login();
            Assert.AreNotEqual(Guid.Empty, sessionId);
        }
    }
}
