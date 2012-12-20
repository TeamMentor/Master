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
		[TestMethod]	public void Test_User()
		{
			var user = IRESTAdmin.user("admin");
			Assert.IsNotNull(user, "user object was null");
			Assert.IsTrue(user.UserName.valid(), "userUserName was not vaild");
			//"UserData : {0}".writeLine_Trace(user.serialize(false));

			var userById = IRESTAdmin.user(user.UserId.str());
			Assert.IsNotNull(userById, "userById object was null");
			Assert.IsTrue(userById.UserName.valid(), "userById.UserName was not vaild");
		}
		[TestMethod]	public void Test_GetUsers()
		{
			var users = IRESTAdmin.users();
			Assert.IsNotNull(users, "users object was null");
			Assert.IsTrue(users.size() > 0 , "users object was empty");
			"There were {0} users fetched: {0}".writeLine_Trace(users.size());
		}		
	}
}
