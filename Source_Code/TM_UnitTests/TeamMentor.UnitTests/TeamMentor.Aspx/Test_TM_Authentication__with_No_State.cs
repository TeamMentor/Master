using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TeamMentor.Aspx
{
    [TestFixture]
    class Test_TM_Authentication__with_No_State
    {
        [Test]
        public void mapUserRoles()
        {
            TMConfig.Current = null;
            HttpContextFactory._context = null;
            var tmAuthentication = new TM_Authentication(null);
            Assert.AreEqual(tmAuthentication, tmAuthentication.mapUserRoles(false) , "This should not thrown an exception");

        }
    }
}
