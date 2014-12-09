using System;
using System.Collections.Generic;
using System.Security;
using FluentSharp.NUnit;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

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
            Assert.AreEqual		(userRoles.size()			, 3					, "userRoles size");
            Assert.AreEqual		(UserRole.ReadArticles      , userRoles[0]		, "first userRole");
            Assert.AreEqual		(UserRole.ReadArticlesTitles, userRoles[1]		, "second userRole");
            Assert.AreEqual		(UserRole.ViewLibrary       , userRoles[2]		, "third userRole");
        }

        [Test]
        public void RBAC_test_Security_Demands()
        {
            //None
            UserGroup.None.setThreadPrincipalWithRoles();
            Assert.Throws<SecurityException>(() => UserGroup.Reader				.demand(), "None: UserGroup.Reader Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Editor				.demand(), "None: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin				.demand(), "None: UserGroup.Admin Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Viewer				.demand(), "None: UserGroup.Viewer Demand");
            Assert.Throws<SecurityException>(() => UserRole	.Admin				.demand(), "None: UserRole.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole	.EditArticles		.demand(), "None: UserRole.EditArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole	.ReadArticles		.demand(), "None: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole	.ManageUsers		.demand(), "None: UserRole.ManageUsers Demand");
            Assert.Throws<SecurityException>(() => UserRole .ReadArticlesTitles	.demand(), "None: UserRole.ReadArticlesTitles Demand");

            //Viewer
            UserGroup.Viewer.setThreadPrincipalWithRoles();
            Assert.Throws<SecurityException>(() => UserGroup.Reader				.demand(), "Viewer: UserGroup.Reader Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Editor				.demand(), "Viewer: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin				.demand(), "Viewer: UserGroup.Admin  Demand");
            Assert.DoesNotThrow             (() => UserGroup.Viewer				.demand(), "Viewer: UserGroup.Viewer Demand");
            Assert.Throws<SecurityException>(() => UserRole	.Admin				.demand(), "Viewer: UserRole.Admin Viewer");            
            Assert.Throws<SecurityException>(() => UserRole	.EditArticles		.demand(), "Viewer: UserRole.EditArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole	.ReadArticles		.demand(), "Viewer: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole	.ManageUsers		.demand(), "Viewer: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow             (() => UserRole	.ViewLibrary		.demand(), "Viewer: UserRole.ViewLibrary Demand");
            Assert.DoesNotThrow(			 () => UserRole .ReadArticlesTitles	.demand(), "Viewer: UserRole.ReadArticlesTitles Demand");

            //Reader
            UserGroup.Reader.setThreadPrincipalWithRoles();
            Assert.DoesNotThrow(			 () => UserGroup.Reader				.demand(), "Reader: UserGroup.Reader Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Editor				.demand(), "Reader: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin				.demand(), "Reader: UserGroup.Admin Demand");
            Assert.DoesNotThrow             (() => UserGroup.Viewer				.demand(), "Viewer: UserGroup.Viewer Demand");
            Assert.Throws<SecurityException>(() => UserRole. Admin				.demand(), "Reader: UserRole.Admin Demand");
            Assert.Throws<SecurityException>(() => UserRole. EditArticles		.demand(), "Reader: UserRole.EditArticles Demand");
            Assert.DoesNotThrow(			 () => UserRole. ReadArticles		.demand(), "Reader: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole. ManageUsers		.demand(), "Reader: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow             (() => UserRole	.ViewLibrary		.demand(), "Reader: UserRole.ViewLibrary Demand");
            Assert.DoesNotThrow				(() => UserRole. ReadArticlesTitles	.demand(), "Reader: UserRole.ReadArticlesTitles Demand");

            //Editor
            UserGroup.Editor.setThreadPrincipalWithRoles();
            Assert.DoesNotThrow             (() => UserGroup.Reader             .demand(), "Editor: UserGroup.Reader Demand");
            Assert.DoesNotThrow             (() => UserGroup.Editor             .demand(), "Editor: UserGroup.Editor Demand");
            Assert.Throws<SecurityException>(() => UserGroup.Admin              .demand(), "Editor: UserGroup.Admin Demand");
            Assert.DoesNotThrow             (() => UserGroup.Viewer				.demand(), "Editor: UserGroup.Viewer Demand");
            Assert.Throws<SecurityException>(() => UserRole. Admin              .demand(), "Editor: UserRole.Admin Demand");
            Assert.DoesNotThrow             (() => UserRole. EditArticles       .demand(), "Editor: UserRole.EditArticles Demand");
            Assert.DoesNotThrow             (() => UserRole. ReadArticles       .demand(), "Editor: UserRole.ReadArticles Demand");
            Assert.Throws<SecurityException>(() => UserRole. ManageUsers        .demand(), "Editor: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow             (() => UserRole	.ViewLibrary		.demand(), "Editor: UserRole.ViewLibrary Demand");
            Assert.DoesNotThrow             (() => UserRole. ReadArticlesTitles .demand(), "Editor: UserRole.ReadArticlesTitles Demand");

            //Admin
            UserGroup.Admin.setThreadPrincipalWithRoles();
            Assert.DoesNotThrow             (() => UserGroup.Reader             .demand(), "Admin: UserGroup.Reader Demand");
            Assert.DoesNotThrow             (() => UserGroup.Editor             .demand(), "Admin: UserGroup.Editor Demand");
            Assert.DoesNotThrow             (() => UserGroup.Admin              .demand(), "Admin: UserGroup.Admin Demand");
            Assert.DoesNotThrow             (() => UserGroup.Viewer				.demand(), "Admin: UserGroup.Viewer Demand");
            Assert.DoesNotThrow             (() => UserRole.Admin               .demand(), "Admin: UserRole.Admin Demand");
            Assert.DoesNotThrow             (() => UserRole.EditArticles        .demand(), "Admin: UserRole.EditArticles Demand");
            Assert.DoesNotThrow             (() => UserRole.ReadArticles        .demand(), "Admin: UserRole.ReadArticles Demand");
            Assert.DoesNotThrow             (() => UserRole.ManageUsers         .demand(), "Admin: UserRole.ManageUsers Demand");
            Assert.DoesNotThrow             (() => UserRole	.ViewLibrary		.demand(), "Admin: UserRole.ViewLibrary Demand");
            Assert.DoesNotThrow             (() => UserRole.ReadArticlesTitles  .demand(), "Admin: UserRole.ReadArticlesTitles Demand");            
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
                       
            //None users can create users
            UserGroup.None.setThreadPrincipalWithRoles();

            var userId = createUser();
            Assert.That(userId > 0, "Anonymous: CreateUser");

            // confirm that new user role is 2 (Reader)
            UserGroup.Admin.setThreadPrincipalWithRoles();
            var tmUser = userData.tmUser(userId);
            Assert.AreEqual(tmUser.GroupID, 2, "Anonymous created user: group id");

            //only admins can delete user
            UserGroup.None	    .setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "None     : DeleteUser");
            UserGroup.Viewer    .setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "Viewer   : DeleteUser");
            UserGroup.Reader	.setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "Reader	: DeleteUser");
            UserGroup.Editor	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.deleteTmUser(userId), "Editor	: DeleteUser");
            UserGroup.Admin		.setThreadPrincipalWithRoles(); Assert.DoesNotThrow(			 () => userData.deleteTmUser(userId), "Admin    : DeleteUser");

            //check that only admins can create users with GroupId specificed			                        
            UserGroup.None  	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => createUser_In_Group(), "None      : CreateUser with groupd ID");
            UserGroup.Viewer 	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => createUser_In_Group(), "Viewer    : CreateUser with groupd ID");
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
            var batchUserCreation = "123,qwe,asd,zxc";			
            UserGroup.Viewer	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.createTmUsers(batchUserCreation), "Viewer : BatchUserCreation");
            UserGroup.Reader	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.createTmUsers(batchUserCreation), "Reader : BatchUserCreation");
            UserGroup.Editor	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(() => userData.createTmUsers(batchUserCreation), "Editor : BatchUserCreation");
            UserGroup.Admin		.setThreadPrincipalWithRoles(); Assert.DoesNotThrow(			 () => userData.createTmUsers(batchUserCreation), "Admin  : BatchUserCreation");
        }

        [Test]
        public void Check_UserGroup_Mappings()
        {
            var tmUser = new TMUser();
            Assert.IsTrue(tmUser.isViewer());

            tmUser.make_Viewer().assert_True (tmUser.isViewer)
                                .assert_False(tmUser.isReader)
                                .assert_False(tmUser.isEditor)
                                .assert_False(tmUser.isAdmin );

            tmUser.make_Reader().assert_False(tmUser.isViewer)
                                .assert_True (tmUser.isReader)
                                .assert_False(tmUser.isEditor)
                                .assert_False(tmUser.isAdmin );
            
            tmUser.make_Editor().assert_False(tmUser.isViewer)
                                .assert_False(tmUser.isReader)
                                .assert_True (tmUser.isEditor)
                                .assert_False(tmUser.isAdmin );

            tmUser.make_Admin().assert_False (tmUser.isViewer)
                               .assert_False (tmUser.isReader)
                               .assert_False (tmUser.isEditor)
                               .assert_True  (tmUser.isAdmin );            
        }

    }
}
