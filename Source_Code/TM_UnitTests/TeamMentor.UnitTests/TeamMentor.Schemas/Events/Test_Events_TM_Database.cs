using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Schemas
{
    [TestFixture]
    public class Test_Events_TM_Database
    {        
        TM_Xml_Database        tmDatabase;        
        TM_UserData            tmUserData; 
        
        [SetUp]
        public void setup()
        {
            UserGroup.Admin.assert();
            tmDatabase = new TM_Xml_Database();    
            tmUserData = new TM_UserData();
                  
            Assert.NotNull (tmDatabase);            
            Assert.NotNull (tmDatabase.Events);                                    
        }
        [Test]
        public void TM_Events_TM_Xml_Database_Ctor()
        {
            var tmEvents = new Events_TM_Xml_Database(tmDatabase);            
            Assert.NotNull (tmEvents);
            Assert.AreEqual(tmEvents.Target, tmDatabase);
            Assert.NotNull (tmEvents.Before_Setup);     
            Assert.NotNull (tmEvents.After_Setup);     
            Assert.NotNull (tmEvents.After_Set_Default_Values);   
            Assert.NotNull (tmEvents.After_Set_Path_XmlLibraries);   
            Assert.NotNull (tmEvents.After_Load_UserData);   
            Assert.NotNull (tmEvents.After_Load_SiteData);   
            Assert.NotNull (tmEvents.After_Load_Libraries);               
        }
        [Test]
        public void event_Before_Setup()
        {
            var tmEventsDb  = tmDatabase.Events;
            var beforeSetup = tmEventsDb.Before_Setup;
            Assert.NotNull (beforeSetup);
            Assert.AreEqual(beforeSetup, tmDatabase.Events.Before_Setup);            
            Assert.AreEqual(beforeSetup.Target, tmDatabase);            
            Assert.AreEqual(beforeSetup.Total_Invocations,0);

            // test directly
            beforeSetup.raise();
                                                
            Assert.AreEqual(beforeSetup.Total_Invocations,1);                        
            Assert.AreEqual(beforeSetup.Total_Invocations,tmDatabase.Events.Before_Setup.Total_Invocations);
        }
    }
}
