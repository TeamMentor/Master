using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

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
            TM_FileStorage.Current  = null;
        }
        [Test]    
        public void TM_Xml_Database_Ctor()
        {            
            Assert.IsNull (TM_Xml_Database.Current);
            Assert.IsTrue (TM_Xml_Database.SkipServerOnlineCheck);       // set on TeamMentor.UnitTests.Tests_Config.RunBeforeAllTests()

            var tmDatabase = new TM_Xml_Database();                                                

            Assert.IsNotNull(TM_Xml_Database.Current);
            Assert.False    (tmDatabase.usingFileStorage());        // new TM_Xml_Database() defaults to UsingFileStorage = false              
            Assert.IsNotNull(tmDatabase.Events);                    // this is the only list that should be set on the Ctor            
            Assert.IsNotNull(tmDatabase.Cached_GuidanceItems);            
            Assert.IsNotNull(tmDatabase.GuidanceExplorers_XmlFormat);                        
            Assert.IsNotNull(tmDatabase.VirtualArticles);                                    
        }
        
    }
}
