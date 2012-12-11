using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
//using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
//using Moq;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
using System.IO;
//O2File:../IJavascriptProxy.cs
//O2File:../UtilMethods.cs   
//O2File:../Schemas/library.cs 
//O2File:../Schemas/guidanceItem.cs
//O2File:../TMConfig.cs
//O2File:TM_Xml_Database.Library.cs
//O2File:TM_Xml_Database.Library.GuidanceExplorer.cs
//O2File:TM_Xml_Database.Library.GuidanceItem.cs
//O2File:TM_Xml_Database.Library.Load_and_FileCache.cs
//O2File:TM_Xml_Database.Library.Views_and_Folders.cs
//O2File:TM_Xml_Database.Users.cs
//O2File:../O2_Scripts_APIs/_O2_Scripts_Files.cs
//O2Ref:moq.dll
//O2Ref:System.dll
//O2Ref:System.Web.Abstractions.dll
//O2Ref:System.Web.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{	
	public partial class TM_Xml_Database 
	{
		public static TM_Xml_Database	Current				{ get; set;}
		
		//users
		public static List<TMUser> TMUsers { get; set; }
		public static O2.DotNetWrappers.DotNet.Items TMUsersPasswordHashes				{ get; set; }
		public static Dictionary<Guid, TMUser>				ActiveSessions				{ get; set; }		
		public static Dictionary<Guid, guidanceExplorer>	GuidanceExplorers_XmlFormat { get; set; }	
		public static Dictionary<Guid, string>				GuidanceItems_FileMappings	{ get; set; }			
        public static Dictionary<Guid, TeamMentor_Article>	Cached_GuidanceItems		{ get; set; }

		public static Dictionary<Guid, VirtualArticleAction> VirtualArticles			{ get; set; }
        
		
		//public static Dictionary<Guid, List<GuidanceItem_V3>> GuidanceItems_InViews { get; set; }
									
		public static string 	Path_XmlDatabase 		{ get; set; }					
		public static string 	Path_XmlLibraries 		{ get; set; }					
		//public string 		 	DatabasePath		{  get {	return TM_Xml_Database.XmlDatabasePath; } }
		//public string 		 	LibrariesPath		{  get {	return TM_Xml_Database.XmlLibrariesPath; } }
		public List<TM_Library> Libraries  				{  get { 	return this.tmLibraries(); } }
		public List<Folder_V3> 	Folders  				{  get { 	return this.tmFolders(); } } 		
		public List<View_V3> 	Views  					{  get { 	return this.tmViews(); } } 
		public List<TeamMentor_Article> GuidanceItems	{  get {	return this.tmGuidanceItems(); } } 
		        
		
		//public static string defaultLibrariesPath = ;
		 
		static TM_Xml_Database()
		{			
			Cached_GuidanceItems		= new Dictionary<Guid, TeamMentor_Article> ();            
			GuidanceItems_FileMappings	= new Dictionary<Guid,string>();
			GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();			

			TMUsers						= new List<TMUser>();
			TMUsersPasswordHashes		= new O2.DotNetWrappers.DotNet.Items ();
			ActiveSessions				= new Dictionary<Guid, TMUser>();

			setPathsAndloadData();

			TM_Xml_Database.Current		= new TM_Xml_Database();

			//TM_Xml_Database.Current.handleDefaultInstallActions();			
		} 
		
		public TM_Xml_Database()
		{	
			TM_Xml_Database.Current = this;
			try
			{							
				this.xmlDB_Load_GuidanceItems();
				this.createDefaultAdminUser();	// make sure this user exists						
			}
			catch(Exception ex)
			{
				"[TM_Xml_Database] .ctor: {0} \n\n".error(ex.Message, ex.StackTrace);
			}
		} 
		/// <summary>
		/// Executed once on AppDomain start
		/// </summary>
		public static void setPathsAndloadData()
		{            
			try
			{								
				var xmlDatabasePath = TMConfig.Current.xmlDatabasePath();;
				var xmlVirtualLibraryPath = TMConfig.Current.XmlLibrariesPath;
				TM_Xml_Database.Path_XmlDatabase = xmlDatabasePath;
				TM_Xml_Database.setLibraryPath_and_LoadDataIntoMemory(xmlVirtualLibraryPath);

				"[TM_Xml_Database][setDataFromCurrentScript] TM_Xml_Database.Path_XmlDatabase: {0}".debug(xmlDatabasePath);
				"[TM_Xml_Database][setDataFromCurrentScript] TMConfig.Current.XmlLibrariesPath: {0}".debug(xmlVirtualLibraryPath);
				TM_Xml_Database_Load_and_FileCache_Utils.populateGuidanceItemsFileMappings();	//only do this once
			}
			catch(Exception ex)
			{
				"[TM_Xml_Database] static .ctor: {0} \n\n".error(ex.Message, ex.StackTrace);
			}
		}
		
		public string reloadData()
		{
			return reloadData(null);
		}				
	
		[Admin(SecurityAction.Demand)]
		public string reloadData(string newLibraryPath)
		{	
			"In Reload data".info();
			if (newLibraryPath.notNull())
			{
				var tmConfig = TMConfig.Current;
				tmConfig.XmlLibrariesPath = newLibraryPath;
				tmConfig.SaveTMConfig();			
			}
			
			GuidanceItems_FileMappings.Clear();
			GuidanceExplorers_XmlFormat.Clear();								
			Libraries.Clear();
			Folders.Clear();
			Views.Clear();
			GuidanceItems.Clear();
			
			if(newLibraryPath.valid())
				setLibraryPath_and_LoadDataIntoMemory(newLibraryPath);
			else
				TM_Xml_Database.loadDataIntoMemory();			
				
			this.reCreate_GuidanceItemsCache();	
			this.xmlDB_Load_GuidanceItems();
			
			this.createDefaultAdminUser();	// make sure this user exists		
			
			return "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems".
						format(TM_Xml_Database.Path_XmlLibraries.directoryName(), Libraries.size(), Views.size(), GuidanceItems.size());
		}
		
	}		
}
