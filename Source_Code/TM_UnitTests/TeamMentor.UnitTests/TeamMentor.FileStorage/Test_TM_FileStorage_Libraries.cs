using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.FileStorage
{
    public  class Test_TM_FileStorage_Libraries
    {

        [Test]
        public void TM_FileStorage_Ctor_False()
        {
            var tmFileStorage = new TM_FileStorage(loadData: false);
            Assert.NotNull(tmFileStorage.Server);
            Assert.NotNull(tmFileStorage.TMXmlDatabase);
            Assert.NotNull(tmFileStorage.UserData);
            Assert.IsEmpty(tmFileStorage.GuidanceExplorers_Paths);
            Assert.IsEmpty(tmFileStorage.GuidanceItems_FileMappings);
            Assert.IsNull(tmFileStorage.WebRoot);
            Assert.IsNull(tmFileStorage.Path_XmlDatabase);
            Assert.IsNull(tmFileStorage.Path_UserData);
            Assert.IsNull(tmFileStorage.Path_XmlLibraries);
        }

        /// <summary>
        /// This test verifies that delete article event has been registered, otherwise the event won't be trige
        /// </summary>
        [Test]
        public void TM_hook_Events_TM_Xml_Database()
        {
            var tmFileStorage = new TM_FileStorage(loadData: false);
            Assert.NotNull(tmFileStorage);
            UserGroup.Admin.assert();
            var result = tmFileStorage.hook_Events_TM_Xml_Database();

            Assert.AreEqual(result.TMXmlDatabase.Events.Article_Deleted.Count, 1);
            Assert.AreEqual(result.TMXmlDatabase.Events.Article_Saved.Count, 1);
            Assert.AreEqual(result.TMXmlDatabase.Events.GuidanceExplorer_Save.Count, 1);
            Assert.AreEqual(result.TMXmlDatabase.Events.Articles_Cache_Updated.Count, 2);
           
        }
        [Test]
        [ExpectedException (typeof(System.Security.SecurityException))]
        public void TM_hook_Events_TM_Xml_Database_RestrictedTo_Reader()
        {
            var tmFileStorage = new TM_FileStorage(loadData: false);
            Assert.NotNull(tmFileStorage);
            UserGroup.Reader.assert();
            var result = tmFileStorage.hook_Events_TM_Xml_Database();
        }
        [Test]
        [ExpectedException(typeof(System.Security.SecurityException))]
        public void TM_hook_Events_TM_Xml_Database_RestrictedTo_Editor()
        {
            var tmFileStorage = new TM_FileStorage(loadData: false);
            Assert.NotNull(tmFileStorage);
            UserGroup.Reader.assert();
            var result = tmFileStorage.hook_Events_TM_Xml_Database();
        }
        [Test]
        [ExpectedException(typeof(System.Security.SecurityException))]
        public void TM_hook_Events_TM_Xml_Database_RestrictedTo_Anonymous()
        {
            var tmFileStorage = new TM_FileStorage(loadData: false);
            Assert.NotNull(tmFileStorage);
            var result = tmFileStorage.hook_Events_TM_Xml_Database();
        }

        [Test]
        public void library_Deleted_LibraryNotExist()
        {
            UserGroup.Admin.assert();
            var tmFileStorage = new TM_FileStorage(loadData: false);
            Assert.NotNull(tmFileStorage);
            var lib = tmFileStorage.load_Libraries();
            var result= tmFileStorage.library_Deleted(new TM_Library());
            Assert.IsFalse(result);
        }
    }
}
