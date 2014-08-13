using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_IE_TeamMentor_ExtensionMethods_Create
    {
        [Test] public void new_IE_TeamMentor()
        {
            var nUnitTests_Cassini  = new NUnitTests_Cassini_TeamMentor().start();
            var ieTeamMentor       = nUnitTests_Cassini.new_IE_TeamMentor();
            var expected_TBot_Url  = ieTeamMentor.siteUri.append("Html_Pages/Gui/Pages/login.html?LoginReferer=/tbot").str();
            ieTeamMentor.page_TBot();
            ieTeamMentor.ie.url().assert_Is(expected_TBot_Url);
            ieTeamMentor.close();
        }
    }
}