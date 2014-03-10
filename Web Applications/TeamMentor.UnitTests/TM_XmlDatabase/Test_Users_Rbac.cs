using System;
using System.Security;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    class Test_Users_Rbac : TM_XmlDatabase_InMemory
    {
        [Test]
        public void Check_LoggedIn_UserRoles()
        {			
            var user = "testUser_".add_RandomLetters(5);
            var pwd = "bb";

            var userId = userData.newUser(user, pwd);
            Assert.Greater(userId, 0, "userID");
            
            var sessionId = userData.login(user, pwd);
            var userGroup = sessionId.session_UserGroup();					//new users currently default to Reader
            var userRoles = sessionId.session_UserRoles();

            Assert.AreNotEqual	(sessionId					, Guid.Empty		, "sessionId was empty");			
            Assert.AreEqual		(userGroup					, UserGroup.Reader	, "user group was not reader");			
            Assert.AreEqual		(userRoles.size()			, 2					, "userRoles size");
            Assert.AreEqual		(UserRole.ReadArticlesTitles, userRoles[0]		, "first userRole");
        }

        [Test]
        public void RBAC_test_Security_Demands()
        {
            //anonymous
            UserGroup.Anonymous.setThreadPrincipalWithRoles();
            Assert.Throws<SecurityException>(() => UserGroup.Reader				.demand(), "Anonymous: UserGroup.Reader Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Editor				.demand(), "Anonymous: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin				.demand(), "Anonymous: UserGroup.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole	.Admin				.demand(), "Anonymous: UserRole.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole	.EditArticles		.demand(), "Anonymous: UserRole.EditArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole	.ReadArticles		.demand(), "Anonymous: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole	.ManageUsers		.demand(), "Anonymous: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow(			 () => UserRole .ReadArticlesTitles	.demand(), "Anonymous: UserRole.ReadArticlesTitles Demand");

            //Reader
            UserGroup.Reader.setThreadPrincipalWithRoles();
            Assert.DoesNotThrow(			 () => UserGroup.Reader				.demand(), "Reader: UserGroup.Reader Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Editor				.demand(), "Reader: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin				.demand(), "Reader: UserGroup.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole. Admin				.demand(), "Reader: UserRole.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole. EditArticles		.demand(), "Reader: UserRole.EditArticles Demand");
            Assert.DoesNotThrow(			 () => UserRole. ReadArticles		.demand(), "Reader: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole. ManageUsers		.demand(), "Reader: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow				(() => UserRole. ReadArticlesTitles	.demand(), "Reader: UserRole.ReadArticlesTitles Demand");

            //Editor
            UserGroup.Editor.setThreadPrincipalWithRoles();
            Assert.DoesNotThrow(() => UserGroup.Reader.demand(), "Editor: UserGroup.Reader Demand");
            Assert.DoesNotThrow(() => UserGroup.Editor.demand(), "Editor: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin.demand(), "Editor: UserGroup.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole.Admin.demand(), "Editor: UserRole.Admin Demand");
            Assert.DoesNotThrow(() => UserRole.EditArticles.demand(), "Editor: UserRole.EditArticles Demand");
            Assert.DoesNotThrow(() => UserRole.ReadArticles.demand(), "Editor: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole.ManageUsers.demand(), "Editor: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow(() => UserRole.ReadArticlesTitles.demand(), "Editor: UserRole.ReadArticlesTitles Demand");

            //Admin
            UserGroup.Admin.setThreadPrincipalWithRoles();
            Assert.DoesNotThrow(() => UserGroup.Reader.demand(), "Admin: UserGroup.Reader Demand");
            Assert.DoesNotThrow(() => UserGroup.Editor.demand(), "Admin: UserGroup.Editor Demand");
            Assert.DoesNotThrow(() => UserGroup.Admin.demand(), "Admin: UserGroup.Admin Demand");
            Assert.DoesNotThrow(() => UserRole.Admin.demand(), "Admin: UserRole.Admin Demand");
            Assert.DoesNotThrow(() => UserRole.EditArticles.demand(), "Admin: UserRole.EditArticles Demand");
            Assert.DoesNotThrow(() => UserRole.ReadArticles.demand(), "Admin: UserRole.ReadArticles Demand");
            Assert.DoesNotThrow(() => UserRole.ManageUsers.demand(), "Admin: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow(() => UserRole.ReadArticlesTitles.demand(), "Admin: UserRole.ReadArticlesTitles Demand");


            //check string based demands
            Assert.DoesNotThrow(() => "ReadArticles".demand(), "Reader: string Demand #1");
            Assert.DoesNotThrow(() => "readArticles".demand(), "Reader: string Demand #1");
            Assert.DoesNotThrow(() => "readarticles".demand(), "Reader: string Demand #1");
            Assert.Throws<SecurityException>(() => "_readarticles".demand(), "Reader: string Demand #1");
            Assert.Throws<SecurityException>(() => "Reader".demand(), "Reader: string Demand #1");
        }

        [Test]
        public void RBAC_UserCreation()
        {
            var tempUserName = "test_user_".add_RandomLetters(4);
            var tmpPassword  = "".add_RandomLetters(20);
            var tmpEmail     = "testUser@teammentor.net";
            var testGroupId = 10;

            Func<int> createUser		  = () => userData.newUser(tempUserName, tmpPassword);
            Func<int> createUser_In_Group = () => userData.newUser(tempUserName, tmpPassword, tmpEmail, testGroupId);	
            

            //Readers cannot get users
            //UserGroup.Reader.setThreadPrincipalWithRoles();
            //Assert.Throws<SecurityException>(() => userData.tmUser(111111111), "Reader: GetUser_byID");

            //Anonymous can create users
            UserGroup.Anonymous.setThreadPrincipalWithRoles();

            var userId = createUser();
            Assert.That(userId > 0, "Anonymous: CreateUser");

            // confirm that new user role is 2 (Reader)
            UserGroup.Admin.setThreadPrincipalWithRoles();
            var tmUser = userData.tmUser(userId);
            Assert.AreEqual(tmUser.GroupID, 2, "Anonymous created user: group id");

            //only admins can delete user
            UserGroup.Anonymous	.setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "Anonymous: DeleteUser");
            UserGroup.Reader	.setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "Reader	 : DeleteUser");
            UserGroup.Editor	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "Editor	 : DeleteUser");
            UserGroup.Admin		.setThreadPrincipalWithRoles(); Assert.DoesNotThrow(			 () => userData.deleteTmUser(userId), "Admin    : DeleteUser");

            //check that only admins can create users with GroupId specificed			
            
            //newUser = new NewUser();
            //newUser.username = "test_user_".add_RandomLetters(4);
            
            UserGroup.Anonymous	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => createUser_In_Group(), "Anonnymous: CreateUser with groupd ID");
            UserGroup.Reader	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => createUser_In_Group(), "Reader	 : CreateUser with groupd ID");
            UserGroup.Editor	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => createUser_In_Group(), "Editor	 : CreateUser with groupd ID");
            UserGroup.Admin		.setThreadPrincipalWithRoles();
            userId = createUser_In_Group();
            Assert.That(userId > 0, "Admin: CreateUser with groupID");
            tmUser = userData.tmUser(userId);
            Assert.AreEqual(tmUser.GroupID, testGroupId, "Admin created user: group id");
            var result = userData.deleteTmUser(userId);
            Assert.That(result, "user delete failed");

            //check that only admins can call BatchUserCreation
            var batchUserCreation = "";			
            UserGroup.Anonymous	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.createTmUsers(batchUserCreation), "Anonymous: BatchUserCreation");
            UserGroup.Reader	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.createTmUsers(batchUserCreation), "Reader	  : BatchUserCreation");
            UserGroup.Editor	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.createTmUsers(batchUserCreation), "Editor   : BatchUserCreation");
            UserGroup.Admin		.setThreadPrincipalWithRoles(); Assert.DoesNotThrow(			 () => userData.createTmUsers(batchUserCreation), "Admin	  : BatchUserCreation");
        }

    }
}
