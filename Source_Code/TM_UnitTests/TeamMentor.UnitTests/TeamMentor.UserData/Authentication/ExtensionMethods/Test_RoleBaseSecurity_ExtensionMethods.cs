using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TeamMentor.UserData.Authentication.ExtensionMethods
{
    [TestFixture]
    public class Test_RoleBaseSecurity_ExtensionMethods
    {
        [Test] public void assert()
        {
            UserRole.Admin.assert       (()=>UserRole.Admin       .demand());
            UserRole.EditArticles.assert(()=>UserRole.EditArticles.demand());
            UserRole.ReadArticles.assert(()=>UserRole.ReadArticles.demand());
            UserRole.ViewLibrary.assert (()=>UserRole.ViewLibrary .demand());
            UserRole.None.assert        (()=>UserRole.None        .demand());

            Assert.Throws<SecurityException>(()=>UserRole.Admin.assert (()=>UserRole.ViewLibrary .demand()));
            Assert.Throws<SecurityException>(()=>UserRole.Admin.assert (()=>UserRole.EditArticles.demand()));
            Assert.Throws<SecurityException>(()=>UserRole.Admin.assert (()=>UserRole.ViewLibrary .demand()));
            Assert.Throws<SecurityException>(()=>UserRole.Admin.assert (()=>UserRole.None .demand()));
            
            Assert.DoesNotThrow             (()=>UserRole.None        .demand());
            Assert.Throws<SecurityException>(()=>UserRole.ViewLibrary .demand());

            // a call to assert should leave the UserRole in the current thread
            UserRole.ViewLibrary.assert();
            Assert.Throws<SecurityException>(()=>UserRole.None        .demand());
            Assert.DoesNotThrow             (()=>UserRole.ViewLibrary .demand());

            // a call to assert(Action) should NOT leave the UserRole in the current thread (i.e. it should be set to UserRole.None)
            UserRole.ViewLibrary.assert(()=>{});
            Assert.DoesNotThrow             (()=>UserRole.None        .demand());
            Assert.Throws<SecurityException>(()=>UserRole.ViewLibrary .demand());
        }
    }
}
