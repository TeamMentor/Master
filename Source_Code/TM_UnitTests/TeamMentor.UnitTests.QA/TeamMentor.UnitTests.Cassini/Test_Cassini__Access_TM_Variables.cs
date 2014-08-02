using System;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA
{
    [TestFixture]
    public class Test_Cassini__Access_TM_Variables : NUnitTests_Cassini_TeamMentor
    {
        O2Proxy o2Proxy;

        [SetUp]
        public void SetUp()
        {
            apiCassini.url().GET().assert_Contains("TeamMentor");                           // make one request so that TM Startup is invoked

            o2Proxy = apiCassini.appDomain().o2Proxy().assert_Not_Null();                   // get a reference to the O2Proxy object
        }
        
        [Test] public void Confirm_That_We_Can_Access_Variables_Across_AppDomains_Using_Reflection()
        {
            Func<object>   get_TMConfig = ()     => o2Proxy.staticInvocation("TeamMentor.Schemas", 
                                                                             "TeamMentor.CoreLib.TMConfig",
                                                                             "get_Current", null);
                                                            
            var tmConfig = get_TMConfig().assert_Not_Null();                                  // this is an MarshalByRefObject

            var originalValue = "TM_Reader";
            var newValue      = "TM_Reader".add_5_RandomLetters();
            

            tmConfig.prop("WindowsAuthentication").prop<string>("ReaderGroup")                // get current value
                                                 .assert_Is_Equal_To(originalValue);
            
            tmConfig.prop("WindowsAuthentication").prop("ReaderGroup", newValue);             // change it it, with no need to call 
                                                                                              //   TMConfig.set_Current(tmConfig)

            var tmConfig_AfterChange = get_TMConfig().assert_Not_Null();                      // get a copy of the modified TMconfig 
            
            tmConfig_AfterChange.prop("WindowsAuthentication").prop<string>("ReaderGroup")
                                                              .assert_Is_Equal_To(newValue);  // confirm change      

            tmConfig.prop("WindowsAuthentication").prop("ReaderGroup", originalValue);        // restore originalValue            
        }

        [Test] public void Confirm_That_We_Can_Access_TM_StartUp_As_MarshalByRefObject()
        {            
            var tmStartUp = (TM_StartUp)o2Proxy.staticInvocation("TeamMentor.AspNet", 
                                                                 "TeamMentor.CoreLib.TM_StartUp",
                                                                 "get_Current", new object[] {});
            //check that we got a valid MarshalByRef object
            tmStartUp.assert_Not_Null();
            
            //check that we can get the Version property (which is of type: System.String)
            tmStartUp.Version.assert_Not_Null()
                             .assert_Is(typeof(TM_StartUp).assembly().version());

            //check that we can get the TMEngine property (which is of type: TeamMentor.CoreLib.TM_Engine )
            tmStartUp.TMEngine.assert_Not_Null();

            //check that we can get the TrackingApplication property (which is of type: TeamMentor.CoreLib.Tracking_Application )
            tmStartUp.TrackingApplication.assert_Not_Null();
            
            //check that we can get the TmFileStorage property (which is of type: TeamMentor.FileStorage.TM_FileStorage )
            tmStartUp.TmFileStorage.assert_Not_Null();
            
            //check that (from tmStartUp.TmFileStorage) we can access the main TM (in memory) Database Objects

            tmStartUp.TmFileStorage.TMXmlDatabase       .assert_Not_Null();
            tmStartUp.TmFileStorage.UserData            .assert_Not_Null();
            tmStartUp.TmFileStorage.Server              .assert_Not_Null();

            tmStartUp.TmFileStorage.WebRoot             .assert_Folder_Exists();            
            tmStartUp.TmFileStorage.Path_UserData       .assert_Folder_Exists();
            tmStartUp.TmFileStorage.Path_SiteData       .assert_Folder_Exists();
            tmStartUp.TmFileStorage.Path_XmlDatabase    .assert_Folder_Exists();
            tmStartUp.TmFileStorage.Path_XmlLibraries   .assert_Folder_Exists();

            tmStartUp.TmFileStorage.GuidanceExplorers_Paths   .assert_Not_Null();//.assert_Not_Empty();
            tmStartUp.TmFileStorage.GuidanceItems_FileMappings.assert_Not_Null();//.assert_Not_Empty();
        }
        [Test] public void Create_User_Via_MarshalByRefObject_Proxies()
        {
            var tmFileStorage = (TM_FileStorage)o2Proxy.staticInvocation("TeamMentor.FileStorage", 
                                                                         "TeamMentor.FileStorage.TM_FileStorage",
                                                                         "get_Current", null);
            var tmConfig      = (TMConfig      )o2Proxy.staticInvocation("TeamMentor.Schemas", 
                                                                         "TeamMentor.CoreLib.TMConfig",
                                                                         "get_Current", null);
            tmFileStorage.assert_Not_Null();
            tmConfig     .assert_Not_Null();
                                    
            var sizeBefore  = tmFileStorage.UserData.TMUsers.size();

            var tmUser      = (TMUser)o2Proxy.staticInvocation("TeamMentor.Users", 
                                                              "TeamMentor.UserData.Users_Creation",
                                                              "createUser", new object[] {tmFileStorage.UserData});
                        
            tmUser.assert_Not_Null();                               // confirm that we received a valid TMUser object
            tmUser.UserName.assert_Valid();                         //    and that the UserName value is set
            tmUser.UserID.assert_Bigger_Than(1);                    //    and that the UserID is a valid number

            var sizeAfter  = tmFileStorage.UserData.TMUsers.size(); // get an updated value of TMUsers.size
            sizeAfter.assert_Is(sizeBefore + 1);                              
        }             
    }
}
