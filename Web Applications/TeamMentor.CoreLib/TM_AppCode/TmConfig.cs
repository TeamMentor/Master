using System;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib    
{
    //public values 
    public partial class TMConfig
    {
        public TMSetup_Config               TMSetup                     { get; set; }
        public TMSecurity_Config			TMSecurity				    { get; set; }        
        public WindowsAuthentication_Config WindowsAuthentication		{ get; set; }
        public Git_Config			        Git				            { get; set; }
        public OnInstallation_Config		OnInstallation				{ get; set; }

        public class TMSetup_Config
        {
            public bool			UseAppDataFolder			{ get; set; }
            public string       TMLibraryDataVirtualPath    { get; set; }
            public string 		XmlLibrariesPath 	        { get; set; }
            public string 		UserDataPath 	            { get; set; }		                
            public string 		LibrariesUploadedFiles	    { get; set; }	                           
            public bool         EnableGZipForWebServices	{ get; set; }
            public bool         Enable302Redirects			{ get; set; }			
        }

        public class TMSecurity_Config
        {
            public bool 		Show_ContentToAnonymousUsers { get; set; }
            public bool         SSL_RedirectHttpToHttps      { get; set; }
            public bool         EvalAccounts_Enabled         { get; set; }
            public int          EvalAccounts_Days            { get; set; }
            public bool         REST_AllowCrossDomainAccess  { get; set; }
            public bool         SingleSignOn_Enabled         { get; set; }            
            public bool 		Sanitize_HtmlContent         { get; set; }            
            public string 		Default_AdminUserName        { get; set; }
            public string 		Default_AdminPassword        { get; set; }	
            public string 		Default_AdminEmail           { get; set; }	            
        }

        public class WindowsAuthentication_Config
        {				
            public bool		    Enabled		                { get; set; }
            public string	    ReaderGroup                 { get; set; }
            public string	    EditorGroup                 { get; set; }
            public string	    AdminGroup	                { get; set; }
        }        

        public class Git_Config
        {
            public bool         AutoCommit_UserData         { get; set; }
            public bool         AutoCommit_LibraryData      { get; set; }         // not implemented in 3.3
        }
        public class OnInstallation_Config
        {
            public bool         ForceAdminPasswordReset			    { get; set; }
            public string       DefaultLibraryToInstall_Name		{ get; set; }
            public string       DefaultLibraryToInstall_Location	{ get; set; }
        }        
    }
    
    
    //load and save functionality
    public partial class TMConfig
    {	
        private static string		_baseFolder;
        private static TMConfig		_current;

        public static string        BaseFolder      
        { 
            get {
                    if (_baseFolder.isNull())
                        _baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                    return _baseFolder;
                } 
                
            set {
                    _baseFolder = value;
                }
        }
        public static String        AppData_Folder  
        {
            get { return BaseFolder.pathCombine("App_Data"); }
        }
        public static string        Location	    
        {
            get
            {				
                return BaseFolder.pathCombine("TMConfig.config");
            }
        }				        
        public static TMConfig      Current         
        { 
            get
            {					                
                if (_current.isNull())
                    loadConfig();
                return _current;
            }
            set { _current = value; }
        }
        public static TMConfig      loadConfig()    
        {             
            if (Location.fileExists())
                _current = Location.load<TMConfig>();
            else
            {
             //   "In TMConfig.loadConfig, provided location was not found(returning default object): {0}".debug(Location);
                _current = new TMConfig();
                _current.SaveTMConfig();
            }
            return _current;
        }

        public TMConfig()
        {
            this.setDefaultValues();
        }				
                                                
        public bool SaveTMConfig()
        {
            if (Location.valid())
                return this.saveAs(Location);
            return false;
        }
    }

    public static class TmConfig_ExtensionMethods
    {		
        public static TMConfig  setDefaultValues    (this TMConfig tmConfig)
        {
            tmConfig.TMSetup = new TMConfig.TMSetup_Config
                {
                    TMLibraryDataVirtualPath    = "..\\..",
                    XmlLibrariesPath            = "TM_Libraries",
                    UserDataPath                = "User_Data",
                    LibrariesUploadedFiles      = "LibrariesUploadedFiles",
                    Enable302Redirects          = true,
                    EnableGZipForWebServices    = true
                };

            tmConfig.TMSecurity = new TMConfig.TMSecurity_Config
                {
                    Show_ContentToAnonymousUsers = false,
                    SSL_RedirectHttpToHttps      = true,
                    EvalAccounts_Enabled         = true,
                    EvalAccounts_Days            = 15,
                    Default_AdminUserName        = "admin",
                    Default_AdminPassword        = "!!tmadmin",
                    Default_AdminEmail           = "tm_alerts@securityinnovation.com"
                };
            

            tmConfig.WindowsAuthentication = new TMConfig.WindowsAuthentication_Config
                {
                    Enabled               = false,
                    ReaderGroup           = "TM_Reader",
                    EditorGroup           = "TM_Editor",
                    AdminGroup            = "TM_Admin"
                };            

            tmConfig.OnInstallation = new TMConfig.OnInstallation_Config
                {
                    ForceAdminPasswordReset          = false,
                    DefaultLibraryToInstall_Name     = "",
                    DefaultLibraryToInstall_Location = ""
                };
            tmConfig.Git            = new TMConfig.Git_Config
                {
                    AutoCommit_LibraryData          = true,
                    AutoCommit_UserData             = true
                };
            
            return tmConfig;	
        }
        public static string    virtualPathMapping  (this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.TMLibraryDataVirtualPath.valid())
                return tmConfig.TMSetup.TMLibraryDataVirtualPath;
            return TMConsts.VIRTUAL_PATH_MAPPING;
        }
        public static string    xmlDatabasePath     (this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.UseAppDataFolder)
                return TMConfig.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH);
            return tmConfig.rootDataFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
        }
        public static string    rootDataFolder      (this TMConfig tmConfig)
        {									
            //set xmlDatabasePath based on virtualPathMapping			
            var virtualPathMapping = tmConfig.virtualPathMapping();			
            var xmlDatabasePath = TMConfig.BaseFolder.pathCombine(virtualPathMapping).fullPath();

            //check if we can write to xmlDatabasePath (and default to App_Data if we can't write to provided direct)
            if (xmlDatabasePath.canNotWriteToPath())
                xmlDatabasePath = TMConfig.AppData_Folder; 

            return xmlDatabasePath;
        }
    }
    
    
}