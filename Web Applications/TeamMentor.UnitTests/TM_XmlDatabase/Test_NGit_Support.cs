using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_NGit_Support
    {
        public string TempRepo   { get; set; }
        public API_NGit NGitApi  { get; set; }

        public Test_NGit_Support()
        {
            TempRepo = "_tempRepo".tempDir(false);
            NGitApi = new API_NGit();
        }

        [Test]
        public void CreateLocalTestRepo()
        {
            "TestRepo is: {0}".info(TempRepo);
            "Parent Dir exists: {0}".info(TempRepo.parentFolder().dirExists());
            TempRepo.parentFolder().createDir();
            "Parent Dir exists: {0}".info(TempRepo.parentFolder().dirExists());
            //Creating a local temp Repo
            Assert.IsFalse(TempRepo.isGitRepository());              
            NGitApi.init(TempRepo);            
            Assert.IsTrue(TempRepo.isGitRepository());            
            Assert.IsNull(NGitApi.head());

            //Adding a file (using method 1)
            NGitApi.create_File("testFile.txt", "some Text");
            NGitApi.add_and_commit_using_Status();
            var head1 = NGitApi.head();
            Assert.IsNotNull(head1);
                        
            //Adding another file (using method 2)
            NGitApi.create_File("testFile2.txt", "some Text");
            NGitApi.add("testFile2.txt");
            NGitApi.commit("Adding Another file");
            
            //making sure the head has changed
            var head2 = NGitApi.head();
            Assert.AreNotEqual(head1,head2);            
        }
    }
}
