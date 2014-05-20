using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Config
    {
        public static TM_Xml_Database set_Path_XmlDatabase(this TM_Xml_Database tmXmlDatabase)
        {
            return tmXmlDatabase;
        }
        public static string xmlDatabasePath(this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.UseAppDataFolder)
                return TMConfig.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH);
            return tmConfig.rootDataFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
        }
        public static string rootDataFolder(this TMConfig tmConfig)
        {
            //set xmlDatabasePath based on virtualPathMapping			
            var virtualPathMapping = tmConfig.virtualPathMapping();
            var xmlDatabasePath = TMConfig.WebRoot.pathCombine(virtualPathMapping).fullPath();

            //check if we can write to xmlDatabasePath (and default to App_Data if we can't write to provided direct)
            if (xmlDatabasePath.canNotWriteToPath())
                xmlDatabasePath = TMConfig.AppData_Folder;

            return xmlDatabasePath;
        }
        public static string virtualPathMapping(this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.TMLibraryDataVirtualPath.valid())
                return tmConfig.TMSetup.TMLibraryDataVirtualPath;
            return TMConsts.VIRTUAL_PATH_MAPPING;
        }
        [Admin]
        public static TM_Xml_Database SetPaths_XmlDatabase(this TM_Xml_Database tmXmlDatabase)
        {
            try
            {
                var tmConfig = TMConfig.Current;
                var xmlDatabasePath = tmConfig.xmlDatabasePath();
                var libraryPath = tmConfig.TMSetup.XmlLibrariesPath;

                "[TM_Xml_Database] [setDataFromCurrentScript] TM_Xml_Database.Path_XmlDatabase: {0}".debug(xmlDatabasePath);
                "[TM_Xml_Database] [setDataFromCurrentScript] TMConfig.Current.XmlLibrariesPath: {0}".debug(libraryPath);


                if (libraryPath.dirExists().isFalse())
                {
                    libraryPath = xmlDatabasePath.pathCombine(libraryPath);
                    libraryPath.createDir();  // make sure it exists
                }

                tmXmlDatabase.Path_XmlDatabase = xmlDatabasePath;
                tmXmlDatabase.Path_XmlLibraries = libraryPath;
                "[TM_Xml_Database] Paths configured".info();
            }
            catch (Exception ex)
            {
                "[TM_Xml_Database] [SetPaths_XmlDatabase]: {0} \n\n {1}".error(ex.Message, ex.StackTrace);
            }
            return tmXmlDatabase;
        }    
    }
}
