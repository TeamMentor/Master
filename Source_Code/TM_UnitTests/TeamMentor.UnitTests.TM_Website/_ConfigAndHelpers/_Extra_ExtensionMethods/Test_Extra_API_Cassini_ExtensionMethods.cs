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
        [Test] public void url()
        {            
            
            //check API_Cassini.url()
            var url          = apiCassini.url();
            var expected_Url = "http://{0}:{1}/".format(apiCassini.ipAddress(), apiCassini.port());
            url.assert_Equal_To(expected_Url);

            //check API_Cassini.url(virtualPath)
            var file = "test.txt"; 
            apiCassini.url(file).assert_Equal_To(expected_Url + file);            
        }
        [Test] public void file_From_Url()
        {
            var file    = "a/test.aspx";
            var fileUrl  = apiCassini.url(file);
            var filePath = apiCassini.file_From_Url(fileUrl);

            fileUrl .assert_Equal_To(apiCassini.url()     + file);
            filePath.assert_Equal_To(apiCassini.webRoot().pathCombine(file));
            
            apiCassini.file_From_Url(null).assert_Null();
        }
        [Test] public void url_From_File()
        {
            var file     = "a/test.aspx";
            var filePath  = apiCassini.webRoot().pathCombine(file);
            var fileUrl = apiCassini.url_From_File(filePath);

            fileUrl .assert_Equal_To(apiCassini.url()     + file);
                        
            apiCassini.url_From_File(null).assert_Null();
        }
        [Test] public void create_File()
        {
            var fileName = "test.txt"  .insert_5_RandomLetters();
            var contents = "some contents".add_5_RandomLetters();
            var filePath = apiCassini.create_File(fileName,contents);

            filePath.assert_File_Exists()
                    .assert_Are_Equal(file=>file.contents(), contents)
                    .assert_File_Deleted();
            
            
            //check nulls and empty
            apiCassini.create_File(fileName,""  ).assert_Null();
            apiCassini.create_File(fileName,null).assert_Null();
                        
        }
    }
}