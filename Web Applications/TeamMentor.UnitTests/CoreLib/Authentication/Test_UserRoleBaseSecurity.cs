using System;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Authentication 
{
    [TestFixture]
    public class Test_UserRoleBaseSecurity : TM_XmlDatabase_InMemory
    {
        public UserRoleBaseSecurity userRoleBaseSecurity;        

        public Test_UserRoleBaseSecurity()
        {
            userRoleBaseSecurity       = new UserRoleBaseSecurity();
            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
            //userData = new TM_UserData();
            Assert.NotNull(userRoleBaseSecurity);
            Assert.NotNull(HttpContextFactory.Current);
            
        }
        [Test]
        public void MapRolesBasedOnSessionGuid()
        {
            
            Assert.AreEqual(UserGroup.None, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(null         ));
            Assert.AreEqual(UserGroup.None, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(""           ));
            Assert.AreEqual(UserGroup.None, userRoleBaseSecurity.MapRolesBasedOnSessionGuid("aaaaa"      ));
            Assert.AreEqual(UserGroup.None, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(Guid.Empty   ));
            Assert.AreEqual(UserGroup.None, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(Guid.NewGuid()));
            Assert.AreEqual(UserGroup.None, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(Guid.NewGuid().str()));
                
            // create temp user
            var newUsedId   = userData.newUser();
            var tmUser      = newUsedId.tmUser();            
            tmUser.GroupID  = (int)UserGroup.Editor;
            var userSession = tmUser.add_NewSession();

            //check user session
            Assert.NotNull    (userSession);
            Assert.NotNull    (userSession.IpAddress);
            Assert.AreNotEqual(Guid.Empty, userSession.SessionID);
            Assert.AreEqual   (DateTime.Now.ToShortDateString(), userSession.CreationDate.ToShortDateString());
            Assert.IsTrue     (userSession.SessionID.validSession());
            Assert.AreEqual   (userSession.SessionID.session_TmUser   (), tmUser);
            Assert.AreEqual   (userSession.SessionID.session_UserName (), tmUser.UserName);
            Assert.AreEqual   (userSession.SessionID.session_UserGroup(), tmUser.userGroup());
            Assert.AreEqual   (userSession.SessionID.session_UserRoles(), tmUser.userRoles());

            //check MapRolesBasedOnSessionGuid
            Assert.AreEqual(UserGroup.Editor, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(userSession.SessionID));
            tmUser.GroupID = (int)UserGroup.Admin;
            Assert.AreEqual(UserGroup.Admin, userRoleBaseSecurity.MapRolesBasedOnSessionGuid(userSession.SessionID));

            //delete temp user
            Assert.IsTrue     (tmUser.deleteTmUser());
            Assert.IsNull     (newUsedId.tmUser());
        }

        [Test]
        public void currentIdentity_Name()
        {
            Assert.AreEqual("TM_User", userRoleBaseSecurity.currentIdentity_Name());
        }

        [Test]
        public void currentIdentity_IsAuthenticated()
        {
            Assert.IsTrue(userRoleBaseSecurity.currentIdentity_IsAuthenticated());
        }
    }
}
