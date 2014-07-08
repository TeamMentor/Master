using System;
using System.Web;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Moq;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.SiteData;

namespace TeamMentor.UnitTests.TeamMentor.SiteData
{
    [TestFixture]
    public class Test_SiteData_Routing
    {
        public HttpContextBase      context;
        public HandleUrlRequest     handleUrlRequest;
        public TM_FileStorage       tmFileStorage;
         
        [SetUp]
        public void Setup()
        {            
            context                         =  HttpContextFactory.Context.mock();
            handleUrlRequest                = new HandleUrlRequest();
            tmFileStorage                   = new TM_FileStorage(false) { Path_XmlDatabase = "_siteData".tempDir() };

            Assert.IsTrue(tmFileStorage.Path_XmlDatabase.dirExists());
        }

        [TearDown]
        public void tearDown()
        {            
//            fileStorage.Path_XmlDatabase.startProcess();
            Assert.IsTrue(Files.deleteFolder(tmFileStorage.Path_XmlDatabase, true)); 
            Assert.IsFalse(tmFileStorage.Path_XmlDatabase.dirExists());
        }

        [Test]
        public void handledBySiteData()
        {            
            Assert.IsFalse( "/aaa.txt"              .siteData_Handle_VirtualPath());     
            Assert.IsFalse(tmFileStorage            .siteData_Handle_VirtualPath("abc"));
            Assert.IsFalse((null as string)         .siteData_Handle_VirtualPath());
            Assert.IsFalse((null as TM_FileStorage) .siteData_Handle_VirtualPath("aaa.txt"));
            Assert.IsFalse((null as TM_FileStorage) .siteData_Handle_VirtualPath(null));
            
            tmFileStorage.set_Path_SiteData();

            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("siteData"));
            Assert.IsFalse(tmFileStorage            .siteData_Handle_VirtualPath("abc123"));
            //Create test file in siteData
            
            
            var file = util_Create_Test_File_In_SiteData("aaa.txt", "some content").fileName();
            
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath(file               ));
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("/" + file         ));
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("/abc/" + file     ));
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("/abc/xyz/" + file ));
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("/abc/../" + file  ));
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("//" + file        ));
            Assert.IsTrue (tmFileStorage            .siteData_Handle_VirtualPath("../" + file       ));
            Assert.IsFalse(tmFileStorage            .siteData_Handle_VirtualPath("../abc.123"       ));
            Assert.IsFalse(tmFileStorage            .siteData_Handle_VirtualPath("abc.123"          ));
            Assert.IsFalse(tmFileStorage            .siteData_Handle_VirtualPath("//abc.123"        ));
            Assert.IsFalse(tmFileStorage            .siteData_Handle_VirtualPath(null               ));
        }

        [Test]
        public void siteData_WriteFileToResponseStream()
        {
            tmFileStorage.set_Path_SiteData();
            var file = "aaa.txt";
            var contents = "some content";
            var filePath = util_Create_Test_File_In_SiteData(file, contents);            

            //check that a bad filename does not write to response stream
            Assert.IsEmpty(context.response_Read_All());
            Assert.IsFalse((filePath + "a.txt").siteData_WriteFileToResponseStream());
            Assert.IsFalse((null as string).siteData_WriteFileToResponseStream());
            Assert.IsEmpty(context.response_Read_All());
            Assert.IsEmpty(context.Response.ContentType);       

            //check that a valid filename writes to response stream
            Assert.IsTrue(filePath.siteData_WriteFileToResponseStream());
            var responseRead = context.response_Read_All(); 
            Assert.IsNotEmpty(responseRead); 
            Assert.AreEqual(responseRead, filePath.fileContents());
            Assert.AreEqual(context.Response.ContentType, "text/plain");
        }
        [Test]
        public void siteData_SetContentType_For_File()
        {
            var response = context.Response;

            Action<string,string> checkContentType = (file,contentType)=>
                {
                    response.ContentType = null;                // confirm that the ContentType is Null
                    Assert.IsNull(response.ContentType);       
                    file.siteData_SetContentType_For_File(response);
                    Assert.AreEqual(response.ContentType, contentType);
                };
            
            checkContentType("aaa.txt"  , "text/plain");
            checkContentType(".txt"     , "text/plain");
            checkContentType("aaa.abc"  , "text/plain");

            checkContentType("aaa.js"   , "application/javascript");
            
            checkContentType("aaa.aspx" , "text/html");
            checkContentType("aaa.rz"   , "text/html");
            checkContentType("aaa.html" , "text/html");
            checkContentType("aaa.htm"  , "text/html");

            checkContentType("aaa.jpg"  , "image/jpeg");
            checkContentType("aaa.jpeg" , "image/jpeg");
            checkContentType("aaa.gif"  , "image/gif");
            checkContentType("aaa.png"  , "image/png");

            checkContentType("aaa.css"  , "text/css");            
            checkContentType("aaa.xml"  , "application/xml");
            checkContentType("aaa.xslt" , "application/xslt+xml");
        }
        //Utils
        public string util_Create_Test_File_In_SiteData(string fileName, string fileContents)
        {
            var pathSiteData  = tmFileStorage.path_SiteData();
            var filePath      = pathSiteData.pathCombine(fileName);

            Assert.IsFalse(filePath.fileExists());
            fileContents.saveAs(filePath);
            Assert.IsTrue(filePath.fileExists());
            return filePath;
        }

        
        //context.Response.ContentType = "application/json";

        
    }
}
