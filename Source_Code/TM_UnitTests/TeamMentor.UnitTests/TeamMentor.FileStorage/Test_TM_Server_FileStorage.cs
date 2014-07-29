using NUnit.Framework;
using TeamMentor.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.TeamMentor.FileStorage
{
    [TestFixture]
    public class Test_TM_Server_FileStorage : TM_XmlDatabase_FileStorage
    {
        [Test] public void this_TmServer()          
        {
            UserGroup.Admin.assert();

            Assert.NotNull (tmServer);
            Assert.AreEqual(tmServer, tmFileStorage.tmServer());                          
        }  

        [Test]
        public void LoadAndSave_TMServer_To_Disk()           
        {
            UserGroup.Admin.assert();

            var saved_Path_XmlDatabase = tmFileStorage.path_XmlDatabase();              // save these so we can confirm that the restore worked as expected (at the end of the test)
            var saved_TmServerPath     = tmFileStorage.tmServer_Location()            
                                                      .assert_File_Exists();            // confirm that it exists (before the Path_XmlLibraries path change)              
            var saved_TmServer_ToXml   = tmFileStorage.tmServer().toXml();

            var temp_Path_XmlLibraries = "temp_Path_XmlLibraries".tempDir();
           
            tmFileStorage.set_Path_XmlDatabase(temp_Path_XmlLibraries)                  // set the set_Path_XmlDatabase to temp_Path_XmlLibraries
                         .path_XmlDatabase().assert_Equal_To(temp_Path_XmlLibraries);   // and confirm that it was set

            var tmServerPath  = tmFileStorage.tmServer_Location()                       // get the current path to the TM_Server object                         
                                             .assert_File_Not_Exists()                  // confirm it doesn't exist and that its parent folder is temp_Path_XmlLibraries
                                             .assert_File_Parent_Folder_Is(temp_Path_XmlLibraries);

            //create  temp TM_Server, modify it and saved it
            tmFileStorage.tmServer(null).tmServer().assert_Null();                      // remove the loaded TM_Server object (from TM_FileStorage)
            tmFileStorage.tmServer_Load();                                              // trigger the load of TM_Server, which in this case will create the file on tmServerPath
            tmFileStorage.tmServer().notNull();                                         // confirm that the TM_Server object is now set (inside TM_FileStorage)
            tmFileStorage.tmServer_Location().assert_File_Exists()                      // and that it exist on disk
                                             .assert_Equal_To(tmServerPath)
                                             .assert_Not_Equal_To(saved_TmServerPath);
            
            var temp_TmServer = tmFileStorage.tmServer();
            var test_Config   = new TM_Server.Config
                                    {
                                        Name           = "Test_Config",
//                                        Local_GitPath  = 10.randomLetters(),
                                        Remote_GitPath = 10.randomLetters()
                                    };      
      
            temp_TmServer.UserData_Configs.assert_Size_Is(1);                           // check that default UserData mapping is there
            temp_TmServer.userData_Config("User_Data").assert_Not_Null();

            temp_TmServer.add_UserData(test_Config);
            temp_TmServer.UserData_Configs.assert_Size_Is(2); 

            tmFileStorage.tmServer_Location().load<TM_Server>().UserData_Configs.assert_Size_Is(1);     // before save there should only be one there
            tmFileStorage.tmServer_Save();
            tmFileStorage.tmServer_Location().load<TM_Server>().UserData_Configs.assert_Size_Is(2);      // after save we should have two
            tmFileStorage.tmServer_Location().load<TM_Server>().userData_Config("Test_Config")           // load this value from disk and comparate it with the version in memory
                                                               .toXml().assert_Is_Equal_To(test_Config.toXml());

            // test load of saved file
            tmFileStorage.tmServer(null).tmServer().assert_Null(); 
            tmFileStorage.tmServer_Load()                                               // trigger reload
                         .tmServer().userData_Config("Test_Config")                     // load this value from loaded version and comparate it with the original one
                         .toXml().assert_Is_Equal_To(test_Config.toXml());
          
            tmFileStorage.set_Path_XmlDatabase()                                        // restore current mapped value of Path_XmlDatabase
                         .path_XmlDatabase()                                            // get it
                         .assert_Equal_To(saved_Path_XmlDatabase);                      // confirms it is expected result
            
            tmFileStorage.tmServer_Location()
                         .assert_Is_Equal_To(saved_TmServerPath);

            tmFileStorage.tmServer_Load()
                         .tmServer()
                         .toXml().assert_Is_Equal_To(saved_TmServer_ToXml);

            tmServer = tmFileStorage.tmServer();                                        // also restore this value (so that other tests in this TestFixture are not affected)

            temp_Path_XmlLibraries.assert_Folder_Deleted();
        }
    }
}
