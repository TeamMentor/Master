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
            Assert.NotNull    (tmDatabase.Path_XmlDatabase);
            Assert.AreNotEqual(tmDatabase.Path_XmlDatabase, TM_Server.WebRoot);
            //Assert.IsTrue     (tmDatabase.Path_XmlDatabase.dirExists());

            if (tmDatabase.Path_XmlDatabase.dirExists())                                            // check if the folder exists      
            {
                //Assert.IsNotEmpty(tmDatabase.Path_XmlDatabase.files());

                tmDatabase.Path_XmlDatabase.files(true).files_Attribute_ReadOnly_Remove();          // make all files writable

                Files.deleteFolder(tmDatabase.Path_XmlDatabase, true);                              // delete all files recusively

                Assert.IsFalse(tmDatabase.Path_XmlDatabase.dirExists());                            // ensure the deletion happened
                Assert.IsEmpty(tmDatabase.Path_XmlDatabase.files());

                "[Test][TM_Xml_Database][delete_Database]TM database files were deleted from: {0}".info(tmDatabase.Path_XmlDatabase);
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
