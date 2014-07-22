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

        [Test] public void Issue_838__SiteData_custom_TBot_pages_can_conflict_with_the_main_TBot_pages()
        {   
            //Open main Tbot Page and capture number of links          
            var ieTeamMentor = this.new_IE_TeamMentor_Hidden(true);
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
