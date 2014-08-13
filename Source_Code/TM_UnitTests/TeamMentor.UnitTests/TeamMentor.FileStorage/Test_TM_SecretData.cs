using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.FileStorage
{
    [TestFixture]
    public class Test_TM_SecretData 
    {
        TM_FileStorage tmFileStorage;
        TM_UserData tmUserData;

        [SetUp]
        public void setup()
        {
            tmFileStorage = new TM_FileStorage(false);
            tmUserData    = tmFileStorage.UserData;
        }
        [TearDown]
        public void tearDown()
        {
            //delete temp files
            
            var secretDataFile = tmFileStorage.secretData_Location();
            Assert.IsTrue(secretDataFile.fileExists());
            
            tmFileStorage.Path_UserData.delete_Folder();

            Assert.IsFalse(tmFileStorage.Path_UserData.dirExists());
            Assert.IsFalse(secretDataFile.fileExists());
        }
        [Test]
        public void secretData_Load__UsingFileStorage() 
        {            
            var secretData = tmUserData.SecretData;

            Assert.NotNull(secretData);                     // the ctor above will set this value
            Assert.IsNull(tmFileStorage.Path_UserData);     // but since it was not created from the TM_Xml_Database this path was not set

            tmFileStorage.secretData_Load();

            Assert.IsNull(tmUserData.SecretData);          // when Path_UserData is null, the secretData_Load should be leave the SecretData value with null
            Assert.IsNull(tmFileStorage.secretData_Location());

            // Set temp path and create default TM_SecretData
            tmFileStorage.Path_UserData = "temp_userData".tempDir();
            var secretDataFile = tmFileStorage.secretData_Location();

            Assert.NotNull(tmFileStorage.secretData_Location());
            Assert.IsFalse(secretDataFile.fileExists());

            tmFileStorage.secretData_Load();                 //  should create the file at secretDataFile

            Assert.IsTrue(secretDataFile.fileExists());
            Assert.NotNull(tmUserData.SecretData);

            // Edit secretDataFile file and reload it
            var testValue = 10.randomLetters(); ;

            Assert.AreNotEqual(tmUserData.SecretData, testValue);

            secretData.Rijndael_IV = testValue;
            secretData.saveAs(secretDataFile);

            tmFileStorage.secretData_Load();              // should load edited file

            Assert.NotNull(tmUserData.SecretData);
            Assert.AreEqual(tmUserData.SecretData.Rijndael_IV, testValue);

            // create corrupted TM_SecretData file

            "AAAA".saveAs(secretDataFile);
            tmFileStorage.secretData_Load();

            Assert.IsNull(tmUserData.SecretData);            
        }
    }
}
