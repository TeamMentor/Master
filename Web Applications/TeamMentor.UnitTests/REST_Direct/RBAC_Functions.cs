using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST
{
    [TestFixture]
    public class RBAC_Functions : TM_Rest_Direct
    {
        public RBAC_Functions()
        {
            UserGroup.Admin.setThreadPrincipalWithRoles();
        }

        [Test]	public void Test_User()
        {
            var user = TmRest.user("admin");
            Assert.IsNotNull(user, "user object was null");
            Assert.IsTrue(user.UserName.valid(), "userUserName was not vaild");
            //"UserData : {0}".writeLine_Trace(user.serialize(false));

            var userById = TmRest.user(user.UserId.str());
            Assert.IsNotNull(userById, "userById object was null");
            Assert.IsTrue(userById.UserName.valid(), "userById.UserName was not vaild");
        }

        [Test]	
        public void Test_GetUsers()
        {            
            var users = TmRest.users();
            Assert.IsNotNull(users, "users object was null");
            Assert.IsTrue(users.size() > 0 , "users object was empty");
            "There were {0} users fetched: {0}".info(users.size());
        }		
    }
}
