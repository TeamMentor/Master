using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Tbot_Users_Main : TestFixture_TBot
    {                
        /*public Tbot_Users_Main()
        {                    
        } */       
        [Test]
        public void Login_Into_TBot()
        {
            tbot.page_Login("/tbot");
            "URL: {0}".info(ie.url());
            Assert.IsTrue(ie.url().contains("Html_Pages/Gui/Pages/login.html"));
            Assert.IsTrue(ie.hasField("username"));
            Assert.IsTrue(ie.hasField("password"));
            Assert.IsTrue(ie.hasButton("Login"));
            
            ie.field("username").value(Tests_Consts.DEFAULT_ADMIN_USERNAME);
            ie.field("password").value(Tests_Consts.DEFAULT_ADMIN_PASSWORD);
            ie.button("Login").click();

            //script_IE_WaitForClose();                     
        }        
    }
}
