using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_IE_TBot
    {
        IE_TBot ieTBot;

        [SetUp] public void setup()
        {
            //ieTBot = IE_TBot.Current;
            ieTBot = new IE_TBot();
            //ieTBot.openIE();
        }        
        [Test] public void openIE()
        {            
            Assert.NotNull(ieTBot.parentForm);
            Assert.NotNull(ieTBot.ie);            
        }

        /*[Test] public void Check_Static_Prop_Current()
        {
            Assert.IsNotNull(IE_TBot.Current);
        }*/
        [Test] public void Open_Site_Google()
        {
            var ie = ieTBot.ie;            
            ie.open("https://www.google.com");            
            Assert.IsTrue(ie.url().contains("google"));
        }        
    }
}
