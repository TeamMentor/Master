using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture][Ignore]
    public class Test_IE_TBot
    {
        IE_UnitTest ieUnitTest;        

        [SetUp] public void setup()
        {                        
            ieUnitTest = new IE_UnitTest();         
        }        

        [Test] public void Test_IE_TBot_Ctor()
        {
            Assert.IsNull(ieUnitTest.TargetServer);
            Assert.NotNull(ieUnitTest.parentForm);
            Assert.NotNull(ieUnitTest.ie);
        }
        [Test] public void openIE()
        {            
            Assert.NotNull(ieUnitTest.parentForm);
            Assert.NotNull(ieUnitTest.ie);            
        }
        
        [Test] public void Open_Site_Google()
        {
            var google = "https://www.google.com";
            if (google.uri().HEAD().isFalse())
                Assert.Ignore();

            var ie = ieUnitTest.ie;            
            ie.open(google);     
            "URL: {0}".info(ie.url());
            Assert.IsTrue(ie.url().contains("google"));
        }        
    }
}
