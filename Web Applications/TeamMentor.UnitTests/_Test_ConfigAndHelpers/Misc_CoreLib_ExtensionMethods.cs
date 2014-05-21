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
            Assert.IsNotEmpty (tmDatabase.Path_XmlDatabase.files());
            Assert.IsTrue     (tmDatabase.Path_XmlDatabase.dirExists());
            //make all files writable
            tmDatabase.Path_XmlDatabase.files(true).files_Attribute_ReadOnly_Remove();
            
            //delete all files recusively
            Files.deleteFolder(tmDatabase.Path_XmlDatabase, true);

            //ensure the deletion happened
            Assert.IsFalse    (tmDatabase.Path_XmlDatabase.dirExists());
            Assert.IsEmpty    (tmDatabase.Path_XmlDatabase.files());

            "[Test][TM_Xml_Database][delete_Database]TM database files were deleted from: {0}".info(tmDatabase.Path_XmlDatabase);
            return tmDatabase;
        }
    }
}
