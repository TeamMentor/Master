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
            Assert.IsTrue (TM_Xml_Database.SkipServerOnlineCheck);       // set on TeamMentor.UnitTests.Tests_Config.RunBeforeAllTests()

            var tmDatabase = new TM_Xml_Database();                                                

            Assert.IsNotNull(TM_Xml_Database.Current);
            Assert.False    (tmDatabase.UsingFileStorage);          // new TM_Xml_Database() defaults to UsingFileStorage = false              
            Assert.IsNotNull(tmDatabase.Events);                    // this is the only list that should be set on the Ctor            
            Assert.IsNull   (tmDatabase.Cached_GuidanceItems);
            Assert.IsNull   (tmDatabase.GuidanceItems_FileMappings);
            Assert.IsNull   (tmDatabase.GuidanceExplorers_XmlFormat);            
            Assert.IsNull   (tmDatabase.GuidanceExplorers_Paths);           
            Assert.IsNull   (tmDatabase.VirtualArticles);                        

            Assert.IsNull   (tmDatabase.Path_XmlDatabase);
            Assert.IsNull   (tmDatabase.Path_XmlLibraries);
            Assert.IsNull   (tmDatabase.UserData);
            Assert.IsNull   (tmDatabase.Server);

            
            //check using ctor that sets UsingFileStorage
            tmDatabase = new TM_Xml_Database(true);                                                

            Assert.IsTrue   (tmDatabase.UsingFileStorage);          
                   
        }
        
    }
}
