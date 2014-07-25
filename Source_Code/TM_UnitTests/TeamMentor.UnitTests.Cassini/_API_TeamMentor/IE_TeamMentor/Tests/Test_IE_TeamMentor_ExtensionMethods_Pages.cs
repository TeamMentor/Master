using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_IE_TeamMentor_ExtensionMethods_Pages : NUnitTests_Cassini_TeamMentor
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
            ieTeamMentor.ie.close();
            ieTeamMentor.close();
        }
        
        [Test] public void page_Admin()
        {
            ieTeamMentor.page_Admin();            
            ie.assert_Uri_Is(siteUri.append("html_pages/ControlPanel/controlpanel.html"));
        }
        [Test] public void page_Login()
        {
            ieTeamMentor.page_Login();            
            ie.assert_Uri_Is(siteUri.append("Html_Pages/Gui/Pages/login.html?LoginReferer=/"));
        }
        [Test] public void page_TBot()
        {
            if (ieTeamMentor.am_I_Default_Admin().isFalse())
                ieTeamMentor.login_Default_Admin_Account();
            ieTeamMentor.page_TBot();            
            ie.assert_Uri_Is(siteUri.append("rest/tbot/run/Commands"));
        }     
        [Test] public void page_Home()
        {
            ieTeamMentor.page_Home();            
            ie.assert_Uri_Is(siteUri.append("TeamMentor"));
        }  
        
    }
}
