using System;
using System.Linq;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_Folder
    {
        public static List<urn.microsoft.guidanceexplorer.Folder>   xmlDB_Folders       (this TM_Xml_Database tmDatabase)
        {						
            return (from tmLibrary in tmDatabase.tmLibraries()
                    from folder in tmLibrary.xmlDB_Folders(tmDatabase)
                    select folder).toList();
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders       (this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
        {
            return tmDatabase.xmlDB_Folders(tmLibrary.Id);
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders       (this TM_Xml_Database tmDatabase, Guid libraryId)
        {			
            try
            {			
                if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId))
                {
                    var libraryStructure = TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure;					
                    if (libraryStructure.notNull())		
                        return libraryStructure.folder;
        //				folders.AddRange(libraryStructure.folder.xmlDB_Folders());												
                }
                else
                    "[xmlDB_Folders] library with id {0} was not present".error(libraryId);
            }
            catch(Exception ex)
            {
                "[xmlDB_Folders] xmlDB_Folders: {0}".error(ex.Message);				
            }
            return new List<urn.microsoft.guidanceexplorer.Folder>() ;
        }						
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders       (this urn.microsoft.guidanceexplorer.Folder folderToMap)
        {            
            return folderToMap.isNull() 
                        ? new List<urn.microsoft.guidanceexplorer.Folder>() 
                        : folderToMap.folder1;
        }
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase)
        {
            return (from tmLibrary in tmDatabase.tmLibraries()
                    from folder in tmDatabase.xmlDB_Folders_All(tmLibrary.Id)
                    select folder).toList();
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase, Guid libraryId)
        {		
            return tmDatabase.xmlDB_Folders_All(tmDatabase.xmlDB_Folders(libraryId));			
        }				
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase, IList<urn.microsoft.guidanceexplorer.Folder> foldersToMap)
        {
            var folders = new List<urn.microsoft.guidanceexplorer.Folder> ();
            
            if (foldersToMap.notNull())
                foreach(var folderToMap in foldersToMap)
                {
                    folders.add(folderToMap);
                    folders.AddRange(tmDatabase.xmlDB_Folders_All(folderToMap.folder1));
                }
            return folders;
        }		
        public static IList<urn.microsoft.guidanceexplorer.Folder>  xmlDB_Folders_All   (this TM_Xml_Database tmDatabase, urn.microsoft.guidanceexplorer.Folder folderToMap)
        {
            var folders = new List<urn.microsoft.guidanceexplorer.Folder> ();			
            if (folderToMap.notNull())
            {
                folders.add(folderToMap);
                folders.AddRange(tmDatabase.xmlDB_Folders_All(folderToMap.folder1));
            }
            return folders;
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid folderId)
        {
            return tmDatabase.xmlDB_Folders_All()
                             .FirstOrDefault(folder => folder.folderId == folderId.str());
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
        {
            return tmDatabase.xmlDB_Folders_All(libraryId)
                             .FirstOrDefault(folder => folder.folderId == folderId.str());
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid libraryId, Guid parentFolder,string folderName)
        {
            return tmDatabase.xmlDB_Folders_All(libraryId)
                             .Where(folder => folder.folderId == parentFolder.str())
                             .SelectMany(folder => folder.folder1)
                             .FirstOrDefault(subFolder => subFolder.caption == folderName);
        }
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Xml_Database tmDatabase, Guid libraryId,  string folderCaption)
        {
            var tmLibrary = tmDatabase.tmLibrary(libraryId); 
            return tmLibrary.xmlDB_Folder(folderCaption, tmDatabase);
        }		
        public static urn.microsoft.guidanceexplorer.Folder         xmlDB_Folder        (this TM_Library tmLibrary, string folderCaption, TM_Xml_Database tmDatabase)
        {
            return (from folder in tmLibrary.xmlDB_Folders(tmDatabase)
                    where folder.caption == folderCaption
                    select folder).first();				
        }		

        [EditArticles] 	public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder    (this TM_Xml_Database tmDatabase, Guid libraryId, string folderCaption)
        {
            UserRole.EditArticles.demand();
            return tmDatabase.xmlDB_Add_Folder(libraryId, Guid.Empty, folderCaption);
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder    (this TM_Xml_Database tmDatabase, Guid libraryId, Guid parentFolderId, string folderCaption)
        {
            UserRole.EditArticles.demand();
            var tmLibrary = tmDatabase.tmLibrary(libraryId); 
            return tmLibrary.xmlDB_Add_Folder(parentFolderId, folderCaption, tmDatabase);
        }		
        [EditArticles] 	public static urn.microsoft.guidanceexplorer.Folder xmlDB_Add_Folder    (this TM_Library tmLibrary, Guid parentFolderId, string folderCaption, TM_Xml_Database tmDatabase)
        {		
            UserRole.EditArticles.demand();
            try
            {	
                var newFolder = new urn.microsoft.guidanceexplorer.Folder
                    { 
                            caption = folderCaption, 
                            folderId = Guid.NewGuid().str()
                        };
                var guidanceExplorer= tmLibrary.guidanceExplorer(tmDatabase);                
                if (parentFolderId == Guid.Empty)
                {
                    //adding a folder to the library
                    guidanceExplorer.library.libraryStructure.folder.Add(newFolder);
                }
                else
                {
                    //adding a folder to folder
                    var folder = tmDatabase.xmlDB_Folder(tmLibrary.Id, parentFolderId);
                    if (folder == null)
                    {
                        "[xmlDB_Add_Folder] couldn't find parent folder (to add folder) with id: {0}".error(parentFolderId);
                        return null;
                    }
                    folder.folder1.Add(newFolder);                                        
                }                        
                guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);	                    
                return tmDatabase.xmlDB_Folder(tmLibrary.Id, newFolder.folderId.guid());                                                
            }
            catch(Exception ex)
            {
                ex.log();
                return null;
            }
        }				
        [EditArticles] 	public static bool                                  xmlDB_Rename_Folder (this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId, string newFolderName)
        {
            UserRole.EditArticles.demand();
            //if (orginalFolderName.inValid() || newFolderName.inValid())
            //	return false;
            var folder = tmDatabase.xmlDB_Folder(libraryId, folderId);
            if (folder.isNull())
                return false;
            folder.caption = newFolderName;
            tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId); 
            return true;
        }		
        [EditArticles] 	public static bool                                  xmlDB_Delete_Folder (this TM_Xml_Database tmDatabase, Guid libraryId, Guid folderId)
        {
            UserRole.EditArticles.demand();
            try
            {
                if (folderId == Guid.Empty)
                    return false;
                
                var folderToDelete = tmDatabase.xmlDB_Folder(libraryId, folderId);
                if (folderToDelete.isNull())
                    return false;

                var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);                

                //see if the folder is on the library root
                if (guidanceExplorer.library.libraryStructure.folder.contains(folderToDelete))
                {
                    guidanceExplorer.library.libraryStructure.folder.remove(folderToDelete);
                    tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId);
                    return true;
                }
                // see if the folder to delete is a subfolder
                foreach (var folder in tmDatabase.xmlDB_Folders_All(libraryId))
                {
                    if (folder.folder1.contains(folderToDelete))
                    {
                        folder.folder1.remove(folderToDelete);
                        tmDatabase.xmlDB_Save_GuidanceExplorer(libraryId);
                        return true;
                    }
                }
                "[xmlDB_Delete_Folder] could not find folder to delete: {0} {1}".error(libraryId, folderId);
            }
            catch(Exception ex)
            {
                ex.log("in xmlDB_Delete_Folder");                
            }
            return false;
        }		
        /*public static urn.microsoft.guidanceexplorer.View xmlDB_RemoveView(urn.microsoft.guidanceexplorer.Folder , View tmView, TM_Xml_Database tmDatabase)
        {
            
        }*/
    }
}