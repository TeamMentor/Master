using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using Moq;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Utils;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
//O2File:TM_Xml_Database.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{	

	// this is a (bit) time consumining (less 1s for 8000 files), so it should only be done once (this is another good cache target)
	public static class TM_Xml_Database_Load_and_FileCache_Utils
	{		
		public static void populateGuidanceItemsFileMappings()
		{			
			var o2Timer = new O2Timer("populateGuidanceExplorersFileMappings").start();
			foreach(var filePath in TM_Xml_Database.Path_XmlLibraries.files(true, "*.xml"))
			{
				var fileId = filePath.fileName().remove(".xml");
				if (fileId.isGuid())
				{
					//"[populateGuidanceItemsFileMappings] loading GuidanceItem ID {0}".info(fileId);
					var guid = fileId.guid();
					if (TM_Xml_Database.GuidanceItems_FileMappings.hasKey(guid))
					{
						"[populateGuidanceItemsFileMappings] duplicate GuidanceItem ID found {0}".error(guid);
					}
					else
						TM_Xml_Database.GuidanceItems_FileMappings.Add(guid, filePath);				
				}
			}
			o2Timer.stop();
			"There are {0} files mapped in GuidanceItems_FileMappings".info(TM_Xml_Database.GuidanceItems_FileMappings.size());			
		}
	}

	public static class TM_Xml_Database_ExtensionMethods_Load_and_FileCache
	{
		public static TM_Xml_Database setGuidanceExplorerObjects(this TM_Xml_Database tmDatabase)
		{			
			"in setGuidanceExplorerObjects".info();
			var pathXmlLibraries = TM_Xml_Database.Path_XmlLibraries;
			TM_Xml_Database.GuidanceExplorers_XmlFormat = pathXmlLibraries.getGuidanceExplorerObjects();				
			return tmDatabase;
		}
		
		public static guidanceExplorer getGuidanceExplorerObject(this string xmlFile)
		{			
			try
			{
				if (TMConfig.Current.Libraries_Disabled.contains(Path.GetFileNameWithoutExtension(xmlFile)))
					"[getGuidanceExplorerObject] Skipping xmlFile {0} because it was listed in tmConfig.Libraries_Disabled".info(xmlFile);
				else
				{
					"loading :{0}".debug(xmlFile);
					return guidanceExplorer.Load(xmlFile); 					
				};
			}
			catch(Exception ex)
			{
				"[getGuidanceExplorerObject]: {0}".error(ex.Message);
			}
			return null;
		}
		
		public static Dictionary<Guid, guidanceExplorer> addGuidanceExplorerObject(this Dictionary<Guid, guidanceExplorer> guidanceExplorers, string xmlFile)
		{
			var guidanceExplorer = xmlFile.getGuidanceExplorerObject();
			return guidanceExplorers.addGuidanceExplorerObject(guidanceExplorer);			
		}
		
		public static Dictionary<Guid, guidanceExplorer> addGuidanceExplorerObject(this Dictionary<Guid, guidanceExplorer> guidanceExplorers, guidanceExplorer _guidanceExplorer)
		{
			if (_guidanceExplorer.notNull())
			{			
				try
				{
					var libraryGuid = _guidanceExplorer.library.name.guid();
					
					//check if the name is already there
					foreach(guidanceExplorer guidanceExplorer in guidanceExplorers.Values)
					{						
						if (guidanceExplorer.library.caption == _guidanceExplorer.library.caption)
						{
							"[addGuidanceExplorerObject]: Skipping load due to duplicate Library name '{0}' was in both library {1} and {2}".error(guidanceExplorer.library.caption, guidanceExplorer.library.name,  _guidanceExplorer.library.name);
							return guidanceExplorers;
						}
					}
					//check if the guid is already there
					if (guidanceExplorers.hasKey(libraryGuid))
					{
						"[addGuidanceExplorerObject]: for {0} , duplicate LibraryID detected, assiging a new Library Id: {0}".error(_guidanceExplorer.library.caption, libraryGuid);
						libraryGuid = Guid.NewGuid();
						_guidanceExplorer.library.name = libraryGuid.str();
						"[addGuidanceExplorerObject]: new ID for library {0} is now {1}".error(_guidanceExplorer.library.caption, libraryGuid);
						_guidanceExplorer.xmlDB_Save_GuidanceExplorer(TM_Xml_Database.Current, false);
					}					
					guidanceExplorers.Add(libraryGuid, _guidanceExplorer);

				}
				catch//(Exception ex)
				{
					"[addGuidanceExplorerObject] error importing guidanceExplorer: {0}".error(_guidanceExplorer);
				}
			}
			return guidanceExplorers;
		}
		
		public static Dictionary<Guid, guidanceExplorer> getGuidanceExplorerObjects(this string pathXmlLibraries)
		{			
			"in getGuidanceExplorerObjects".info();
			var guidanceExplorers = new Dictionary<Guid,guidanceExplorer>();
			
			//try first to load the library by finding it on the library root (original mode)
			foreach(var xmlFile in pathXmlLibraries.files("*.xml"))
				guidanceExplorers.addGuidanceExplorerObject(xmlFile);
				
			//then try to find the guidanceItems xml file by looking for an xml file with the same name as the folder
			foreach(var folder in pathXmlLibraries.folders())
			{
				var xmlFile = "{0}\\{1}.xml".format(folder, folder.fileName());				
				if(xmlFile.fileExists())
				{					
					guidanceExplorers.addGuidanceExplorerObject(xmlFile);
				}
			}
			return guidanceExplorers;			
		}
		
		public static string getCacheLocation(this string pathXmlLibraries) //, TM_Library library)
		{
			
			return pathXmlLibraries.pathCombine("Cache_guidanceItems.cacheXml");//.format(library.Caption));
		}
		
		public static string loadGuidanceItemsFromCache(this string pathXmlLibraries)
		{
			//"Loading items from cache".info();
			var chacheFile = pathXmlLibraries.getCacheLocation();			
			if (chacheFile.fileExists().isFalse())
				"[TM_Xml_Database] in loadGuidanceItemsFromCache, cached file not found: {0}".error(chacheFile);
			else
			{
				var o2Timer = new O2Timer("loadGuidanceItemsFromCache").start();
				var loadedGuidanceItems = chacheFile.load<List<GuidanceItem_V3>>();
				o2Timer.stop();
				o2Timer = new O2Timer("mapping to memory loadGuidanceItemsFromCache").start();
				foreach(var loadedGuidanceItem in loadedGuidanceItems)
					TM_Xml_Database.Cached_GuidanceItems.add(loadedGuidanceItem.guidanceItemId, loadedGuidanceItem);
				o2Timer.stop();					
			}
			return pathXmlLibraries;
		}
		
		public static string saveGuidanceItemsToCache(this string pathXmlLibraries)
		{
			var cacheFile = pathXmlLibraries.getCacheLocation();			
			var o2Timer = new O2Timer("saveGuidanceItemsToCache").start();
			lock(TM_Xml_Database.Cached_GuidanceItems)
			{				
				TM_Xml_Database.Cached_GuidanceItems.Values.toList().saveAs(cacheFile);
			}
			o2Timer.stop();
			return pathXmlLibraries;
		}			
		
		public static TM_Xml_Database clear_GuidanceItemsCache(this TM_Xml_Database tmDatabase)
		{
			"[TM_Xml_Database] clear_GuidanceItemsCache".info();
			TM_Xml_Database.Cached_GuidanceItems.Clear();
			return tmDatabase;
		}
						
		public static TM_Xml_Database save_GuidanceItemsCache(this TM_Xml_Database tmDatabase)
		{
			"[TM_Xml_Database] save_GuidanceItemsCache".info();
			TM_Xml_Database.Path_XmlLibraries.saveGuidanceItemsToCache();
			return tmDatabase;
		}
		
		public static TM_Xml_Database load_GuidanceItemsCache(this TM_Xml_Database tmDatabase)
		{
			"[TM_Xml_Database] load_GuidanceItemsCache".info();
			TM_Xml_Database.Path_XmlLibraries.loadGuidanceItemsFromCache();
			return tmDatabase;
		}
		
		public static TM_Xml_Database reCreate_GuidanceItemsCache(this TM_Xml_Database tmDatabase)
		{
			"[TM_Xml_Database] reCreate_GuidanceItemsCache".info();
			var cacheFile = TM_Xml_Database.Path_XmlLibraries.getCacheLocation();			
			Files.deleteFile(cacheFile);
			"cache file deleted:{0}".info(!cacheFile.fileExists());
			tmDatabase.clear_GuidanceItemsCache(); 	
			return tmDatabase.xmlDB_Load_GuidanceItems();
			//return tmDatabase.load_GuidanceItemsCache();			
		}
		
		public static guidanceItem update_Cache_GuidanceItems(this guidanceItem guidanceItem,  TM_Xml_Database tmDatabase)
		{
			var guidanceItemGuid = guidanceItem.id.guid();
			if (TM_Xml_Database.Cached_GuidanceItems.hasKey(guidanceItemGuid))
				TM_Xml_Database.Cached_GuidanceItems[guidanceItemGuid] = guidanceItem.tmGuidanceItemV3();
			else
				TM_Xml_Database.Cached_GuidanceItems.Add(guidanceItem.id.guid(), guidanceItem.tmGuidanceItemV3());
				
			//TM_Xml_Database.mapGuidanceItemsViews();  		// update views (to make sure they are pointing to the correct GuidanceItem object	
			// do this on a separate thread so that we don't hang the current request
			O2Thread.mtaThread(()=> tmDatabase.save_GuidanceItemsCache());;
			return guidanceItem;
		}	
	
		public static Dictionary<Guid, string> guidanceItemsFileMappings(this TM_Xml_Database tmDatabase)
		{
			return TM_Xml_Database.GuidanceItems_FileMappings;
		}
		
		public static string removeGuidanceItemFileMapping(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
		{
			if (TM_Xml_Database.GuidanceItems_FileMappings.hasKey(guidanceItemId))
			{
				var xmlPath = TM_Xml_Database.GuidanceItems_FileMappings[guidanceItemId];
				TM_Xml_Database.GuidanceItems_FileMappings.Remove(guidanceItemId);
				return xmlPath;
			}
			return null;
		}
		
		public static string getXmlFilePathForGuidanceId(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
		{
			return tmDatabase.getXmlFilePathForGuidanceId(guidanceItemId, Guid.Empty);
		}
		
		public static string getXmlFilePathForGuidanceId(this TM_Xml_Database tmDatabase, Guid guidanceItemId, Guid libraryId)	
		{		
			//first see if we already have this mapping
			if (TM_Xml_Database.GuidanceItems_FileMappings.hasKey(guidanceItemId))
			{
				//"in getXmlFilePathForGuidanceId, found id in current mappings: {0}".info( guidanceItemId);
				return TM_Xml_Database.GuidanceItems_FileMappings[guidanceItemId];
			}
			//then update the GuidanceItems_FileMappings dictionary

			//tmDatabase.populateGuidanceItemsFileMappings();
			
			if (TM_Xml_Database.GuidanceItems_FileMappings.hasKey(guidanceItemId))
			{
				"[getXmlFilePathForGuidanceId] found id after reindex: {0}".info( guidanceItemId);
				return TM_Xml_Database.GuidanceItems_FileMappings[guidanceItemId];
			}
			
			if (libraryId == Guid.Empty)
			{
				"[getXmlFilePathForGuidanceId] When creating a new GuidanceItem a libraryId must be provided".error();
				return null;
			}
			var tmLibrary = tmDatabase.tmLibrary(libraryId);
			if (tmLibrary == null)
			{
				"[getXmlFilePathForGuidanceId] When creating a new GuidanceItem could not find library for libraryId: {0}".error(libraryId);
				return null;
			}
			var newGuidanceVirtualFolder = "{0}\\_GuidanceItems".format(tmLibrary.Caption);
			// if not store it on a _GuidanceItems folder
			var xmlPath = TM_Xml_Database.Path_XmlLibraries
										 .pathCombine(newGuidanceVirtualFolder)
										 .createDir()
										 .pathCombine("{0}.xml".format(guidanceItemId));
			"in getXmlFilePathForGuidanceId, no previous mapping found so guidanceitem to :{0}".info(xmlPath);
			
			TM_Xml_Database.GuidanceItems_FileMappings.add(guidanceItemId,xmlPath); //add it to the file_mappings dictionary so that we know it for next time
			return xmlPath;
			
		}
	}
	
}