using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git.APIs;
using urn.microsoft.guidanceexplorer;
using System.Threading;

namespace TeamMentor.CoreLib
{	
    public partial class TM_Xml_Database 
    {		    
        public static TM_Xml_Database   Current               { get; set; }         
        public static bool              SkipServerOnlineCheck { get; set; }        
        
        //public object			        setupLock             = new object();
        public bool			            UsingFileStorage	  { get; set; }         //config                   
        public bool                     ServerOnline          { get; set; }         
        public TM_Server                TM_Server_Config      { get; set; }         
        public TM_UserData              UserData              { get; set; }         //users and tracking             
        public List<API_NGit>           NGits                 { get; set; }         // Git object, one per library that has git support
        public string 	                Path_XmlDatabase      { get; set; }					
        public string 	                Path_XmlLibraries 	  { get; set; }    
//        public Thread                   SetupThread           { get; set; } 

        public Dictionary<Guid, guidanceExplorer>	    GuidanceExplorers_XmlFormat { get; set; }	 //Xml Library and Articles   
        public Dictionary<guidanceExplorer, string>	    GuidanceExplorers_Paths     { get; set; }	 
        public Dictionary<Guid, string>				    GuidanceItems_FileMappings	{ get; set; }			
        public Dictionary<Guid, TeamMentor_Article>	    Cached_GuidanceItems		{ get; set; }
        public Dictionary<Guid, VirtualArticleAction>   VirtualArticles			    { get; set; }
                                                            
        
        
        public TM_Xml_Database          () : this(false)                    // defaults to creating a TM_Instance in memory    
        {
        }
        [Admin]
        public TM_Xml_Database(bool useFileStorage)
        {
            if (TM_Status.In_Setup_XmlDatabase)
                throw new Exception("TM Exeption: TM_Xml_Database ctor was called twice in a row (without the first ctor sequence had ended)");
            
            TM_Status.In_Setup_XmlDatabase = true;

            Current = this;                                                       
            UsingFileStorage = useFileStorage;                
            Setup();

            TM_Status.In_Setup_XmlDatabase = false;
        }

        [Admin] public TM_Xml_Database ResetDatabase()
        {
            NGits                       = new List<API_NGit>();
            Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();
            GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();
            GuidanceExplorers_Paths     = new Dictionary<guidanceExplorer, string>();
            TM_Server_Config            = new TM_Server();
            UserData                    = new TM_UserData(UsingFileStorage); 
            VirtualArticles             = new Dictionary<Guid, VirtualArticleAction>();
            return this;
        }

/*        [Admin] public TM_Xml_Database  Setup()
        {
            SetupThread = O2Thread.mtaThread(
                ()=>{
                        lock (this)
                        {    
                            TM_Setup_Thread();                            
                            SetupThread = null;
                        }                        
                });
            return this;
        }
        public void TM_Setup_Thread()*/
        [Admin]
        public TM_Xml_Database Setup()
        {            
            try
            {
                ResetDatabase();

                this.set_Path_XmlDatabase();

                this.load_TMServer_Config();

                if (UsingFileStorage)
                {
                    SetPaths_UserData();                    
                }
                
                UserData.SetUp();
                Logger_Firebase.createAndHook();
                "[TM_Xml_Database] TM is Starting up".info();
                this.logTBotActivity("TM Xml Database", "TM is (re)starting and user Data is now loaded");
                this.userData().copy_FilesIntoWebRoot();
                if (UsingFileStorage)
                {
                    this.SetPaths_XmlDatabase();
                    this.handle_UserData_GitLibraries();
                    loadDataIntoMemory();
                    this.logTBotActivity("TM Xml Database", "Library Data is loaded");
                }
                UserData.createDefaultAdminUser();  // make sure the admin user exists and is configured
                this.logTBotActivity("TM Xml Database", "TM started at: {0}".format(DateTime.Now));
            }
            catch (Exception ex)
            {
                "[TM_Xml_Database] Setup: {0} \n\n".error(ex.Message, ex.StackTrace);
                if (TM_StartUp.Current.notNull())                       //will happen when TM_Xml_Database ctor is called by an user with no admin privs
                    TM_StartUp.Current.TrackingApplication.saveLog();
            }
            return this;
        }
                
        [Admin] public void             SetPaths_UserData()
        {
            try
            {
                var tmConfig        = TMConfig.Current;
                var userDataPath    = tmConfig.TMSetup.UserDataPath;
                var xmlDatabasePath = tmConfig.xmlDatabasePath();

                "[TM_Xml_Database] [setDataFromCurrentScript] TMConfig.Current.UserDataPath: {0}".debug(userDataPath);

                if (userDataPath.dirExists().isFalse())
                {
                    userDataPath     = xmlDatabasePath.pathCombine(userDataPath);
                    userDataPath.createDir(); // make sure it exists
                    Path_XmlDatabase = xmlDatabasePath;
                }
                UserData.Path_UserData      = userDataPath;   
                UserData.Path_UserData_Base = userDataPath;   // we need to keep an copy of this since the Path_UserData might change with git usage                
            }        
            catch(Exception ex)
            {
                "[TM_Xml_Database] [SetPaths_UserData] {0} \n\n {1}".error(ex.Message, ex.StackTrace);
            }
        }
           
        [Admin] public string           ReloadData()
        {
            return ReloadData(null);
        }				    
        [Admin] public string           ReloadData(string newLibraryPath)
        {	
            "In Reload data".info();
            this.clear_GuidanceItemsCache();                            // start by clearing the cache
            if (newLibraryPath.notNull())                               // check if we are changing the library path
            {
                var tmConfig = TMConfig.Current;
                tmConfig.TMSetup.XmlLibrariesPath = newLibraryPath;
                tmConfig.SaveTMConfig();			
            }
                                   
            Setup();                                                    // trigger the set (which will load all data
          //  this.setupThread_WaitForComplete();

            var stats = "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems"                        
                            .format(Current.Path_XmlLibraries.directoryName(), 
                                    this.tmLibraries().size(), 
                                    this.tmViews().size(),
                                    this.tmGuidanceItems().size());
            return stats;                                               // return some stats
        }        
    }
}
