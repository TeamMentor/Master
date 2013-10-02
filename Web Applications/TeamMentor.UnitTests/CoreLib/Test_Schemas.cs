using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_Schemas
    {
        [Test]
        public void Validation_TM_User()
        {            
            var tmUser            = new TM_User();                        
            //var requiredValues    = "Company,Country,FirstName,LastName,State,Title,UserName,Email".split(",");
            var requiredValues    = "UserName,Email".split(",");
            var validationResults = tmUser.validate();
            var resultsMapped     = validationResults.indexed_By_MemberName();
            var validationok      = tmUser.validation_Ok();

            //foreach (var result in validationResults)
            //    "{0} - {1}".info(result.MemberNames.asString(), result.ErrorMessage);
            
            Assert.IsNotEmpty(validationResults                              , "Validation results should not be empty");
            Assert.IsFalse   (validationok                                   , "Validation Ok should be false");
            Assert.IsFalse   (resultsMapped.hasKey("UserName__A")            , "There should be no mapping here");  
            Assert.AreEqual  (requiredValues.size(), validationResults.size(), "# of validation results");

            foreach (var requiredValue in requiredValues)
            {
                Assert.IsTrue  (resultsMapped.hasKey(requiredValue), "requiredValue not found in mapped data: {0}".format(requiredValue));
                Assert.AreEqual(resultsMapped[requiredValue].first(), "The {0} field is required.".format(requiredValue));
            }


        }

        [Test]
        public void Validation_NewUser_RequiredFields()
        {            
            var newUser            = new NewUser();                        
            var requiredValues    = "Company,Country,Firstname,Lastname,Note,Password,State,Title,Username,Email".split(",");
            var validationResults = newUser.validate();
            var resultsMapped     = validationResults.indexed_By_MemberName();
            var validationok      = newUser.validation_Ok();

            //foreach (var result in validationResults)
            //    "{0} - {1}".info(result.MemberNames.asString(), result.ErrorMessage);
            
            Assert.IsNotEmpty(validationResults                              , "Validation results should not be empty");
            Assert.IsFalse   (validationok                                   , "Validation Ok should be false");
            Assert.IsFalse   (resultsMapped.hasKey("UserName__A")            , "There should be no mapping here");  
            Assert.AreEqual  (requiredValues.size(), validationResults.size(), "# of validation results");

            foreach (var requiredValue in requiredValues)
            {
                Assert.IsTrue  (resultsMapped.hasKey(requiredValue), "requiredValue not found in mapped data: {0}".format(requiredValue));
                Assert.AreEqual(resultsMapped[requiredValue].first(), "The {0} field is required.".format(requiredValue));
            }
        }

        [Test]  
        public void Validation_NewUser_LargeDataInFields()
        {            
            var newUser = new NewUser();                                // new empty user object            
            foreach (var property in newUser.type().properties())       // populate all strings 
                newUser.prop(property.Name, 5001.randomLetters());      //  ... with a random 5001 char value           

            var validationResults = newUser.validate();                 // validate
            var resultsMapped     = validationResults.indexed_By_MemberName();

            foreach (var result in resultsMapped)                       // checks validation errors                
                Assert.IsTrue(result.Value.first() .contains("The field {0} must be a string with a maximum length of".format(result.Key)) ||
                              result.Value.second().contains("The field {0} must be a string with a maximum length of".format(result.Key)) , 
                             "mappings : {0}".format(result.Value.toString()));
        }
        
        [Test]
        public void Validation_Email_Size()
        {
            var newUser             = new NewUser();                        
            var loopMax             = 100;         
            var expectedMaxLength   = 50;
            for (int i = 1; i < loopMax; i++)
            {
                newUser.Email = (i*10).randomLetters();     //works quite fast even with values as hight as 1000000                
                var dateStart = DateTime.Now;
                var validationResults = newUser.validate();
                var resultsMapped     = validationResults.indexed_By_MemberName();
                var seconds = (DateTime.Now - dateStart).TotalSeconds;
                Assert.Less(seconds,1, "A email with size {0} took more than 1 sec to calculate".format(i));
                Assert.IsTrue(resultsMapped["Email"].contains("The field Email must match the regular expression '{0}'.".format(ValidationRegex.Email)), "It was {0}".format(resultsMapped["Email"].toString()));
                if (i > expectedMaxLength)
                {
                    Assert.AreEqual(resultsMapped["Email"].size()  , 2);
                    Assert.IsTrue  (resultsMapped["Email"].contains("The field Email must be a string with a maximum length of {0}.".format(expectedMaxLength)));
                }                
            }
        }

        [Test]
        public void Validation_Email_Format()
        {
            var shouldFailValidation = new []
                {
                    "aaa", "bbb", "aa.bb", "aa.bb", "a@b","a@.b.c",
                    "a;aaa@email.com","aaa@em;ail.com", "aaa@email.c;om"
                };
            var shouldPassValidation = new []
                {
                    "aaaa@email.com","dcruz@securityinnovation.com", "dinis.cruz@owasp.org", "dinis-cruz@owasp.org", "dinis-cruz@securityinnovation-europe.com", "dinis+cruz@owasp.org"
                }; 

            Func<string,bool> validEmail = 
                (email)=>{
                             var newUser = new NewUser { Email = email };   // create new user  
                             return newUser.validate()                      // validate
                                           .indexed_By_MemberName()         // get dictionary with results
                                           .hasKey("Email")                 // see if email 
                                           .isFalse();                      // is not there
                };
            
            foreach(var testEmail in shouldFailValidation )                 // these should all fail
                Assert.IsFalse(validEmail(testEmail), "Should had failed for: {0}".format(testEmail));
            
            foreach(var testEmail in shouldPassValidation )                 // these should all work
                Assert.IsTrue(validEmail(testEmail), "Should had worked for: {0}".format(testEmail));
        }

        /*
        //see blog post
        //with this regex in ValidationRegex 
        // public const string Email = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
        [Test]
        public void Validation_Email_DOS()
        {
            var newUser             = new NewUser();                        
            var loopMax             = 30;               // starts to struglle from 20;
            var maxSeconds          = 10;
            for (int i = 1; i < loopMax; i++)
            {
                newUser.Email = i.randomLetters();     //works quite fast even with values as hight as 1000000                
                var dateStart = DateTime.Now;
                var validationResults = newUser.validate();
                var resultsMapped     = validationResults.indexed_By_MemberName();
                var seconds = (DateTime.Now - dateStart).TotalSeconds;
                Assert.Less(seconds,maxSeconds, "A email with size {0} took more {1} sec to calculate (max is {2})".format(i, seconds, maxSeconds));
                Assert.IsTrue(resultsMapped["Email"].first().contains("The field Email must match the regular expression"));
                "for {0} size, it took {2}".info(i, validationResults.size(), dateStart.duration_to_Now());
            }
        }
         */
    }
}
