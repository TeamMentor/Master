using System;
using System.Linq;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using urn.microsoft.guidanceexplorer;
using Items = urn.microsoft.guidanceexplorer.Items;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceExplorer
    {		
        public static string REGEX_SAFE_FILE_NAME = @"^[a-zA-Z0-9\-_\s+.']{1,50}$";		

        public static bool                   isValidGuidanceExplorerName(this string name)
        {
            if (name.regEx(REGEX_SAFE_FILE_NAME))
                return true;
            "[isValidGuidanceExplorerName] failed validation for: {0}".info(name);
            return false;
        }		
        
        public static guidanceExplorer       xmlDB_NewGuidanceExplorer(this TM_Xml_Database tmDatabase, Library library)
        {
            if (library.notNull())
                return tmDatabase.xmlDB_NewGuidanceExplorer(library.id.guid(), library.caption);
            return null;
        }
        public static guidanceExplorer       xmlDB_NewGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption)
        {			
            if (caption.isNull() || caption.isValidGuidanceExplorerName().isFalse())
            {
                "[TM_Xml_Database] [xmlDB_NewGuidanceExplorer] provided caption didn't pass validation regex".error();
                throw new Exception("Provided Library name didn't pass validation regex"); 				
            }
            
            if (tmDatabase.tmLibrary(caption).notNull())
            {
                "[TM_Xml_Database] in xmlDB_NewGuidanceExplorer, a library with that name already existed: {0}".error(caption);
                return null;
            }
            if (libraryId == Guid.Empty)
                libraryId = Guid.NewGuid();
            var newGuidanceExplorer = new guidanceExplorer
                {
                    library = new urn.microsoft.guidanceexplorer.Library
                                {
                                    items = new Items(),
                                    libraryStructure = new LibraryStructure(),
                                    name = libraryId.str(),
                                    caption = caption
                                }
                };
            
            TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.add(libraryId, newGuidanceExplorer);    //add to in memory database
            newGuidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);                             
            "[TM_Xml_Database] [xmlDB_NewGuidanceExplorer] Created new Library with id {0} and caption {1}".info(libraryId, caption);
            return newGuidanceExplorer;
        }		
        public static bool                   xmlDB_DeleteGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var tmLibrary = tmDatabase.tmLibrary(libraryId);

            if (tmLibrary.isNull())
                return false;


            tmDatabase.Events.Library_Deleted.raise(tmLibrary);

            TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.remove(tmLibrary.Id);

            
            return true;    
        }        
        public static bool       xmlDB_Save_GuidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)        
        {
            return tmDatabase.xmlDB_Save_GuidanceExplorer(tmLibrary.Id);
        }
        public static bool       xmlDB_Save_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var guidanceExplorer = tmDatabase.xmlDB_GuidanceExplorer(libraryId);
            return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);//, reloadGuidanceItemsMappings);	
        }		        
        public static bool       xmlDB_Save_GuidanceExplorer(this guidanceExplorer guidanceExplorer, TM_Xml_Database tmDatabase)
        {
            tmDatabase.Events.GuidanceExplorer_Save.raise(guidanceExplorer);        // raise events
            return true;
        }        	
        public static TM_Xml_Database        xmlDB_Save_GuidanceExplorers(this TM_Xml_Database tmDatabase)
        {
            foreach(var guidanceExplorer in tmDatabase.xmlDB_GuidanceExplorers())
                guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);
            return tmDatabase;
        }
		public static bool       xmlDB_UpdateGuidanceExplorer(this TM_Xml_Database tmDatabase, Library library)
		{
		    if (library.notNull())
                return tmDatabase.xmlDB_UpdateGuidanceExplorer(library.id.guid(), library.caption, library.delete);
            return false;
		}
        
        public static bool       xmlDB_UpdateGuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId, string caption, bool deleteLibrary)
        {
            //"[xmlDB_UpdateGuidanceExplorer]".info();
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId).isFalse())
            {
                "[TM_Xml_Database] in xmlDB_UpdateGuidanceExplorer, could not find library to update with id: {0}".error(libraryId);
                return false;
            }						
            if (deleteLibrary)
            {				
                return tmDatabase.xmlDB_DeleteGuidanceExplorer(libraryId);                
            }
                        
            var guidanceExplorerToUpdate = TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId];
                
            // this is a rename 
            if (guidanceExplorerToUpdate.library.caption != caption)
                return tmDatabase.xmlDB_RenameGuidanceExplorer(guidanceExplorerToUpdate, caption);
            return false;			
        }		
        public static bool       xmlDB_RenameGuidanceExplorer(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer, string newCaption)
        {
            if (newCaption.isValidGuidanceExplorerName().isFalse())
            {
                "[TM_Xml_Database] [xmlDB_RenameGuidanceExplorer] provided caption didn't pass validation regex".error();
                //throw new Exception("Provided Library name didn't pass validation regex"); 				                
            }
            else if(guidanceExplorer.notNull())
            {                
                guidanceExplorer.library.caption = newCaption;  // update in memory library name value
                 
                return guidanceExplorer.xmlDB_Save_GuidanceExplorer(tmDatabase);                // save it 
               

                /*if (tmDatabase.usingFileStorage())                // soft try to rename the library (disabled for now)
                {
                    try
                    {
                        var current_LibraryRootFolder = tmDatabase.xmlDB_Path_Library_RootFolder(guidanceExplorer);
                        var new_LibraryRootFolder = tmDatabase.Path_XmlLibraries.pathCombine(newCaption);
                        Files.renameFolder(current_LibraryRootFolder, new_LibraryRootFolder);
                        if (new_LibraryRootFolder.dirExists())
                            tmDatabase.GuidanceExplorers_Paths.add(guidanceExplorer, new_LibraryRootFolder);
                    }
                    catch (Exception ex)
                    {
                        ex.log("[xmlDB_RenameGuidanceExplorer] in trying to rename the library folder");                            
                    }
                    
                }*/                       
            }
            return false;			
        }		
        		
        //public static string                 xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, string caption)
        

        
        public static guidanceExplorer       xmlDB_GuidanceExplorer(this TM_Xml_Database tmDatabase, string caption)
        {
            foreach(var guidanceExplorer in TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.Values)
                if (guidanceExplorer.library.caption == caption || guidanceExplorer.library.name == caption)
                    return guidanceExplorer;
            "[xmlDB_GuidanceExplorer] Could not find is library with caption: {0}".error(caption);		
            return null;
        }		
        public static guidanceExplorer       xmlDB_GuidanceExplorer(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.notNull())		
                if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.hasKey(libraryId))
                    return TM_Xml_Database.Current.GuidanceExplorers_XmlFormat[libraryId];
            "[xmlDB_GuidanceExplorer] Could not find is library with id: {0}".error(libraryId);
            return null;
        }		
        public static List<guidanceExplorer> xmlDB_GuidanceExplorers(this TM_Xml_Database tmDatabase)
        {			
            if (TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.notNull())				
                return TM_Xml_Database.Current.GuidanceExplorers_XmlFormat.Values.toList();
            "[xmlDB_GuidanceExplorers] GuidanceExplorers_XmlFormat is null".error();
            return new List<guidanceExplorer>();
        }				
        public static guidanceExplorer       guidanceExplorer(this TM_Library tmLibrary, TM_Xml_Database tmDatabase)
        {
            return tmDatabase.xmlDB_GuidanceExplorer(tmLibrary.Id);
        }
        public static List<Library_V3>       librariesV3(this List<TM_Library> libraries)
        {
            return (from library in libraries select library.libraryV3()).toList();
        }
        public static Library_V3             libraryV3(this TM_Library library)
        {
            if (library.isNull())
                return null;
            return new Library_V3
                            {
                                libraryId = library.Id,
                                name = library.Caption
                            };
        }
        public static Library_V3             libraryV3(this Library library)		
        {
            if (library.isNull())
                return null;
            return new Library_V3
                            {
                                libraryId = library.id.guid(),
                                name = library.caption
                            };			
        }		
        public static Library_V3             libraryV3(this guidanceExplorer guidanceExplorer)		
        {
            if (guidanceExplorer.isNull())
                return null;
            return new Library_V3
                            {
                                libraryId = guidanceExplorer.library.name.guid(), 
                                name = guidanceExplorer.library.caption
                            };			
        }

    }    
}

