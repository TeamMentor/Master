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
    public class Tbot_Main : API_IE_TBot
    {                
        public Tbot_Main()
        {            
            if (TargetServer.GET().notValid())
                Assert.Ignore("Target server is offline: {0}".format(TargetServer));
        }        
        [Test]
        public void Login_Into_TBot()
        {
            this.page_Login("/tbot");
            Assert.IsTrue(ie.url().contains("Html_Pages/Gui/Pages/login.html"));
            Assert.IsTrue(ie.hasField("username"));
            Assert.IsTrue(ie.hasField("password"));
            Assert.IsTrue(ie.hasButton("Login"));
            var username = "admin";
            var password = "!!tmadmin";
            ie.field("username").value(username);
            ie.field("password").value(password);
            ie.button("Login").click();
         /*   this.open_ASync("tbot_v2")            
                .script_IE()            
                .waitForClose();*/

            //tbot_V2();
            //"continue".alert();
        }        
    }
}
