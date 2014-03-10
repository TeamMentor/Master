// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Security;
using NUnit.Framework; 
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{		  
    [TestFixture] 
    public class Test_Authentication : TM_WebServices_InMemory
    {
        public string default_Pwd   { get; set;}
        public string user_admin    { get; set;}    	
        public string user_editor   { get; set;}
        public string user_reader   { get; set;}
        public string user_test   { get; set;}

        [Assert_Admin]
        public Test_Authentication()
        {
            user_admin   = TMConfig.Current.TMSecurity.Default_AdminUserName;
            default_Pwd  = TMConfig.Current.TMSecurity.Default_AdminPassword;
            user_editor  = "editor";
            user_reader  = "reader";
            user_test    = "test";

            var id_Editor = tmXmlDatabase.UserData.newUser(user_editor, default_Pwd, "", (int) UserGroup.Editor);
            var id_Reader = tmXmlDatabase.UserData.newUser(user_reader, default_Pwd, "", (int) UserGroup.Reader);
            var id_Test   = tmXmlDatabase.UserData.newUser(user_test, default_Pwd );

            Assert.Greater(id_Editor, 0, "id_Editor");
            Assert.Greater(id_Reader, 0, "id_Reader");
            Assert.Greater(id_Test  , 0, "id_Test");
        }

        [SetUp]public void SetUp()
        {
            UserGroup.None.setThreadPrincipalWithRoles();   // reset current thread roles
        }

        [Test] public void Login_PwdInClearText()
        {			
            var sessionId_Admin  = tmWebServices.Login(user_admin , default_Pwd);
            var sessionId_Editor = tmWebServices.Login(user_editor, default_Pwd);
            var sessionId_Reader = tmWebServices.Login(user_reader, default_Pwd);
            Assert.AreNotEqual(sessionId_Admin  , Guid.Empty,"sessionId_Admin was empty");			
            Assert.AreNotEqual(sessionId_Editor , Guid.Empty,"sessionId_Editor was empty");			
            Assert.AreNotEqual(sessionId_Reader , Guid.Empty,"sessionId_Reader was empty");			

        }						
        [Test] public void Current_SessionID_Current_User_GetCurrentUserRoles()
        {
            //create test user
            var user = "test_user_aaa";
            var pwd = "bb";						
            var newUser = tmXmlDatabase.UserData.newUser(user, pwd);
            

            //test on tmWebServices
            var sessionId = tmWebServices.Login(user, pwd);
            Assert.AreEqual(sessionId, tmWebServices.Current_SessionID(), "tmWebServices.CurrentSessionID");
            Assert.AreEqual(user, tmWebServices.Current_User().UserName, "tmWebServices.CurrentSessionID");
            var roles = tmWebServices.GetCurrentUserRoles();			
            Assert.AreEqual(roles.size(), 2, "userRoles size");
            Assert.AreEqual("ReadArticlesTitles", roles[0], "first userRole");						
             
            "deleting user".info(); 
            UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin
            
            //delete user
            Assert.That(tmWebServices.DeleteUser(newUser).isTrue() , "failed to test user");						
            
        }	
        [Test] public void RBAC_batchUserCreation()
        {	
            //3 users to create
            var userName1 = "test_user_".add_RandomLetters(4);
            var userName2 = "test_user_".add_RandomLetters(4);
            var userName3 = "test_user_".add_RandomLetters(4);
            var batchUserCreation =  userName1 + ",pwd!@#asd,firstname,lastname, 1".line() + 
                                     userName2 + ",pwd!@#asd,firstname,lastname, 3".line() + 
                                     userName3 + "".line() + 
                                     userName2;
                
            //create users			 
            UserGroup.Admin.setThreadPrincipalWithRoles();  			  
            var newUsers = tmWebServices.BatchUserCreation(batchUserCreation);
            Assert.NotNull(newUsers[0], "user1 ok");
            Assert.NotNull(newUsers[1], "user2 ok");
            Assert.IsNull (newUsers[2], "user3 should not had been created");
            Assert.IsNull (newUsers[3], "duplicate user was created"); 
            Assert.AreEqual(newUsers[0].UserName, userName1, "userName1");
            Assert.AreEqual(newUsers[1].UserName, userName2, "userName2"); 			
            
            //check if users where created
            var userIds = newUsers.userIds();
            newUsers = tmWebServices.GetUsers_byID(userIds);
            Assert.AreEqual(newUsers.size(), 2, "fetched new users");
            
            //delete users
            var result = tmWebServices.DeleteUsers(userIds);
            Assert.That(result[0] && result[1], "users deleted ok");
            
            //check if users where deleted
            var deletedUsers = tmWebServices.GetUsers_byID(userIds);
            Assert.That(deletedUsers[0].isNull() && deletedUsers[1].isNull(), "users deleted where not there");			
        }

        public Guid Login_As_User(string username, string password)
        {
            var sessionId = tmWebServices.Login(username, password);
            HttpContextFactory.Context.addCookieFromResponseToRequest("Session");
            Assert.AreNotEqual(sessionId , Guid.Empty, "Failed to login As {0}".format(username));
            Assert.AreEqual   (tmWebServices.Current_User().UserName, username);
            return sessionId;
        }

        [Test] 
        public void Login_As_Admin()
        {
            Login_As_User(user_admin, default_Pwd);
        }
        [Test] 
        public void Login_As_Editor()
        {
            Login_As_User(user_editor, default_Pwd);
        }
        [Test] 
        public void Login_As_Reader()
        {
            Login_As_User(user_reader, default_Pwd);
        }
        [Test] 
        public void Login_As_Test()
        {
            Login_As_User(user_test, default_Pwd);
        }

        
                                
        
        
        
        
        ///**********************
        ///*** webMethod_IsUserAdmin
        ///*** 
        [Test]
        public void RBAC_IsAdmin()
        {   
            Login_As_Admin();    	        		    					
            var admin_Is_Admin = tmWebServices.RBAC_IsAdmin();						
            Login_As_Editor();
            var editor_Is_Admin = tmWebServices.RBAC_IsAdmin();
            Login_As_Reader();
            var reader_Is_Admin = tmWebServices.RBAC_IsAdmin();
            Login_As_Test();
            var test_Is_Admin = tmWebServices.RBAC_IsAdmin();
            Login_As_Admin();                                       // try again just to make sure 	        		    					
            var admin_Is_Admin2 = tmWebServices.RBAC_IsAdmin();						

            Assert.IsTrue (admin_Is_Admin, "admin_Is_admin");
            Assert.IsFalse(editor_Is_Admin, "editor_Is_Admin");
            Assert.IsFalse(reader_Is_Admin, "reader_Is_Admin");
            Assert.IsFalse(test_Is_Admin, "test_Is_Admin");
            Assert.IsTrue (admin_Is_Admin2, "admin_Is_Admin2");    	    
        }    	
    
                
        ///**********************
        ///*** webMethods_DemandPrivileges_Admin_and_DemandPrivileges_ReadArticles
        ///*** 
        [Test]
        public void RBAC_Demand_Admin_Editor_Reader_Tester()
        {
            Login_As_Test();
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_Admin       (), "logged in as Test");
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_EditArticles(), "logged in as Test");   
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_ManageUsers (), "logged in as Test");
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_ReadArticles(), "logged in as Test");

            Login_As_Reader();
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_Admin       (), "logged in as Reader");
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_EditArticles(), "logged in as Reader");   
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_ManageUsers (), "logged in as Reader");
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_ReadArticles(), "logged in as Reader");

            Login_As_Editor();
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_Admin       (), "logged in as Editor");
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_EditArticles(), "logged in as Editor");   
            Assert.Throws<SecurityException>(()=> tmWebServices.RBAC_Demand_ManageUsers (), "logged in as Editor");
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_ReadArticles(), "logged in as Editor");

            Login_As_Admin();
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_Admin       (), "logged in as Admin");
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_EditArticles(), "logged in as Admin");   
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_ManageUsers (), "logged in as Admin");
            Assert.DoesNotThrow             (()=> tmWebServices.RBAC_Demand_ReadArticles(), "logged in as Admin");
            /*var randomSessionID = Guid.NewGuid();			//TMLoginHelper.login_As_Admin();  ;//Guid.NewGuid();			
            teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = randomSessionID } ;
            
            try { teamMentorSecurity.DemandPrivileges_Admin(); } 
            catch(Exception ex) { Assert.That(ex.Message.contains("Request for principal permission failed"), "wrong exception");}
            
            try { teamMentorSecurity.DemandPrivileges_ReadArticles(); } 
            catch(Exception ex) { Assert.That(ex.Message.contains("Request for principal permission failed"), "wrong exception."); }
                        
            var adminSessionID = TMLoginHelper.login_As_Admin();  
            teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = adminSessionID } ;
            teamMentorSecurity.DemandPrivileges_Admin();
            
            var readerSessionID = TMLoginHelper.login_As_Reader();  
            teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = readerSessionID } ;
            teamMentorSecurity.DemandPrivileges_ReadArticles();
                        
            return "ok: webMethods_DemandPrivileges_Admin_and_DemandPrivileges_ReadArticles";
             */
        }    
        
    }
}
