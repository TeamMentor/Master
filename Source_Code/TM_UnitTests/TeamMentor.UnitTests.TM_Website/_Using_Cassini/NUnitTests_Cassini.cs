using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    public class NUnitTests_Cassini : NUnitTests
    {
        public API_Cassini apiCassini;
        public string      webRoot;
        public int         port;

        [TestFixtureSetUp]        
        public void start()
        {       
            apiCassini.assert_Null();
            webRoot   .assert_Folder_Not_Exists();
            port      .assert_Default();

            apiCassini = new API_Cassini();
            webRoot    = apiCassini.webRoot();
            port       = apiCassini.port();
                        

            webRoot   .assert_Folder_Exists();
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
              
          
            Files.deleteFolder(webRoot, true).assert_True("webRoot could not be deleted"); 
            webRoot.folder_Wait_For_Deleted();
            //   webRoot.parentFolder().startProcess();
            webRoot.assert_Folder_Not_Exists();
            // 
        }        
    }
}