using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_SecretData : TM_XmlDatabase_InMemory
    {
        [Test]
        public void SecretDataDefault()
        {
            userData.ResetData();

            var tmSecretData = userData.SecretData;

            Assert.IsNotNull(tmSecretData);
            Assert.IsNotNull(tmSecretData.Rijndael_IV);
            Assert.IsNotNull(tmSecretData.Rijndael_Key);
            Assert.IsNotNull(tmSecretData.SMTP_Server);
            Assert.IsNotNull(tmSecretData.SMTP_UserName);
            Assert.IsNull   (tmSecretData.SMTP_Password);

            "TMSecretData xml: \n {0}".info(tmSecretData.toXml());
        }

        [Test]
        public void ExecuteSecretDataScriptFile()
        {
            userData.Path_UserData  = "userData".tempDir();
            var h2ScriptFile        = userData.secretDataScript_FileLocation();
            
            Assert.IsNotNull(h2ScriptFile);
            Assert.IsFalse  (h2ScriptFile.fileExists());
            
            h2ScriptFile.parentFolder().createDir();
            "return 42;".saveAs (h2ScriptFile);
            var scriptContents  = h2ScriptFile.h2_SourceCode();
            var assembly        = scriptContents.compileCodeSnippet();
            var result          = assembly.firstMethod().invoke();

            Assert.IsTrue   (h2ScriptFile.fileExists());
            Assert.IsNotNull(scriptContents);
            Assert.IsNotNull(assembly);            
            Assert.AreEqual(result, 42);
            //"h2ScriptFile xml: \n {0}".info(h2ScriptFile);
        }

        [Test]
        public void UseSecretDataScriptFileToSetSmtpPassword()
        {            
            userData.Path_UserData  = "userData".tempDir();
            var h2ScriptFile        = userData.secretDataScript_FileLocation();            

            userData.ResetData();
            var tmSecretData        = userData.SecretData;            

            Assert.IsNotNull(h2ScriptFile);
            Assert.IsNull   (tmSecretData.SMTP_Password, "tmSecretData.SMTP_Password should be null after tmSecretData");
            
            var h2Script = testSecretDataScript();
            
            //Compile and Execute Directly
            var assembly = h2Script.compileCodeSnippet();
            var result = assembly.firstMethod().invoke();

            Assert.IsNotNull(assembly);            
            Assert.AreEqual(result, "done");
            Assert.AreEqual   (tmSecretData.SMTP_UserName, "A name");
            Assert.AreEqual   (tmSecretData.SMTP_Password, "A pwd");
            
            "TMSecretData xml: \n {0}".info(tmSecretData.toXml());

            //Compile and Execute using expected file location

            
            userData.ResetData();

            userData.secretDataScript_FileLocation().parentFolder().createDir();
            h2Script.saveAs(userData.secretDataScript_FileLocation());

            tmSecretData      = userData.SecretData;
            Assert.IsNull     (tmSecretData.SMTP_Password);
            result            = userData.secretDataScript_Invoke();

            Assert.IsNotNull  (tmSecretData.SMTP_Password);            
            Assert.AreEqual   (tmSecretData.SMTP_Password, "A pwd");
            Assert.AreEqual   (result,"done");

            var newPwd = 10.randomLetters();
            h2Script.replace("A pwd",newPwd)
                    .replace("done" ,"done again")
                    .saveAs(userData.secretDataScript_FileLocation());
            
            result            = userData.secretDataScript_Invoke();

            Assert.AreEqual   (tmSecretData.SMTP_Password,newPwd);
            Assert.AreEqual   (result,"done again");

            "TMSecretData xml: \n {0}".info(tmSecretData.toXml());            
        }

        [Test]
        public void CheckThatSecretDataScriptFileWasInvoked()
        {
            userData.ResetData();
            userData.Path_UserData  = "userData".tempDir();

            Assert.IsNull   (userData.SecretData.SMTP_Password);
            
            var scriptLocation = userData.secretDataScript_FileLocation();
            scriptLocation.parentFolder().createDir();
            testSecretDataScript().saveAs(scriptLocation);
            
            userData.ResetData();                        

            Assert.IsTrue    (scriptLocation.fileExists());
            Assert.IsNotEmpty(scriptLocation.fileContents());
            
            Assert.IsNotNull (userData.SecretData.SMTP_Password);


//            Assert.IsNotNull(userData.SecretData.SMTP_Password);
        }


        //helper methods
        public string testSecretDataScript()
        {
 	        return @"
var userData                = TM_UserData.Current;
var tmSecretData            = userData.SecretData;
tmSecretData.SMTP_UserName  = ""A name"";
tmSecretData.SMTP_Password  = ""A pwd"";
return ""done"";

//using TeamMentor.CoreLib;
//O2Ref:TeamMentor.CoreLib.dll";
        }
    }
}
