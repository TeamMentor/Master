using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.TeamMentor.FileStorage
{
    [TestFixture]
    public class Test_TM_FileStorage_Utils
    {
        [Test] public void set_WebRoot()
        {
            TM_FileStorage.Custom_WebRoot = null;
            var custom_WebRoot            = "Custom_WebRoot".tempDir().assert_Folder_Exists();
            var expected_Path_XmlDatabase = custom_WebRoot.pathCombine(@"App_Data\TeamMentor");

            UserRole.Admin.assert();

            TM_FileStorage.Custom_WebRoot.assert_Null();
            var tmFileStorage = new TM_FileStorage(loadData: false);
            tmFileStorage.webRoot().assert_Null();
            tmFileStorage.set_WebRoot();
            tmFileStorage.webRoot().assert_Not_Null()
                                   .assert_Equal(AppDomain.CurrentDomain.BaseDirectory);           

            TM_FileStorage.Custom_WebRoot = custom_WebRoot;
            tmFileStorage.webRoot().assert_Equal(AppDomain.CurrentDomain.BaseDirectory)     // should still point to the AppDomain base directory
                                   .assert_Not_Equal(custom_WebRoot);
            
            tmFileStorage.using_Custom_WebRoot().assert_False();                            // this should only be true when the values match

            tmFileStorage.set_WebRoot()                                                     // set WebRoot 
                         .webRoot().assert_Equals(custom_WebRoot);                          // and confirm its location

            tmFileStorage.using_Custom_WebRoot().assert_True();                             // now it should be true
            tmFileStorage.path_XmlDatabase().assert_Null();                                 // confirm that not set
            tmFileStorage.set_Path_XmlDatabase();                                           // this should set the TM_Xml_Database inside the Web_Root            
            tmFileStorage.path_XmlDatabase().contains(custom_WebRoot);  
            tmFileStorage.path_XmlDatabase().assert_Is_Equal_To(expected_Path_XmlDatabase); // check that the current Path_XmlDatabase matches the expected location
            
            
            //reset values
            TM_FileStorage.Custom_WebRoot = null;

            tmFileStorage.set_WebRoot()
                         .webRoot().assert_Not_Equals(custom_WebRoot);

            Files.delete_Folder_Recursively(custom_WebRoot).assert_True();
            custom_WebRoot.assert_Folder_Doesnt_Exist();
        }
    }
}
