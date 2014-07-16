using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_Extra_API_Cassini_ExtensionMethods
    {
        public API_Cassini apiCassini;
        [SetUp] public void setup()
        {
            apiCassini = new API_Cassini();            

            apiCassini.webRoot().assert_Folder_Empty();
        }
        [TearDown] public void tearDown()
        {
            apiCassini.webRoot().assert_Folder_Deleted();
        }
    }
}