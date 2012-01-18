using System;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

namespace SecurityInnovation.TeamMentor.WebClient
{
	//public values 
	public partial class TMConfig
	{	
		public string 		Version 			 { get; set; }		
		public string 		WebSite_Port 		 { get; set; }
		public string 		WebSite_IP 			 { get; set; }
		public string 		XmlLibrariesPath 	 { get; set; }
		public List<string> Libraries_Disabled 	 { get; set; }
		public string 		DefaultAdminUserName { get; set; }
		public string 		DefaultAdminPassword { get; set; }	
		public string 		GitHubPassword		 { get; set; }	
		public string 		LibrariesZipsFolder	 { get; set; }	
		public bool 		ShowContentToAnonymousUsers { get; set; }	
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
				return TMConfig.Location.load<TMConfig>();				
				/*if (_current.isNull())
					_current =  TMConfig.Location.load<TMConfig>();
				return _current;*/
			}
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
			tmConfig.Version = "TM v3.0 Beta";	
			tmConfig.WebSite_IP = "localhost";
			tmConfig.WebSite_Port = "12345";
			tmConfig.XmlLibrariesPath = "TM_Library";
			tmConfig.DefaultAdminUserName = "admin";
			tmConfig.DefaultAdminPassword = "!!tmbeta";
			return tmConfig;
		}
	}
	
	
}