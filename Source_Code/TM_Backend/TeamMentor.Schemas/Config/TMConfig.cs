
using System;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib    
{
    //public values 
    [Serializable]
    [XmlRoot(ElementName="TMConfig")]
    public class TMConfig
    {
        public static TMConfig              Current                     { get; set; }

        public TMSetup_Config               TMSetup                     { get; set; }
        public TMSecurity_Config			TMSecurity				    { get; set; }        
        public WindowsAuthentication_Config WindowsAuthentication		{ get; set; }       
        public OnInstallation_Config		OnInstallation				{ get; set; }
        public VirtualArticles_Config       VirtualArticles             { get; set; }


        /*static TMConfig()
        {
            Current = new TMConfig();
        }*/

        public TMConfig()
        {
            Current                 = this;

            TMSetup                 = new TMSetup_Config();
            TMSecurity              = new TMSecurity_Config();
            WindowsAuthentication   = new WindowsAuthentication_Config();
            OnInstallation          = new OnInstallation_Config();            
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
                Default_AdminUserName        = TMConsts.USERDATA_DEFAULT_ADMIN_USERNAME;
                Default_AdminPassword        = TMConsts.USERDATA_DEFAULT_ADMIN_PASSWORD;
                Default_AdminEmail           = TMConsts.USERDATA_DEFAULT_ADMIN_EMAIL;
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
}