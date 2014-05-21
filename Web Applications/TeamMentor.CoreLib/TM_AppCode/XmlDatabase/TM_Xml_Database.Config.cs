using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Config
    {
        /// <summary>
        /// This is the function that calculates the path to the TM XML Database (i.e. local file storage of TM files)
        /// </summary>
        /// <param name="tmXmlDatabase">this</param>
        /// <returns></returns>
        public static TM_Xml_Database set_Path_XmlDatabase(this TM_Xml_Database tmXmlDatabase)
        {       
            // defaults to null values
            tmXmlDatabase.Path_XmlDatabase  = null;
            tmXmlDatabase.Path_XmlLibraries = null;

            // try to find a local folder to hold the TM Database data
            if (tmXmlDatabase.UsingFileStorage)
            {
                var webRoot  = TM_Server.WebRoot;
                var tmStatus = TM_Status.Current;

                
                var usingAppData = webRoot.contains(@"TeamMentor.UnitTests\bin") ||             // when running UnitTests under NCrunch
                                   webRoot.contains(@"site\wwwroot");                           // when running from Azure (or directly on IIS)
                if (usingAppData.isFalse())
                {
                    //calculate location and see if we can write to it
                    var virtualPathMapping = TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY;         //use by default the 'Library_Data\\XmlDatabase" value due to legacy support (pre 3.3)
                    var xmlDatabasePath = TM_Server.WebRoot.pathCombine(virtualPathMapping).fullPath();
                    
                    if (xmlDatabasePath.createDir().dirExists() && xmlDatabasePath.canWriteToPath())
                    {                        
                        tmXmlDatabase.Path_XmlDatabase              = xmlDatabasePath;           // if can write it then make it the Path_XmlDatabase
                        tmStatus.TM_Database_Location_Using_AppData = false;
                        "[TM_Xml_Database][set_Path_XmlDatabase] Path_XmlDatabase set to: {0}".info(xmlDatabasePath);
                        return tmXmlDatabase;
                    }
                    else
                        "[TM_Xml_Database][set_Path_XmlDatabase] It was not possible to write to mapped folder: {0}".error(xmlDatabasePath);
                }
                var appData_Path = TM_Server.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH).fullPath();   // inside App_Data we can use the better folder value 'TeamMentor' 
                if (appData_Path.createDir().dirExists() && appData_Path.canWriteToPath())
                {
                    tmXmlDatabase.Path_XmlDatabase              = appData_Path;           // if can write it then make it the Path_XmlDatabase
                    tmStatus.TM_Database_Location_Using_AppData = false;
                    "[TM_Xml_Database][set_Path_XmlDatabase] Path_XmlDatabase set to: {0}".info(appData_Path);
                    return tmXmlDatabase;       
                }   
                else
                {                    
                    "[TM_Xml_Database][set_Path_XmlDatabase] It was not possible to write to App_Data folder: {0}".error(appData_Path);    
                }
            }            
            "[TM_Xml_Database][set_Path_XmlDatabase] Path_XmlDatabase was set to null".info();
            return tmXmlDatabase;
        }

        public static string get_Path_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            return (tmDatabase.notNull() && tmDatabase.UsingFileStorage)
                        ? tmDatabase.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME)
                        : null;
        }
        /*public static string xmlDatabasePath(this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.UseAppDataFolder)
                return TM_Server.AppData_Folder.pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH);
            return tmConfig.rootDataFolder().pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY).fullPath();
        }
        public static string rootDataFolder(this TMConfig tmConfig)
        {
            //set xmlDatabasePath based on virtualPathMapping			
            var virtualPathMapping = tmConfig.virtualPathMapping();
            var xmlDatabasePath = TM_Server.WebRoot.pathCombine(virtualPathMapping).fullPath();

            //check if we can write to xmlDatabasePath (and default to App_Data if we can't write to provided direct)
            if (xmlDatabasePath.canNotWriteToPath())
                xmlDatabasePath = TM_Server.AppData_Folder;

            return xmlDatabasePath;
        }
        public static string virtualPathMapping(this TMConfig tmConfig)
        {
            if (tmConfig.TMSetup.TMLibraryDataVirtualPath.valid())
                return tmConfig.TMSetup.TMLibraryDataVirtualPath;
            return TMConsts.VIRTUAL_PATH_MAPPING;
        }*/
        [Admin]
        public static TM_Xml_Database SetPaths_XmlDatabase(this TM_Xml_Database tmXmlDatabase)
        {
            try
            {
                var tmConfig        = TMConfig.Current;
                var xmlDatabasePath = tmXmlDatabase.Path_XmlDatabase;  // tmConfig.xmlDatabasePath();
                var libraryPath     = tmConfig.TMSetup.XmlLibrariesPath;

                "[TM_Xml_Database] [SetPaths_XmlDatabase] TM_Xml_Database.Path_XmlDatabase: {0}".debug(xmlDatabasePath);
                "[TM_Xml_Database] [SetPaths_XmlDatabase] TMConfig.Current.XmlLibrariesPath: {0}".debug(libraryPath);


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
