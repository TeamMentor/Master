using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.Database
{
    [TestFixture]
    public class Test_TM_Server_Utils
    {
        [Test]
        public void tmServer()
        {
            var userData = new TM_UserData();
            Assert.NotNull(userData.Server);
            userData.Server = null;
            Assert.IsNull(userData.Server);
            Assert.IsNull((null as TM_UserData).tmServer());
        }
    }
}
