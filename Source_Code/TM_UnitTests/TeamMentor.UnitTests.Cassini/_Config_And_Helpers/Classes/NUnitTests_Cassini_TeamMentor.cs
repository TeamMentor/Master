using FluentSharp.CassiniDev;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    public class NUnitTests_Cassini_TeamMentor : NUnitTests
    {
        public API_Cassini apiCassini;
        public string      webRoot;
        public int         port;
        [TestFixtureSetUp]        
        public void start()
        {
            webRoot    = this.teamMentor_Root_OnDisk().assert_Folder_Exists();
            apiCassini = new API_Cassini(webRoot);
            port       = apiCassini.port();
            apiCassini.webRoot().assert_Equal_To(webRoot);

            port      .tcpClient().assert_Null();
            apiCassini.start();
            port      .tcpClient().assert_Not_Null();
        }

        [TestFixtureTearDown]
        public void stop()
        {
            port      .tcpClient().assert_Not_Null();
            apiCassini.stop();
            port      .tcpClient().assert_Null();                        

            webRoot   .assert_Folder_Exists();          // make sure we didn't delete this by accident
        }

        [Test]
        public void Check_Cassini()
        {
            apiCassini.assert_Not_Null();
            webRoot   .assert_Not_Null();
            port      .assert_Bigger_Than(0);

        }
    }
}