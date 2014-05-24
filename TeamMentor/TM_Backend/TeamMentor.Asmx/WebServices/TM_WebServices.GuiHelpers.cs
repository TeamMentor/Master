using System;
using System.Collections.Generic;
using System.Web.Services;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{ 					
    //WebServices related to: Data Viewers
    public partial class TM_WebServices 
    {
        
        [WebMethod(EnableSession= true)]
        public List<Library_V3> GetFolderStructure_Libraries()
        {
            var libraryId = GetCurrentSessionLibrary();
            if (libraryId == Guid.Empty)
                return this.getFolderStructure_Libraries(GetGUIObjects());
            return this.getFolderStructure_Library(libraryId, GetGUIObjects()).wrapOnList();
        }
        
        [WebMethod(EnableSession= true)]
        public Library_V3 GetFolderStructure_Library(Guid libraryId)
        {
            return this.getFolderStructure_Library(libraryId, GetGUIObjects());
        }
        
        //********  Gui Objects
        
        //******** New Data Transfer mode based on String indexes
        
        public static TM_GUI_Objects last_Gui_Objects = null;		
        public static bool           guiObjectsCacheOk = false;
        
        public void resetCache()     {  	guiObjectsCacheOk = false;  }
        
        [WebMethod(EnableSession = true)]
        public Guid GetCurrentSessionLibrary()
        {
            var libraryId = Guid.Empty;
                        
            if (HttpContextFactory.Session["Library"].notNull())                
            {
                var libraryValue = HttpContextFactory.Session["Library"].str();
                var library = (libraryValue.isGuid())
                                    ? GetLibraryById(libraryValue.guid())
                                    : GetLibraryByName(libraryValue);
                if (library.notNull())
                {
                    guiObjectsCacheOk = false;
                    return library.id.guid();
                }
                "[GetCurrentSessionLibrary] could not find library for provided value: {0}".error(libraryValue);
            }
            return libraryId;
        }

        [WebMethod(EnableSession= true)]
        public bool ClearGUIObjects()
        {
            last_Gui_Objects = null;
            guiObjectsCacheOk = false;
            return last_Gui_Objects.isNull();
        }
        
        [WebMethod(EnableSession= true)]        
        public TM_GUI_Objects GetGUIObjects()
        {            
         //   var sessionLibrary = "CWE";
         //   Session["Library"] = sessionLibrary;

            var libraryId = GetCurrentSessionLibrary();

            
            if (guiObjectsCacheOk &&  last_Gui_Objects.notNull())		// returns cached version on next calls
                return last_Gui_Objects; 
                
            var guiObjects = new TM_GUI_Objects();
            var guidanceItems = (libraryId == Guid.Empty) 
                                        ? tmXmlDatabase.tmGuidanceItems()
                                        : GetGuidanceItemsInLibrary(libraryId);
            foreach (var row in guidanceItems)      
            {  
                var guidanceItemMappings = "{0},{1},{2},{3},{4},{5},{6}".format(
                                                //guiObjects.add_UniqueString(row.guidanceItemId.str().hash().str()),	// this will shave off another 80k from the request
                                                guiObjects.add_UniqueString(row.Metadata.Id.str()),
                                                guiObjects.add_UniqueString(""), //row.libraryId.str()),
                                                guiObjects.add_UniqueString(row.Metadata.Title),
                                                guiObjects.add_UniqueString(row.Metadata.Technology),
                                                guiObjects.add_UniqueString(row.Metadata.Phase),
                                                guiObjects.add_UniqueString(row.Metadata.Type),
                                                guiObjects.add_UniqueString(row.Metadata.Category));
                
                guiObjects.GuidanceItemsMappings.Add(guidanceItemMappings);												
            }
            last_Gui_Objects = guiObjects;
            //for the big library this is now a 360k string
            guiObjectsCacheOk = true;
            return guiObjects;
        }
            
        [WebMethod(EnableSession= true)]        
        public List<string> GetStringIndexes()
        {
            var guiObjects = GetGUIObjects();
            return guiObjects.UniqueStrings;
        }
        
        [WebMethod(EnableSession= true)]
        public List<string> GetGuidanceItemsMappings()
        {
            var guiObjects = GetGUIObjects();
            return guiObjects.GuidanceItemsMappings;
        }

        [WebMethod]
        public bool Upload_File_To_Library(Guid libraryId, string filename, byte[] contents)
        {
            return tmXmlDatabase.upload_File_to_Library(libraryId, filename, contents);            
        }
    }
}
