using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_Cassini_Create_Pages : NUnitTests_Cassini
    {        
        [Test] public void Check_Server_Is_Up()
        {
            apiCassini.url().GET().contains("Directory");
        }
        [Test] public void Open_Text_Page()
        { 
            var file_Name     = "test.txt";
            var file_Contents = "some text".add_5_RandomLetters();            
            var file_Url      = apiCassini.url(file_Name);
            var file_Path     = apiCassini.mapPath(file_Name);                        
            
            file_Url    .HEAD_StatusCode().assert_Http_NotFound();              // check that it is not there
            file_Path   .assert_File_Not_Exists();

            file_Contents.saveAs(file_Path);                                    // create file and GET it            

            file_Path                  .assert_File_Exists();                   // check that it is there
            file_Url .HEAD_StatusCode().assert_Http_OK();
            file_Url .GET()            .assert_Equal_To(file_Contents);        //delete txt file
            
            file_Path.assert_File_Deleted();
        }

        [Test] public void Open_ASPX_Page()
        { 
            var file_Name     = "test.aspx";
            var dynamicText   = "some text".add_5_RandomLetters();
            var file_Contents = "<%= \"{0}\"%>".format(dynamicText);
            var file_Url      = apiCassini.url(file_Name);
            var file_Path     = apiCassini.file_From_Url(file_Url);

            file_Url    .HEAD_StatusCode().assert_Http_NotFound();
            file_Path   .assert_File_Not_Exists();
            file_Contents.saveAs(file_Path);

            file_Path                  .assert_File_Exists();                   // check that it is there
            file_Url .HEAD_StatusCode().assert_Http_OK();
            file_Url .GET()            .assert_Equal_To(dynamicText);

            file_Path.assert_File_Deleted();                                    //delete aspx file
        }

        [Test] public void Open_Html_Page()
        {
            var file_Name  = "test.html".insert_5_RandomLetters();
            var file_Html  = "<h1>html test</h1>";
            var file_Path  =  apiCassini.create_File(file_Name, file_Html);
            var file_Url   =  apiCassini.url_From_File(file_Path);
            
            file_Url.GET().assert_Is(file_Html);

            file_Path.assert_File_Deleted();
        }
    }
}