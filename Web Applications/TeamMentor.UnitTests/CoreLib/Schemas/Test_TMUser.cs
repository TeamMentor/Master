using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Schemas
{
    [TestFixture]
    public class Test_TMUser
    {
        [Test]
        public void TMUser_Ctor()
        {
            var tmUser = new TMUser();
            Assert.AreNotEqual(Guid.NewGuid(), tmUser.ID);
            //SecretData
            Assert.IsNotNull  (tmUser.SecretData);
            Assert.IsNull     (tmUser.SecretData.PasswordHash);
            Assert.IsNull     (tmUser.SecretData.PasswordResetToken);
            Assert.IsNull     (tmUser.SecretData.DecryptionKey);
            Assert.IsNull     (tmUser.SecretData.CSRF_Token);
            //Sessions
            Assert.IsNotNull  (tmUser.Sessions);
            Assert.IsEmpty    (tmUser.Sessions);
            //AuthTokens
            Assert.IsNotNull  (tmUser.AuthTokens);
            Assert.IsEmpty    (tmUser.AuthTokens);
            //UserActivities
            Assert.IsNotNull  (tmUser.UserActivities);
            Assert.IsEmpty    (tmUser.UserActivities);
            //AccountStatus
            Assert.IsNotNull  (tmUser.AccountStatus);
            Assert.AreEqual   (tmUser.AccountStatus.ExpirationDate.ToLongDateString(), TMConfig.Current.currentExpirationDate().ToLongDateString());
            Assert.IsFalse    (tmUser.AccountStatus.PasswordExpired);
            Assert.IsTrue     (tmUser.AccountStatus.UserEnabled);
            //Stats
            Assert.IsNotNull  (tmUser.Stats);
            Assert.AreEqual   (tmUser.Stats.CreationDate.ToLongDateString(), DateTime.Now.ToLongDateString());            
        }
    }
}
