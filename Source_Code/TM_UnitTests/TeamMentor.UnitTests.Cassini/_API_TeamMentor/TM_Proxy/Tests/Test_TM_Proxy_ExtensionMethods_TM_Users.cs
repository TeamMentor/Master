using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini._API_TeamMentor.TM_Proxy.Tests
{
    [TestFixture]
    public class Test_TM_Proxy_ExtensionMethods_TM_Users : NUnitTests_Cassini_TeamMentor
    {
        [SetUp]
        public void setUp()
        {
            this.tmProxy_Refresh();
        }
        [Test] public void user()
        {
            var tmUser = tmProxy.user("admin").assert_Not_Null();
            tmProxy.user(tmUser.UserID) .assert_Not_Null()
                                        .assert_Equal(tmUser)
                   .UserID.user(tmProxy).assert_Not_Null()
                                        .assert_Equal(tmUser);
        }
        [Test] public void user_New()
        {
            tmProxy.user_New()             .assert_Not_Null()
                   .UserID   .user(tmProxy).assert_Not_Null()
                   .UserName .user(tmProxy).assert_Not_Null();

            tmProxy.user_New(5.randomLetters()).assert_Not_Null()
                   .UserID   .user(tmProxy)    .assert_Not_Null()
                   .UserName .user(tmProxy)    .assert_Not_Null();

            var userName = 10.randomLetters();
            var password = "!abc!".add_5_RandomLetters();
            tmProxy.user_New(userName, password).assert_Not_Null()
                   .UserID   .user(tmProxy)     .assert_Not_Null()
                   .UserName .user(tmProxy)     .assert_Not_Null();
        }
    }
}
