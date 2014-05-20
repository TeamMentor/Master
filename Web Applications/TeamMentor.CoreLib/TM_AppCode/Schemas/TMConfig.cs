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
        public VirtualArticles_Config       VirtualArticles             { get; set; }

        public TMConfig()
        {            
            TMSetup                 = new TMSetup_Config();
            TMSecurity              = new TMSecurity_Config();
            WindowsAuthentication   = new WindowsAuthentication_Config();
            OnInstallation          = new OnInstallation_Config();
            Git                     = new Git_Config();
            VirtualArticles         = new VirtualArticles_Config();
        }

        public class TMSetup_Config
        {
            public bool			UseAppDataFolder			{ get; set; }           
            public string       TMLibraryDataVirtualPath    { get; set; }
            public string 		XmlLibrariesPath 	        { get; set; }
            public string 		UserDataPath 	            { get; set; }		                
            public string 		LibrariesUploadedFiles	    { get; set; }	                           
            public bool         EnableGZipForWebServices	{ get; set; }
            public bool         Enable304Redirects			{ get; set; }
            public bool         ShowDotNetDebugErrors	    { get; set; }

            public TMSetup_Config()
            {
                TMLibraryDataVirtualPath    = "..\\..";
                XmlLibrariesPath            = "TM_Libraries";
                UserDataPath                = "User_Data";
                LibrariesUploadedFiles      = "LibrariesUploadedFiles";
                Enable304Redirects          = true;
                EnableGZipForWebServices    = true;                  
                ShowDotNetDebugErrors       = false;
            }
        }
        public class TMSecurity_Config
        {
            public bool 		Show_ContentToAnonymousUsers { get; set; }
            public bool 		Show_LibraryToAnonymousUsers { get; set; }
            public bool         SSL_RedirectHttpToHttps      { get; set; }
            public bool         NewAccounts_Enabled          { get; set; }
            public bool         NewAccounts_DontExpire       { get; set; }            
            public bool         EvalAccounts_Enabled         { get; set; }
            public int          EvalAccounts_Days            { get; set; }
            public bool         REST_AllowCrossDomainAccess  { get; set; }                       
            public bool 		Sanitize_HtmlContent         { get; set; }            
            public string 		Default_AdminUserName        { get; set; }
            public string 		Default_AdminPassword        { get; set; }	
            public string 		Default_AdminEmail           { get; set; }
            public bool 		EmailAdmin_On_NewUsers       { get; set; }

            public TMSecurity_Config()                                          //need to do this here so that they survive serialization
            {
                Show_LibraryToAnonymousUsers = true;
                Show_ContentToAnonymousUsers = false;               
                SSL_RedirectHttpToHttps      = true;
                EvalAccounts_Enabled         = true;
                NewAccounts_Enabled          = true;
                EvalAccounts_Days            = 15;
                Default_AdminUserName        = "admin";
                Default_AdminPassword        = "!!tmadmin";
                Default_AdminEmail           = "TM_alerts@securityinnovation.com";
                EmailAdmin_On_NewUsers       = true; 
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
            public bool         UserData_Git_Enabled         { get; set; }
            public bool         UserData_Auto_Pull           { get; set; }
            public bool         UserData_Auto_Push           { get; set; }
            public bool         LibraryData_Git_Enabled      { get; set; }
            public bool         LibraryData_Auto_Pull        { get; set; }
            public bool         LibraryData_Auto_Push        { get; set; }   

            public Git_Config()
            {
                LibraryData_Git_Enabled          = true;                 // all user and library data should be controled by Git
                UserData_Git_Enabled             = true;            
                LibraryData_Auto_Pull            = true;                 // pull is automatic (or changed on TMConfig file                
                UserData_Auto_Pull               = true;               
                LibraryData_Auto_Push            = false;                // push must be set on the TM config
                UserData_Auto_Push               = false;
            }
        }
        public class OnInstallation_Config
        {
            public bool         ForceDefaultAdminPassword			    { get; set; }
            public string       DefaultLibraryToInstall_Name		{ get; set; }
            public string       DefaultLibraryToInstall_Location	{ get; set; }

            public OnInstallation_Config()
            {
                ForceDefaultAdminPassword          = false;
                DefaultLibraryToInstall_Name     = "";
                DefaultLibraryToInstall_Location = "";
            }
        }  
        public class VirtualArticles_Config
        { 
            public bool         AutoRedirectIfGuidNotFound          { get; set; }
            public string       AutoRedirectTarget                  { get; set; }

            public VirtualArticles_Config()
              {
                AutoRedirectIfGuidNotFound = false;
                AutoRedirectTarget         = "https://teammentor.net/article/";
            }
        }
    }
    
    
    //with static vars load and save 
    public partial class TMConfig
    {
        public static TMConfig Current          { get; set; }
        public static string   Location         { get; set; }
        public static string   WebRoot          { get; set; }
        public static string   AppData_Folder   { get; set; }

        static TMConfig()
        {
            Current         = new TMConfig();
            WebRoot         = AppDomain.CurrentDomain.BaseDirectory;
            AppData_Folder  = WebRoot.pathCombine("App_Data");
        }

        public TMConfig      reloadConfig()
        {
            return Current = Location.fileExists() 
                                ? Location.load<TMConfig>() 
                                : new TMConfig();            
        }

        public bool SaveTMConfig()
        {
            return Location.valid() && this.saveAs(Location);
        }

        public static bool      setCurrent(TMConfig tmConfig)
        {
            if (tmConfig.isNull())
                return false;
            Current = tmConfig;
            return Current.SaveTMConfig();    
        }

    }
}