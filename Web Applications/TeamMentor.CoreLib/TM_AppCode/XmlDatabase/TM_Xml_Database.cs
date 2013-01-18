using System;
using System.Collections.Generic;
using System.Security.Permissions;
using O2.DotNetWrappers.ExtensionMethods;
using urn.microsoft.guidanceexplorer;
using Items = O2.DotNetWrappers.DotNet.Items;

namespace TeamMentor.CoreLib
{	
	public partial class TM_Xml_Database 
	{		
		public static TM_Xml_Database _current;

		//config
		public bool			UsingFileStorage				{ get; set; }

		//users
		public List<TMUser>	TMUsers							{ get; set; }
		public Items TMUsersPasswordHashes { get; set; }

		//articles
		public Dictionary<Guid, TMUser>				ActiveSessions				{ get; set; }		
		public Dictionary<Guid, guidanceExplorer>	GuidanceExplorers_XmlFormat { get; set; }	
		public Dictionary<Guid, string>				GuidanceItems_FileMappings	{ get; set; }			
        public Dictionary<Guid, TeamMentor_Article>	Cached_GuidanceItems		{ get; set; }

		public Dictionary<Guid, VirtualArticleAction> VirtualArticles			{ get; set; }
        
				
									
		public string 	Path_XmlDatabase 		{ get; set; }					
		public string 	Path_XmlLibraries 		{ get; set; }							
		public List<TM_Library> Libraries  				{  get { 	return this.tmLibraries(); } }
		public List<Folder_V3> 	Folders  				{  get { 	return this.tmFolders(); } } 		
		public List<View_V3> 	Views  					{  get { 	return this.tmViews(); } } 
		public List<TeamMentor_Article> GuidanceItems	{  get {	return this.tmGuidanceItems(); } } 
								

		[Log("TM_Xml_Database Setup")]
		

		public TM_Xml_Database() : this(true)
		{
		}

		public TM_Xml_Database(bool useFileStorage)
		{
			UsingFileStorage = useFileStorage;
			TM_Xml_Database.Current = this;
			Setup();
		}

		public static TM_Xml_Database Current
		{
			get
			{
				return _current.isNull()
					       ? new TM_Xml_Database()
					       : _current;
			}
			set { _current = value; }
		}

		public TM_Xml_Database Setup()
		{
			try
			{
				Cached_GuidanceItems = new Dictionary<Guid, TeamMentor_Article>();
				GuidanceItems_FileMappings = new Dictionary<Guid, string>();
				GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();

				TMUsers = new List<TMUser>();
				TMUsersPasswordHashes = new Items();
				ActiveSessions = new Dictionary<Guid, TMUser>();

				if (UsingFileStorage)
				{
					this.setPathsAndloadData();
					this.handleDefaultInstallActions();
					this.xmlDB_Load_GuidanceItems();
					this.createDefaultAdminUser(); // make sure this user exists						
				}
			}
			catch(Exception ex)
			{
				"[TM_Xml_Database] .ctor: {0} \n\n".error(ex.Message, ex.StackTrace);
			}
			return this;
		} 
		
		


		public void setPathsAndloadData()
		{            
			try
			{								
				var xmlDatabasePath = TMConfig.Current.xmlDatabasePath();;
				var xmlVirtualLibraryPath = TMConfig.Current.XmlLibrariesPath;
				TM_Xml_Database.Current.Path_XmlDatabase = xmlDatabasePath;
				TM_Xml_Database.Current.setLibraryPath_and_LoadDataIntoMemory(xmlVirtualLibraryPath);

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
				TM_Xml_Database.Current.loadDataIntoMemory();			
				
			this.reCreate_GuidanceItemsCache();	
			this.xmlDB_Load_GuidanceItems();
			
			this.createDefaultAdminUser();	// make sure this user exists		
			
			return "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems".
						format(TM_Xml_Database.Current.Path_XmlLibraries.directoryName(), Libraries.size(), Views.size(), GuidanceItems.size());
		}
		
	}		
}
