using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluenSharp;
using TeamMentor.CoreLib.WebServices;

namespace SecurityInnovation.TeamMentor.WebClient
{
	//public values 
	public partial class TMConfig
	{
		public bool			UseAppDataFolder			{ get; set; }
        public string       TMLibraryDataVirtualPath    { get; set; }
		public string 		XmlLibrariesPath 	        { get; set; }		
		public string 		DefaultAdminUserName        { get; set; }
		public string 		DefaultAdminPassword        { get; set; }	
		public string 		GitHubPassword		        { get; set; }	
		public string 		LibrariesUploadedFiles	    { get; set; }	
		public bool 		ShowContentToAnonymousUsers { get; set; }
        public bool         SSL_RedirectHttpToHttps     { get; set; }
        public bool 		SanitizeHtmlContent         { get; set; }
        public bool         SingleSignOn_Enabled        { get; set; }
		
		
        public WindowsAuthentication_Config WindowsAuthentication			{ get; set; }
		public TMDebugAndDev_Config			TMDebugAndDev					{ get; set; }
		public OnInstallation_Config		OnInstallation { get; set; }

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
	}
	
	
	//load and save functionality
    public partial class TMConfig
	{	
		static string		_baseFolder;
		static TMConfig		_current;
		
		public static string BaseFolder 
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

		public static String AppData_Folder 
		{
			get { return BaseFolder.pathCombine("App_Data"); }
		}

	    public static string Location	
		{
			get
			{				
				return BaseFolder.pathCombine("TMConfig.config");
			}
		}				
		
		public static TMConfig Current 
		{ 
			get
			{					
			    //return TMConfig.Location.load<TMConfig>() ?? new TMConfig();                
				if (_current.isNull())
					loadConfig();
				return _current;
			}
		}

        public static TMConfig loadConfig()
        {
			if (TMConfig.Location.fileExists())
				_current = TMConfig.Location.load<TMConfig>();
			else
			{
				"In TMConfig.loadConfig, provided location was not found(returning default object): {0}".debug(TMConfig.Location);
				_current = new TMConfig();
			}
            return _current;
        }

        private TMConfig ensureDefaultValues()
        {
            throw new NotImplementedException();
        }
		
		public TMConfig()
		{
			this.setDefaultValues();
		}				
								
		public bool SaveTMConfig()
		{
			return this.saveAs(TMConfig.Location);			
		}
	}

	public static class TmConfig_ExtensionMethods
	{		
		public static TMConfig setDefaultValues(this TMConfig tmConfig)
		{									
            tmConfig.TMLibraryDataVirtualPath = "..\\..";
			tmConfig.XmlLibrariesPath = "TM_Library";
			tmConfig.DefaultAdminUserName = "admin";
			tmConfig.DefaultAdminPassword = "!!tmbeta";
            tmConfig.LibrariesUploadedFiles = "LibrariesUploadedFiles";

			tmConfig.WindowsAuthentication = new TMConfig.WindowsAuthentication_Config()
				{
					Enabled = false,
					ReaderGroup = "TM_Reader",
					EditorGroup = "TM_Editor",
					AdminGroup = "TM_Admin"
				};
			tmConfig.TMDebugAndDev = new TMConfig.TMDebugAndDev_Config()
				{
					Enable302Redirects = true,
					EnableGZipForWebServices = true
				};
			tmConfig.OnInstallation = new TMConfig.OnInstallation_Config()
				{
					ForceAdminPasswordReset = true,
					DefaultLibraryToInstall_Name = "",
					DefaultLibraryToInstall_Location = ""
				};
			return tmConfig;	
		}

		public static string virtualPathMapping(this TMConfig tmConfig)
		{
			if (tmConfig.TMLibraryDataVirtualPath.valid())
				return tmConfig.TMLibraryDataVirtualPath;
			return TMConsts.VIRTUAL_PATH_MAPPING;
		}

		public static string xmlDatabasePath(this TMConfig tmConfig)
		{
			if (tmConfig.UseAppDataFolder)
				return TMConfig.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH);
			return tmConfig.rootDataFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
		}

		public static string rootDataFolder(this TMConfig tmConfig)
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