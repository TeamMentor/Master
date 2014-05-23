using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.UnitTests.TM_Website;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_TM_QA_Config_Loader
    {
        public TM_QA_Config         qaConfig;
        public TM_QA_Config_Loader  qaConfigLoader;
        
        [SetUp] public void setup() 
        {
            qaConfigLoader = new TM_QA_Config_Loader();
            qaConfig       = new TM_QA_Config_Loader().load();            

            Assert.IsNotNull(qaConfigLoader);
            Assert.IsNotNull(qaConfig);
            Assert.IsFalse  (TM_QA_Config.Force_Server_Offline);
        }
        // methods
        [Test] public void TM_QA_Config_Loader_Ctor()                       
        {
            Assert.AreEqual(qaConfigLoader.LocalFolder, Tests_Consts.TM_QA_ConfigFile_LOCAL_FOLDER);
            Assert.AreEqual(qaConfigLoader.FileName   , Tests_Consts.TM_QA_ConfigFile_FILE_NAME   );
        }   
        [Test] public void localFilePath()                                  
        {
            //check that file exists
            var tmQaConfig_Path = qaConfigLoader.localFilePath();
            tmQaConfig_Path.info();
            Assert.IsNotNull(tmQaConfig_Path);
            Assert.IsTrue   (tmQaConfig_Path.fileExists());

            //
            var tmQAConfig2 = tmQaConfig_Path.load<TM_QA_Config>();            
            Assert.IsNotNull(tmQAConfig2);
            Assert.IsNotNull(tmQAConfig2.Default_Admin_Email);                        
            Assert.AreEqual (tmQAConfig2.toXml(), tmQaConfig_Path.load<TM_QA_Config>().toXml());
        }
        [Test] public void load_create()                                    
        {
            var tmpFolder = "TM_QA_Config".tempDir(false); 
            var tmpFile = 10.randomLetters().append(".txt");
           
            qaConfigLoader = new TM_QA_Config_Loader(tmpFolder, tmpFile);

            Assert.IsFalse(qaConfigLoader.localFilePath().fileExists());

            qaConfig = qaConfigLoader.create();

            //check values
            Assert.NotNull (qaConfig                             );            
            Assert.IsTrue  (qaConfig.Firebase_Site       .valid());
            Assert.IsTrue  (qaConfig.Firebase_Area       .valid());            
            Assert.IsTrue  (qaConfig.SMTP_Server         .valid());
            Assert.IsTrue  (qaConfig.SMTP_UserName       .valid());            
            Assert.IsTrue  (qaConfig.Default_Admin_Email .valid());
            Assert.IsTrue  (qaConfig.Default_Admin_User  .valid());
            Assert.IsTrue  (qaConfig.Default_Admin_Pwd   .valid());
            Assert.AreEqual(qaConfig.Firebase_AuthToken  , ""    );
            Assert.AreEqual(qaConfig.SMTP_Password       , ""    );
            Assert.NotNull (qaConfig.TestUsers                   );
            Assert.AreEqual(qaConfig.TestUsers.size()    , 4     );            

            // check that is exists and it is also created using .load()
            var localFilePath = qaConfigLoader.localFilePath();
            Assert.IsTrue   (localFilePath.fileExists());            
            localFilePath.file_Delete();
            Assert.IsFalse  (localFilePath.fileExists());            
            Assert.IsNotNull(qaConfigLoader.load());
            Assert.IsTrue   (localFilePath.fileExists());            

            //delete temp folder
            Assert.IsTrue  (tmpFolder.dirExists());
            Assert.IsTrue  (Files.deleteFolder(tmpFolder, true));
            Assert.IsFalse(tmpFolder.dirExists());
        }        
        [Test] public void offline()
        {
            var targetServer = qaConfig.Url_Target_TM_Site.uri().append("default.htm");
            Assert.NotNull(targetServer);
            qaConfig.assertIgnore_If_Offine(targetServer);            

            Assert.IsTrue (qaConfig.serverOnline());
            Assert.IsTrue (qaConfig.serverOnline(targetServer));
            Assert.IsFalse(qaConfig.serverOffline());
            Assert.IsFalse(qaConfig.serverOffline(targetServer));
            
            Assert.IsTrue(qaConfig.serverOffline("".uri()));                        
            Assert.IsTrue(qaConfig.serverOffline(null));
            Assert.IsFalse(qaConfig.serverOnline("".uri()));                        
            Assert.IsFalse(qaConfig.serverOnline(null));
        }
        [Test] public void save_QAConfig()
        {
            
        }
        //Misc workflows
        [Test] public void TM_QA_Config_TestUsers()                         
        {
            Assert.AreEqual(qaConfig.TestUsers.size() , 4 );            
            
            Assert.NotNull (qaConfig.testUser("qa-admin"    )); 
            Assert.NotNull (qaConfig.testUser("qa-editor"   )); 
            Assert.NotNull (qaConfig.testUser("qa-reader"   )); 
            Assert.NotNull (qaConfig.testUser("qa-developer")); 
            
            Assert.AreEqual(qaConfig.testUser("qa-admin"    ).GroupId , 1);
            Assert.AreEqual(qaConfig.testUser("qa-reader"   ).GroupId , 2);
            Assert.AreEqual(qaConfig.testUser("qa-editor"   ).GroupId , 3);            
            Assert.AreEqual(qaConfig.testUser("qa-developer").GroupId , 4);

            Assert.IsNull  (qaConfig.testUser("qa-anonymous"));               // not used anymore
            Assert.IsNull  (qaConfig.testUser("aaaaaa")); 
            Assert.IsNull  (qaConfig.testUser("")); 
            Assert.IsNull  (qaConfig.testUser(null)); 
        }                        
    }
}
