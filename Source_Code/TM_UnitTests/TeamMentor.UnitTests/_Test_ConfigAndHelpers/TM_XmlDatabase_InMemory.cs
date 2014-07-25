using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests
{    
    public class TM_XmlDatabase_InMemory : TM_UserData_InMemory
    {
        public TM_Xml_Database  tmXmlDatabase;                
        
        public TM_XmlDatabase_InMemory()
        {
            SetupDatabase();            
        }

        [Assert_Admin]
        public void SetupDatabase()
        {                        
            UserGroup.Admin.assert();
            
            TM_FileStorage.Current = null;              // ensure this is not setup

            tmXmlDatabase   = new TM_Xml_Database().setup();
            
            
            userData.createDefaultAdminUser(); 
            
            Assert.IsEmpty(tmXmlDatabase.Cached_GuidanceItems	    , "Cached_GuidanceItems");
            Assert.IsEmpty(userData.validSessions()                 , "ActiveSessions");
            Assert.AreEqual(userData.TMUsers.size()              ,1 , "TMUsers");	                 // there should be admin            
            
            UserGroup.None.assert();
        }

        [TestFixtureSetUp]		
        public void TestFixtureSetUp()
        {			            
        }
    }
}
