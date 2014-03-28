using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests._Test_ConfigAndHelpers
{
    public class TM_QA_ConfigFile   
    {
        public String Firebase_Site      { get; set; }
        public String Firebase_AuthToken { get; set; }
        public String SMTP_Server        { get; set; }
        public String SMTP_UserName      { get; set; }
        public String SMTP_Password      { get; set; }
        public String Default_AdminEmail { get; set; }
        

        public TM_QA_ConfigFile()
        {
            // so that the new file created by TM_QA_ConfigFile.Current has the property's values placeholders
            Firebase_Site      = "";            
            Firebase_AuthToken = "";
        }
        
        public static TM_QA_ConfigFile Current
        {
            get
            {
                var configFolder = Tests_Consts.TM_QA_ConfigFile_LOCAL_FOLDER;
                var configFile   = configFolder.pathCombine(Tests_Consts.TM_QA_ConfigFile_FILE_NAME);
                if(configFolder.dirExists())
                {
                    if (configFile.fileExists().isFalse())
                        new TM_QA_ConfigFile().saveAs(configFile);
                    return configFile.load<TM_QA_ConfigFile>();
                }
                return null;
            }
        }
    }

    public static class TM_QA_ConfigFile_ExtensionMethods
    {
        //public static string 
    }

    [TestFixture]
    public class Test_TM_QA_ConfigFile
    {
        public TM_QA_ConfigFile tmQAConfig;

        public Test_TM_QA_ConfigFile()
        {
            if (Tests_Consts.TM_QA_ConfigFile_LOCAL_FOLDER.dirExists().isFalse())
                Assert.Ignore("TM_QA_ConfigFile_LOCAL_FOLDER did not exist: {0}".format(Tests_Consts.TM_QA_ConfigFile_LOCAL_FOLDER));
            tmQAConfig = TM_QA_ConfigFile.Current;
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
