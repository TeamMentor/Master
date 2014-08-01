using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN;
using FluentSharp.WatiN.NUnit;
using NUnit.Framework;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_IE
{
    [TestFixture]
    public class Test_QA__Editor_Markdown  : NUnitTests_Cassini_TeamMentor
    {
        [SetUp] public void setup()
        {            
            this.tmProxy_Refresh();
            tmProxy.assert_Not_Null();
            tmProxy.TmFileStorage.assert_Not_Null();
            tmProxy.TmXmlDatabase.assert_Not_Null();
        }
        [Test] public void View_Markdown_Article__Edit__Save()
        {
            var article          = tmProxy.editor_Assert()                       // assert the editor user (or the calls below will fail due to security demands)
            	                          .library_New_Article_New()             // create new article
                                          .assert_Not_Null(); 

            var ieTeamMentor     = this.new_IE_TeamMentor_Hidden();
            var ie               = ieTeamMentor.ie; 
            ieTeamMentor.login_Default_Admin_Account("/article/{0}".format(article.Metadata.Id));               // Login as admin and redirect to article page
            
            var original_Content = ie.element("guidanceItem").innerText().assert_Not_Null();                    // get reference to current content
            ie.assert_Has_Link("Markdown Editor")       
              .link           ("Markdown Editor").click();                                                      // open markdown editor page          
                        
            ie.wait_For_Element_InnerHtml("Content").assert_Not_Null()
              .element                   ("Content").innerHtml()
                                                    .assert_Is(original_Content);                               // confirm content matches what was on the view page

            var new_Content      = "This is the new content of this article".add_5_RandomLetters();             // new 'test content'

            ie.element("Content").to_Field().value(new_Content);                                                // put new content in markdown editor
            ie.button("Save").click();                                                                          // save 
            ie.wait_For_Element_InnerHtml("guidanceItem").assert_Not_Null()        
              .element                   ("guidanceItem").innerHtml()
                                                         .assert_Is("<P>{0}</P>".format(new_Content));         // confirm that 'test content' was saved ok (and was markdown transformed)            
       
            ieTeamMentor.close();
        }
    }
}
