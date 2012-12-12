using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{
	[TestClass]
	public class RBAC_Functions : RestClass_Direct
	{
		[TestMethod]	public void Test_GetUser_byName()
		{
			var user = IrestAdmin.GetUser_byName("admin");
			Assert.IsNotNull(user, "user object was null");
			Assert.IsTrue(user.UserName.valid(),"UserName was not vaild");
			"UserData : {0}".writeLine_Trace(user.serialize(false));
		}
		[TestMethod]	public void Test_GetUsers()
		{
			var users = IrestAdmin.GetUsers();
			Assert.IsNotNull(users, "users object was null");
			Assert.IsTrue(users.size() > 0 , "users object was empty");
			"There were {0} users fetched: {0}".writeLine_Trace(users.size());
		}
		[TestMethod]	public void Test_GetUser_byID()
		{
			var user = IrestAdmin.GetUser_byName("admin");
			var user2 = IrestAdmin.GetUser_byID(user.UserId.str());
			Assert.AreEqual(user.UserName, user2.UserName, "user names didn't match");			
		}
	}
}
