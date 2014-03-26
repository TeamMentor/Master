using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_IE_TBot
    {
        IE_UnitTest _ieUnitTest;

        [SetUp] public void setup()
        {            
            _ieUnitTest = new IE_UnitTest();         
        }        
        [Test] public void openIE()
        {            
            Assert.NotNull(_ieUnitTest.parentForm);
            Assert.NotNull(_ieUnitTest.ie);            
        }
        
        [Test] public void Open_Site_Google()
        {
            var ie = _ieUnitTest.ie;            
            ie.open("https://www.google.com");     
            "URL: {0}".info(ie.url());
            Assert.IsTrue(ie.url().contains("google"));
        }        
    }
}
