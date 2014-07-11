using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website._Using_Cassini.TM_Admin_Workflows
{
    [TestFixture]
    public class Test_Cassini_User_Management : NUnitTests_Cassini_TeamMentor
    {
        WatiN_IE ie;
        string   admin_Name;
        string   admin_Pwd;
        string   server;

        public Test_Cassini_User_Management()
        {
            ie = "Test_Cassini_User_Management".add_IE_PopupWindow(1000,600);
            
            admin_Name = "admin";
            admin_Pwd  = "!!tmadmin";
            
        }
       
        [Test]
        public void Login_As_Admin()
        {
            server 	   = apiCassini.url();
            //ie.downloadAndExecJavascriptFile
            //var whoAmI = ie.open_and_HandleFileDownload(server.append("WhoAmI")).html();
            //whoAmI.assert_Equal_To("123");
            ie.open(server.append("login").info());
			ie.field("username").value(admin_Name);
			ie.field("password").value(admin_Pwd);
			ie.button("login").click();
            ie.waitForLink("About").assert_Not_Null();
            ie.showMessage("found About link");
            ie.parentForm().close();
        }

    }
    [TestFixture]
    public class Test_Cassini_Library_Management : NUnitTests_Cassini_TeamMentor
    {
        WatiN_IE ie;
        string admin_Name;
        string admin_Pwd;
        string server;

        public Test_Cassini_Library_Management()
        {
            ie = "Test_Cassini_Library_Management".add_IE_PopupWindow(1000,600);
            
            admin_Name = "admin";
            admin_Pwd  = "!!tmadmin";
            server 	   = apiCassini.url();
        }
        [Test] [Ignore("Under Dev")]
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
            Action tbot 	  = ()=> ie.open(server.append("tbot"));		
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
									            teamMentor();
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
			            if (ie.waitForLink("OWASP").notNull().assert_True()) 
				            ie.eval("window.TM.Gui.LibraryTree.remove_Library_from_Database('4738d445-bc9b-456c-8b35-a35057596c16')");		
			
			            5.loop((i)=>{				
						                100.sleep();
							            return (ie.getJsVariable("window.TM.WebServices.Data.AllLibraries.length").cast<int>()) > 0;
						            });
                        ie.getJsVariable("window.TM.WebServices.Data.AllLibraries.length").cast<int>().assert_Is_Equal_To(0);
		            };

            teamMentor();
            login_AsAdmin();
            installTestLibrary();
            deleteTestLibrary();

            ie.hasLink("OWASP").assert_True();
        }
    }
}
