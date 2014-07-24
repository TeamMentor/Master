using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_IE
{
    [TestFixture]
    public class Test_QA_Users : NUnitTests_Cassini_TeamMentor
    {
        [SetUp] public void setup()
        {            
            this.tmProxy_Refresh();
        }
        [Test] public void Sign_Up_For_New_Account()
        {
            var ieTeamMentor = this.new_IE_TeamMentor();
            var ie = ieTeamMentor.ie;

            Action<NewUser> signup_For_New_Account = (newUser)=>
	        {
		        ie.waitForField("ctl00_ContentPlaceHolder1_UsernameBox"      ).value(newUser.Username   ).assert_Not_Null();
		        ie.field       ("ctl00_ContentPlaceHolder1_PasswordBox"      ).value(newUser.Password   ).assert_Not_Null();
		        ie.field       ("ctl00_ContentPlaceHolder1_RepeatPasswordBox").value(newUser.Password   ).assert_Not_Null();
		        ie.field       ("ctl00_ContentPlaceHolder1_EmailBox"	     ).value(newUser.Email      ).assert_Not_Null(); 
		        ie.field       ("ctl00_ContentPlaceHolder1_FNameBox"	     ).value(newUser.Firstname  ).assert_Not_Null();
		        ie.field       ("ctl00_ContentPlaceHolder1_LNameBox"	     ).value(newUser.Lastname   ).assert_Not_Null();  
		        ie.field       ("ctl00_ContentPlaceHolder1_Company"	         ).value(newUser.Company    ).assert_Not_Null(); 
		        ie.field       ("ctl00_ContentPlaceHolder1_Title"		     ).value(newUser.Title      ).assert_Not_Null();  
		        ie.field       ("ctl00_ContentPlaceHolder1_Country"	         ).value(newUser.Country    ).assert_Not_Null();  
		        ie.field       ("ctl00_ContentPlaceHolder1_State"		     ).value(newUser.State      ).assert_Not_Null();
		        ie.button      ("Sign Up").click();
	        };
            
            //create Random user
            var random_NewUser = new NewUser();
            foreach(var name in random_NewUser.property_Values_AsStrings().keys())
	            random_NewUser.property(name,"!!10".add_5_RandomLetters());
            random_NewUser.Email = "testUser".random_Email();

            ieTeamMentor.page_Logout();
            
            signup_For_New_Account(random_NewUser);
            ie.wait_For_Element_InnerText("signupMessage", "Account created successfully\r\n\r\nPlease Login").assert_Not_Null();	
            ie.link        ("Login")       .click()                                                           .assert_Not_Null();
            ie.waitForField("UsernameBox") .value(random_NewUser.Username)                                    .assert_Not_Null();
            ie.field       ("PasswordBox") .value(random_NewUser.Password)                                    .assert_Not_Null();
            ie.button      ("<span class=ui-button-text> Login</span>").click()                               .assert_Not_Null();
            ie.waitForLink ("Logout")                                                                         .assert_Not_Null();
            ie.element     ("topRightMenu").innerText()
                                           .assert_Contains("Logged in as", random_NewUser.Username);

            ieTeamMentor.close();
        }

        [Test] public void Password_Reset_Workflow()
        {
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden();
            var ie           = ieTeamMentor.ie;

            Func<List<EmailMessage>> email_Sent_EmailMessages = ()=>
	            {
		            return tmProxy.get_Property_Static<List<EmailMessage>>(typeof(SendEmails), "Sent_EmailMessages");
	            };
            Func<string> email_TM_Server_URL = ()=>
	            {
		            return tmProxy.get_Property_Static<string>(typeof(SendEmails), "TM_Server_URL");
	            };	

            var user = tmProxy.user_New().assert_Not_Null();            // create temp user

            ieTeamMentor.page_Login();                                  // go to login page
            ie.link("Forgot your password?").click();                   // go to password forgot
            
            ie.waitForField("email").value(user.EMail);                 // request reset link
            ie.button("submitButton").click();
            
            for(var i=0; i< 5; i++)
            {
                var sentMessages = email_Sent_EmailMessages();
                if (sentMessages.last().Message.contains("You can change the password of your"))
                    break;
                250.sleep();                                            // need better solution to have async email sending TM architecture 
            }
            email_Sent_EmailMessages().last()
                                      .Message.info()
                                      .assert_Contains("You can change the password of your");            
            ieTeamMentor.close();
        }
    }
}
