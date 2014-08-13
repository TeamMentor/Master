using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_Http
{
    [TestFixture]
    public class Test_QA__View_Article_Content : NUnitTests_Cassini_TeamMentor
    {
        [SetUp] public void setup()
        {
            //tmProxy.assert_Null();
            this.tmProxy_Refresh();
            tmProxy.assert_Not_Null();
            tmProxy.TmFileStorage.assert_Not_Null();
            tmProxy.TmXmlDatabase.assert_Not_Null();

        }
    
        [Test] public void Render_Article_Html()
        {
            var article = tmProxy.admin_Assert()                        // assert the admin user (or the calls below will fail due to security demands)
            	                    .library_New_Article_New()             // create a new Library with new Article inside it (this returns get a (byRef) object reference to the article created)
                                    .assert_Not_Null();                    // confirm that we did receive an article
            
            var metadata = article.Metadata .assert_Not_Null();         // get (byRef) object reference to the article's Metadata
            var content  = article.Content  .assert_Not_Null();         // get (byRef) object reference to the article's Content


            var ieTeamMentor        = this.new_IE_TeamMentor_Hidden();  // get an IE window mapped with the IE_TeamMentor API
            var adminUser           = "admin".tmUser();                 // get (byRef) object of the current admin user     
            var adminUser_AuthToken = tmProxy.user_AuthToken_Valid(adminUser);  // get a valid auth token (with can be used to login
            ieTeamMentor.login_Using_AuthToken(adminUser_AuthToken);            // login (in IE) as admin using the auth token

            ieTeamMentor.article_Raw(article)                           // open the article_Raw view (ie /raw/{artice Guid} )
                        .html()                                         // get the Html of the current page
                        .assert_Contains    (metadata.Title   )         // confirm that the article's Title is shown on the page
                        .assert_Contains    (metadata.Type    )         // and the type
                        .assert_Contains    (metadata.Id.str())         // and the Id
                        .assert_Contains    (content.Data_Json);        // and the actually article's content text (or html)

            ieTeamMentor.close();                                       // get a new clean IE instance (to solve a cache issue with getting the 
            ieTeamMentor = this.new_IE_TeamMentor_Hidden();             // html from the 'html' version
            
            ieTeamMentor.article_Html(article)                          // open the article_Html view (ie /html/{artice Guid} )
                        .html()                                         // get the Html of the current page
                        .assert_Contains    (metadata.Title  )          // confirm that the article's title is shown on the page
                        .assert_Not_Contains(metadata.Type   )          // the title is NOT there (since we don't shown it on the html view)
                        .assert_Not_Contains(metadata.Id.str())         // the id is NOT there
                        .assert_Contains    (content.Data_Json);        // but the actually article's content text (or html) is
        }
        [Test] public void Direct_Links_To_Article()
        {
            var ieTM = this.new_IE_TeamMentor_Hidden();
            tmProxy.editor_Assert();
            var article = tmProxy.article_New();

            var html_Guid_Article = ieTM.open("/article/" + article.Metadata.Id);
            var html_Guid_A       = ieTM.open("/a/" + article.Metadata.Id);

            html_Guid_Article.assert_Is(html_Guid_A);

            var directLlink             = "link_".add_5_RandomLetters();;
            article.Metadata.DirectLink = directLlink;

            var html_DirectLink_Article = ieTM.open("/article/" + directLlink);
            var html_DirectLink_A       = ieTM.open("/a/" + directLlink);

            html_Guid_Article.assert_Is(html_DirectLink_Article)
                             .assert_Is(html_DirectLink_A      );
            ieTM.close();
        }
    }
}
