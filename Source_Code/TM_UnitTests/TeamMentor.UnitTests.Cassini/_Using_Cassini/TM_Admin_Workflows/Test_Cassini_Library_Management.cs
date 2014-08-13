using System;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.Web;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_Cassini_User_Management : NUnitTests_Cassini_TeamMentor
    {
        WatiN_IE ie;
        string   admin_Name;
        string   admin_Pwd;
        string   server;

        [TestFixtureSetUp]
        public void testFixtureSetup()
        {
            admin_Name = "admin";
            admin_Pwd  = "!!tmadmin";
            ie = "Test_Cassini_User_Management".add_IE_Hidden_PopupWindow();
            //ie.parentForm().show();
            //ie.script_IE().waitForClose();
        }
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {            
            ie.close();
            ie.parentForm().close();
        }        
        [Test]
        public void Login_As_Admin()
        {            
            server 	   = apiCassini.url();
            ie.open(server.append("login").info());
			ie.field("username").value(admin_Name);
			ie.field("password").value(admin_Pwd);
			ie.button("login").click();            
            ie.waitForLink("About",250,20).assert_Not_Null();         
            ie.waitForLink("Logout").assert_Not_Null();
        }

        /// <summary>
        /// This issue occured when the login was performed on an IP adress and the
        /// post-login redirection was done into the DNS representation of that IP
        /// 
        /// For example if the login was done on http://127.0.0.1:12345/login and the 
        /// post-login redirection target was http://localhost:12345/
        /// 
        /// Issue Ref: https://github.com/TeamMentor/Master/issues/827 
        /// Status: Fixed on 3.5 RC1
        /// </summary>
        [Test]
        public void Issue_827_Login_redirect_does_not_always_work()
        {
            var expectedUrl = apiCassini.url().append("teamMentor");
            ie.parentForm().show();            
            Login_As_Admin();
            ie.url().assert_Is(expectedUrl);            
        }
            

    }
    [TestFixture]
    public class Test_Cassini_Library_Management : NUnitTests_Cassini_TeamMentor
    {
        WatiN_IE ie;
        string admin_Name;
        string admin_Pwd;
        string server;

        [SetUp]
        public void setup()
        {
            if (WebUtils.offline())   
                "Skipping test because we are offline".assert_Ignore();
            ie = "Test_Cassini_Library_Management".add_IE_PopupWindow(1000,600);            
            admin_Name = "admin";
            admin_Pwd  = "!!tmadmin";
            server 	   = apiCassini.url();
        }
        [TearDown]
        public void tearDown()
        {
            if (ie.notNull())
                ie.parentForm().close();
        }
        [Test]
        public void Workflow_Install_And_Delete_Library()
        {
            Action<string,string> waitForElementText = 
	            (elementId, text)=>{
							            "waiting for '{0}' in element '{1}'".info(text, elementId);
							            for(int i =0 ; i<5 ; i++)
								            if (ie.element(elementId).text().contains(text))																 	
								 	            return;								 
								            else	
									            elementId.sleep(1000,true);
							            "could not find '{0}' in element '{1}'".error(text, elementId);
					               };

            Action<string,string> login = 
	            (username,password)=>{
							            ie.open(server.append("login"));
                                        ie.url().assert_Contains("Login");
							            ie.field("username").value(username);
							            ie.field("password").value(password);                    
							            ie.button("login").click();
						             };
            Action logout 	  = ()=> ie.open(server.append("logout"));								             
            Action teamMentor = ()=> {
							            ie.open(server.append("teamMentor"));
							            ie.waitForLink("About");
						            };
            Action admin = ()=> ie.open(server.append("admin"));		

            Action login_AsAdmin = ()=>{
								            if (ie.hasLink("Control Panel").isFalse())		
								            {
									            logout();
									            login(admin_Name, admin_Pwd);
	                                            ie.waitForLink("About",250,20).assert_Not_Null();         
                                                ie.waitForLink("Logout").assert_Not_Null();
									            //teamMentor();
								            }
							            };
            Action installTestLibrary =
	            ()=>{
			            admin();			
			            ie.waitForComplete();
			            ie.waitForLink("advanced admin tools")    .click().assert_Not_Null();
			            ie.waitForLink("install/upload libraries").click().assert_Not_Null();;
			            ie.waitForLink("OWASP")                   .click().assert_Not_Null();
			            ie.button    ("Install")                  .click().assert_Not_Null();			            
			            waitForElementText("installMessage","> Library installed was successful");
			            ie.link      ("Admin Tasks")              .click().assert_Not_Null();			
			            waitForElementText("jsonResult", "...Via Proxy");
			            ie.link      ("Reload Server Cache")      .click().assert_Not_Null();
			            waitForElementText("jsonResult", "In the Folder");
			            ie.link      ("Open Main Page")           .click().assert_Not_Null();;
			            //ie.waitForLink("Reload Server Cache").click();
		            };
            Action deleteTestLibrary = 
	            ()=>{	
                        ie.waitForLink("OWASP").notNull().assert_True();

	                    var librariesBeforeRemove = ie.getJsVariable("window.TM.WebServices.Data.AllLibraries.length").cast<int>();
			            
				        ie.eval("window.TM.Gui.LibraryTree.remove_Library_from_Database('4738d445-bc9b-456c-8b35-a35057596c16')");		
			                       
                        for(var i=0; i < 5 ; i++)
			                if(ie.getJsVariable("window.TM.WebServices.Data.AllLibraries.length").cast<int>() < librariesBeforeRemove)		
                                return;
                            else
                                100.sleep();

                        "Library was not deleted".assert_Fail();
		            };

            teamMentor();
            login_AsAdmin();
            //ie.script_IE_WaitForClose();
            installTestLibrary();
            deleteTestLibrary();

            ie.hasLink("OWASP").assert_False();
        }
    }
}
