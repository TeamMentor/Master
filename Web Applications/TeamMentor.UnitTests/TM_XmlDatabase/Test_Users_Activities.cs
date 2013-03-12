using System;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_Users_Activities  : TM_XmlDatabase_InMemory
    {
        [Test]
        public void NewUserActivityData()
        {
            var name = "activity name";
            var detail = "activity detail";            
            var testUser = new TMUser();
            var userActivity = testUser.logUserActivity(name, detail);
            Assert.AreEqual(name, userActivity.Action);
            Assert.AreEqual(detail, userActivity.Detail);
            var whenAsDate = DateTime.FromFileTimeUtc(userActivity.When);
            Assert.AreEqual(userActivity.When, whenAsDate.ToFileTimeUtc());
        }
    }
}
