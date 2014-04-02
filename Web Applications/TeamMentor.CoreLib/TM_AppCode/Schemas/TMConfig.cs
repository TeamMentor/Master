using System;
using FluentSharp.CoreLib;

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
            public bool			OnlyLoadUserData			{ get; set; }
            public string       TMLibraryDataVirtualPath    { get; set; }
            public string 		XmlLibrariesPath 	        { get; set; }
            public string 		UserDataPath 	            { get; set; }		                
            public string 		LibrariesUploadedFiles	    { get; set; }	                           
            public bool         EnableGZipForWebServices	{ get; set; }
            public bool         Enable304Redirects			{ get; set; }

            public TMSetup_Config()
            {
                TMLibraryDataVirtualPath    = "..\\..";
                XmlLibrariesPath            = "TM_Libraries";
                UserDataPath                = "User_Data";
                LibrariesUploadedFiles      = "LibrariesUploadedFiles";
                Enable304Redirects          = true;
                EnableGZipForWebServices    = true;
            }
        }
        public class TMSecurity_Config
        {
            public bool 		Show_ContentToAnonymousUsers { get; set; }
            public bool         SSL_RedirectHttpToHttps      { get; set; }
            public bool         NewAccounts_Enabled          { get; set; }
            public bool         EvalAccounts_Enabled         { get; set; }
            public int          EvalAccounts_Days            { get; set; }
            public bool         REST_AllowCrossDomainAccess  { get; set; }
            public bool         SingleSignOn_Enabled         { get; set; }            
            public bool 		Sanitize_HtmlContent         { get; set; }            
            public string 		Default_AdminUserName        { get; set; }
            public string 		Default_AdminPassword        { get; set; }	
            public string 		Default_AdminEmail           { get; set; }

            public TMSecurity_Config()                                          //need to do this here so that they survive serialization
            {
                Show_ContentToAnonymousUsers = false;               
                SSL_RedirectHttpToHttps      = true;
                EvalAccounts_Enabled         = true;
                NewAccounts_Enabled          = true;
                EvalAccounts_Days            = 15;
                Default_AdminUserName        = "admin";
                Default_AdminPassword        = "!!tmadmin";
                Default_AdminEmail           = "TM_alerts@securityinnovation.com";
            }
        }

        public class WindowsAuthentication_Config
        {				
            public bool		    Enabled		                { get; set; }
            public string	    ReaderGroup                 { get; set; }
            public string	    EditorGroup                 { get; set; }
            public string	    AdminGroup	                { get; set; }

            public WindowsAuthentication_Config()
            {
                Enabled               = false;
                ReaderGroup           = "TM_Reader";
                EditorGroup           = "TM_Editor";
                AdminGroup            = "TM_Admin";
            }
        }        

        public class Git_Config
        {
            public bool         AutoCommit_UserData         { get; set; }
            public bool         AutoCommit_LibraryData      { get; set; }         

            public Git_Config()
            {
                AutoCommit_LibraryData          = false;            // (disabled by default) will only work if the library is a git repository
                AutoCommit_UserData             = false;            // (disabled by default)  should always trigger, since the UserData folder repo is created if not there before
            }
        }
        public class OnInstallation_Config
        {
            public bool         ForceAdminPasswordReset			    { get; set; }
            public string       DefaultLibraryToInstall_Name		{ get; set; }
            public string       DefaultLibraryToInstall_Location	{ get; set; }

            public OnInstallation_Config()
            {
                ForceAdminPasswordReset          = false;
                DefaultLibraryToInstall_Name     = "";
                DefaultLibraryToInstall_Location = "";
            }
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

        public static bool      setCurrent(TMConfig tmConfig)
        {
            Current = tmConfig;
            return Current.SaveTMConfig();    
        }

        public static TMConfig      loadConfig()    
        {             
            if (Location.fileExists())
                _current = Location.load<TMConfig>();
            else
            {             
                _current = new TMConfig();
                _current.SaveTMConfig();
            }
            return _current;
        }

        public TMConfig()
        {            
            TMSetup                 = new TMSetup_Config();
            TMSecurity              = new TMSecurity_Config();
            WindowsAuthentication   = new WindowsAuthentication_Config();
            OnInstallation          = new OnInstallation_Config();
            Git                     = new Git_Config();
        }
                                                
        public bool SaveTMConfig()
        {
            if (Location.valid())
                return this.saveAs(Location);
            return false;
        }
    }
}