using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.Web35;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_IE
{
    [TestFixture]
    public class Test_QA__TM_Config_Security_Settings
    {
        NUnitTests_Cassini_TeamMentor nUnitTests_Cassini;
        TM_Proxy      tmProxy;
        IE_TeamMentor                 ieTeamMentor;
        WatiN_IE                      ie;
        TMConfig                      tmConfig;
        
        [TestFixtureSetUp] public void testFixtureSetUp()
        {
            nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor().start();
            tmProxy            = nUnitTests_Cassini.tmProxy();
            ieTeamMentor       = nUnitTests_Cassini.new_IE_TeamMentor(true);
            ie                 = ieTeamMentor.ie;

            nUnitTests_Cassini.siteUri.GET().assert_Contains("TeamMentor");     // make a request to trigger Asp.NET pipeline and TM Startup
            tmProxy.map_ReferencesToTmObjects();                                // these should be alive after startup
            tmConfig = tmProxy.TmConfig     .assert_Not_Null();                 // specially the TMConfig object
        }
        [TestFixtureTearDown] public void testFixtureTearDown()
        {
            nUnitTests_Cassini.stop();
            ie.parentForm().close();
        }
        [Test] public void TM_Config_TM_Security__Show_Content_Rules()
        {            
            var texts_WithLoginForm = new [] {"Login required","If you have a valid account","if not, please sign up for an account to gain access"};
            var texts_LibraryView   = new [] {"Guidance Libraries", "Technology", "Phase", "Type"}; 
            
            //Mode 1: Anonymous users cannot see library and content (just a login form)
            tmConfig.TMSecurity.Show_ContentToAnonymousUsers = false;
            tmConfig.TMSecurity.Show_LibraryToAnonymousUsers = false;
            ieTeamMentor.page_Home(); 
            
            ie.html().assert_Contains    (texts_WithLoginForm);
            ie.html().assert_Not_Contains(texts_LibraryView);

            //Mode 2: Anonymous users can see library but no content (still asked to login using Form)
            tmConfig.TMSecurity.Show_ContentToAnonymousUsers = false;
            tmConfig.TMSecurity.Show_LibraryToAnonymousUsers = true;
            ieTeamMentor.page_Home(); 
            ie.waitForField("ctl00_ContentPlaceHolder1_UsernameBox");       // wait for the right hand ajax page to load

            ie.html().assert_Contains    (texts_WithLoginForm);
            ie.html().assert_Contains    (texts_LibraryView);

            //Mode 3: Anonymous users can see library and content
            tmConfig.TMSecurity.Show_ContentToAnonymousUsers = true;
            tmConfig.TMSecurity.Show_LibraryToAnonymousUsers = true;
            ieTeamMentor.page_Home(); 
            
            ie.waitForField("ctl00_ContentPlaceHolder1_UsernameBox",150,10);      // will fail to find it

            ie.html().assert_Not_Contains    (texts_WithLoginForm);
            ie.html().assert_Contains        (texts_LibraryView);
        }
    }
}
