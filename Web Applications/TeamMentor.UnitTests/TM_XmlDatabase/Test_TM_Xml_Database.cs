using NUnit.Framework;
using System;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_TM_Xml_Database
    {
        [SetUp]
        public void setup()
        {
            UserGroup.Admin.assert();               // all these tests need TM Admin priv
            TM_Xml_Database.Current = null;         // ensure there are TM_Xml_Database set 
        }
        [Test]    
        public void TM_Xml_Database_Ctor()
        {            
            Assert.IsNull (TM_Xml_Database.Current);
            Assert.IsTrue(TM_Xml_Database.SkipServerOnlineCheck);       // set on TeamMentor.UnitTests.Tests_Config.RunBeforeAllTests()
            Assert.IsFalse(TM_Status.Current.TM_Database_In_Setup_Workflow); 
            
            TM_Xml_Database tmDatabase = new TM_Xml_Database();

            Assert.IsFalse(TM_Status.Current.TM_Database_In_Setup_Workflow); 
            //check default values
            Assert.NotNull(tmDatabase);            
            Assert.False  (tmDatabase.UsingFileStorage);
            Assert.NotNull(tmDatabase.UserData);
            Assert.NotNull(tmDatabase.NGits);
            Assert.IsNull (tmDatabase.Path_XmlDatabase);
            Assert.IsNull (tmDatabase.Path_XmlLibraries);            
            Assert.NotNull(tmDatabase.GuidanceExplorers_XmlFormat);
            Assert.NotNull(tmDatabase.GuidanceExplorers_Paths);
            Assert.NotNull(tmDatabase.GuidanceItems_FileMappings);
            Assert.NotNull(tmDatabase.Cached_GuidanceItems);
            Assert.NotNull(tmDatabase.VirtualArticles);            
        }

        [Test]
        public void ResetDatabase()
        {            
            TM_Xml_Database tmDatabase = new TM_Xml_Database();
            var resetReturnValue = tmDatabase.ResetDatabase();
            
            Assert.AreEqual(tmDatabase, resetReturnValue);
            Assert.IsEmpty(tmDatabase.NGits);
            Assert.IsEmpty(tmDatabase.Cached_GuidanceItems);
            Assert.IsEmpty(tmDatabase.GuidanceItems_FileMappings);
            Assert.IsEmpty(tmDatabase.GuidanceExplorers_XmlFormat);
            Assert.IsEmpty(tmDatabase.GuidanceExplorers_Paths);
            Assert.AreEqual(tmDatabase.TM_Server_Config.toXml(), new TM_Server().toXml());            
            Assert.IsEmpty(tmDatabase.VirtualArticles);
            Assert.NotNull(tmDatabase.UserData); // see Test_TM_SecretData for the Ctor checks
        }
    }
}
