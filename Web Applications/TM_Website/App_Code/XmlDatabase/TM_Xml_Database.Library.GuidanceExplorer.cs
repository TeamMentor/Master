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
using O2.DotNetWrappers.Network;
using O2.XRules.Database.Utils;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
//O2File:TM_Xml_Database.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceExplorer
	{		
		public static string REGEX_SAFE_FILE_NAME = @"^[a-zA-Z0-9\-_\s+.']{1,50}$";
		
		public static bool isValidGuidanceExplorerName(this string name)
		{
			if (name.regEx(REGEX_SAFE_FILE_NAME))
				return true;
			"[isValidGuidanceExplorerName] failed validation for: {0}".info(name);
			return false;
		}
		
		public static guidanceExplorer xmlDB_NewGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption)
		{
			"[TM_Xml_Database][xmlDB_NewGuidanceExplorer] Creating new Library with id {0} and caption {1}".info(libraryId, caption);
			if (caption.isValidGuidanceExplorerName().isFalse())
			{
				"[TM_Xml_Database][xmlDB_NewGuidanceExplorer] provided caption didn't pass validation regex".error();
				throw new Exception("Provided Library name didn't pass validation regex"); 				
			}
			
			if (tmDatabase.tmLibrary(caption).notNull())
			{
				"[TM_Xml_Database] in xmlDB_NewGuidanceExplorer, a library with that name already existed: {0}".error(caption);
				return null;
			}
			if (libraryId == Guid.Empty)
				libraryId = Guid.NewGuid();
			var newGuidanceExplorer = new urn.microsoft.guidanceexplorer.guidanceExplorer();  
			newGuidanceExplorer.library = new urn.microsoft.guidanceexplorer.Library(); 			
			newGuidanceExplorer.library.items = new urn.microsoft.guidanceexplorer.Items();
			newGuidanceExplorer.library.libraryStructure = new urn.microsoft.guidanceexplorer.LibraryStructure();			
			//newGuidanceExplorer.library.libraryStructure.folder = new List<urn.microsoft.guidanceexplorer.Folder>();  
			//newGuidanceExplorer.library.libraryStructure.view = new List<urn.microsoft.guidanceexplorer.View>();  
			newGuidanceExplorer.library.name = libraryId.str();
			newGuidanceExplorer.library.caption = caption; 
			"xmlLibraryPath: {0}".info(TM_Xml_Database.Path_XmlLibraries);
			//var newLibraryPath = TM_Xml_Database.Path_XmlLibraries.pathCombine("{0}.xml".format(caption));
			
			newGuidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);
			//"saving new library to: {0}".info(newLibraryPath);			
			//newGuidanceExplorer.Save(newLibraryPath);			
			//tmDatabase.setGuidanceExplorerObjects();	// reload these values
			
			return newGuidanceExplorer;
		}
		
		public static string xmlDB_DeleteGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
		{
			var tmLibrary = tmDatabase.tmLibrary(libraryId);
			if (tmLibrary.notNull())			
			{
				var caption = tmLibrary.Caption;
				if (caption.isValidGuidanceExplorerName())
				{
					var backupFile = tmDatabase.xmlDB_Libraries_BackupLibrary(tmLibrary.Id);
					"[xmlDB_DeleteGuidanceExplorer] deleting library with caption: {0}".info(caption);
					var pathToLibrary = tmDatabase.xmlDB_LibraryPath(caption);								
					
					var pathToGuidanceItemsFolder = tmDatabase.xmlDB_LibraryPath_GuidanceItems(caption);
									
					if (pathToGuidanceItemsFolder.dirExists())
					{
						"[xmlDB_DeleteGuidanceExplorer] deleting library guidanceItems folder: {0}".debug(pathToGuidanceItemsFolder);
						if (Files.deleteFolder(pathToGuidanceItemsFolder,true).isFalse())
						{
							"[xmlDB_DeleteGuidanceExplorer] there was an error deleting the folder: {0}".error(pathToGuidanceItemsFolder);
							return null;
						}						
					}
					
					"[xmlDB_DeleteGuidanceExplorer] deleting library guidanceItems file: {0}".debug(pathToLibrary);
					Files.deleteFile(pathToLibrary);
					
					if(pathToLibrary.fileExists())
							"[xmlDB_DeleteGuidanceExplorer] there was problem deleting the file: {0}".error(pathToLibrary);				
					
					//check if there is a root directory with the caption name (happens when imported from ZIP
					pathToGuidanceItemsFolder = tmDatabase.xmlDB_LibraryPath_GuidanceItems(caption);
					
					if (pathToGuidanceItemsFolder.dirExists() && pathToGuidanceItemsFolder.files().size() ==0)
						Files.deleteFolder(pathToGuidanceItemsFolder);					
					
					//finally reset these						
					tmDatabase.setGuidanceExplorerObjects(); //reset these
					
					return backupFile;
				}
			}
			return null;
		}
		
		public static guidanceExplorer xmlDB_Save_GuidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
		{
			return tmDatabase.xmlDB_Save_GuidanceExplorer(tmLibrary.Id);			
		}
		
		public static guidanceExplorer xmlDB_Save_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
		{
			var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
			return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);	
		}
		
		public static guidanceExplorer xmlDB_Save_GuidanceExplorer(this guidanceExplorer _guidanceExplorer, TM_Xml_Database tmDatabase)		
		{
			return _guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase, true);
		}
		public static guidanceExplorer xmlDB_Save_GuidanceExplorer(this guidanceExplorer _guidanceExplorer, TM_Xml_Database tmDatabase, bool reloadGuidanceItemsMappings)
		{			
			var caption = _guidanceExplorer.library.caption;
			var libraryPath =  tmDatabase.xmlDB_LibraryPath(caption);
			"[xmlDB_Save_GuidanceExplorer] saving GuidanceExplorer '{0}' to {1}'".debug(caption, libraryPath);			
			if (libraryPath.notNull())
			{
				_guidanceExplorer.Save(libraryPath);
				if (reloadGuidanceItemsMappings)
					tmDatabase.setGuidanceExplorerObjects();			
				return _guidanceExplorer;
			}			
			"[xmlDB_Save_GuidanceExplorer] could not find libraryPath for GuidanceExplorer: {0} - {1}".error(_guidanceExplorer.library.caption, _guidanceExplorer.library.name);
			return null;
			//TM_Xml_Database.mapGuidanceItemsViews();			
		}
		
		public static TM_Xml_Database xmlDB_Save_GuidanceExplorers(this TM_Xml_Database tmDatabase)
		{
			foreach(var guidanceExplorer in tmDatabase.xmlDB_GuidanceExplorers())
				guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);
			return tmDatabase;
		}
		
		public static guidanceExplorer xmlDB_UpdateGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption, bool deleteLibrary)
		{
			"[xmlDB_UpdateGuidanceExplorer]".info();
			if (TM_Xml_Database.GuidanceExplorers_XmlFormat.hasKey(libraryId).isFalse())
			{
				"[TM_Xml_Database] in xmlDB_UpdateGuidanceExplorer, could not find library to update with id: {0}".error(libraryId);
				return null;
			}						
			if (deleteLibrary)
			{				
				tmDatabase.xmlDB_DeleteGuidanceExplorer(libraryId);
				return null;
			}
			else
			{			
				var guidanceExplorerToUpdate = TM_Xml_Database.GuidanceExplorers_XmlFormat[libraryId];
				
				// this is a rename 
				if (guidanceExplorerToUpdate.library.caption != caption)
					return tmDatabase.xmlDB_RenameGuidanceExplorer(guidanceExplorerToUpdate, caption);
				return guidanceExplorerToUpdate;
			}
		}
		
		public static guidanceExplorer xmlDB_RenameGuidanceExplorer(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer, string newCaption)
		{
			if (newCaption.isValidGuidanceExplorerName().isFalse())
			{
				"[TM_Xml_Database][xmlDB_RenameGuidanceExplorer] provided caption didn't pass validation regex".error();
				throw new Exception("Provided Library name didn't pass validation regex"); 				
			}
			"[xmlDB_RenameGuidanceExplorer]".info();
			if(guidanceExplorer.notNull())
			{	
				var existingCaption = guidanceExplorer.library.caption;
				var existingLibraryPath = tmDatabase.xmlDB_LibraryPath(existingCaption); // TM_Xml_Database.Path_XmlLibraries.pathCombine("{0}.xml".format(guidanceExplorer.library.caption));
				if(existingLibraryPath.fileExists().isFalse())
					"[xmlDB_RenameGuidanceExplorer] something is wrong since existingLibraryPath was not there: {0}".error(existingLibraryPath);
				else
				{	
					var newLibraryPath = tmDatabase.xmlDB_LibraryPath(newCaption);
					if (newLibraryPath.fileExists())
						"[xmlDB_RenameGuidanceExplorer] there was already a library and/or file with that name, so stopping rename): {0}".error(newLibraryPath);
					else
					{
					
						var pathToGuidanceItems_Existing = tmDatabase.xmlDB_LibraryPath_GuidanceItems(existingCaption);
						var pathToGuidanceItems_New = tmDatabase.xmlDB_LibraryPath_GuidanceItems(newCaption);
						
						"pathToGuidanceItems_Existing: {0}".error(pathToGuidanceItems_Existing);
						"pathToGuidanceItems_New: {0}".error(pathToGuidanceItems_New);
						if(pathToGuidanceItems_Existing.dirExists())	
						{
							"RENAMING {0}-> {1}".error(pathToGuidanceItems_Existing, pathToGuidanceItems_New);
							Files.renameFolder(pathToGuidanceItems_Existing,  pathToGuidanceItems_New);
							tmDatabase.updateGuidanceItems_FileMappings_withNewPath(pathToGuidanceItems_Existing,pathToGuidanceItems_New); 
						}
						
						guidanceExplorer.library.caption = newCaption;									
						
						
						
						//xmlDB_LibraryPath_GuidanceItems
						if (Files.deleteFile(existingLibraryPath).isFalse())
							"[xmlDB_RenameGuidanceExplorer] could not delete existing library file: {0}".error(existingLibraryPath);
						else
						{
							guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);
							return guidanceExplorer;
						}
					}
					return guidanceExplorer;
				}
			}
			return null;			
		}
		
		public static TM_Xml_Database updateGuidanceItems_FileMappings_withNewPath(this TM_Xml_Database tmDatabase, string oldPath, string newPath)
		{
			foreach(var key in TM_Xml_Database.GuidanceItems_FileMappings.Keys.toList())
			{
				var value = TM_Xml_Database.GuidanceItems_FileMappings[key];
				if(value.contains(oldPath))
					TM_Xml_Database.GuidanceItems_FileMappings[key] = value.replace(oldPath, newPath);
			}
			return tmDatabase;
		}
		
		public static string xmlDB_LibraryPath(this TM_Xml_Database tmDatabase, string caption)
		{
			var libraryPath = TM_Xml_Database.Path_XmlLibraries.pathCombine("{0}\\{0}.xml".format(caption));			
			if (libraryPath.fileExists())
				return libraryPath;
			libraryPath = TM_Xml_Database.Path_XmlLibraries.pathCombine("{0}.xml".format(caption));
			//if (libraryPath.fileExists())
			return libraryPath;
			//"[xmlDB_LibraryPath] could not find library path for library called '{0}'".info(caption);
			//return null;
		}		
		
		public static string xmlDB_LibraryPath_GuidanceItems(this TM_Xml_Database tmDatabase, string caption)
		{
			var libraryPath = tmDatabase.xmlDB_LibraryPath(caption);
			if (libraryPath.notNull())
				return libraryPath.directoryName().pathCombine("{0}".format(caption));
			return null;
		}
		
		public static string xmlDB_Libraries_BackupFolder(this TM_Xml_Database tmDatabase)
		{
			return TM_Xml_Database.Path_XmlLibraries
								  .pathCombine("_Library_Backups")
								  .createDir();
		}
		
		public static string xmlDB_Libraries_BackupLibrary(this TM_Xml_Database tmDatabase , Guid libraryId)
		{			
			var tmLibrary = tmDatabase.tmLibrary(libraryId);
			if (tmLibrary.isNull())
			{
				"[xmlDB_Libraries_BackupLibrary] could not find library with ID: {0}".error(libraryId);				
			}	
			else
			{
				var libraryName = tmLibrary.Caption;
				"[xmlDB_Libraries_BackupLibrary] backing up library: {0}".info(libraryName);
				if (libraryName.isValidGuidanceExplorerName())
				{
					var libraryPath_originalName  = tmDatabase.xmlDB_LibraryPath(libraryName);    
					var libraryPath_GuidanceItemsFolder = tmDatabase.xmlDB_LibraryPath_GuidanceItems(libraryName); 
					var backupsFolder = tmDatabase.xmlDB_Libraries_BackupFolder();
					var filesToBackup = libraryPath_GuidanceItemsFolder.files(true);
					filesToBackup.add(libraryPath_originalName);	
					"[xmlDB_Libraries_BackupLibrary] backing up {0} files".info(filesToBackup.size());
					var backupFile = backupsFolder.pathCombine("{0}_{1}.zip".format(libraryName, DateTime.Now.Ticks)); 	
					"[xmlDB_Libraries_BackupLibrary] created backup file: {0}".info(backupFile);
					return filesToBackup.zip_Files(backupFile);	  
				}
			}
			return null;
		}
		
		public static bool xmlDB_Libraries_ImportFromZip(this TM_Xml_Database tmDatabase, string zipFileToImport)
		{
			if (zipFileToImport.isUri())
			{
				"[xmlDB_Libraries_ImportFromZip] provided value was an URL so, downloading it: {0}".info(zipFileToImport);
				zipFileToImport = new Web().downloadBinaryFile(zipFileToImport);
				//zipFileToImport =  zipFileToImport.uri().download(); 		
			}
			"[xmlDB_Libraries_ImportFromZip] importing library from: {0}".info(zipFileToImport);
			if (zipFileToImport.fileExists().isFalse())
				"[xmlDB_Libraries_ImportFromZip] could not find file to import".error(zipFileToImport);
			else
			{		
				var currentLibraryPath = TM_Xml_Database.Path_XmlLibraries;
				var libraryName = Path.GetFileNameWithoutExtension(zipFileToImport);
				var libraryFilePath	= tmDatabase.xmlDB_LibraryPath(libraryName); 
				var guidanceItemsPath = tmDatabase.xmlDB_LibraryPath_GuidanceItems(libraryName);
				if(libraryFilePath.fileExists().isFalse() && guidanceItemsPath.dirExists().isFalse())
				{							
					zipFileToImport.unzip_File(currentLibraryPath); 					
					return true;
				}
				else
					"[xmlDB_Libraries_ImportFromZip] could not import library with name {0} since there was already one with that name".error(libraryName);
			}
			return false;
		}
		
		
		public static guidanceExplorer xmlDB_GuidanceExplorer(this TM_Xml_Database tmDatabase, string caption)
		{
			foreach(var guidanceExplorer in TM_Xml_Database.GuidanceExplorers_XmlFormat.Values)
				if (guidanceExplorer.library.caption == caption || guidanceExplorer.library.name == caption)
					return guidanceExplorer;
			"[xmlDB_GuidanceExplorer] Could not find is library with caption: {0}".error(caption);		
			return null;
		}
		
		public static guidanceExplorer xmlDB_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
		{
			if (TM_Xml_Database.GuidanceExplorers_XmlFormat.notNull())		
				if (TM_Xml_Database.GuidanceExplorers_XmlFormat.hasKey(libraryId))
					return TM_Xml_Database.GuidanceExplorers_XmlFormat[libraryId];
			"[xmlDB_GuidanceExplorer] Could not find is library with id: {0}".error(libraryId);
			return null;
		}
		
		public static List<guidanceExplorer> xmlDB_GuidanceExplorers(this TM_Xml_Database tmDatabase)
		{			
			if (TM_Xml_Database.GuidanceExplorers_XmlFormat.notNull())				
				return TM_Xml_Database.GuidanceExplorers_XmlFormat.Values.toList();
			"[xmlDB_GuidanceExplorers] GuidanceExplorers_XmlFormat is null".error();
			return new List<guidanceExplorer>();
		}		
		
		public static guidanceExplorer guidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
		{
			return tmDatabase.xmlDB_GuidanceExplorer(tmLibrary.Id);
		}			

		public static Library_V3 libraryV3(this Library library)		
		{
			if (library.isNull())
				return null;
			return new Library_V3()
							{
								libraryId = library.id.guid(),
								name = library.caption
							};			
		}
		
		public static Library_V3 libraryV3(this guidanceExplorer _guidanceExplorer)		
		{
			if (_guidanceExplorer.isNull())
				return null;
			return new Library_V3()
							{
								libraryId = _guidanceExplorer.library.name.guid(), 
								name = _guidanceExplorer.library.caption
							};			
		}
	}
}

