using FluentSharp.CoreLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_SecretData
    {
        [Test]
        public void SecretData_Ctor()                   
        {
            var tmSecretData = new TM_SecretData();

            Assert.IsNotNull(tmSecretData);
            Assert.IsNotNull(tmSecretData.Rijndael_IV);
            Assert.IsNotNull(tmSecretData.Rijndael_Key);

            Assert.IsNotNull(tmSecretData.SmtpConfig.Server);
            Assert.IsNotNull (tmSecretData.SmtpConfig.UserName);
            Assert.AreEqual(tmSecretData.SmtpConfig.Password, "");
            Assert.IsNotNull(tmSecretData.SmtpConfig.Default_From);
            Assert.IsNotNull(tmSecretData.SmtpConfig.Default_To);
            Assert.IsNotNull(tmSecretData.SmtpConfig.Server);
            
        }
        [Test]
        public void secretData_Load__InMemory()         
        {
            var userData = new TM_UserData                  // default is to set UsingFileStorage to false
                                {
                                    SecretData = null       // so that we can check below that this property was set
                                };

            userData.secretData_Load();
            Assert.NotNull(userData.SecretData);
            Assert.IsFalse(userData.UsingFileStorage);

            //test nulls
            userData = null;
            Assert.IsNull(userData.secretData_Load());

        }
        [Test]
        public void secretData_Load__UsingFileStorage() 
        {
            var userData = new TM_UserData(new TM_Server() { UseFileStorage = true});       // UsingFileStorage to true
            var secretData = userData.SecretData;

            Assert.NotNull(secretData);                   // the ctor above will set this value
            Assert.IsNull(userData.Path_UserData);       // but since it was not created from the TM_Xml_Database this path was not set

            userData.secretData_Load();

            Assert.IsNull(userData.SecretData);           // when Path_UserData is null, the SecretData should be set to null (since this will checked later on a TM Health check)
            Assert.IsNull(userData.secretData_Location());

            // Set temp path and create default TM_SecretData
            userData.Path_UserData = "temp_userData".tempDir();
            var secretDataFile = userData.secretData_Location();

            Assert.NotNull(userData.secretData_Location());
            Assert.IsFalse(secretDataFile.fileExists());

            userData.secretData_Load();                 //  should create the file at secretDataFile

            Assert.IsTrue(secretDataFile.fileExists());
            Assert.NotNull(userData.SecretData);

            // Edit secretDataFile file and reload it
            var testValue = 10.randomLetters(); ;

            Assert.AreNotEqual(userData.SecretData, testValue);

            secretData.Rijndael_IV = testValue;
            secretData.saveAs(secretDataFile);

            userData.secretData_Load();              // should load edited file

            Assert.NotNull(userData.SecretData);
            Assert.AreEqual(userData.SecretData.Rijndael_IV, testValue);

            // create corrupted TM_SecretData file

            "AAAA".saveAs(secretDataFile);
            userData.secretData_Load();

            Assert.IsNull(userData.SecretData);

            //delete temp files
            userData.Path_UserData.delete_Folder();
            Assert.IsFalse(userData.Path_UserData.dirExists());
            Assert.IsFalse(secretDataFile.fileExists());
        }
    }
}
