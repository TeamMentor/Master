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
        /// <summary>
        /// returns value of tmFileStorage.WebRoot
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <returns></returns>
        [Admin] public static string   webRoot(this TM_FileStorage tmFileStorage)
        {
            admin.demand();
            if(tmFileStorage.notNull())
                return tmFileStorage.WebRoot;
            return null;
        }
        /// <summary>
        /// Sets the tmFileStorage.WebRoot
        /// 
        /// If the TM_FileStorage.Custom_WebRoot is set and the folder exists, then that value will be used. 
        /// If not, TM_FileStorage.Custom_WebRoot will be set to AppDomain.CurrentDomain.BaseDirectory
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <returns></returns>
        [Admin] public static TM_FileStorage   set_WebRoot(this TM_FileStorage tmFileStorage)
        {
            admin.demand();
            if(tmFileStorage.notNull())
            {
                if (TM_FileStorage.Custom_WebRoot.folderExists())
                    tmFileStorage.WebRoot = TM_FileStorage.Custom_WebRoot;
                else
                    tmFileStorage.WebRoot = AppDomain.CurrentDomain.BaseDirectory;
            }
            return tmFileStorage;
        }
        /// <summary>
        /// Forces tmFileStorage.WebRoot to be set to a specific folder 
        /// 
        /// Note: If <code>webRoot</code> folder doesn't exist, the value is not changed
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <param name="webRoot"></param>
        /// <returns></returns>
        [Admin] public static TM_FileStorage   set_WebRoot(this TM_FileStorage tmFileStorage, string webRoot)
        {
            admin.demand();
            if(tmFileStorage.notNull() && webRoot.folderExists())
            { 
                tmFileStorage.WebRoot = webRoot;                
            }
            return tmFileStorage;
        }
        /// <summary>
        /// Sets (after calculation) the value of tmFileStorage.Path_XmlDatabas
        /// 
        /// The logic is a bit complicated since it takes into account the different execution locations of NCrunch, Resharper and IIS
        /// 
        /// There is support for using the special ASP.NET App_Data folder (also used when the current running used does not have priviledges
        /// the mapped tmFileStorage.Path_XmlDatabase value)
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <returns></returns>
                
        [Admin] public static TM_FileStorage   set_Path_XmlDatabase(this TM_FileStorage tmFileStorage)  
        {            
            admin.demand();
            var tmStatus = TM_Status.Current;
            try
            { 
                if (tmFileStorage.isNull())
                    return null;
               
                if(TM_FileStorage.Custom_Path_XmlDatabase.folder_Exists())
                {
                    "[TM_Server][set_Path_XmlDatabase] using TM_FileStorage.Custom_Path_XmlDatabase value".info();
                    tmFileStorage.Path_XmlDatabase = TM_FileStorage.Custom_Path_XmlDatabase;
                    return tmFileStorage;
                }

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
        /// <summary>
        /// Forces tmFileStorage.Path_XmlDatabase to be set to a specific folder 
        /// 
        /// Note: If <code>path_XmlDatabase</code> folder doesn't exist, the value is not changed
        /// </summary>
        /// <param name="tmFileStorage"></param>
        /// <param name="path_XmlDatabase"></param>
        /// <returns></returns>
        [Admin] public static TM_FileStorage   set_Path_XmlDatabase(this TM_FileStorage tmFileStorage, string path_XmlDatabase)
        {
            admin.demand();
            if(tmFileStorage.notNull() && path_XmlDatabase.folderExists())
                tmFileStorage.Path_XmlDatabase = path_XmlDatabase;
            return tmFileStorage;
        }
        [Admin] public static bool using_Custom_WebRoot(this TM_FileStorage tmFileStorage)
        {
            admin.demand();
            return tmFileStorage.notNull() && 
                   TM_FileStorage.Custom_WebRoot.notNull() &&
                   TM_FileStorage.Custom_WebRoot == tmFileStorage.webRoot();
        }
        [Admin] public static bool using_Custom_Path_XmlDatabase(this TM_FileStorage tmFileStorage)
        {
            admin.demand();
            var tmStatus = TM_Status.Current;
            return tmFileStorage.notNull() && 
                   TM_FileStorage.Custom_Path_XmlDatabase.notNull() &&
                   TM_FileStorage.Custom_Path_XmlDatabase == tmFileStorage.webRoot();
        }

        



    }
}
