using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.TeamMentor.FileStorage
{
    [TestFixture]
    public class Test_TM_Server_FileStorage : TM_XmlDatabase_FileStorage
    {
        [Test] public void load_TM_Server_Config()                  
        {
         //   TM_Status.In_Setup_XmlDatabase = false;
            UserGroup.Admin.assert();

            //first test with in memory version of TM_Xml_Database
            //var tmServer = TM_Server.Load();            
            
            Assert.NotNull (tmServer);
            Assert.AreEqual(tmServer, tmFileStorage.Server);

            //now test with filebase
            //tmXmlDatabase = new TM_Xml_Database();

            //tmXmlDatabase.Path_XmlDatabase = "_tmpLocation".tempDir();
            //tmXmlDatabase.useFileStorage(true);
        }
        [Test] public void _tmServer()          // needs underscore or it will conflic with the tmServer variable from TM_XmlDatabase_FileStorage                       
        {
            UserGroup.Admin.assert();

            Assert.NotNull (tmServer);
            Assert.AreEqual(tmServer, tmFileStorage.tmServer());                          
        }  

        [Test] [Ignore("TO DO - broke after refactoring")]
        public void LoadAndSave_TMServer_To_Disk()           
        {
            UserGroup.Admin.assert();
            var tmServer = tmFileStorage.tmServer_Load();
            
            
            tmServer.tmServer_Location().info();
            var tmServerPath  = tmServer.tmServer_Location();

            Assert.IsNotNull(tmServer                   , "tmServer");
            Assert.IsNotNull(tmServerPath               , "tmServerPath");
            Assert.AreEqual (tmServerPath.parentFolder(), tmServer.Path_XmlDatabase , "tmServerPath.folderName()");
            
            Assert.Fail("Add save to disk test");
        }
    }
}
