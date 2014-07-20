using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.QA.To_Do_Ignored
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
