using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_API_TeamMentor_WatiN_ExtensionMethods_CTors
    {
        [Test] public void new_IE_TeamMentor()
        {
            var nUnitTests_Cassini  = new NUnitTests_Cassini_TeamMentor().start();
            var ieTeamMentor       = nUnitTests_Cassini.new_IE_TeamMentor();
            var expected_TBot_Url  = ieTeamMentor.siteUri.append("Html_Pages/Gui/Pages/login.html?LoginReferer=/tbot").str();
            ieTeamMentor.page_TBot();
            ieTeamMentor.ie.url().assert_Is(expected_TBot_Url);
            ieTeamMentor.close();
        }
    }
    [TestFixture]
    public class Test_API_TeamMentor_WatiN_ExtensionMethods : NUnitTests_Cassini_TeamMentor
    {        
        public IE_TeamMentor ieTeamMentor;
        public WatiN_IE      ie;
        [SetUp] public void setUp()
        {                        
            webRoot          .assert_Folder_Exists();
            path_XmlLibraries.assert_Folder_Exists();
            siteUri          .assert_Not_Null();
            ieTeamMentor = new IE_TeamMentor(webRoot, path_XmlLibraries, siteUri, startHidden: true);
            ie           = ieTeamMentor.ie;
        }
        [TearDown] public void tearDown()
        {
            ieTeamMentor.ie.parentForm().close();
        }
        
        [Test] public void page_Admin()
        {
            ieTeamMentor.page_Admin();            
            ie.url().assert_Is(siteUri.append("html_pages/ControlPanel/controlpanel.html").str());
        }
        [Test] public void page_Login()
        {
            ieTeamMentor.page_Login();            
            ie.url().assert_Is(siteUri.append("Html_Pages/Gui/Pages/login.html?LoginReferer=/").str());
        }
        [Test] public void page_TBot()
        {
            ieTeamMentor.page_TBot();            
            ie.url().assert_Is(siteUri.append("Html_Pages/Gui/Pages/login.html?LoginReferer=/tbot").str());
        }     
    }
}
