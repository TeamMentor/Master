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
        public TM_UserData_Git userData;
        public API_NGit     nGit;

        [SetUp]
        public void setUp()
        {
            TM_Xml_Database.Current = null;
            //create temp repo with no Admin user
            userData = new TM_UserData_Git(true)
                                {
                                    Path_UserData = "nonGitRepo".tempDir()
                                };                                    
            //userData .loadData(); 
            nGit     = userData.NGit;     

            Assert.AreEqual(1, nGit.commits().size() , "there should be one commits here");

            Assert.NotNull(userData);
            Assert.IsNull(TM_Xml_Database.Current);
            Assert.IsNull(TM_Xml_Database.Current.tmServer());
            //Assert.IsNull(TM_Xml_Database.Current.tmServer().userData().Remote_GitPath);
        }

        [Test][Assert_Admin]
        //[Ignore("Fix when Git support for libraries is fixed")]
        public void CheckNonGitRepoDoesntCommit()
        {
            var tmServer = TM_Xml_Database.Current.tmServer();

            Assert.NotNull(tmServer);           // need a better way to expose the Git user settings to the UserData 

            tmServer.Git.UserData_Git_Enabled = false;

            userData.Path_UserData = "nonGitRepo".tempDir();            

            Assert.IsTrue   (userData.UsingFileStorage);
            Assert.IsTrue   (userData.Path_UserData.dirExists());            
            Assert.IsEmpty  (userData.Path_UserData.files());

            //userData.loadData()
            //        .newUser();

            var users = userData.tmUsers();
            Assert.IsNotEmpty(users, "There should be at least one user (the admin)");
            Assert.IsNotEmpty(userData.Path_UserData.files());                        
            Assert.AreEqual  (2,userData.Path_UserData.files(true).size());
            Assert.IsFalse   (userData.Path_UserData.isGitRepository());

            tmServer.Git.UserData_Git_Enabled = true;
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
            var userXmlFile = tmUser.user_XmlFile_Location().fileName();
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
        [Test][Assert_Admin] public void CheckGitRepoDoesCommit_OnNewUser()
        {
            var tmServer = TM_Xml_Database.Current.tmServer();

            Assert.NotNull(tmServer);
            Assert.IsTrue(tmServer.Git.UserData_Git_Enabled);
            
            userData            .newUser();            // adding a user
            Assert.IsNotNull    (nGit.head());
            userData            .newUser();            // adding another user

            var headBeforeUser  = nGit.head();
            userData            .newUser();
            var headAfterUser   = nGit.head();

            Assert.IsFalse    (nGit.head().isNull());
            Assert.AreNotEqual(headBeforeUser, headAfterUser, "Git Head value should now be different");    
            Assert.AreEqual   (4, nGit.commits().size());
            Assert.IsEmpty    (nGit.status());
        }
        [Test][Assert_Admin] public void CheckGitRepo_DoesNotCommit_OnUserSave()
        {
            var tmServer = TM_Xml_Database.Current.tmServer();
            Assert.NotNull(tmServer);
            Assert.IsTrue(tmServer.Git.UserData_Git_Enabled);
             
            var tmUser          = userData.newUser().tmUser();
            var headBeforeSave  = nGit.head();
            
            tmUser.FirstName    = "New Name";
            tmUser.saveTmUser   ();
            
            var headAfterSave   = nGit.head();

            Assert.AreEqual(headBeforeSave, headAfterSave, "Git Head value should be the same after a TMUser save");
            Assert.IsNotEmpty(nGit.status());
            
            userData.triggerGitCommit();
            var headAfterCommit = nGit.head();
            Assert.AreNotEqual(headAfterCommit, headAfterSave, "Git Head value should be different after triggerGitCommit");
            Assert.IsEmpty(nGit.status());
        }
        [Test][Assert_Admin] public void CheckGitRepo_DoesCommits_OnUserAddAndDelete()
        {
            var commitsBeforeNewUser    = nGit.commits().size();         
            var tmUser                  = userData.newUser().tmUser();
            var commitsAfterNewUser     = nGit.commits().size();
            tmUser                      .deleteTmUser();
            var commitsAfterDeleteUser  = nGit.commits().size();

            //nGit.refLogs().toString().info();

            Assert.AreEqual(1, commitsBeforeNewUser,    "There should be 1 commits before user create");
            Assert.AreEqual(2, commitsAfterNewUser    , "There should be 2 commits after user create");
            Assert.AreEqual(3, commitsAfterDeleteUser , "There should be 3 commits after user delete");                                  
            Assert.IsEmpty(nGit.status());
            
        }
        [Test][Assert_Admin] public void CheckGitRepo_DoesNotCommit_OnActivites()
        {
            var tmUser = userData.newUser().tmUser();            
            Assert.AreEqual(2, nGit.commits().size());
            tmUser.logUserActivity("testAction", "testDetail");
            Assert.IsNotEmpty(nGit.status());
            Assert.AreEqual(2, nGit.commits().size());
        }
    }
}