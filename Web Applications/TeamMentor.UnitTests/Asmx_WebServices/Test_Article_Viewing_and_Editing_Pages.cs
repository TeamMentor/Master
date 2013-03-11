using NUnit.Framework;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    public class Test_Article_Viewing_and_Editing_Pages : TM_WebServices_InMemory
    {
        [Test]
        public void ViewArticle_In_MultipleFormats()
        {
            var tmArticles = tmWebServices.GetAllGuidanceItems();

        }
    }
}
