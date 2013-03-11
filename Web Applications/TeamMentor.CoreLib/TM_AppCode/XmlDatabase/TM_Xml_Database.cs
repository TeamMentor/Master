using System;
using System.Collections.Generic;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{	
    public partial class TM_Xml_Database 
    {		    
        public static TM_Xml_Database   Current               { get; set; }         
        public static bool              SkipServerOnlineCheck { get; set; }        

        public bool			            UsingFileStorage	  { get; set; }         //config   
        public bool                     ServerOnline          { get; set; }         
        public bool                     AutoGitCommit         { get; set; }                
        public TM_UserData              UserData              { get; set; }         //users and tracking             
        public API_NGit                 NGit                  { get; set; }         // Git object
        public string 	                Path_XmlDatabase      { get; set; }					
        public string 	                Path_XmlLibraries 	  { get; set; }    
         
        public Dictionary<Guid, guidanceExplorer>	    GuidanceExplorers_XmlFormat { get; set; }	 //Xml Library and Articles   
        public Dictionary<Guid, string>				    GuidanceItems_FileMappings	{ get; set; }			
        public Dictionary<Guid, TeamMentor_Article>	    Cached_GuidanceItems		{ get; set; }
        public Dictionary<Guid, VirtualArticleAction>   VirtualArticles			    { get; set; }
                                                            
        

        [Log("TM_Xml_Database Setup")]        
        public TM_Xml_Database          () : this(false)                    // defaults to creating a TM_Instance in memory    
        {
        }
        public TM_Xml_Database          (bool useFileStorage)
        {
            O2Thread.mtaThread(CheckIfServerIsOnline);
            UsingFileStorage = useFileStorage;
            Current = this;
            Setup();
        }

        [Admin] public TM_Xml_Database ResetDatabase()
        {
            Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();
            GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();
            UserData                    = new TM_UserData(UsingFileStorage);            

            return this;
        }

        [Admin] public TM_Xml_Database  Setup()
        {                       
            try
            {
                ResetDatabase();
                if (UsingFileStorage)
                {
                    SetPathsAndloadData();                    
                    this.handleDefaultInstallActions();
                   // this.xmlDB_Load_GuidanceItems();
                    
                }
                UserData.SetUp();                
            }
            catch(Exception ex)
            {
                "[TM_Xml_Database] .ctor: {0} \n\n".error(ex.Message, ex.StackTrace);
            }
            return this;
        } 
        [Admin] public void             CheckIfServerIsOnline()
        {
            if (SkipServerOnlineCheck)
                return;
            ServerOnline = MiscUtils.online();              // only check this once
        }
        [Admin] public void             SetPathsAndloadData()
        {            
            try
            {
                var tmComfig            = TMConfig.Current;
                var xmlDatabasePath     = tmComfig.xmlDatabasePath();
                var xmlLibraryPath      = tmComfig.TMSetup.XmlLibrariesPath;
                var userDataPath        = tmComfig.TMSetup.UserDataPath;

                AutoGitCommit = tmComfig.Git.AutoCommit_LibraryData;

                setLibraryPath_and_LoadDataIntoMemory(xmlDatabasePath, xmlLibraryPath, userDataPath);

                "[TM_Xml_Database][setDataFromCurrentScript] TM_Xml_Database.Path_XmlDatabase: {0}" .debug(xmlDatabasePath);
                "[TM_Xml_Database][setDataFromCurrentScript] TMConfig.Current.XmlLibrariesPath: {0}".debug(xmlLibraryPath);
                "[TM_Xml_Database][setDataFromCurrentScript] TMConfig.Current.UserDataPath: {0}"    .debug(userDataPath);
                TM_Xml_Database_Load_and_FileCache_Utils.populateGuidanceItemsFileMappings();	//only do this once
            }
            catch(Exception ex)
            {
                "[TM_Xml_Database] SetPathsAndloadData .ctor: {0} \n\n {1}".error(ex.Message, ex.StackTrace);
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

            var stats = "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems"                        
                            .format(Current.Path_XmlLibraries.directoryName(), 
                                    this.tmLibraries().size(), 
                                    this.tmViews().size(),
                                    this.tmGuidanceItems().size());
            return stats;                                               // return some stats
        }        
    }

    public static class TM_Xml_Database_ExtensionMethods
    {
        public static TM_UserData userData(this TM_Xml_Database tmDatabase)
        {
            return tmDatabase.notNull()
                       ? tmDatabase.UserData
                       : null;
        }
    }
}
