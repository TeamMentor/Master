using System;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    public class NUnitTests_Cassini_TeamMentor : NUnitTests
    {
        public API_Cassini              apiCassini;     
        public TM_Proxy tmProxy;
        public string                   webRoot;
        public string                   path_XmlLibraries;
        public int                      port;
        public Uri                      siteUri;

        [TestFixtureSetUp]        
        public virtual void testFixtureSetUp()
        {
            webRoot           = this.teamMentor_Root_OnDisk() .assert_Folder_Exists();  // this points to a folder with the full TM 
            path_XmlLibraries = "path_XmlLibraries".tempDir() .assert_Folder_Exists();  // this points to a temp folder
                                                              
            apiCassini = new API_Cassini(webRoot)        .assert_Not_Null();            
            port       = apiCassini.port()               .assert_Not_Default();
            siteUri    = apiCassini.url().uri();

            apiCassini.webRoot().assert_Equal_To(webRoot);

            port      .tcpClient().assert_Null();
            apiCassini.start();

            this.tmProxy().set_Custom_Path_XmlDatabase(path_XmlLibraries);              // configure TM to the temp path_XmlLibraries folder for all files created            
        }
        public NUnitTests_Cassini_TeamMentor start(bool makeTcpRequestToPort = true)
        {
            testFixtureSetUp();
            if (makeTcpRequestToPort)
                port      .tcpClient().assert_Not_Null();
            return this;
        }
        [TestFixtureTearDown] public virtual void testFixtureTearDown()
        {
            port      .tcpClient().assert_Not_Null();
            apiCassini.stop();                                      
            port      .tcpClient().assert_Null();                        

            webRoot   .assert_Folder_Exists();                               // make sure we didn't delete this by accident (since this is the actualy TM code :)  )
            
            this.tmProxy().set_Custom_Path_XmlDatabase("");                  // reset this value
            apiCassini.appDomain().unLoadAppDomain();                        // unload the AppDomain to remove any file locks that might have existed

            path_XmlLibraries.files(true).files_Attribute_ReadOnly_Remove(); // Remove ReadOnly attributes added by git
            Files.delete_Folder_Recursively(path_XmlLibraries);              // remove temp XmlDatabase folder
            path_XmlLibraries.folder_Wait_For_Deleted();                     // give is sometime
            if (path_XmlLibraries.folder_Exists())
                path_XmlLibraries.startProcess();
            //path_XmlLibraries.assert_Folder_Doesnt_Exist();                // double check the deletion
        }
        public NUnitTests_Cassini_TeamMentor stop()
        {
            testFixtureTearDown();
            return this;
        }
        //having this test is a catch 22. 
        //   If I don't include it we get an ignore on the NUnitTests_Cassini_TeamMentor
        //   If I include it, we get one of these per class that extends NUnitTests_Cassini_TeamMentor
        /*[Test]
        public void Check_Cassini()
        {
            apiCassini.assert_Not_Null();
            webRoot   .assert_Not_Null();
            port      .assert_Bigger_Than(0);

        }*/
    }
}