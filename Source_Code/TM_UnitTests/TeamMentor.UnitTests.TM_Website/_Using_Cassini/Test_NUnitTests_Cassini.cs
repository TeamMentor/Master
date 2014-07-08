using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture] 
    public class Test_NUnitTests_Cassini
    {        
        [Test] public void NUnitTests_Cassini_Ctor()
        {
            // Checks that the Ctor doesn't start the server
            var nUnitTests_Cassini = new NUnitTests_Cassini();
            nUnitTests_Cassini.apiCassini.assert_Null();
            nUnitTests_Cassini.webRoot   .assert_Folder_Not_Exists();
            nUnitTests_Cassini.port      .assert_Default();
        }
        [Test] public void start()         
        {
            // stop() is also tests here

            var nUnitTests_Cassini = new NUnitTests_Cassini();
            nUnitTests_Cassini.apiCassini.assert_Null();
            nUnitTests_Cassini.webRoot   .assert_Folder_Not_Exists();
            nUnitTests_Cassini.port      .assert_Default();

            nUnitTests_Cassini.start();
            
            nUnitTests_Cassini.port      .tcpClient().assert_Not_Null();

            nUnitTests_Cassini.stop();
            nUnitTests_Cassini.port      .tcpClient().assert_Null();  
            nUnitTests_Cassini.webRoot.assert_Folder_Not_Exists();
        }
    }
}