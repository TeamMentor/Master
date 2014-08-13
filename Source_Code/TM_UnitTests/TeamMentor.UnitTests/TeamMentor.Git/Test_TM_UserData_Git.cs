using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.TeamMentor.Git
{
    [TestFixture]
    public class Test_TM_UserData_Git
    {
        [SetUp]
        public void setup()
        {
            TM_UserData_Git.Current = null;            
        }
        [Test] public void TM_UserData_Git_Ctor()
        {
            var userDataGit = new TM_UserData_Git(null);

            Assert.AreEqual(userDataGit.NGit_Author_Name , TMConsts.NGIT_DEFAULT_AUTHOR_NAME);
            Assert.AreEqual(userDataGit.NGit_Author_Email, TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL);            
        }        
    }
}
