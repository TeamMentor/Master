using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests
{
    public static class Misc_TeamMenTor_Database_ExtensionMethods
    {
        public static TM_UserData delete_All_Users(this TM_UserData tmUserData)
        {
            foreach(var tmUser in tmUserData.tmUsers())
                tmUser.deleteTmUser();
            Assert.IsEmpty(tmUserData.tmUsers());
            return tmUserData;
        }
        public static TM_FileStorage delete_Database(this TM_FileStorage tmFileStorage)
        {            

            tmFileStorage.assert_Not_Null()
                         .path_XmlDatabase().assert_Not_Null()
                                            .assert_Is_Not_Equal_To(tmFileStorage.WebRoot);
            
            if (tmFileStorage.path_XmlDatabase().dirExists())                                            // check if the folder exists      
            {
                //Assert.IsNotEmpty(tmDatabase.path_XmlDatabase().files());

                tmFileStorage.path_XmlDatabase().files(true).files_Attribute_ReadOnly_Remove();          // make all files writable

                
                tmFileStorage .waitForComplete_Save_GuidanceItemsCache();
                tmFileStorage.clear_GuidanceItemsCache();

                Files.deleteFolder(tmFileStorage.path_XmlDatabase(), true);                              // delete all files recusively
                tmFileStorage.path_XmlDatabase().folder_Wait_For_Deleted();
                tmFileStorage.path_XmlDatabase().assert_Folder_Doesnt_Exist();
                

                Assert.IsFalse(tmFileStorage.path_XmlDatabase().dirExists());                            // ensure the deletion happened
                Assert.IsEmpty(tmFileStorage.path_XmlDatabase().files());
                Assert.IsEmpty(tmFileStorage.tmXmlDatabase().Cached_GuidanceItems);
                Assert.IsFalse(tmFileStorage.getCacheLocation().fileExists());
                "[Test][TM_Xml_Database][delete_Database]TM database files were deleted from: {0}".info(tmFileStorage.path_XmlDatabase());
            }
            return tmFileStorage;
        }        
        /*public static TM_Xml_Database loadData_No_Admin_User(this TM_Xml_Database tmDatabase)
        {
            tmDatabase.Server.Users_Create_Default_Admin = false;
            return tmDatabase.setup();
        }*/
    }
}
