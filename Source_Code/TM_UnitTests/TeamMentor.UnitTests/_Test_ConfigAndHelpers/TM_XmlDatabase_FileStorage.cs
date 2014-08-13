using FluentSharp.CoreLib.API;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests
{    
    public class TM_XmlDatabase_FileStorage 
    {        
        public TM_FileStorage   tmFileStorage;
        public TM_UserData      userData;
        public TM_Xml_Database  tmXmlDatabase;    
        public TMConfig         tmConfig;    
        public TM_Server        tmServer;
        
        public TM_XmlDatabase_FileStorage()
        {
            SendEmails.Disable_EmailEngine = true;

            UserGroup.Admin.assert();
            tmFileStorage   = new TM_FileStorage();
            tmXmlDatabase   = tmFileStorage.TMXmlDatabase; //new TM_Xml_Database().setup();
            userData        = tmFileStorage.UserData;
            tmServer        = tmFileStorage.Server;
            tmConfig        = TMConfig.Current;

            CheckDatabaseSetup(); 
            UserGroup.None.assert();
        }

        [Assert_Admin]
        public void CheckDatabaseSetup()
        {           
            UserGroup.Admin.assert(); 
            
            Assert.NotNull(tmXmlDatabase);
            Assert.NotNull(userData);
            Assert.NotNull(tmServer);
            Assert.NotNull(tmConfig);
 
            if(tmXmlDatabase.tmLibraries().notEmpty())      // temp while refactoring
            { 
                tmFileStorage.delete_Database();                        
            }
            if(tmFileStorage.getCacheLocation().fileExists())
                tmFileStorage.clear_GuidanceItemsCache();

            if(userData.tmUsers().notEmpty())
                userData.delete_All_Users();
          //  userData.createDefaultAdminUser(); 
            
            Assert.NotNull(tmFileStorage.path_XmlDatabase()		    , "path_XmlDatabase()");        // null since we are running TM memory (default setting)
            Assert.NotNull(tmFileStorage.path_XmlLibraries()		, "Path_XmlLibraries");         // null since we are running TM memory (default setting)
            Assert.IsEmpty(tmXmlDatabase.Cached_GuidanceItems	    , "Cached_GuidanceItems");
            Assert.IsEmpty (userData.validSessions()                , "ActiveSessions");
            Assert.AreEqual(userData.TMUsers.size()              ,0 , "TMUsers");	                // there should be no users
            
            UserGroup.None.assert(); 
        }
    }
}
