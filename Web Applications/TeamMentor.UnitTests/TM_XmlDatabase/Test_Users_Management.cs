using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.XmlDatabase_InMemory
{
	[TestFixture]
	class Test_Users_Management : TM_XmlDatabase_InMemory
	{		
		[TestFixtureSetUp]
		public void setUp()
		{
			UserGroup.Admin.setThreadPrincipalWithRoles();	
		}

		[Test]
		public void CreateLoginDelete_User()
		{			
			string testUserName = "testUser_".add_RandomLetters(5);
			string password = "123";

			Assert.IsNull(tmXmlDatabase.tmUser(testUserName), "testUserName shouldn't exist before create");
			//create user
			var userCount = tmXmlDatabase.UserData.TMUsers.size();
			var userId = tmXmlDatabase.newUser(testUserName, password);			
			Assert.AreEqual(userCount+1, tmXmlDatabase.UserData.TMUsers.size() , "TMUsers,size() after create");
			var tmUser = tmXmlDatabase.tmUser(userId);
			Assert.NotNull(tmUser, "tmUser was null after create");
			Assert.AreEqual(tmUser.UserID, userId, "UserID");
			Assert.AreEqual(tmUser.UserName, testUserName, "UserID");

			//Login with new user
			var sessionId = tmXmlDatabase.login(testUserName, password);
			Assert.AreNotEqual(Guid.Empty, sessionId, "empty session id");
			sessionId = tmXmlDatabase.login(testUserName, "BAD PWD");
			Assert.AreEqual(Guid.Empty, sessionId, "session id should be empty");

			//delete user			
			var result = tmXmlDatabase.deleteTmUser(tmUser.UserID);
			Assert.That(result, "User delete");
			tmUser = tmXmlDatabase.tmUser(userId);
			Assert.IsNull(tmUser, "tmUser should be null after delete");			
		}
	}
}
