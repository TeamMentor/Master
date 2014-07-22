using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.NUnit;
using TeamMentor.UnitTests.Cassini;
using TeamMentor.WatiN.NUnit;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_IE
{
    [TestFixture]
    public class Test_QA__SiteData_Pages: NUnitTests_Cassini_TeamMentor
    {
        TM_Server      tmServer;
        TM_FileStorage tmFileStorage;
        string         path_SiteData;

        [SetUp] public void setup()
        {            
            this.tmProxy_Refresh();
            tmFileStorage = tmProxy.TmFileStorage.assert_Not_Null();
            tmServer      = tmProxy.TmServer.assert_Not_Null();
            path_SiteData = tmFileStorage.path_SiteData();
        }
        [Test] public void Confirm_That_Default_SiteData_Is_Set()
        {
            tmServer.siteData_Config().assert_Not_Null()
                                      .Name.assert_Is("Site_Data");
            tmFileStorage.path_SiteData().assert_Not_Null()
                                         .assert_Folder_Exists();

        }
        [Test] public void Test_Rendering_Of_Static_Pages()
        {           
            // test with TXT file
           path_SiteData.folder_Create_File("textFile.txt",                    "This is a text")
                        .append_FileName_To(siteUri       ).assert_HTTP_GET_Is("This is a text");

           // test with HTM file
           path_SiteData.folder_Create_File("textFile.htm",                    "This is in <h1>HTML</h1>")
                        .append_FileName_To(siteUri       ).assert_HTTP_GET_Is("This is in <h1>HTML</h1>");

            // test with GIf file
           path_SiteData.folder_Create_File("textFile.gif",                    "this is not really an image")
                        .append_FileName_To(siteUri       ).assert_HTTP_GET_Is("this is not really an image");
        }
        /// <summary>
        /// This is an interresting security blind spot that only seems to happen on Cassini (IIS blocks these)
        /// </summary>
        [Test] public void Test_Cassini_Rendering_Of_Protected_Resources()
        {
            path_SiteData.folder_Create_File("textFile.cshtml", "abc")
                             .append_FileName_To(siteUri).assert_HTTP_GET_Is_Not("");
            path_SiteData.folder_Create_File("textFile.cs"    , "abc")
                             .append_FileName_To(siteUri).assert_HTTP_GET_Is_Not("");
            path_SiteData.folder_Create_File("textFile.config", "abc")
                             .append_FileName_To(siteUri).assert_HTTP_GET_Is_Not("");
        }

        [Test] public void Test_Rendering_Of_Aspx_Pages()        
        {
           path_SiteData.folder_Create_File("textFile.aspx",                    "This is <%=\"dynamic text\"%>");
           siteUri.append                  ("textFile.aspx").assert_HTTP_GET_Is("This is dynamic text");
        }

        /// <summary>
        /// Adding *.cshtml pages to an TBot folder located inside the SiteData folder 
        /// should add it as part of the current TBot pages
        /// </summary>
        [Test] public void Check_Extra_Tbot_Pages()
        {            
            //admin.assert();
            var tbotBrain = tmProxy.tbot_Brain().assert_Not_Null();
            
            var originalSize = tbotBrain.availableScripts().size().assert_Is_Bigger(10);
            
            path_SiteData.folder_Create_Folder("Tbot")                                                      // create a TBot folder in the Site_Data folder
                         .folder_Create_File("tbotPage.cshtml", "this is an TBot page: @(40+2)");                    // create a test Razor page inside it
            
            tmProxy.TmFileStorage.path_SiteData().mapPath(@"TBot\tbotPage.cshtml").assert_File_Exists();    // confirm file was created on correct location

            tmProxy.tbot_Brain_SetAvailableScripts();                                                       // reload the mappings
            
            tbotBrain.availableScripts().assert_Size_Is(originalSize + 1);                                  // there should be one Tbot page now

            tbotBrain.ExecuteRazorPage("tbotPage").assert_Is("this is an TBot page: 42");                   // confirming correct parsing and execution of the razor/cshtml page       
        }

    }    
}
