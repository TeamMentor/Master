using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.WebServices
{
    [TestFixture] 
    public class Test_API_TM_WebServices : API_TM_WebServices
    {
        [SetUp]
        public void setup()
        {
            if (WebSite_Url.HEAD().isFalse())
                Assert.Ignore("TM server is offline");

            Assert.IsTrue(this.logout());
        }
        [Test] public void Test_login_As_Admin()
        {
            Assert.IsNull   (this.current_User());
            Assert.IsTrue   (this.login_As_Admin());
            Assert.IsNotNull(this.current_User());
            Assert.AreEqual (this.current_User().UserName, Tests_Consts.DEFAULT_ADMIN_USERNAME);
        }
    }
}
