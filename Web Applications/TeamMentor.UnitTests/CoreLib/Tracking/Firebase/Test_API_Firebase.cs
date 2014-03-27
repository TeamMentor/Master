using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib 
{
    [TestFixture]
    public class Test_API_Firebase : TM_UserData_InMemory
    {
        public API_Firebase firebase;

        public Test_API_Firebase()
        {
            firebase = API_Firebase.Current;
        }
        [Test]
        public void API_Firebase_Ctor()
        {
            Assert.IsNotNull(firebase);
            Assert.IsNotNull(firebase.Site);
            Assert.IsNotNull(firebase.Area);
            Assert.IsNotNull(firebase.AuthToken);
            Assert.IsNotNull(firebase.MessageFormat);
        }
    }
}
