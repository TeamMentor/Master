using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.UnitTests
{
    public static class Misc_CoreLib_ExtensionMethods
    {
        public static TM_Xml_Database delete_Database(this TM_Xml_Database tmDatabase)
        {
            Assert.NotNull    (tmDatabase);
            Assert.NotNull    (tmDatabase.path_XmlDatabase());
            Assert.AreNotEqual(tmDatabase.path_XmlDatabase(), TM_Server.WebRoot);
            //Assert.IsTrue     (tmDatabase.path_XmlDatabase().dirExists());

            if (tmDatabase.path_XmlDatabase().dirExists())                                            // check if the folder exists      
            {
                //Assert.IsNotEmpty(tmDatabase.path_XmlDatabase().files());

                tmDatabase.path_XmlDatabase().files(true).files_Attribute_ReadOnly_Remove();          // make all files writable

                Files.deleteFolder(tmDatabase.path_XmlDatabase(), true);                              // delete all files recusively

                Assert.IsFalse(tmDatabase.path_XmlDatabase().dirExists());                            // ensure the deletion happened
                Assert.IsEmpty(tmDatabase.path_XmlDatabase().files());

                "[Test][TM_Xml_Database][delete_Database]TM database files were deleted from: {0}".info(tmDatabase.path_XmlDatabase());
            }
            return tmDatabase;
        }        
        public static TM_Xml_Database loadData_No_Admin_User(this TM_Xml_Database tmDatabase)
        {
            tmDatabase.Server.Users_Create_Default_Admin = false;
            return tmDatabase.setup();
        }
    }
}
