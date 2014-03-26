using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Tbot_Users_Main : API_IE_TBot
    {                
        public Tbot_Users_Main()
        {            
            if (TargetServer.GET().notValid())
                Assert.Ignore("Target server is offline: {0}".format(TargetServer));
        }        
        [Test]
        public void Login_Into_TBot()
        {
            this.page_Login("/tbot");
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
