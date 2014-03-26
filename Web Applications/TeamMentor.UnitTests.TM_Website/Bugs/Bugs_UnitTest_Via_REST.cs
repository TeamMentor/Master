using System;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.Bugs 
{
    [TestFixture][Ignore("move to a different project")]
    public class Bugs_TBot_Via_REST : API_IE_TBot
    {
        [Test]
        public void Bug_IE_Out_of_Sync_afer_REST_Call___PoC()
        {
            var page1_Url = "http://localhost:3187/tbot_users/default.htm?";
            var page1_Text = "TBot v2.0";
            var page2_Url  = "http://localhost:3187/tbot_users/users.htm?";
            var page2_Text = "Welcome to the Users page";

            Action checkPage1 = ()=>
                {
                    Assert.AreEqual   (ie.url(),page1_Url);
                    Assert.AreNotEqual(ie.url(),page2_Url);
                    Assert.IsTrue     (html().contains(page1_Text));               
                    Assert.IsFalse    (html().contains(page2_Text));          
                };

            Action checkPage2 = ()=>
                {
                    Assert.AreNotEqual(ie.url(),page1_Url);
                    Assert.AreEqual   (ie.url(),page2_Url);
                    Assert.IsFalse    (html().contains(page1_Text));  
                    Assert.IsTrue     (html().contains(page2_Text));                     
                };       
     
            //open and check page1 and page1
            ie.open(page1_Url);
            checkPage1();
            
            ie.open(page2_Url);
            checkPage2();

            // rest request that confuses the IE object
            var restRequest = TargetServer + "/rest/login/user/pwd";
            ie.open(restRequest);
            // and now all these are wrong (i.e. the next set of tests should be reversed)
            Assert.AreNotEqual(ie.url(),restRequest);   // ie.url() should be == restRequest  
            Assert.AreEqual   (ie.url(),page2_Url);     // ie.url() should not be page2_Url
            
            ie.open(page1_Url);                         // this is also not working as expected
            checkPage2();                               // since this will still pass (and it should fail)            

            this.close_IE().open_IE();                  // see Bug_IE_Out_of_Sync_afer_REST_Call___Fix()
        }

        [Test]public void Bug_IE_Out_of_Sync_afer_REST_Call___Fix   ()
        {
            ie.open(TargetServer + "/logout");
            var page1_Url   = TargetServer + "/tbot_users/default.htm?";
            var page2_Url   = TargetServer + "/tbot_users/users.htm?";
            var restRequest = TargetServer + "/rest/login/user/pwd";
            ie.open(page1_Url);
            ie.open(restRequest);                       // open rest request that confuses the IE object
            ie.open(page2_Url);
            Assert.AreNotEqual(ie.url(),restRequest);   // ie.url() should be == restRequest  
            Assert.AreEqual   (ie.url(),page2_Url);     // ie.url() should not be page2_Url
            
            this.close_IE();                            // forces the creation of a new IE object on next UnitTest
            this.open_IE();                             // or like this

            ie.open(page1_Url);
            Assert.AreEqual   (ie.url(),page1_Url);    
            ie.open(page2_Url);
            Assert.AreEqual   (ie.url(),page2_Url);    
            ie.open(page1_Url);
            Assert.AreEqual   (ie.url(),page1_Url);    
        }
    }
}
