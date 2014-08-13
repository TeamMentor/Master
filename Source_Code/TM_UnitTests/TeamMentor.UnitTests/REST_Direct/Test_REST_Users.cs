using FluentSharp.CoreLib;
using FluentSharp.Moq;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.REST;

namespace TeamMentor.UnitTests.REST
{
    [TestFixture]
    public class Test_REST_Users : TM_Rest_Direct
    {
        private TM_REST TMRestUser;
        [SetUp]
        public void SetUp()
        {
            var assembly = this.type().Assembly;
            var dllLocation = assembly.CodeBase.subString(8);
            var webApplications = dllLocation.parentFolder().pathCombine(@"\..\..\..");
            TMRestUser = new TM_REST();
            var tmWebsite = webApplications.pathCombine("TM_Website");
            moq_HttpContext = new API_Moq_HttpContext(tmWebsite);
            var tmConfig = TMConfig.Current;
            var credentials = new TM_Credentials { UserName = tmConfig.TMSecurity.Default_AdminUserName, Password = tmConfig.TMSecurity.Default_AdminPassword };
            //login with default value
            var sessionId = TmRest.Login_using_Credentials(credentials);
        }
        #region VerifyUserData
        [Test]
        public void VerifyUserDataFirstEmailNotValid()
        {
            var payload = "user_1_reader,Secure1Pwd!!,Email,FirstName,LastName,Company,JobTitle,Country,State,ExpirationDate,2,N,Y \n" +
           "user_2_editor, Secure1Pwd!!,Email,FirstName,LastName,Company,JobTitle,Country,State,ExpirationDate,3,N,Y";
            var result = TMRestUser.VerifyUserData(payload);
            Assert.IsTrue(result.contains("The field Email must match the regular expression '^[\\w-+\\.]{1,}\\@([\\w-]{1,}\\.){1,}[a-zA-Z]{2,4}$'"));

        }
        [Test]
        public void VerifyUserDataPayloadMissingField()
        {
            var payload ="user_1_reader,Secure1Pwd!!,someemail@domain.com,FirstName,LastName,Company,JobTitle,State,2050-01-01,2,N,Y \n";
            var result = TMRestUser.VerifyUserData(payload);
            Assert.AreEqual(result,"There is a missing field for user user_1_reader.Please verify.");
        }
        [Test]
        public void VerifyUserDataPayloadIncomplete()
        {
            var payload = "user_1_reader,Secure1Pwd!!,someemail@domain.com,FirstName,LastName,Company,JobTitle,,2050-01-01,2,N,Y \n";
            var result = TMRestUser.VerifyUserData(payload);
            Assert.AreEqual(result, "There is a missing field for user user_1_reader.Please verify.");
        }
        [Test]
        public void VerifyUserDataUserNameAlreadyExist()
        {
            var payload = "admin,Secure1Pwd!!,Email,FirstName,LastName,Company,JobTitle,Country,State,ExpirationDate,2,N,Y";
            var result = TMRestUser.VerifyUserData(payload);
            Assert.AreEqual(result, "Username admin already exist.");
        }
        [Test]
        public void VerifyUserDataFirstNameIsRequired()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,,LastName,Company,JobTitle,Country,State,2050-01-01,2,N,Y";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.IsTrue(result.Contains("FirstName is a required field"));
        }
        [Test]
        public void VerifyUserDataLastNameIsRequired()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,,Company,JobTitle,Country,State,2050-01-01,2,N,Y \n";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.IsTrue(result.Contains("Last Name is a required field"));
        }
        [Test]
        public void VerifyUserDataExpiryDateIsInvalid()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,xyza,2,N,Y \n";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.IsTrue(result.Contains("Please enter a valid Expiration date for user username1test. Format must be yyyy/mm/dd."));
        }
        [Test]
        public void VerifyUserDataExpiryDateMustBeGreaterThanToday()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2000-01-01,2,N,Y \n";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.IsTrue(result.Contains("Expiry date cannot be prior  or equal than today. User username1test"));
        }
        [Test]
        public void VerifyUserDataExpiryPasswordFlagMustBeYesorNo()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,M,Y \n";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.AreEqual(result, "Please verify data for user username1test, Password expire value must be Y (for yes) or N (for No)");
        }
        [Test]
        public void VerifyUserDataUserEnabledFlagMustBeYesorNo()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,M \n";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.AreEqual(result, "Please verify data for user username1test, User Enabled value must be Y (for yes) or N (for No)");
        }
        [Test]
        public void VerifyUserDataUserGroupMustBeValid()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,5,N,N \n";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.AreEqual(result, "The group value set for user username1test is invalid. Valid groups are Admin Editor and Reader");
        }
        [Test]
        public void VerifyUserDataUserUserNameUsed()
        {
            var payload = "username1test,Secure1Pwd!!,a@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername1test,Secure1Pwd!!,b@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername2test,Secure1Pwd!!,c@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.AreEqual(result, "Username username1test is already being used in this import.Please verify.");
        }
        [Test]
        public void VerifyUserDataUserSameEmailAddressUsed()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername2test,Secure1Pwd!!,somesmail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername3test,Secure1Pwd!!,somesmail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N";
            var result = TMRestUser.VerifyUserData(payload.TrimEnd());
            Assert.AreEqual(result, "Email address somesmail@gmail.com is already being used for another user in this import.Please verify.");
        }

        [Test]
        public void VerifyUserDataUserCreationUserSameEmailAddressUsed()
        {
            var payload = "username1test,Secure1Pwd!!,someemail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername2test,Secure1Pwd!!,somesmail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername3test,Secure1Pwd!!,somesmail@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N";
            var result = TMRestUser.CreateCSVUsers(payload.TrimEnd());
            Assert.AreEqual(result, "Email address somesmail@gmail.com is already being used for another user in this import.Please verify.");
        }
        [Test]
        public void VerifyUserDataUserCreationUserUserNameUsed()
        {
            var payload = "username1test,Secure1Pwd!!,a@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername1test,Secure1Pwd!!,b@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N" +
                          "\nusername2test,Secure1Pwd!!,c@gmail.com,FirstName,Last Name,Company,JobTitle,Country,State,2030-01-01,2,N,N";
            var result = TMRestUser.CreateCSVUsers(payload.TrimEnd());
            Assert.AreEqual(result, "Username username1test is already being used in this import.Please verify.");
        }
    }
       #endregion
}
