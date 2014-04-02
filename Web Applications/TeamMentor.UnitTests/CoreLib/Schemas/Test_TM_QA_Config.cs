using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Schemas
{
    [TestFixture]
    public class Test_TM_QA_Config
    {
        public TM_QA_Config tmQAConfig;

        public Test_TM_QA_Config()
        {
            if (TMConsts.TM_QA_ConfigFile_LOCAL_FOLDER.dirExists().isFalse())
                Assert.Ignore("TM_QA_ConfigFile_LOCAL_FOLDER did not exist: {0}".format(TMConsts.TM_QA_ConfigFile_LOCAL_FOLDER));
            tmQAConfig = TM_QA_Config.Current;
            Assert.IsNotNull(tmQAConfig);
        }

        [Test]
        public void ConfigFile_Current()
        {                        
            Assert.IsTrue(tmQAConfig.Firebase_Site      .valid());
            Assert.IsTrue(tmQAConfig.Firebase_AuthToken .valid());
            Assert.IsTrue(tmQAConfig.SMTP_Server        .valid());
            Assert.IsTrue(tmQAConfig.SMTP_UserName      .valid());
            Assert.IsTrue(tmQAConfig.SMTP_Password      .valid());
            Assert.IsTrue(tmQAConfig.Default_AdminEmail .valid());

         /*   var dllLocation		 = "TeamMentor.CoreLib.dll".assembly_Location();
            //var dllLocation		 = assembly.CodeBase.subString(8);   
            
            Assert.AreEqual("", dllLocation);
            */
        }
        
    }
}
