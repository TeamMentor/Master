using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public static class TM_FileStorage_Utils
    {
        [Admin] public static string   webRoot(this TM_FileStorage tmFileStorage)
        {
            UserRole.Admin.demand();
            if(tmFileStorage.notNull())
                return tmFileStorage.WebRoot;
            return null;
        }
        [Admin] public static TM_FileStorage   set_WebRoot(this TM_FileStorage tmFileStorage)
        {
            UserRole.Admin.demand();
            if(tmFileStorage.notNull())
            {
                if (TM_FileStorage.Custom_WebRoot.folderExists())
                    tmFileStorage.WebRoot = TM_FileStorage.Custom_WebRoot;
                else
                    tmFileStorage.WebRoot = AppDomain.CurrentDomain.BaseDirectory;
            }
            return tmFileStorage;
        }
        [Admin] public static TM_FileStorage   set_Path_XmlDatabase(this TM_FileStorage tmFileStorage, string path_XmlDatabase)
        {
            if(tmFileStorage.notNull())
                tmFileStorage.Path_XmlDatabase = path_XmlDatabase;
            return tmFileStorage;
        }
        
        [Admin] public static TM_FileStorage   set_Path_XmlDatabase(this TM_FileStorage tmFileStorage)  
        {            
            UserRole.Admin.demand();
            var tmStatus = TM_Status.Current;
            try
            { 
                if (tmFileStorage.isNull())
                    return null;
                
                var webRoot            = tmFileStorage.WebRoot;    

                tmFileStorage.Path_XmlDatabase = null;                

                var usingAppData = webRoot.contains(@"TeamMentor.UnitTests\bin") ||             // when running UnitTests under NCrunch
                                   webRoot.contains(@"site\wwwroot")             ||             // when running from Azure (or directly on IIS)
                                   tmFileStorage.using_Custom_WebRoot();                        // when the TM_FileStorage.Custom_WebRoot has been set        
                

                if (usingAppData.isFalse())
                {
                    //calculate location and see if we can write to it
                
                    var xmlDatabasePath = webRoot.pathCombine(TMConsts.VIRTUAL_PATH_MAPPING)
                                                 .pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH_LEGACY)   //use by default the 'Library_Data\\XmlDatabase" value due to legacy support (pre 3.3)
                                                 .fullPath();
                    
                    if (xmlDatabasePath.createDir().dirExists() && xmlDatabasePath.canWriteToPath())
                    {                        
                        tmFileStorage.Path_XmlDatabase              = xmlDatabasePath;           // if can write it then make it the Path_XmlDatabase
                        tmStatus.TM_Database_Location_Using_AppData = false;                                                
                        return tmFileStorage;
                    }
                    "[TM_Server][set_Path_XmlDatabase] It was not possible to write to mapped folder: {0}".error(xmlDatabasePath);
                }
                
                var appData_Path = webRoot.pathCombine("App_Data")
                                          .pathCombine(TMConsts.XML_DATABASE_VIRTUAL_PATH)        // inside App_Data we can use the folder value 'TeamMentor' 
                                          .fullPath();   
                if (appData_Path.createDir().dirExists() && appData_Path.canWriteToPath())
                {
                    tmFileStorage.Path_XmlDatabase              = appData_Path;           // if can write it then make it the Path_XmlDatabase
                    tmStatus.TM_Database_Location_Using_AppData = true;                    
                    return tmFileStorage;       
                }   
                           
                "[TM_Server][set_Path_XmlDatabase] It was not possible to write to App_Data folder: {0}".error(appData_Path);                                
                //TM_Server.UseFileStorage = false;                 
                return tmFileStorage;
            }
            finally
            {
                "[TM_Server][set_Path_XmlDatabase] Path_XmlDatabase set to: {0}"                   .info(tmFileStorage.Path_XmlDatabase);
                "[TM_Server][set_Path_XmlDatabase] tmStatus.TM_Database_Location_Using_AppData:{0}".info(tmStatus.TM_Database_Location_Using_AppData);                
            }
        }    
        public static bool using_Custom_WebRoot(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.notNull() && 
                   TM_FileStorage.Custom_WebRoot.notNull() &&
                   TM_FileStorage.Custom_WebRoot == tmFileStorage.webRoot();

        }



    }
}
