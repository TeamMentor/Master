using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.Schemas.Events
{
    [TestFixture]
    public class Test_Events_TM_UserData
    {
        TM_UserData        tmUserData;
        
        [SetUp]
        public void setup()
        {
            tmUserData = new TM_UserData();                                    
            Assert.NotNull (tmUserData);            
            Assert.NotNull (tmUserData.Events);                        
            Assert.IsFalse(tmUserData.usingFileStorage());
        }

        [Test] public void TM_Events_TM_UserData_Ctor()
        {
            var tmEvents = new Events_TM_UserData(tmUserData);  
            Assert.NotNull (tmEvents);
            Assert.AreEqual(tmEvents.Target, tmUserData);
            Assert.NotNull (tmEvents.Before_TM_Config_Load);     
            Assert.NotNull (tmEvents.After_TM_Config_Load);
            Assert.NotNull (tmEvents.After_TM_SecretData_Load);       
            Assert.NotNull (tmEvents.After_Users_Load);                   
        }
        [Test] public void Events_inside_tmConfig_Load()
        {
            var tmEvents = tmUserData.Events;
            Assert.AreEqual(tmEvents.Before_TM_Config_Load.Total_Invocations, 0);
            Assert.AreEqual(tmEvents.After_TM_Config_Load .Total_Invocations, 0);
            tmUserData.tmConfig_Load();
            Assert.AreEqual(tmEvents.Before_TM_Config_Load.Total_Invocations, 1);
            Assert.AreEqual(tmEvents.After_TM_Config_Load .Total_Invocations, 1);
        }
        [Test] public void Events_inside_secretData_Load()
        {
            var tmEvents = tmUserData.Events;            
            Assert.AreEqual(tmEvents.After_TM_SecretData_Load .Total_Invocations, 0);
            tmUserData.secretData_Load();
            Assert.AreEqual(tmEvents.After_TM_SecretData_Load.Total_Invocations , 1);            
        }
        [Test] public void Events_inside_users_Load()
        {
            var tmEvents = tmUserData.Events;            
            Assert.AreEqual(tmEvents.After_Users_Load .Total_Invocations, 0);
            tmUserData.users_Load();
            Assert.AreEqual(tmEvents.After_Users_Load.Total_Invocations , 1);            
        }
    }
}
