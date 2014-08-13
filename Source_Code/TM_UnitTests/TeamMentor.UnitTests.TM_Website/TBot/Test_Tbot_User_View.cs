using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.REPL;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_Tbot_User_View : TestFixture_TBot
    {      
        [SetUp]
        public void setup()
        {
            tbot.login_As_Admin();
        }
        [Test]
        public void page__User_View()
        {   
            //ie.parentForm().show();
                        
            ie.assert_Not_Null();
            var testUserName = "CUfDduDCgQ";
            var url = "/rest/tbot/run/User_View?userName={0}".format(testUserName);
            tbot.open(url);
            
            Assert.AreEqual(ie.url(), tbot.TargetServer + url);
            Assert.IsTrue (ie.hasLink("View User"));
            Assert.IsTrue (ie.hasLink("Edit User"));
            Assert.IsTrue (ie.hasLink("View Activity/Logs"));
            Assert.IsTrue (ie.hasLink("Raw/Xml Data"));                        
        }        
    }
}
