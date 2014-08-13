using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA
{
    [TestFixture]
    public class Test_Cassini_TM_Started_With_Temp_Path_XmlDatabase
    {
                //Workflows
        [Test] public void Start_TM_Using_TM_StartUp_Application_Start()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor().start(makeTcpRequestToPort : false);
            nUnitTests_Cassini.use_Temp_Path_XmlDatabase();           
            
            var tmProxy = nUnitTests_Cassini.tmProxy();
            tmProxy.map_ReferencesToTmObjects().TmFileStorage.assert_Null();               // before call to tmStartUp.Application_Start() should not be set TmFileStorage

            tmProxy.invoke_Instance(typeof(TM_StartUp), "get_Version").assert_Not_Null();  // this will call the TM_StartUp .ctor()
            var tmStartUp = tmProxy.get_Current<TM_StartUp>()         .assert_Not_Null();  // get byRef object of TM_StartUp

            tmStartUp.Application_Start();                                                 // after this completes we should have a full TM loaded

            tmProxy.map_ReferencesToTmObjects();                                           // which means that after remapping the TM object references
            tmProxy.TmConfig        .assert_Not_Null();                                    // we have native access to the main TM objects :)
            tmProxy.TmFileStorage   .assert_Not_Null();            
            tmProxy.TmServer        .assert_Not_Null();
            tmProxy.TmStatus        .assert_Not_Null();
            tmProxy.TmUserData      .assert_Not_Null();
            tmProxy.TmXmlDatabase   .assert_Not_Null();
            tmProxy.TmConfig.assert_Not_Null();
            nUnitTests_Cassini.delete_Temp_Path_XmlDatabase();
        }

        [Test] public void Call_Pages_From_TM_Started_With_Temp_Path_XmlDatabase()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor();

            nUnitTests_Cassini.start                            (makeTcpRequestToPort : false)
                              .use_Temp_Path_XmlDatabase        ();
            
            var homePageUrl = nUnitTests_Cassini.apiCassini.url();
            
            //try via direct Http requests
            homePageUrl.append("")    .GET().assert_Not_Contains("Login");      
            homePageUrl.append("TBot").GET().assert_Contains    ("Login");

            //try via browser requests
            var ie = "Call_Pages_From_TM_Started_With_Temp_Path_XmlDatabase".add_IE_Hidden_PopupWindow();
            ie.open(homePageUrl)
              .assert_Has_Links("About", "Login", "Help");
            
            ie.open(homePageUrl               ).waitForLink("About");ie.assert_Url_Is(homePageUrl.append("teamMentor"));
            ie.open(homePageUrl.append("Tbot")).assert_Url_Is(homePageUrl.append("Html_Pages/Gui/Pages/login.html?LoginReferer=/tbot"));
            
            ie.parentForm().Close();  
            
            nUnitTests_Cassini.stop_And_Delete_Temp_XmlDatabase();
        }
        

    }
}
