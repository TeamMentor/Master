using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace SecurityInnovation.TeamMentor.WebClient
{
	//public values 
	public partial class TMConfig
	{			
		public string 		WebSite_Port 		        { get; set; }
		public string 		WebSite_IP 			        { get; set; }
		public string 		XmlLibrariesPath 	        { get; set; }
		public List<string> Libraries_Disabled 	        { get; set; }
		public string 		DefaultAdminUserName        { get; set; }
		public string 		DefaultAdminPassword        { get; set; }	
		public string 		GitHubPassword		        { get; set; }	
		public string 		LibrariesUploadedFiles	    { get; set; }	
		public bool 		ShowContentToAnonymousUsers { get; set; }	
        public bool 		SanitizeHtmlContent         { get; set; }	        		
        public WindowsAuthentication_Config WindowsAuthentication			{ get; set; }

		public class WindowsAuthentication_Config
		{				
			public bool		Enabled		{ get; set; }
			public string	ReaderGroup { get; set; }
			public string	EditorGroup { get; set; }
			public string	AdminGroup	{ get; set; }
		}

	}
	
	
	//load and save functionality
    public partial class TMConfig
	{	
		public static string _baseFolder;
		
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
		
		public static string Location	
		{
			get
			{				
				return BaseFolder.pathCombine("TMConfig.config");
			}
		}
		public static TMConfig _current;
		
		public static TMConfig Current 
		{ 
			get
			{					
			    //return TMConfig.Location.load<TMConfig>() ?? new TMConfig();                
				if (_current.isNull())
					_current =  TMConfig.Location.load<TMConfig>();
				return _current;
			}
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
			tmConfig.Libraries_Disabled = new List<string>();				
			tmConfig.WebSite_IP = "localhost";
			tmConfig.WebSite_Port = "12345";
			tmConfig.XmlLibrariesPath = "TM_Library";
			tmConfig.DefaultAdminUserName = "admin";
			tmConfig.DefaultAdminPassword = "!!tmbeta";
            tmConfig.LibrariesUploadedFiles = "LibrariesUploadedFiles";

            tmConfig.WindowsAuthentication	= new TMConfig.WindowsAuthentication_Config();
			tmConfig.WindowsAuthentication.Enabled = false;
			tmConfig.WindowsAuthentication.ReaderGroup = "TM_Reader";
			tmConfig.WindowsAuthentication.EditorGroup = "TM_Editor";
			tmConfig.WindowsAuthentication.AdminGroup  = "TM_Admin";

			return tmConfig;
		}
	}
	
	
}