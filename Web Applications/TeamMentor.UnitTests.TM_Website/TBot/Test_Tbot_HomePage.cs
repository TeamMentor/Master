using System.Threading;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.TBot
{
    [TestFixture]
    public class Test_Tbot_HomePage : API_IE_TBot
    {
        [Test]
        public void tbot_HomePage()
        {            
            //parentForm.closeForm_InNSeconds(2);
            //parentForm.normal(); 
            this.page_TBot();
            Assert.IsTrue(ie.url().contains("Html_Pages/Gui/Pages/login.html"));  // means we are not logged in

            //Add way to login user
      //      parentForm.waitForClose();    
        }
    }
}
