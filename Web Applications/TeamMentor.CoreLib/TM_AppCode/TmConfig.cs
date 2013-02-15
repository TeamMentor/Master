using System;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib    
{
    //public values 
    public partial class TMConfig
    {
        public bool			UseAppDataFolder			{ get; set; }
        public string       TMLibraryDataVirtualPath    { get; set; }
        public string 		XmlLibrariesPath 	        { get; set; }
        public string 		UserDataPath 	            { get; set; }		
        public string 		DefaultAdminUserName        { get; set; }
        public string 		DefaultAdminPassword        { get; set; }	
        public string 		GitHubPassword		        { get; set; }	
        public string 		LibrariesUploadedFiles	    { get; set; }	
        public bool 		ShowContentToAnonymousUsers { get; set; }
        public bool         SSL_RedirectHttpToHttps     { get; set; }
        public bool 		SanitizeHtmlContent         { get; set; }
        public bool         SingleSignOn_Enabled        { get; set; }
        
        
        public REST_Config					REST						{ get; set; }
        public OnInstallation_Config		OnInstallation				{ get; set; }
        public WindowsAuthentication_Config WindowsAuthentication		{ get; set; }						
        public TMDebugAndDev_Config			TMDebugAndDev				{ get; set; }
        public Eval_Accounts_Config			Eval_Accounts				{ get; set; }
        

        public class WindowsAuthentication_Config
        {				
            public bool		Enabled		{ get; set; }
            public string	ReaderGroup { get; set; }
            public string	EditorGroup { get; set; }
            public string	AdminGroup	{ get; set; }
        }

        public class TMDebugAndDev_Config
        {
            public bool EnableGZipForWebServices			{ get; set; }
            public bool Enable302Redirects					{ get; set; }			
        }

        public class OnInstallation_Config
        {
            public bool   ForceAdminPasswordReset			{ get; set; }
            public string DefaultLibraryToInstall_Name		{ get; set; }
            public string DefaultLibraryToInstall_Location	{ get; set; }
        }
        public class REST_Config
        {
            public bool AllowCrossDomainAccess { get; set; }			
        }

        public class Eval_Accounts_Config
        {
            public bool Enabled { get; set; }
            public int  Days { get; set; }
        }
    }
    
    
    //load and save functionality
    public partial class TMConfig
    {	
        private static string		_baseFolder;
        private static TMConfig		_current;

        public static string    BaseFolder 
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
        public static String    AppData_Folder 
        {
            get { return BaseFolder.pathCombine("App_Data"); }
        }
        public static string    Location	
        {
            get
            {				
                return BaseFolder.pathCombine("TMConfig.config");
            }
        }				        
        public static TMConfig  Current 
        { 
            get
            {					                
                if (_current.isNull())
                    loadConfig();
                return _current;
            }
            set { _current = value; }
        }
        public static TMConfig  loadConfig()
        {            
            if (Location.fileExists())
                _current = Location.load<TMConfig>();
            else
            {
                "In TMConfig.loadConfig, provided location was not found(returning default object): {0}".debug(Location);
                _current = new TMConfig();
            }
            return _current;
        }

        public TMConfig()
        {
            this.setDefaultValues();
        }				
                                                
        public bool SaveTMConfig()
        {
            return this.saveAs(Location);			
        }
    }

    public static class TmConfig_ExtensionMethods
    {		
        public static TMConfig  setDefaultValues(this TMConfig tmConfig)
        {									
            tmConfig.TMLibraryDataVirtualPath   = "..\\..";
            tmConfig.XmlLibrariesPath           = "TM_Library";
            tmConfig.UserDataPath               = "User_Data";
            tmConfig.DefaultAdminUserName       = "admin";
            tmConfig.DefaultAdminPassword       = "!!tmadmin";
            tmConfig.LibrariesUploadedFiles     = "LibrariesUploadedFiles";

            tmConfig.WindowsAuthentication = new TMConfig.WindowsAuthentication_Config
                {
                    Enabled = false,
                    ReaderGroup = "TM_Reader",
                    EditorGroup = "TM_Editor",
                    AdminGroup = "TM_Admin"
                };
            tmConfig.TMDebugAndDev = new TMConfig.TMDebugAndDev_Config
                {
                    Enable302Redirects = true,
                    EnableGZipForWebServices = true
                };

            tmConfig.OnInstallation = new TMConfig.OnInstallation_Config
                {
                    ForceAdminPasswordReset = true,
                    DefaultLibraryToInstall_Name = "",
                    DefaultLibraryToInstall_Location = ""
                };

            tmConfig.REST			= new TMConfig.REST_Config();
            tmConfig.Eval_Accounts  = new TMConfig.Eval_Accounts_Config();

            return tmConfig;	
        }
        public static string    virtualPathMapping(this TMConfig tmConfig)
        {
            if (tmConfig.TMLibraryDataVirtualPath.valid())
                return tmConfig.TMLibraryDataVirtualPath;
            return TMConsts.VIRTUAL_PATH_MAPPING;
        }
        public static string    xmlDatabasePath(this TMConfig tmConfig)
        {
            if (tmConfig.UseAppDataFolder)
                return TMConfig.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH);
            return tmConfig.rootDataFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
        }
        public static string    rootDataFolder(this TMConfig tmConfig)
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