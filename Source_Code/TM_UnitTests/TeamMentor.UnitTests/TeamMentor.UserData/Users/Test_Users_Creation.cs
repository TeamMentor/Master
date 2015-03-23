using System;
using System.Linq;
using System.Security;
using FluentSharp.CoreLib;
using Moq;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]    
    public class Test_Users_Creation : TM_UserData_InMemory
    {                

        [Test] public void createDefaultAdminUser() 
        {
            //UserRole.ManageUsers.setPrivilege();        
            UserGroup.Admin.assert();

            var adminName  = tmConfig.TMSecurity.Default_AdminUserName;
            Assert.IsNotEmpty(adminName);
            Assert.IsEmpty   (userData.users());
            Assert.IsNull    (adminName.tmUser());

            //when admin user doesn't exist
            var userId       = userData.createDefaultAdminUser();
            var tmUser       = userId.tmUser();
            var passwordHash = tmUser.SecretData.PasswordHash; 
            Assert.IsNotNull (adminName.tmUser());            
            Assert.AreEqual  (tmUser, adminName.tmUser());
            Assert.NotNull   (tmUser.SecretData.PasswordHash);

            //when adminUser.SecretData.PasswordHash.notValid()
            tmUser.SecretData.PasswordHash = null;
            Assert.IsNull    (tmUser.SecretData.PasswordHash);
            var userId2      = userData.createDefaultAdminUser();
            Assert.AreEqual (userId,userId2);

            //when tmConfig.OnInstallation.ForceDefaultAdminPassword
            var otherPasswordHash = tmUser.createPasswordHash("123");
            tmUser.SecretData.PasswordHash = otherPasswordHash;
            userData.createDefaultAdminUser();
            Assert.AreEqual(otherPasswordHash, tmUser.SecretData.PasswordHash);
            Assert.AreNotEqual(otherPasswordHash, passwordHash);
            tmConfig.OnInstallation.ForceDefaultAdminPassword = true;
            userData.createDefaultAdminUser();
            Assert.AreNotEqual(otherPasswordHash, tmUser.SecretData.PasswordHash);
            Assert.AreEqual   (passwordHash     , tmUser.SecretData.PasswordHash);

            //adminUser.GroupID != (int) UserGroup.Admin
            Assert.IsTrue(tmUser.isAdmin());
            tmUser.set_UserGroup(UserGroup.Editor);
            Assert.IsTrue (tmUser.isEditor());
            userData.createDefaultAdminUser();
            Assert.IsTrue (tmUser.isAdmin());
            Assert.IsFalse(tmUser.isEditor());

            //Handle nulls
            Assert.AreEqual((null as TM_UserData).createDefaultAdminUser() , -1);
        }
        [Test] public void createUser()             
        {
            //UserRole.ManageUsers.setPrivilege();       // needed for userData.users()   
            UserGroup.Admin.assert();                    // TODO: fix to reflect new RBAC model

            var newUserName = 10.randomLetters();
            var userCount   = userData.users().size();
            var tmUser      = newUserName.createUser();
            Assert.AreEqual(userCount +1, userData.users().size());
            Assert.AreEqual(newUserName.tmUser(), tmUser);
            Assert.NotNull (tmUser);
            Assert.Contains(tmUser, userData.users());
        }        
        [Test] public void createTmUser()           
        {           
            UserGroup.None.assert();                //  change current thread privildges to None

            var newUser = new NewUser()
                {
                    Company     = 10.randomLetters(),
                    Country     = 10.randomLetters(),
                    Firstname   = 10.randomLetters(),
                    Lastname    = 10.randomLetters(),
                    Note        = 10.randomLetters(),
                    Password    = "tE3!"+10.randomLetters(),
                    State       = 10.randomLetters(),
                    Title       = 10.randomLetters(),
                    Username    = 10.randomLetters(),
                    Email       = "{0}@{0}.{0}".format(3.randomLetters())
                };

            Assert.IsEmpty(newUser.validate());
            
            var user1 = newUser.create();
            var user2 = newUser.create();

            Assert.AreNotEqual(user1, 0);
            Assert.AreNotEqual(user1, user2);

            //try with email in upper case
            newUser.Username    = 10.randomLetters();
            newUser.Email       = newUser.Email.upper();

            //try creating a repeated user
            Assert.AreEqual(0, newUser.create());

            //try creating an admin user (which should fail for anonymous users)
            newUser.GroupId  = (int)UserGroup.Admin;
            newUser.Username = 10.randomLetters();
            newUser.Email    = "{0}@{0}.{0}".format(3.randomLetters());
            Assert.Throws<SecurityException>(()=> newUser.create());
            
            UserGroup.Admin.setPrivileges();
            Assert.AreNotEqual(0, newUser.create());

            //try creating a repeated user
            Assert.AreEqual   (0, newUser.create());
            newUser.Username  = 10.randomLetters();                      // just difference username should fail 
            Assert.AreEqual   (0, newUser.create());
            newUser.Email     = "{0}@{0}.{0}".format(3.randomLetters()); // with different username and password should work
            Assert.AreNotEqual(0, newUser.create());

            //test nulls and fail validation
            Assert.AreEqual   (0, userData.createTmUser(null));            
            newUser.Username = null;            
            Assert.AreEqual   (0, newUser.create());            
        }

        [Test]
        public void CreateTmUserSigupResponse_Success()
        {
            UserGroup.None.assert();                //  change current thread privildges to None

            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!r4J"+10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };

            Assert.IsEmpty(newUser.validate());

            var response = newUser.createSigupResponse();
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.UserCreated >0);
            Assert.IsTrue(response.Signup_Status == Signup_Result.SignupStatus.Signup_Ok);
            Assert.IsTrue(response.Validation_Results.count()==0);
            
        }

        [Test]
        public void CreateTmUserSigupResponse_TLD_Email_Success()
        {
            UserGroup.None.assert();                //  change current thread privildges to None

            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!r4J" + 10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "dmin@xn--xn--fsqu00a.xn--g6w251d"
            };

            Assert.IsEmpty(newUser.validate());

            var response = newUser.createSigupResponse();
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.UserCreated > 0);
            Assert.IsTrue(response.Signup_Status == Signup_Result.SignupStatus.Signup_Ok);
            Assert.IsTrue(response.Validation_Results.count() == 0);

        }

        [Test]
        public void CreateTmUserSigupResponse_EmailMaxLength_Allowed()
        {
            UserGroup.None.assert();                //  change current thread privildges to None

            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!r4J" + 10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "".random_Email() + "".add_RandomLetters(230)
            };
             
            Assert.IsTrue(newUser.Email.Length ==256);
            Assert.IsEmpty(newUser.validate());

            var response = newUser.createSigupResponse();
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.UserCreated > 0);
            Assert.IsTrue(response.Signup_Status == Signup_Result.SignupStatus.Signup_Ok);
            Assert.IsTrue(response.Validation_Results.count() == 0);

        }
        [Test]
        public void CreateTmUserSigupResponse_Unicode_Email_Success()
        {
            UserGroup.None.assert();                //  change current thread privildges to None

            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!r4J" + 10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "admin@استفتاء.مصر"
            };

            Assert.IsEmpty(newUser.validate());

            var response = newUser.createSigupResponse();
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.UserCreated > 0);
            Assert.IsTrue(response.Signup_Status == Signup_Result.SignupStatus.Signup_Ok);
            Assert.IsTrue(response.Validation_Results.count() == 0);

        }

        [Test]
        public void CreateTmUserSigupResponse()
        {
            UserGroup.None.assert();                //  change current thread privildges to None

            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!!4Tmd" + 10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };

            Assert.IsEmpty(newUser.validate());

            var user1 = newUser.createSigupResponse();
            var user2 = newUser.createSigupResponse();

            Assert.AreNotEqual(user1.UserCreated, 0);
            Assert.IsFalse(user2.UserCreated > 0);

            //try with email in upper case
            newUser.Username = 10.randomLetters();
            newUser.Email = newUser.Email.upper();

            //try creating a repeated user
            Assert.AreEqual(0, newUser.createSigupResponse().UserCreated);

            //try creating an admin user (which should fail for anonymous users)
            newUser.GroupId = (int)UserGroup.Admin;
            newUser.Username = 10.randomLetters();
            newUser.Email = "{0}@{0}.{0}".format(3.randomLetters());
            Assert.Throws<SecurityException>(() => newUser.createSigupResponse());

            UserGroup.Admin.setPrivileges();
            Assert.AreNotEqual(0, newUser.createSigupResponse().UserCreated);

            //try creating a repeated user
            Assert.AreEqual(0, newUser.createSigupResponse().UserCreated);
            newUser.Username = 10.randomLetters();                      // just difference username should fail 
            Assert.AreEqual(0, newUser.create());
            newUser.Email = "{0}@{0}.{0}".format(3.randomLetters()); // with different username and password should work
            Assert.AreNotEqual(0, newUser.createSigupResponse().UserCreated);

            //test nulls and fail validation
            Assert.AreEqual(0, userData.createTmUserResponse(null).UserCreated);
            newUser.Username = null;
            Assert.AreEqual(0, newUser.create());
        }
        [Test]
        public void CreateTmUserSigupResponse_PasswordIsLessThan8Characters()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!4rJ" + 1.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response!=null));
            Assert.IsTrue(response != null && response.UserCreated==0);           
            Assert.IsTrue(response != null && response.Signup_Status== Signup_Result.SignupStatus.Validation_Failed);
            Assert.IsTrue(response != null && response.Validation_Results!=null & response.Validation_Results.count() >0);
            var results = response.Validation_Results;
            Assert.IsTrue(results.count() >0);
            var result = results.FirstOrDefault();
            Assert.IsTrue(result.notNull());
            Assert.IsTrue(result.Field=="Password");
            Assert.IsTrue(result.Message==tmConfig.TMErrorMessages.PasswordLengthErrorMessage);
        }

        [Test]
        public void CreateTmUserSigupResponse_PasswordIsGreatherThan256Characters()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!rJ10" + 260.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            Assert.IsTrue(response != null && response.Validation_Results != null & response.Validation_Results.count() > 0);
            var results = response.Validation_Results;
            Assert.IsTrue(results.count() > 0);
            var result = results.FirstOrDefault();
            Assert.IsTrue(result.notNull());
            Assert.IsTrue(result.Field == "Password");
            Assert.IsTrue(result.Message == tmConfig.TMErrorMessages.PasswordLengthErrorMessage);
        }

        [Test]
        public void CreateTmUserSigupResponse_PasswordIsWeak()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "rjdq" + 8.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 10.randomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            Assert.IsTrue(response != null && response.Validation_Results != null & response.Validation_Results.count() > 0);
            var results = response.Validation_Results;
            Assert.IsTrue(results.count() > 0);
            var result = results.FirstOrDefault();
            Assert.IsTrue(result.notNull());
            Assert.IsTrue(result.Message == tmConfig.TMErrorMessages.PasswordComplexityErroMessage);
           
        }
        [Test]
        public void CreateTmUserSigupResponse_Username_Mustbe_30CharacterLong_()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!!4tmadmin" + 6.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = 100.randomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            Assert.IsTrue(response != null && response.Validation_Results != null & response.Validation_Results.count() > 0);
            var results = response.Validation_Results;
            Assert.IsTrue(results.count() > 0);
            var result = results.FirstOrDefault();
            Assert.IsTrue(result.notNull());
            Assert.IsTrue(result.Field=="Username");
            Assert.IsTrue((result.Message== "The field Username must be a string with a maximum length of 30."));
            
        }

        [Test]
        public void CreateTmUserSigupResponse_TMUserIsNull()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var response = userData.createTmUserResponse(null);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Signup_Error);
            Assert.IsTrue(response != null && response.Simple_Error_Message.Length > 0);
            Assert.IsTrue(response != null && response.Simple_Error_Message == "An error occurred creating a new account");
        }

        [Test]
        public void CreateTmUserSigupResponse_Username_AlreadyExist()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!rJ4" + 10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = "!!T".add_5_RandomLetters(),
                Email = "{0}@{0}.{0}".format(3.randomLetters())
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated>0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Signup_Ok);

            response = userData.createTmUserResponse(newUser);

            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.Signup_Status== Signup_Result.SignupStatus.Validation_Failed);
            var results = response.Validation_Results;
            Assert.IsTrue(results.count()>0);
            Assert.IsTrue(results.FirstOrDefault().Field=="Username");
            Assert.IsTrue(results.FirstOrDefault().Message == TMConsts.DEFAULT_SIGNUP_USERNAME_EXIST_MESSAGE);

        }
        [Test]
        public void CreateTmUserSigupResponse_Email_AlreadyExist()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Company = 10.randomLetters(),
                Country = 10.randomLetters(),
                Firstname = 10.randomLetters(),
                Lastname = 10.randomLetters(),
                Note = 10.randomLetters(),
                Password = "!rJ4" + 10.randomLetters(),
                State = 10.randomLetters(),
                Title = 10.randomLetters(),
                Username = "tA2!".add_RandomLetters(8),
                Email    = "test@securityinnovation.com"
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated > 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Signup_Ok);

            newUser.Username = "tA2@".add_RandomLetters(8);
            response = userData.createTmUserResponse(newUser);

            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            var results = response.Validation_Results;
            Assert.IsTrue(results.count() > 0);
            Assert.IsTrue(results.FirstOrDefault().Field == "Email");
            Assert.IsTrue(results.FirstOrDefault().Message == TMConsts.DEFAULT_SIGNUP_EMAIL_EXIST_MESSAGE);

        }

        [Test]
        public void CreateTmUserSigupResponse_Email_Address_Invalid_Bad_Format()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Username = "tA2!".add_RandomLetters(8),
                Email = "notValidEmail"
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated ==0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            if (response != null)
            {
                var results = response.Validation_Results;
                Assert.IsTrue(results.count() > 0);
                Assert.IsTrue(results.FirstOrDefault().Field == "Email");
                Assert.IsTrue(results.FirstOrDefault().Message == TMConsts.DEFAULT_EMAIL_ADDRESS_IS_INVALID);
            }
        }
        [Test]
        public void CreateTmUserSigupResponse_Email_Address_Large()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Username = "tA2!".add_RandomLetters(8),
                Email = "".random_Email().add_RandomLetters(300)
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            if (response != null)
            {
                var results = response.Validation_Results;
                Assert.IsTrue(results.count() > 0);
                Assert.IsTrue(results.FirstOrDefault().Field == "Email");
                Assert.IsTrue(results.FirstOrDefault().Message == TMConsts.DEFAULT_EMAIL_ADDRESS_IS_INVALID);
            }
        }
        [Test]
        public void CreateTmUserSigupResponse_Email_Address_IsNull()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Username = "tA2!".add_RandomLetters(8),
                Email = null
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            if (response != null)
            {
                var results = response.Validation_Results;
                Assert.IsTrue(results.count() > 0);
                Assert.IsTrue(results.FirstOrDefault().Field == "Email");
                Assert.IsTrue(results.FirstOrDefault().Message == TMConsts.DEFAULT_EMAIL_ADDRESS_IS_INVALID);
            }
        }
       
        [Test]
        public void CreateTmUserSigupResponse_Email_Address_NullorEmpty()
        {
            UserGroup.None.assert();                //  change current thread privildges to None
            var newUser = new NewUser()
            {
                Username = "tA2!".add_RandomLetters(8),
                Email = String.Empty
            };
            var response = userData.createTmUserResponse(newUser);
            Assert.IsTrue((response != null));
            Assert.IsTrue(response != null && response.UserCreated == 0);
            Assert.IsTrue(response != null && response.Signup_Status == Signup_Result.SignupStatus.Validation_Failed);
            if (response != null)
            {
                var results = response.Validation_Results;
                Assert.IsTrue(results.count() > 0);
                Assert.IsTrue(results.FirstOrDefault().Field == "Email");
                Assert.IsTrue(results.FirstOrDefault().Message == TMConsts.DEFAULT_EMAIL_ADDRESS_IS_INVALID);
            }
        }
        [Test] public void setUserPassword()        
        {
            var tmUser   = "tempUser".createUser();
            var password1 = 10.randomLetters();
            var password2 = 10.randomLetters();
            var password3 = 10.randomLetters();
            var password4 = 10.randomLetters();

            Assert.AreEqual(Guid.Empty,userData.login(tmUser.UserName, password1));
            Assert.AreEqual(Guid.Empty,userData.login(tmUser.UserName, password2));
            Assert.AreEqual(Guid.Empty,userData.login(tmUser.UserName, password3));
            Assert.AreEqual(Guid.Empty,userData.login(tmUser.UserName, password4));

            //change using tmUser
            tmUser.setPassword(password1);
            Assert.AreNotEqual(Guid.Empty,userData.login(tmUser.UserName, password1));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password2));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password3));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password4));

            //change using tmUser.UserID
            userData.setUserPassword(tmUser.UserID,password2);
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password1));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password1));
            Assert.AreNotEqual(Guid.Empty,userData.login(tmUser.UserName, password2));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password3));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password4));

            //change using tmUser.UserName
            userData.setUserPassword(tmUser.UserName,password3);
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password1));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password2));
            Assert.AreNotEqual(Guid.Empty,userData.login(tmUser.UserName, password3));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password4));

            //change using password hash
            tmUser.setPasswordHash(tmUser.createPasswordHash(password4));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password1));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password2));
            Assert.AreEqual   (Guid.Empty,userData.login(tmUser.UserName, password3));
            Assert.AreNotEqual(Guid.Empty,userData.login(tmUser.UserName, password4));
        }
        [Test]
        public void setUserPasswordResult()
        {
           var currentPassword = "!#4uT" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "2!Aa"  + 10.randomLetters();
            var password2 = "2@As"  + 10.randomLetters();
            var password3 = "2$Aa" + 9.randomLetters();
            var password4 = "2&*aA" + 9.randomLetters();

            Assert.AreEqual(Guid.Empty, userData.loginResponse(tmUser.UserName, password1).Token);
            Assert.AreEqual(Guid.Empty, userData.loginResponse(tmUser.UserName, password2).Token);
            Assert.AreEqual(Guid.Empty, userData.loginResponse(tmUser.UserName, password3).Token);
            Assert.AreEqual(Guid.Empty, userData.loginResponse(tmUser.UserName, password4).Token);

            //change using tmUser
            userData.setUserPasswordResponse(tmUser, currentPassword, password1);
            Assert.AreNotEqual(Guid.Empty, userData.loginResponse(tmUser.UserName, password1).Token);
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password2));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password3));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password4));

            //change using tmUser.UserID
            userData.setUserPasswordResponse(tmUser, password1, password2);
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password1));
            Assert.AreNotEqual(Guid.Empty, userData.login(tmUser.UserName, password2));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password3));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password4));

            //change using tmUser.UserName
            userData.setUserPasswordResponse(tmUser, password2, password3);
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password1));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password2));
            Assert.AreNotEqual(Guid.Empty, userData.login(tmUser.UserName, password3));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password4));

            //change using password hash
            userData.setUserPasswordResponse(tmUser, password3, password4);
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password1));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password2));
            Assert.AreEqual(Guid.Empty, userData.login(tmUser.UserName, password3));
            Assert.AreNotEqual(Guid.Empty, userData.login(tmUser.UserName, password4));

        }

        [Test]
        public void setUserPasswordResult_CurrentPasswordDonotMatch()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "!" + 10.randomLetters();
            //change using tmUser
            var response = userData.setUserPasswordResponse(tmUser, password1, password1);
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.PasswordChanged== false);
            Assert.IsTrue(response.Message == TMConsts.CURRENT_PASSWORD_DONOT_MATCH_ERROR_MESSAGE);
        }

        [Test]
        public void setUserPasswordResult_CurrentPasswordAndNewPassword_AreEquals()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "!" + 10.randomLetters();
            //change using tmUser
            var response = userData.setUserPasswordResponse(tmUser, currentPassword, currentPassword);
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.PasswordChanged == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_NEW_PASSWORD_ERROR_MESSAGE);
        }

        [Test]
        public void setUserPasswordResult_PasswordComplexityFails()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "weaujajjkpwd";
            //change using tmUser
            var response = userData.setUserPasswordResponse(tmUser, currentPassword, password1);
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.PasswordChanged == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_PASSWORD_COMPLEXITY_ERROR_MESSAGE);
        }
        [Test]
        public void setUserPasswordResult_PasswordLenghLessThan_8Characters()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "wfr";
            //change using tmUser
            var response = userData.setUserPasswordResponse(tmUser, currentPassword, password1);
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.PasswordChanged == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_PASSWORD_LENGTH_MESSAGE);
        }

        [Test]
        public void setUserPasswordResult_PasswordLenghGreaterThan_256Characters()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "w" + 256.randomLetters();
            //change using tmUser
            var response = userData.setUserPasswordResponse(tmUser, currentPassword, password1);
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.PasswordChanged == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_PASSWORD_LENGTH_MESSAGE);
        }

        [Test]
        public void setUserPasswordResult_DefaulChangePassword_Message()
        {
            tmConfig.TMSetup.ShowDetailedErrorMessages = false;
            tmConfig.save();
           
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var password1 = "w" + 256.randomLetters();
            //change using tmUser
            var response = userData.setUserPasswordResponse(tmUser, currentPassword, password1);
            Assert.IsTrue(response.notNull());
            Assert.IsTrue(response.PasswordChanged == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_PASSWORD_CHANGE_ERROR_MESSAGE);
            tmConfig.TMSetup.ShowDetailedErrorMessages = true;
            tmConfig.save();
        }
        [Test] public void updateTmUser()           
        {       
            //UserRole.ManageUsers.setPrivilege();       // needed for userData.users()   
            UserGroup.Admin.assert();

            var tmUser      = "el User".createUser();
            var userName    = tmUser.UserName;        // userName cannot be changed
            var firstname   = 10.randomLetters();
            var lastname    = 10.randomLetters(); 
            var title       = 10.randomLetters(); 
            var company     = 10.randomLetters(); 
            var email       = 10.randomLetters(); 
            var country     = 10.randomLetters(); 
            var state       = 10.randomLetters(); 
            var accountExpiration   = tmUser.AccountStatus.ExpirationDate.AddSeconds(10); 
            var passwordExpired     = tmUser.AccountStatus.PasswordExpired.not();
            var userEnabled         = tmUser.AccountStatus.UserEnabled.not();
            var accountNeverExpires = false; 
            var groupId           = 4.random(); 
            
            var result1 = userData.updateTmUser(tmUser.UserID, userName, firstname, lastname,  title, company, email,country, state, accountExpiration, passwordExpired,userEnabled,accountNeverExpires, groupId);
            var result2 = userData.updateTmUser(tmUser.UserID, userName, firstname, lastname,  title, company, email,country, state, accountExpiration, passwordExpired,userEnabled,accountNeverExpires, groupId);
            var result3 = userData.updateTmUser(tmUser.UserID, "new value", firstname, lastname,  title, company, email,country, state, accountExpiration, passwordExpired,userEnabled,accountNeverExpires, groupId);

            Assert.IsTrue  (result1, "First update should work");
            Assert.IsTrue  (result2, "Second update (with same data) should work");
            Assert.IsFalse (result3, "Third update (with changed username) should fail");

            Assert.AreEqual(tmUser.UserName  , userName);
            Assert.AreEqual(tmUser.FirstName , firstname);
            Assert.AreEqual(tmUser.LastName  , lastname);
            Assert.AreEqual(tmUser.Title     , title);
            Assert.AreEqual(tmUser.Company   , company);
            Assert.AreEqual(tmUser.EMail     , email);
            Assert.AreEqual(tmUser.Country   , country);
            Assert.AreEqual(tmUser.State     , state);
            Assert.AreEqual(tmUser.GroupID   , groupId);
            Assert.AreEqual(tmUser.AccountStatus.ExpirationDate  , accountExpiration);
            Assert.AreEqual(tmUser.AccountStatus.PasswordExpired , passwordExpired);
            Assert.AreEqual(tmUser.AccountStatus.UserEnabled     , userEnabled);
            
        }
        [Test] public void userEnabled()
        {
            tmConfig.TMSecurity.NewAccounts_Enabled = true;
            Assert.IsTrue(tmConfig.TMSecurity.NewAccounts_Enabled);
            var tmUser1      = userData.createUser();
            Assert.IsTrue(tmUser1.AccountStatus.UserEnabled);

            tmConfig.TMSecurity.NewAccounts_Enabled = false;
            var tmUser2      = userData.createUser();               // becasuse Email system is disabled in these type of UnitTests 
                                                                    // this will NOT set the tmUser2.SecretData.EnableUserToken because the admin will be emailed with the EnableUserToken
            Assert.IsFalse(tmUser2.AccountStatus.UserEnabled);

            tmUser2.enableUser_Token();

            Assert.AreNotEqual(tmUser1, tmUser2);
            Assert.AreEqual   (Guid.Empty, tmUser1.SecretData.EnableUserToken);
            Assert.AreNotEqual(Guid.Empty, tmUser2.SecretData.EnableUserToken);

            var accountEnableToken1 = tmUser1.enableUser_Token();
            var accountEnableToken2 = tmUser2.enableUser_Token();

            Assert.AreNotEqual(Guid.Empty, tmUser1.SecretData.EnableUserToken);
            Assert.AreNotEqual(Guid.Empty, tmUser2.SecretData.EnableUserToken);

            Assert.IsTrue     (accountEnableToken1.enableUser_IsTokenValid());
            Assert.IsTrue     (accountEnableToken2.enableUser_IsTokenValid());            
            Assert.AreEqual   (tmUser1, accountEnableToken1.enableUser_UserForToken());
            Assert.AreEqual   (tmUser2, accountEnableToken2.enableUser_UserForToken());
            
            Assert.IsFalse    (Guid.NewGuid().enableUser_IsTokenValid());
            Assert.IsFalse    (Guid.Empty    .enableUser_IsTokenValid());
            Assert.IsNull     (Guid.NewGuid().enableUser_UserForToken());
            Assert.IsNull     (Guid.Empty    .enableUser_UserForToken());
            
            //enable users
            var tmUser1_Enabled = accountEnableToken1.enableUser_UsingToken();
            var tmUser2_Enabled = accountEnableToken2.enableUser_UsingToken();
            Assert.IsNotNull  (tmUser1_Enabled);
            Assert.IsNotNull  (tmUser2_Enabled);            
            Assert.AreEqual   (tmUser1_Enabled, tmUser1);
            Assert.AreEqual   (tmUser2_Enabled, tmUser2);

            // these should not work anymore:
            Assert.IsFalse    (accountEnableToken1.enableUser_IsTokenValid());
            Assert.IsFalse    (accountEnableToken2.enableUser_IsTokenValid());            
            Assert.IsNull     (accountEnableToken1.enableUser_UserForToken());
            Assert.IsNull     (accountEnableToken2.enableUser_UserForToken());
            Assert.IsNull     (accountEnableToken1.enableUser_UsingToken());
            Assert.IsNull     (accountEnableToken2.enableUser_UsingToken());

            tmConfig.TMSecurity.NewAccounts_Enabled = true;
        }

        [Test(Description = "Verifies that Password reset works fine if email is valid")]
        public void PasswordReset_OK()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            var response = tmUser.EMail.sendPasswordReminder_Response();
            Assert.IsTrue(response.PasswordReseted== true);
            Assert.IsTrue(response.Message== String.Empty);
        }
        [Test(Description = "Password reset should fail if email address does not exist.")]
        public void PasswordReset_EmailNot_Valid()
        {
            var testEmail = 4.randomLetters() + "@" + 4.randomLetters() + ".com";
            var response = testEmail.sendPasswordReminder_Response();
            Assert.IsTrue(response.PasswordReseted == false);
           
        }

        [Test(Description = "Password reset should fail if account is disabled")]
        public void PasswordReset_AccountDisabled()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            tmUser.disable_Account();
            tmUser.save();
            var response = tmUser.EMail.sendPasswordReminder_Response();
            Assert.IsTrue(response.PasswordReseted == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_ACCOUNT_DISABLED_MESSAGE);

        }

        [Test(Description = "Password reset should fail if account is has exired")]
        public void PasswordReset_AccountExpired()
        {
            var currentPassword = "!#4" + 10.randomLetters();
            var username = 5.randomLetters() + 2.randomNumber() + 3.randomLetters();
            var tmUser = userData.newUser(username, currentPassword).tmUser();
            tmUser.expire_Account();
            tmUser.save();
            var response = tmUser.EMail.sendPasswordReminder_Response();
            Assert.IsTrue(response.PasswordReseted == false);
            Assert.IsTrue(response.Message == TMConsts.DEFAULT_ACCOUNT_EXPIRED_MESSAGE);

        }

        [Test (Description ="Checks that only UserRole.ManageUsers is able to invoke the userData.users() method")]
        public void CheckUserListPermissions()
        {           
            UserRole.ManageUsers        .setPrivilege();    Assert.DoesNotThrow              (()=>userData.users());
            UserRole.ReadArticles       .setPrivilege();    Assert.Throws<SecurityException> (()=>userData.users());
            UserRole.ManageUsers        .setPrivilege();    Assert.DoesNotThrow              (()=>userData.users());        // reconfirm previous set.Priviledge had no side effect
            UserRole.EditArticles       .setPrivilege();    Assert.Throws<SecurityException> (()=>userData.users());            
            UserRole.Admin              .setPrivilege();    Assert.Throws<SecurityException> (()=>userData.users());
            UserRole.ReadArticlesTitles .setPrivilege();    Assert.Throws<SecurityException> (()=>userData.users());
            UserRole.ManageUsers        .setPrivilege();    Assert.DoesNotThrow              (()=>userData.users());        // reconfirm previous set.Priviledge had no side effects
            UserRole.ViewLibrary        .setPrivilege();    Assert.Throws<SecurityException> (()=>userData.users());
            UserRole.None               .setPrivilege();    Assert.Throws<SecurityException> (()=>userData.users());
            UserRole.ManageUsers        .setPrivilege();    Assert.DoesNotThrow              (()=>userData.users());        // reconfirm previous set.Priviledge had no side effects
        }
    }
}
