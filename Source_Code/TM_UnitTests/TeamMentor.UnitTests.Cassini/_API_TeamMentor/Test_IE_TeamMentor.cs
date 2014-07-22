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
            ieTeamMentor.page_TBot();            
            ie.assert_Uri_Is(siteUri.append("rest/tbot/run/Commands"));
        }     
        [Test] public void page_Home()
        {
            ieTeamMentor.page_Home();            
            ie.assert_Uri_Is(siteUri.append("TeamMentor"));
        }  
        [Test] public void login_Default_Admin_Account()
        {
            //confirm that login works ok and we are redirected into /whoami page            
            ieTeamMentor.login_Default_Admin_Account();                        
                      
            ieTeamMentor.ie                        .assert_Uri_Is  (siteUri.append("/whoami"))
                        .body().innerHtml()        .assert_Contains("\"UserName\":\"admin\"")
                        .json_Deserialize<WhoAmI>().assert_Not_Null()
                        .UserName                  .assert_Is("admin");
 ;       }
    }
}
