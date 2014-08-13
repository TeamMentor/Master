using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Cassini._API_TeamMentor.IE_TeamMentor.Tests
{
    [TestFixture]
    public class Test_IE_TeamMentor_ExtensionMethods_Login :  NUnitTests_Cassini_TeamMentor
    {
        public Cassini.IE_TeamMentor ieTeamMentor;
        public WatiN_IE      ie;
        [SetUp] public void setUp()
        {                        
            webRoot          .assert_Folder_Exists();
            path_XmlLibraries.assert_Folder_Exists();
            siteUri          .assert_Not_Null();

            ieTeamMentor = new Cassini.IE_TeamMentor(webRoot, path_XmlLibraries, siteUri, startHidden: true);
            ie           = ieTeamMentor.ie;
        }
        [Test] public void login_Default_Admin_Account()
        {
            //confirm that login works ok and we are redirected into /whoami page            
            ieTeamMentor.login_Default_Admin_Account();                        
                      
            ieTeamMentor.ie                        .assert_Uri_Is  (siteUri.append("/whoami"))
                        .body().innerHtml()        .assert_Contains("\"UserName\":\"admin\"")
                        .json_Deserialize<WhoAmI>().assert_Not_Null()
                        .UserName                  .assert_Is("admin");
 ;      }
        [Test] public void whoAmI()
        {
            ieTeamMentor.whoAmI().assert_Not_Null();
            ieTeamMentor.login_Default_Admin_Account()
                        .whoAmI().assert_Not_Null()
                        .UserName.assert_Is("admin");

            ieTeamMentor.am_I_Default_Admin().assert_True();
                                 
        }
    }
}
