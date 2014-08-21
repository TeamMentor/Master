using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.Moq;
using FluentSharp.NUnit;
using FluentSharp.Web;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.NUnit;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_Http
{
    [TestFixture]
    public class Test_QA_Http__3_5_Issues: NUnitTests_Cassini_TeamMentor
    {
        TM_FileStorage tmFileStorage;
        string         path_SiteData;

        [SetUp] public void setup()
        {            
            this.tmProxy_Refresh();
            tmFileStorage = tmProxy.TmFileStorage.assert_Not_Null();            
            path_SiteData = tmFileStorage        .path_SiteData();
        }    

        [Test] public void Issue_877_GuidanceItemViewer_and_GuidanceItemEditor_customizations_are_not_picked_up()
        {   
            //check on hosted cassini appdaomain           
            
            var guidanceItemViewer = this.webRoot.mapPath(@"Html_Pages\GuidanceItemViewer\GuidanceItemViewer.html").assert_File_Exists();            //if it works for guidanceItemEditor it will also work for guidanceItemEditor
            webRoot.mapPath(guidanceItemViewer).assert_File_Exists();            

            siteUri.mapPath("article").GET().assert_Is(guidanceItemViewer.fileContents());            
            siteUri.mapPath("viewer" ).GET().assert_Is(guidanceItemViewer.fileContents());
            
            var custom_Text = "This is a test".add_5_RandomLetters();
            path_SiteData.folder_Create_File("GuidanceItemViewer.html",        custom_Text)
                        .append_FileName_To(siteUri       ).assert_HTTP_GET_Is(custom_Text);

            siteUri.mapPath(@"Html_Pages\GuidanceItemViewer\GuidanceItemViewer.html").GET().assert_Is(custom_Text);
            siteUri.mapPath("article"                                               ).GET().assert_Is(custom_Text);
            siteUri.mapPath("viewer"                                                ).GET().assert_Is(custom_Text);



            // check on this thread's appdomain 
            admin.assert();
            var tmpSiteData = "_tempSitData".tempDir();
            var tmpKey      = "abc"     .add_5_RandomLetters().lower();
            var tmpFileName = "abc.txt" .insert_5_RandomLetters();
            var tmContents  = "12345"   .add_5_RandomLetters();

            HttpContextFactory.Current.mock();

            new TM_FileStorage().Path_SiteData = tmpSiteData;

            var handleUrlRequest = new HandleUrlRequest();            
            HandleUrlRequest.Server_Transfers.add(tmpKey, tmpFileName);

            Assert.DoesNotThrow     (()=> handleUrlRequest.transfer_Request("doesnotexist"));
            Assert.Throws<Exception>(()=> handleUrlRequest.transfer_Request("teammentor"));
            Assert.Throws<Exception>(()=> handleUrlRequest.transfer_Request(tmpKey));
            
            HttpContextFactory.Response.OutputStream.readToEnd().assert_Is(String.Empty);

            tmpSiteData.folder_Create_File(tmpFileName, tmContents);

            Assert.Throws<Exception>(()=> handleUrlRequest.transfer_Request(tmpKey));
            HttpContextFactory.Response.OutputStream.readToEnd().assert_Is(tmContents);

            tmpSiteData.assert_Folder_Deleted();

        }

    }
}
