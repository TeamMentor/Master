using System;
using FluentSharp.CoreLib;
using FluentSharp.Git;
using FluentSharp.NUnit;
using FluentSharp.REPL;
using FluentSharp.Watin;
using FluentSharp.WatiN;
using FluentSharp.WatiN.NUnit;
using FluentSharp.Web35;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_IE
{
    [TestFixture]
    public class Test_QA__3_5_Issues : NUnitTests_Cassini_TeamMentor
    {
        TM_FileStorage tmFileStorage;

        [SetUp] public void setup()
        {      
            this.tmProxy_Refresh()
                .tmProxy.assert_Not_Null();
            tmFileStorage = tmProxy.TmFileStorage.assert_Not_Null();
            
        }
        
        [Test] public void Issue_310_Application_Logs_should_be_written_real_time()
        {
            admin.assert();

            tmProxy.TmServer.assert_Not_Null()
                            .RealTime_Logs.assert_False();

            tmProxy.get_Current<TM_StartUp>()   .assert_Not_Null()
                   .TrackingApplication         .assert_Not_Null()
                   .RealTime_LogFilePath        .assert_Null();

            var tmServerLocation = tmProxy.TmFileStorage.tmServer_Location().assert_File_Exists();
            tmServerLocation.load<TM_Server>()
                            .realTime_Logs(true)
                            .saveAs(tmServerLocation);            
            
            tmProxy.invoke_Instance(typeof(TM_StartUp),"Application_Start");            // restart TM
            
            tmProxy = this.tmProxy();

            tmProxy.TmServer.assert_Not_Null()
                            .RealTime_Logs.assert_True();
            var tm_StartUp = tmProxy.get_Current<TM_StartUp>().assert_Not_Null();
            tm_StartUp.TrackingApplication                    .assert_Not_Null()
                      .RealTime_LogFilePath                   .assert_Not_Null()
                                                              .assert_File_Exists();

            var logFilePath = tm_StartUp.trackingApplication().realTime_LogFilePath()
                                                              .assert_File_Exists();
            var contents_Before =  logFilePath.fileContents();
            {       
                siteUri.append("404").GET().assert_Contains("Error");   
            }
            var contents_After = logFilePath.fileContents();

            contents_After.assert_Is_Not(contents_Before);

            var ieTeamMentor = this.new_IE_TeamMentor();
            ieTeamMentor.login_Default_Admin_Account("/tbot");
            ieTeamMentor.ie.waitForLink("DebugInfo").click();
            ieTeamMentor.ie.html().assert_Contains(logFilePath);

            tmServerLocation.load<TM_Server>()
                            .realTime_Logs(false)
                            .saveAs(tmServerLocation);

        }     
        [Test] public void Issue_681__Navigating_libraries_views_folders__Clicking_the_icon_doesnt_work()
        {
            var tmWebServices  = new TM_WebServices();            
            var ieTeamMentor   = this.new_IE_TeamMentor_Hidden();
            var ie             = ieTeamMentor.ie;

            Func<string, string> clickOnNodeUsingJQuerySelector = 
	            (jQuerySelector)=>
		            {			
			            ie.invokeEval("TM.Gui.selectedGuidanceTitle=undefined");
			            ie.invokeEval("$('#{0}').click()".format(jQuerySelector));
			            ie.waitForJsVariable("TM.Gui.selectedGuidanceTitle");
			            return ie.getJsObject<string>("TM.Gui.selectedGuidanceTitle");
		            };

            if (tmProxy.libraries().notEmpty())
            {
	            "Ensuring the the only library that is there is the TM Documentation".info();
	            foreach(var library in tmProxy.libraries())
		            if(library.Caption != "TM Documentation") 
		            {
			            "deleting library: {0}".debug(library.Caption);
			            tmProxy.library_Delete(library.Caption);
		            }	          
            }   

            UserRole.Admin.assert();

            tmProxy.library_Install_Lib_Docs();
	        tmProxy.cache_Reload__Data();
            tmProxy.show_ContentToAnonymousUsers(true);

            ieTeamMentor.page_Home();        
            //tmWebServices.script_Me_WaitForClose();;
            //ieTeamMentor.script_IE_WaitForComplete();

            ie.waitForJsVariable("TM.Gui.selectedGuidanceTitle");            

            var _jsTree =  tmWebServices.JsTreeWithFolders();
            var viewNodes = _jsTree.data[0].children;				// hard coding to the first library
            var view1_Id    = viewNodes[0].attr.id;
            var view5_Id    = viewNodes[4].attr.id;


            var click_View_1_Using_A    = clickOnNodeUsingJQuerySelector(view1_Id + " a"  );
            var click_View_5_Using_A    = clickOnNodeUsingJQuerySelector(view5_Id + " a"  );
            var click_View_1_Using_Icon = clickOnNodeUsingJQuerySelector(view1_Id + " ins"  );
            var click_View_5_Using_Icon = clickOnNodeUsingJQuerySelector(view5_Id + " ins"  );


            (click_View_1_Using_A != click_View_5_Using_A   ).assert_True(); 
            
            (click_View_5_Using_A == click_View_1_Using_Icon).assert_False(); // (Issue 681) this was true since the view was not updating
            (click_View_5_Using_A == click_View_5_Using_Icon).assert_True(); 
            
            ie.close();
            ieTeamMentor.close();
        }
    
        [Test] public void Issue_812__HTML_view_articles_does_not_show_up_TEAM_Mentor_copyright_footer_note()
        {
            var texts_That_Confirm_Html_Footer = new [] { "TEAM Mentor © 2007-2014 all rights reserved", "eKnowledge Product" };

            var article = tmProxy.admin_Assert()                        // assert admin usergroup
                                    .article_New()                         // create a new article
                                    .assert_Not_Null();

            this.new_IE_TeamMentor_Hidden()                             // get an IE window mapped with the IE_TeamMentor API
                .login_Default_Admin_Account()
                .article_Html(article)                                  // open the article_Html view (ie /html/{artice Guid} )
                .html()                                                 // get the Html of the current page
                .assert_Contains(texts_That_Confirm_Html_Footer);       // assert that the expected texts are there                 
        }

        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/830
        /// </summary>
        [Test] public void Issue_830__Issues_in_git_file_history__Lets_get_rid_of_it_this_release()
        {
            var markdown_EditPage = "/Markdown/Editor?articleId=";            // link that will open the markdown editor
            
            this.new_IE_TeamMentor_Hidden()           
                .login_Default_Admin_Account(markdown_EditPage)               // Login as admin and redirect to markdown edit page
                .ie.assert_Has_Link        ("back to article")                // check that this link is there
                   .assert_Doesnt_Have_Link("View File History and diff");    // (Issue_830) for the 3.5 release this link should not be there            
        }
        
        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/829
        /// </summary>
        [Test] public void Issue_829__Get_rid_of_Control_Panel()
        {
            var ie = this.new_IE_TeamMentor_Hidden()
                         .login_Default_Admin_Account("/TeamMentor")
                         .ie;
           
            // location 1) check link on home page
            ie.assert_Uri_Is(siteUri.mapPath("/TeamMentor"))             // confirm that we are on the main TM page
              .wait_For_Link                ("Logout")                   
              .assert_Has_Link              ("Logout")
              .assert_Has_Link              ("Tbot")
              .assert_Doesnt_Have_Link      ("Control Panel");           // (Issue_830) for the 3.5 release this link should not be there            
            
            // location 2) check link on Tbot page
            ie.link("TBot").click();
            ie.assert_Uri_Is(siteUri.mapPath("/rest/tbot/run/Commands")) 
              .assert_Has_Link              ("Restart Server")
              .assert_Has_Link              ("TBot Monitor")
              .assert_Doesnt_Have_Link      ("Control Panel (legacy)");
            
            //  location 3)check for link on TBot Monitor 
            ie.link("TBot Monitor").click();
            ie.assert_Uri_Is(siteUri.mapPath("/Tbot_Monitor/monitor.htm#/monitor/activities")) 
              .assert_Has_Link              ("TeamMentor Admin (TBot)")
              .assert_Doesnt_Have_Link      ("Control Panel (legacy)");
        }
        
        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/831
        /// 
        /// The fix was to remove a legacy document.redirect capability from login.html 
        /// so this test checks for the correct redirection using the current server-side redirections
        /// </summary>
        [Test] public void Issue_831__Virtual_Article_redirect_does_not_Work_if_not_logged_in()
        {
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden();
            var ie           = ieTeamMentor.ie;
            var userName    = 10.randomLetters();
            var password    = "!abc!".add_5_RandomLetters();
            var articleId   = Guid.NewGuid();                                                          // we can use any GUID here since what we're after is the redirection
            
            tmProxy.get_Current<TMConfig>().TMSetup.ShowDotNetDebugErrors = true;

            var tmUser = tmProxy.user_New(userName, password).assert_Not_Null()                                     // create a temp user
                                .UserName.user(tmProxy)      .assert_Not_Null()
                                .UserID.user(tmProxy)        .assert_Not_Null();

            
            Action<string,string> checkUrl = (originalUrl,redirectUrl)=>
            {
                tmProxy.user_Logout(tmUser);                                                            // its faster to logout the user via tmProxy (which will 'server-side' clean the login sessions for this user)
                
                ieTeamMentor.open(originalUrl);                                                         // open targetUrl
                ie.url().assert_Contains("Html_Pages/Gui/Pages/login.html")                             // which should redirect 
                  .uri().queryParameters_Indexed_ByName().value("LoginReferer").assert_Is(originalUrl); // with LoginReferer set to targetUrl 

                ie.waitForField("username").value(userName).assert_Not_Null();                          // login with the temp reader user
                ie.field       ("password").value(password).assert_Not_Null();           
                ie.button      ("Login"   ).click()        .assert_Not_Null();

                var expectedUri = ieTeamMentor.siteUri.append(redirectUrl);                             // wait for redirect to happen                
                ie.wait_For_Uri(expectedUri,3000)                			                                            
                  .assert_Uri_Is(expectedUri);
            };

            checkUrl("/article/" + articleId, "/article/" + articleId);                                 // these should work for normal users (i.e. tmUser object)            
            checkUrl("/html/"    + articleId, "/html/"    + articleId);
            checkUrl("/content/" + articleId, "/content/" + articleId);
            checkUrl("/jsonp/"   + articleId, "/jsonp/"   + articleId);
            //checkUrl("/raw/"     + articleId, "/raw/"     + articleId);                               // ie doesn't like when there is no content shown on this xml doc
            checkUrl("/tbot"                , "/Html_Pages/Gui/Pages/login.html?LoginReferer=/tbot");   // should fail here for normal users (i.e. readers)
              
            tmProxy.user_Make_Admin(tmUser).assert_Not_Null()                                           // make the temp user an admin            
                                           .userGroup().assert_Is(UserGroup.Admin);
            tmProxy.user_Logout(tmUser);
            ieTeamMentor.page_WhoAmI();            
            checkUrl("/tbot"                , "/rest/tbot/run/Commands");                               // should work for admins
            ieTeamMentor.close();
        }
        [Test] public void Issue_838__SiteData_custom_TBot_pages_can_conflict_with_the_main_TBot_pages()
        {   
            //Open main Tbot Page and capture number of links          
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden();
            var ie = ieTeamMentor.ie;

            ieTeamMentor.login_Default_Admin_Account()
                        .page_TBot()
                        .html().assert_Contains("your friendly TeamMentor Bot", "Welcome to the TBot control center");
                        
            var links_Size = ieTeamMentor.ie.links().size().assert_Bigger_Than(15);

            //Create a Custom TbotPage and confirm it can be executed ok
            tmFileStorage.path_SiteData()
                         .folder_Create_Folder("Tbot")                                                      // create a TBot folder in the Site_Data folder
                         .folder_Create_File("tbotPage.cshtml", "this is an TBot page: @(40+2)");           // create a test Razor page inside it
            
            ieTeamMentor.page_TBot();                              // Refresh page

            ie.assert_Has_Link("tbotPage")                         // check that there is an new Tbot link to 'tbotPage'
              .links().size().assert_Is(links_Size.inc());         // if the name is unique to the current list, then it should add to the list
            
            ie.link("Reset Razor Templates").click();ie.html().assert_Contains("Current Razor Templates");
            ie.link("TBot"                 ).click();ie.html().assert_Contains("Available TBot Commands");
            ie.link("tbotPage"             ).click();ie.html().assert_Contains("this is an TBot page: 42");

                           
            // Create a Tbot page with the same name of an existing Tbot Page (for example DebugInfo)

            tmFileStorage.path_SiteData()
                         .folder_Create_Folder("Tbot")                                                      // create a TBot folder in the Site_Data folder
                         .folder_Create_File("DebugInfo.cshtml", "this is an TBot page: @(40+2)"); 
            
            ieTeamMentor.page_TBot();                              // Refresh page
              
            //**** Here is the core of the Issue_838
            ie.links().size().assert_Is(links_Size.inc());        // there should still be only one new TBOT page (the one added above

            ie.link("Reset Razor Templates").click();
            ie.link("TBot"                 ).click();
            ie.link("DebugInfo"            ).click();              // opening DebugInfo
            ie.html().assert_Contains("this is an TBot page: 42"); // should show the 'overridden' Tbot page

            ieTeamMentor.close();
        }
     
        [Test] public void Issue_839__Git_Commit_for_users_and_articles_is_broken_in_3_5()
        {
            var pathUserData = tmFileStorage.path_UserData().assert_Folder_Exists();
            pathUserData.isGitRepository().assert_True();
            var nGit = pathUserData.git_Open();
            var size_Before = nGit.commits().assert_Empty().size();

            var ieTeamMentor = this.new_IE_TeamMentor();
            ieTeamMentor.login_Default_Admin_Account();

            nGit.commits().assert_Size_Is(size_Before);

            tmProxy.invoke_Instance(typeof(TM_StartUp),"Application_Start");            // restart TM

            nGit.commits().assert_Size_Is(size_Before + 1);
        }

        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/840
        /// </summary>
        [Test] public void Issue_840_New_articles_should_default_to_Markdown()
        {
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden();
            var ie           = ieTeamMentor.ie;

            tmProxy.editor_Assert();

            var article = tmProxy.library_New_Article_New().assert_Not_Null();
            //tmProxy.cache_Reload__Data();

            article.Content.DataType.assert_Is("Markdown");

            ieTeamMentor.login_Default_Admin_Account("/editor/"+ article.Metadata.Id);

            var expectedMarkdownEditLink = ieTeamMentor.siteUri
                                                       .append("/Markdown/Editor?articleId={0}".format(article.Metadata.Id));
            ie.wait_For_Uri (expectedMarkdownEditLink,3000)
              .assert_Uri_Is(expectedMarkdownEditLink);
            
            ie.wait_For_Element_InnerHtml("Content").assert_Not_Null()
              .element                   ("Content").innerHtml()
                                                    .assert_Is(article.Content.Data_Json); 

            ieTeamMentor.close();
        }
    
        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/864
        /// </summary>
        [Test] public void Issue_864_SMTP_Password_needs_to_be_a_password_field_not_a_textbox()
        {
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden();
            ieTeamMentor.login_Default_Admin_Account("/rest/tbot/run/Edit_SecretData");                          // Login and go into Edit_SecretData page

            ieTeamMentor.ie.waitForField("Server").value().assert_Is("smtp.sendgrid.net");                       // confirm that default values are in there
            
            ieTeamMentor.ie.field("Password (Smtp)").assert_Not_Null().attribute("type").assert_Is("password");  //configm that "Password (Smtp)" field type is "password"
            ieTeamMentor.close();
        }


        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/852
        /// </summary>
        [Test][Ignore("To Fix")] public void  Issue_852_Unable_to_load_Configs()
        {
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden();
            var ie           = ieTeamMentor.ie;
            var temp_Server  = 10.randomLetters();

            ieTeamMentor.login_Default_Admin_Account("/TBot");                                              // login
            ie.waitForLink("Edit SecretData").click();                                                      // go into the "Edit SecretData"
            ie.field      ("Server"         ).value().assert_Not_Empty();                                   // confirm values where set
            ie.field      ("Server"         ).value(temp_Server);                                           // set it to a temp_Server value
            ie.button     ("SaveData"       ).click();                                                      // trigger save
            
            ie.invokeEval("_scope.result_Ok = undefined");

            ie.waitForJsVariable(" _scope.result_Ok")                                                       
              .cast<string>().trim().assert_Is("SecretData data saved");                                    // wait for the confirmation message

           /* tmProxy.TmFileStorage.secretData_Location()
                                 .load<TM_SecretData>()
                                 .SmtpConfig.Server       .assert_Is(temp_Server);   */                     
        }
        /*
        
tmProxy = nUnitTests_Cassini.tmProxy();
//
//
ie.field("Rijndael_IV").value("aa");
ie.field("Site").value("aa");
//ie.refresh();
//return ie.element("Site").attribute("val","a");
//return ie.fields();
//ie.button("SaveData").click();
//changing the Rijndael_IV value
//ie.invokeEval("$('input').eq(0).val('abc')");
//return ie.getJsVariable("$('input').eq(0).val()");
//return ie.element("Site").html();
//return ie.element("SaveData").typeName();

ie.button("SaveData").click();
ie.waitForJsVariable(" _scope.result_Ok").cast<string>().trim();
//SecretData data saved
//2000.sleep();
return tmProxy.TmFileStorage.cast<TM_FileStorage>().secretData_Location().fileContents();
var tmSecretData = tmProxy.TmFileStorage.cast<TM_FileStorage>().secretData_Location().load<TM_SecretData>();
return tmSecretData.Rijndael_IV;
         * */
  
    }
}
