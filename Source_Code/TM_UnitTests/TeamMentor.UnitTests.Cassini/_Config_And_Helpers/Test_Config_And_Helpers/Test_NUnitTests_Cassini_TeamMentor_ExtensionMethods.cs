using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Cassini
{
    public class Test_NUnitTests_Cassini_TeamMentor_ExtensionMethods
    {
        [Test] public void teamMentor_Root_OnDisk()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor();
            var teamMentor_Root_OnDisk = nUnitTests_Cassini.teamMentor_Root_OnDisk();
            teamMentor_Root_OnDisk.assert_Folder_Exists()
                .assert_Folder_Has_Files("web.config", "default.htm" ,
                    "javascript/TM/settings.js" ,
                    "javascript/gAnalytics/ga.js");
        }
        [Test] public void teamMentor_Objects_Proxy()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor();
            var apiCassini         = new API_Cassini();
            nUnitTests_Cassini.apiCassini = apiCassini;
            
            nUnitTests_Cassini.teamMentor_Objects_Proxy().assert_Not_Null()
                              .apiCassini.assert_Is(apiCassini);

        }
        [Test] public void configure_Temp_Path_XmlDatabase()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor().start(makeTcpRequestToPort : false);
            
            var tmProxy = nUnitTests_Cassini.tmProxy();
            tmProxy.set_Custom_Path_XmlDatabase("");            
            tmProxy.get_Custom_Path_XmlDatabase().assert_Empty();                            // before a call to configure_Temp_Path_XmlDatabase this should not be set

            nUnitTests_Cassini.use_Temp_Path_XmlDatabase();           

            tmProxy.get_Custom_Path_XmlDatabase().assert_Not_Empty();                        // now it should be temp_Path_XmlDatabase

            var temp_Path_XmlDatabase = tmProxy.get_Custom_Path_XmlDatabase();

            tmProxy.map_ReferencesToTmObjects()                                             // before the first request
                   .TmFileStorage.assert_Null();                                            // TmFileStorage should be null
            
            
            
            nUnitTests_Cassini.apiCassini.url().assert_Not_Null()                           // this is the first GET request which will 
                                               .GET("/").assert_Contains("TeamMentor");     // trigger the load of TM
            
            tmProxy.map_ReferencesToTmObjects()                                             // after the first request
                   .TmFileStorage.assert_Not_Null()                                         // TmFileStorage should be set
                   .Path_XmlDatabase.assert_Equals(temp_Path_XmlDatabase);
            
            nUnitTests_Cassini.stop();
            temp_Path_XmlDatabase.files().files_Attribute_ReadOnly_Remove();
            Files.delete_Folder_Recursively(temp_Path_XmlDatabase);
            if(temp_Path_XmlDatabase.folder_Exists())
                "temp_Path_XmlDatabase was not not deleted ok".assert_Ignore();
                        
            temp_Path_XmlDatabase.assert_Folder_Not_Exists();
        }
        [Test] public void call_TM_StartUp_Application_Start()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor();

            nUnitTests_Cassini.start                            (makeTcpRequestToPort : false)
                              .use_Temp_Path_XmlDatabase        ()
                              .call_TM_StartUp_Application_Start();                 
            nUnitTests_Cassini.tmProxy      ().get_Current<TM_FileStorage>().assert_Not_Null();
            nUnitTests_Cassini.tmFileStorage()                              .assert_Not_Null()
                              .Path_XmlDatabase.assert_Equals(nUnitTests_Cassini.tmProxy().get_Custom_Path_XmlDatabase());

            nUnitTests_Cassini.delete_Temp_Path_XmlDatabase();
        }

    }
}