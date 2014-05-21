using System;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    class Test_Users_Management : TM_XmlDatabase_InMemory
    {		
        [TestFixtureSetUp]
        public void setUp()
        {
            UserGroup.Admin.setThreadPrincipalWithRoles();	
        }

        [Test] public void CreateLoginDelete_User()
        {			
            string testUserName = "testUser_".add_RandomLetters(5);
            string password = "123";

            Assert.IsNull(userData.tmUser(testUserName), "testUserName shouldn't exist before create");
            //create user
            var userCount = userData.TMUsers.size();
            var userId = userData.newUser(testUserName, password);			
            Assert.AreEqual(userCount+1, tmXmlDatabase.UserData.TMUsers.size() , "TMUsers,size() after create");
            var tmUser = userData.tmUser(userId);
            Assert.NotNull(tmUser, "tmUser was null after create");
            Assert.AreEqual(tmUser.UserID, userId, "UserID");
            Assert.AreEqual(tmUser.UserName, testUserName, "UserID");

            //Login with new user
            var sessionId = userData.login(testUserName, password);
            Assert.AreNotEqual(Guid.Empty, sessionId, "empty session id");
            sessionId = userData.login(testUserName, "BAD PWD");
            Assert.AreEqual(Guid.Empty, sessionId, "session id should be empty");

            UserGroup.Admin.setThreadPrincipalWithRoles();  //assert admin so that we can delete the user
            //delete user			
            var result = userData.deleteTmUser(tmUser.UserID);
            Assert.That(result, "User delete");
            tmUser = userData.tmUser(userId);
            Assert.IsNull(tmUser, "tmUser should be null after delete");			
        }
        [Test] public void PBKDF2_Multiple()
        {
            var password = "!1234567!";
            var salt = Guid.NewGuid().str();	        
            "{0}\n{1}\n-------\n".info(password, salt);
            for (int i = 1; i < 5; i++)
            {
                var now = DateTime.Now;
                var interactions = i * 1000;// 5000;                
                var passwordHash = password.hash_PBKDF2(salt, interactions,64);
                var timeSpan = DateTime.Now - now;	            
                "password: {0}\ninteractions: {1}  timespan: {2}\n".info(passwordHash, interactions, timeSpan);
                
                Assert.NotNull  (passwordHash);
                Assert.AreEqual (64, passwordHash.base64Decode_AsByteArray().size());
                Assert.Less     (timeSpan.Seconds,3);                 // slowest calculation should be faster than 2 seconds
                Assert.Greater  (timeSpan.TotalMilliseconds, i * 20); // faster calculation should be bigger than i* 20 (20, 40, 60, 80, 100) milliseconds
            }
        }
        [Test] public void PBKDF2_Default()
        {
            5000.set_DEFAULT_PBKDF2_INTERACTIONS(); // restore this value since that is what we are testing
            Action<string,string> checkPassword = 
                (password, salt) =>
                    {
                        var now = DateTime.Now;
                        var passwordHash = password.hash_PBKDF2(salt);
                        var timeSpan = DateTime.Now - now;	            

                        Assert.NotNull  (passwordHash);
                        Assert.AreEqual (64, passwordHash.base64Decode_AsByteArray().size());
                        Assert.Greater  (2,timeSpan.Seconds, "Calculation took more than 2 sec");
                        Assert.Less     (100, timeSpan.TotalMilliseconds); // slowest calculation should be slower than 500 milliseconds                        
                        "ok: {0} : {1}".info(timeSpan.Milliseconds,passwordHash);
                    };
            checkPassword("!1234567!", Guid.NewGuid().str()); // normal values
            checkPassword("a", Guid.NewGuid().str());         // weak password
            checkPassword("!1234567!", "a");                  // weak salt
            checkPassword("", Guid.NewGuid().str());          // no password
            checkPassword("!1234567!", "");                   // no salt
            checkPassword("","");                             // no password and no salt	        
        }
        [Test] public void ResolveUser_ByNameEmailAndId()
        {
            string userName     = "user_".add_RandomLetters(5);            
            string password     = "pwd_".add_RandomLetters(5);         
            string email        = "testUser@teammentor.net";
            var newUserId       = userData.newUser(userName, password, email);
            var tmUser_ById     = newUserId.tmUser();
            var tmUser_ByName   = userName.tmUser();
            var tmUser_ByEmail  = email.tmUser_FromEmail();

            Assert.Greater  (newUserId, 0);
            Assert.NotNull  (tmUser_ById        ,"tmUser_byId");
            Assert.NotNull  (tmUser_ByName      ,"tmUser_byName");
            Assert.NotNull  (tmUser_ByEmail     ,"tmUser_byEmail");
            Assert.IsTrue   (tmUser_ById == tmUser_ByName &&  tmUser_ById ==tmUser_ByEmail);

            //Creating another user with the same email
            string userName2 = userName + "AA";            
            var newUser2Id    = userData.newUser(userName2, password, email);
            var tmUser2_ById     = newUser2Id.tmUser();
            var tmUser2_ByName   = userName2.tmUser();
            var tmUser2_ByEmail  = email.tmUser_FromEmail();

            Assert.Greater  (newUser2Id, 0);
            Assert.NotNull  (tmUser2_ById        ,"tmUser2_byId");
            Assert.NotNull  (tmUser2_ByName      ,"tmUser2_byName");
            Assert.IsNull   (tmUser2_ByEmail     ,"tmUser2_byEmail should not work for repeated emails");
            Assert.IsTrue   (tmUser2_ById == tmUser2_ByName && tmUser2_ById != tmUser2_ByEmail);
            Assert.IsTrue   (tmUser2_ById != tmUser_ByName  &&  tmUser2_ById != tmUser_ByEmail);
        }
        [Test] public void User_ExtMet_FullName()
        {
             var firstName           = "firstName".add_RandomLetters(5);
             var lastName            = "lastName".add_RandomLetters(5);
             var tmUser_NoValues     = new TMUser();
             var tmUser_JustFirst    = new TMUser() {FirstName = firstName};
             var tmUser_JustLast     = new TMUser() {LastName  = lastName};
             var tmUser_Both         = new TMUser() {FirstName = firstName, LastName  = lastName};

             //Check that values are correctly set
             Assert.IsTrue(tmUser_NoValues.notNull()  && tmUser_NoValues.FirstName.empty()   && tmUser_NoValues.LastName.empty(), "tmUser_NoValues setup");
             Assert.IsTrue(tmUser_JustFirst.notNull() && tmUser_JustFirst.FirstName.valid()  && tmUser_JustFirst.LastName.empty(), "tmUser_JustFirst setup");
             Assert.IsTrue(tmUser_JustLast.notNull()  && tmUser_JustLast.FirstName.empty()   && tmUser_JustLast.LastName.valid(), "tmUser_JustLast setup");
             Assert.IsTrue(tmUser_Both.notNull()      && tmUser_Both.FirstName.valid()       && tmUser_Both.LastName.valid(), "tmUser_Both setup");
             
             //Check that userName extension method is working ok
             var fullName_NoValues   = tmUser_NoValues.fullName();
             var fullName_JustFirst  = tmUser_JustFirst.fullName();
             var fullName_JustLast   = tmUser_JustLast.fullName();
             var fullName_Both       = tmUser_Both.fullName();

             Assert.AreEqual(fullName_NoValues , "");
             Assert.AreEqual(fullName_JustFirst, firstName);
             Assert.AreEqual(fullName_JustLast , "");
             Assert.AreEqual(fullName_Both     , "{0} {1}".format(firstName, lastName));

             //last check
             Assert.AreEqual("John Smith", new TMUser() { FirstName = "John", LastName = "Smith" }.fullName());
         }
        [Test]
        public void getUserGroupName()
        {
            var tmUser = "a user".createUser();
            Assert.AreEqual(userData.getUserGroupName(tmUser.UserID), "Reader");
            Assert.AreEqual(userData.getUserGroupId  (tmUser.UserID),  (int)UserGroup.Reader);
            tmUser.set_UserGroup(UserGroup.Editor);
            Assert.AreEqual(userData.getUserGroupName(tmUser.UserID), "Editor");
            Assert.AreEqual(userData.getUserGroupId  (tmUser.UserID),  (int)UserGroup.Editor);
            

            Assert.AreEqual(userData.getUserGroupName(10000.randomNumber()), null);
            Assert.AreEqual(userData.getUserGroupId  (10000.randomNumber()), -1);
        }

        [Test]
        public void tmUser_FromEmail()
        {
            var tmUser = "a user".createUser();
            Assert.AreEqual(userData.tmUser_FromEmail(tmUser.EMail), tmUser);
            Assert.AreEqual(userData.tmUser_FromEmail(null)        , null);
        }
    }
}
