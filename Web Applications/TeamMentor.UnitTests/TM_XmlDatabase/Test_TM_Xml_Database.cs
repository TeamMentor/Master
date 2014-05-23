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

            //check default values
                   
        }
        //Assert.IsNull  (tmDatabase.Server.toXml(), new TM_Server().toXml());            
        
    }
}
