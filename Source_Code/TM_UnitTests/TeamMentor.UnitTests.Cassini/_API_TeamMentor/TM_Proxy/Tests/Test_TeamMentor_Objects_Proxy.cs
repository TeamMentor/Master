using System;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;


namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_TeamMentor_Objects_Proxy
    {
        public NUnitTests_Cassini_TeamMentor nunitTests_Cassini;
        public TM_Proxy tmProxy;

        [TestFixtureSetUp]
        public void testFixtureSetUp()
        {
            nunitTests_Cassini = new NUnitTests_Cassini_TeamMentor();
            nunitTests_Cassini.start();
            tmProxy           = nunitTests_Cassini.teamMentor_Objects_Proxy();
        }
        [TestFixtureTearDown]
        public void testFixtureTearDown()
        {
            nunitTests_Cassini.stop();
        }

        [Test] public void TeamMentor_Objects_Proxy_Ctor()
        {
            tmProxy           .assert_Not_Null();
            tmProxy.apiCassini.assert_Not_Null();
            tmProxy.o2Proxy   .assert_Not_Null();

            //Tm Objects should be null here
            tmProxy.TmFileStorage.assert_Null();            
        }
        [Test] public void map_ReferencesToTmObjects()
        {
            tmProxy.map_ReferencesToTmObjects();

            tmProxy.TmStatus     .assert_Not_Null();            // until TM is started, this is the only one that should be set
            tmProxy.TmConfig     .assert_Null();
            tmProxy.TmFileStorage.assert_Null();            
            tmProxy.TmServer     .assert_Null();
            tmProxy.TmUserData   .assert_Null();
            tmProxy.TmXmlDatabase.assert_Null();
        }

        [Test] public void set_Custom_WebRoot()
        {
            var folder = "webRoot".tempDir().assert_Folder_Exists();
                        
            tmProxy.set_Custom_WebRoot(folder)
                   .get_Custom_WebRoot(      ).assert_Is(folder);
            
            tmProxy.set_Custom_WebRoot(""    )
                   .get_Custom_WebRoot(      ).assert_Is("");

            folder.assert_Folder_Deleted();
        }

        [Test] public void set_Custom_Path_XmlDatabase()
        {
            var folder = "path_XmlDatabase".tempDir().assert_Folder_Exists();
                               
            tmProxy.set_Custom_Path_XmlDatabase(folder)
                   .get_Custom_Path_XmlDatabase(      ).assert_Is(folder);
            
            folder.assert_Folder_Deleted();
        }
    }
    
    [TestFixture]
    public class Test_TeamMentor_Objects_Proxy_ExtensionMethods
    {
        public NUnitTests_Cassini_TeamMentor nunitTests_Cassini;
        public TM_Proxy tmProxy;

        [TestFixtureSetUp   ] public void testFixtureSetUp()          
        {
            nunitTests_Cassini = new NUnitTests_Cassini_TeamMentor();
            nunitTests_Cassini.start();
            tmProxy        = nunitTests_Cassini.teamMentor_Objects_Proxy();
        }
        [TestFixtureTearDown] public void testFixtureTearDown()       
        {
            nunitTests_Cassini.stop();
        }

        [Test] public void get_Current    ()    
        {
            tmProxy.get_Current<TM_Status>().assert_Not_Null();
        }
        [Test] public void get_Property   ()    
        {
            tmProxy.get_Property_Static<TM_Status>(                  "Current").assert_Not_Null();
            tmProxy.get_Property_Static<TM_Status>(typeof(TM_Status),"Current").assert_Not_Null();
            tmProxy.get_Property_Static<string   >(typeof(TM_Status),"Version").assert_Not_Null();
            tmProxy.get_Property_Static<TM_Status,TM_Status>       ( "Current").assert_Not_Null();
            tmProxy.get_Property_Static<TM_Status,string>          ( "Version").assert_Not_Null();
        }
        [Test] public void set_Property   ()    
        {
           tmProxy.get_Property_Static<TM_Status,string>( "Version").assert_Not_Null()
                                                             .assert_Equal(typeof(TM_Status).assembly().version());;
           
           var version = "version".add_5_RandomLetters();

           tmProxy.set_Property_Static<TM_Status       >("Version", version)
                  .get_Property_Static<TM_Status,string>("Version").assert_Is_Equal_To (version)
                                                            .assert_Not_Equal_To(typeof(TM_Status).assembly().version());;
        }
        [Test] public void invoke_Static  ()    
        {
            tmProxy.invoke_Static<string>   (typeof(TM_Status),"get_Version").assert_Not_Null()
                                                                             .assert_Equal(typeof(TM_Status).assembly().version());
            tmProxy.invoke_Static<TM_Status>(typeof(TM_Status),"get_Current").assert_Not_Null();                                                                            

            tmProxy.invoke_Static<int      >(typeof(TM_Status),"get_Version").assert_Default();  // bad cast
            tmProxy.invoke_Static<TM_Status>(typeof(TM_Status),"get_Version").assert_Null   ();  // bad cast
            tmProxy.invoke_Static<string>   (typeof(TM_Status),"get_Current").assert_Null   ();  // bad cast
            tmProxy.invoke_Static<string   >(typeof(TM_Status),"get_AAAAAAA").assert_Null   ();  // bad method
        }
        [Test] public void invoke_Instance()
        {
            tmProxy.map_ReferencesToTmObjects();

            tmProxy.get_Current<TM_StartUp>().assert_Null();

            tmProxy.invoke_Instance(typeof(TM_StartUp), "get_Version",new object[] {})      // this will invoke store the TM_StartUp value in the TM_StartUp.Current static propery
                   .assert_Not_Null()
                   .assert_Is_Equal_To(typeof(TM_StartUp).assembly().version());

            tmProxy.get_Current<TM_StartUp>().assert_Not_Null();
            
            tmProxy.invoke_Instance<string>(typeof(TM_StartUp),"get_Version")               // another way to invoke_Instance
                   .assert_Not_Null()
                   .assert_Is_Equal_To(typeof(TM_StartUp).assembly().version());
        }

    }
}
