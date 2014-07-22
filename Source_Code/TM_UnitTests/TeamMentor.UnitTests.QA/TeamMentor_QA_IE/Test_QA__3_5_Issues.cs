using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using NUnit.Framework;
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
        [Test] public void Issue__829_Get_rid_of_Control_Panel()
        {
            var ie = this.new_IE_TeamMentor_Hidden(true)
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
    }
}
