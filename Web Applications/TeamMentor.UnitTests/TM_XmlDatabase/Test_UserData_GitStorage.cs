using System;
using FluentSharp.Git;
using FluentSharp.Git.APIs;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]//[Ignore("Git User doesn't happen on LocalRequests")]
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

            Assert.AreEqual(2, nGit.commits().size() , "there should be two commits here");
        }

        [Test][Assert_Admin]
        //[Ignore("Fix when Git support for libraries is fixed")]
        public void CheckNonGitRepoDoesntCommit()
        {
            TMConfig.Current.Git.UserData_Git_Enabled = false;

            userData.Path_UserData = "nonGitRepo".tempDir();            

            Assert.IsTrue   (userData.UsingFileStorage);
            Assert.IsTrue   (userData.Path_UserData.dirExists());            
            Assert.IsEmpty  (userData.Path_UserData.files());

            userData.SetUp()
                    .newUser();

            var users = userData.tmUsers();
            Assert.IsNotEmpty(users, "There should be at least one user (the admin)");
            Assert.IsNotEmpty(userData.Path_UserData.files());                        
            Assert.AreEqual  (3,userData.Path_UserData.files(true).size());
            Assert.IsFalse   (userData.Path_UserData.isGitRepository());
            
            TMConfig.Current.Git.UserData_Git_Enabled = true;
        }
/*
        [Test]
        //[Ignore("Rewrite to take into account new Git Storage")]
        //todo: fix compilation error

        [Assert_Admin] public void ManualyGitCommitNewUsers()
        {
            TMConfig.Current.Git.UserData_Git_Enabled = false;

            var head1              = nGit.head();

            Assert.IsNotNull(nGit);
            Assert.IsTrue   (userData.Path_UserData.isGitRepository());
            Assert.IsFalse   (head1.isNull());
            
            var tmUser      = userData.newUser().tmUser();            
            var userXmlFile = tmUser.getTmUserXmlFile().fileName();
            var untracked   = nGit.status_Untracked();            
            
            Assert.AreEqual (1,untracked.size());
            Assert.AreEqual (userXmlFile, untracked.first().fileName());

            nGit.add_and_Commit_using_Status();            
            untracked         = nGit.status_Untracked();
            var head2         = nGit.head();
            Assert.AreEqual   (0,untracked.size());
            Assert.IsFalse    (nGit.head().isNull());
            Assert.AreNotEqual(head1, head2);
            "Head is now: {0}".info(nGit.head());

            TMConfig.Current.Git.UserData_Git_Enabled = true;
        }*/
        [Test][Assert_Admin] public void CheckGitRepoDoesCommits_OnNewUser()
        {                        
            Assert.IsTrue       (userData.tmConfig().Git.UserData_Git_Enabled);
            
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
            Assert.IsTrue       (userData.tmConfig().Git.UserData_Git_Enabled);
             
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

            Assert.AreEqual(3, commitsAfterNewUser    , "There should be 3 commits after user create");
            Assert.AreEqual(4, commitsAfterDeleteUser , "There should be 3 commits after user delete");
            
            Assert.IsEmpty(nGit.status());
            
        }
        [Test][Assert_Admin] public void CheckActivitiesLogging()
        {
            var tmUser = userData.newUser().tmUser();            
            Assert.AreEqual(3, nGit.commits().size());
            var sessionId = tmUser.login();
            Assert.AreNotEqual(Guid.Empty, sessionId);
        }
    }
}