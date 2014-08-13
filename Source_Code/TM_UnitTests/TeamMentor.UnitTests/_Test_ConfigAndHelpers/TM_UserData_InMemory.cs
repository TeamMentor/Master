using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests
{
    public class TM_UserData_InMemory : TM_Config_InMemory
    {
        public TM_UserData      userData;

        public TM_UserData_InMemory()
        {            
            userData = new TM_UserData();

            1.set_DEFAULT_PBKDF2_INTERACTIONS();                    // improve performance of tests that create users            
            SendEmails.Disable_EmailEngine = true;                  // Disable Email engine by default            

            // check the TM_UserData
            Assert.IsEmpty(userData.validSessions() , "There should be no sessions");
            Assert.IsEmpty(userData.TMUsers         , "There should be no users");
        }
    }
}
