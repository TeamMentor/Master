using System;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_WebServices_Users: API_TM_WebServices
    {

        [SetUp]
        public void setup()
        {
            this.login_As_Admin();
            Assert.AreEqual(this.current_User().UserName, Tests_Consts.DEFAULT_ADMIN_USERNAME);
        }
        [Test]public void Check_GetUsers()
        {
            var users = this.users();
            Assert.Greater(users.size(), 1);            
        }        
    }
}
