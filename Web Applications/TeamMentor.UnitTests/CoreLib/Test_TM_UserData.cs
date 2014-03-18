﻿using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_UserData : TM_XmlDatabase_InMemory
    {
        [Test]
        public void TM_UserData_Ctor()
        {
            Assert.IsFalse(userData.UsingFileStorage);
            userData = new TM_UserData(true);
            Assert.IsTrue (userData.UsingFileStorage);
            userData = new TM_UserData();
            Assert.IsFalse(userData.UsingFileStorage);
        }

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
        public void InvokeUserDataScriptFile()
        {
            userData.Path_UserData  = "userData".tempDir();
            var h2ScriptFile        = userData.firstScript_FileLocation();
            
            Assert.IsNotNull(h2ScriptFile);
            Assert.IsFalse  (h2ScriptFile.fileExists());
            
            h2ScriptFile.parentFolder().createDir();
            "return 42;".saveAs (h2ScriptFile);

            var fileExists          = h2ScriptFile.fileExists();
            var scriptContents      = h2ScriptFile.h2_SourceCode();
            var assembly            = scriptContents.compileCodeSnippet();
            var result_DirectInvoke = assembly.firstMethod().invoke();          // invoke directly
            var result_TmInvoke     = userData.firstScript_Invoke();         // invoke via TM UserData            
            var fileDeleted         = h2ScriptFile.file_Delete() && h2ScriptFile.fileExists().isFalse();

            Assert.IsTrue   (fileExists);
            Assert.IsNotNull(scriptContents);
            Assert.IsNotNull(assembly);            
            Assert.AreEqual (result_DirectInvoke, 42);    
            Assert.AreEqual (result_TmInvoke    , "42");
            Assert.IsTrue   (fileDeleted);

            //Remove temp UserData folder
            Assert.IsTrue   (userData.Path_UserData.dirExists());
            Assert.IsTrue   (Files.deleteFolder(userData.Path_UserData,true));
            Assert.IsFalse  (userData.Path_UserData.dirExists());
        }

    }
}
