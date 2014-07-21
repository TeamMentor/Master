using FluentSharp.NUnit;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_Cassini_Password_Reset : NUnitTests_Cassini_TeamMentor
    {
        WatiN_IE ie;

        [SetUp]
        public void setUp()
        {
            ie = "Test_Cassini_User_Management".add_IE_Hidden_PopupWindow();
        }
        [Test]
        public void Check_Password_Reset_Page_Reset()
        {
            webRoot.assert_Not_Null();
            // TODO
        }
    }
}
