using FluentSharp.CoreLib;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_Tbot_User_View : TestFixture_TBot
    {
        public Test_Tbot_User_View()
        {
            
            tbot.login_As_Admin();            
            /*var adminAuthToken = "30ce16e9-d054-4a87-be76-edf0ba8815ec";
            var tbotPage       = "rest/tbot/run/hello";
            var url            = "{0}?auth={1}".format(tbotPage,adminAuthToken);
            this.open(url);*/

            //parentForm.normal();
        }
        [Test]
        public void page__User_View()
        {
            var testUserName = "CUfDduDCgQ";
            var url = "/rest/tbot/run/User_View?userName={0}".format(testUserName);
            tbot.open(url);
            Assert.AreEqual(ie.url(), tbot.TargetServer + url);
            Assert.IsTrue (ie.hasLink("View User"));
            Assert.IsTrue (ie.hasLink("Edit User"));
            Assert.IsTrue (ie.hasLink("View Activity/Logs"));
            Assert.IsTrue (ie.hasLink("Raw/Xml Data"));
              
            //script_IE_WaitForClose();

            //parentForm.closeForm_InNSeconds(5)
            //          .waitForClose();
        }
        
    }
}
