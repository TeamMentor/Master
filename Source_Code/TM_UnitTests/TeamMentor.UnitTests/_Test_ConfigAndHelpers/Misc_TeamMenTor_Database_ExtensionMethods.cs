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
        public static TM_Xml_Database delete_Database(this TM_Xml_Database tmDatabase)
        {
            var tmFileStorage = TM_FileStorage.Current;

            Assert.NotNull    (tmDatabase);
            Assert.NotNull    (tmDatabase.path_XmlDatabase());
            Assert.AreNotEqual(tmDatabase.path_XmlDatabase(), tmFileStorage.WebRoot);
            //Assert.IsTrue     (tmDatabase.path_XmlDatabase().dirExists());

            if (tmDatabase.path_XmlDatabase().dirExists())                                            // check if the folder exists      
            {
                //Assert.IsNotEmpty(tmDatabase.path_XmlDatabase().files());

                tmDatabase.path_XmlDatabase().files(true).files_Attribute_ReadOnly_Remove();          // make all files writable

                Assert.IsTrue(Files.deleteFolder(tmDatabase.path_XmlDatabase(), true));                              // delete all files recusively
                tmDatabase.clear_GuidanceItemsCache();

                Assert.IsFalse(tmDatabase.path_XmlDatabase().dirExists());                            // ensure the deletion happened
                Assert.IsEmpty(tmDatabase.path_XmlDatabase().files());
                Assert.IsEmpty(tmDatabase.Cached_GuidanceItems);
                Assert.IsFalse(tmDatabase.getCacheLocation().fileExists());
                "[Test][TM_Xml_Database][delete_Database]TM database files were deleted from: {0}".info(tmDatabase.path_XmlDatabase());
            }
            return tmDatabase;
        }        
        /*public static TM_Xml_Database loadData_No_Admin_User(this TM_Xml_Database tmDatabase)
        {
            tmDatabase.Server.Users_Create_Default_Admin = false;
            return tmDatabase.setup();
        }*/
    }
}
