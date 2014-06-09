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
        [Test][Ignore("TO DO - broke after refactoring")]
        public void Test_Events_Fired_During_Setup()
        {

            var tmEventsDb  = tmDatabase.Events;

            tmDatabase.setup();
            
            //these are the events fired by the methods that exist inside the setup method
            Assert.AreEqual(tmEventsDb.Before_Setup                 .Total_Invocations, 1);
            Assert.AreEqual(tmEventsDb.After_Setup                  .Total_Invocations, 1);
            Assert.AreEqual(tmEventsDb.After_Set_Default_Values     .Total_Invocations, 1);
            Assert.AreEqual(tmEventsDb.After_Set_Path_XmlLibraries  .Total_Invocations, 1);            
            Assert.AreEqual(tmEventsDb.After_Load_UserData          .Total_Invocations, 1);
            Assert.AreEqual(tmEventsDb.After_Load_SiteData          .Total_Invocations, 1);
            Assert.AreEqual(tmEventsDb.After_Load_Libraries         .Total_Invocations, 1);

            //These are the events that also fire during the setup process
//            Assert.AreEqual(tmEventsDb.After_UserData_Ctor          .Total_Invocations, 1);
            
            Assert.AreEqual(tmUserData.Events.Before_TM_Config_Load  .Total_Invocations, 1);            
            Assert.AreEqual(tmUserData.Events.After_TM_Config_Load   .Total_Invocations, 1);
        }
        [Test][Ignore("TO DO - broke after refactoring")]
        public void Test_Individual_Setup_Events()
        {
            //After_Set_Default_Values
            Assert.AreEqual(tmDatabase.Events.After_Set_Default_Values.Total_Invocations,0);
            tmDatabase.set_Default_Values();
            Assert.AreEqual(tmDatabase.Events.After_Set_Default_Values.Total_Invocations,1);

            //After_Set_Path_XmlDatabase
            Assert.AreEqual(tmDatabase.Events.After_Set_Path_XmlLibraries.Total_Invocations,0);
//            tmDatabase.set_Path_XmlLibraries();
            Assert.AreEqual(tmDatabase.Events.After_Set_Path_XmlLibraries.Total_Invocations,1);

            //After_Load_UserData
            Assert.AreEqual(tmDatabase.Events.After_Load_UserData.Total_Invocations,0);
  //          tmDatabase.load_UserData();
            Assert.AreEqual(tmDatabase.Events.After_Load_UserData.Total_Invocations,1);

            //After_Load_SiteData
            Assert.AreEqual(tmDatabase.Events.After_Load_SiteData.Total_Invocations,0);
            tmDatabase.load_SiteData();
            Assert.AreEqual(tmDatabase.Events.After_Load_SiteData.Total_Invocations,1);
            
            //After_Load_Libraries
            Assert.AreEqual(tmDatabase.Events.After_Load_Libraries.Total_Invocations,0);
//            tmDatabase.load_Libraries();
            Assert.AreEqual(tmDatabase.Events.After_Load_Libraries.Total_Invocations,1);
        }
    }
}
