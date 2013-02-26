﻿using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_LoadLibraries_FromExternalSource : TM_XmlDatabase_InMemory
    {
        [Test]
        public void DownloadAndInstallLibraryFromZip()
        {
            if(new O2.Kernel.CodeUtils.O2Kernel_Web().online().isFalse())
                Assert.Ignore("Ignoring Test because we are offline");   

            var tmLibraries_Before = tmXmlDatabase.tmLibraries();            

            Install_LibraryFromZip_TopVulns();
            Install_LibraryFromZip_TopVulns();          //2nd time should skip install

            Assert.IsEmpty(tmLibraries_Before, "No Libraries should be there before install");
            Assert.IsNotEmpty(tmXmlDatabase.tmLibraries(), "After install, no Libraries");
            Assert.IsNotEmpty(tmXmlDatabase.tmViews(), "After install, no Views");
            //Assert.IsNotEmpty(tmXmlDatabase.tmFolders(), "After install, no Folders");
            Assert.IsNotEmpty(tmXmlDatabase.tmGuidanceItems(), "After install, no Articles");

            Install_LibraryFromZip_OWASP();

            Assert.AreEqual  (2, tmXmlDatabase.tmLibraries().size() , "After OWASP install, there should be 2");
            Assert.IsNotEmpty(tmXmlDatabase.tmViews(), "After OWASP install, no Views");
            Assert.IsNotEmpty(tmXmlDatabase.tmFolders(), "After OWASPinstall, no Folders");
            Assert.IsNotEmpty(tmXmlDatabase.tmGuidanceItems(), "After OWASP install, no Articles");
        }
        
    }
}
