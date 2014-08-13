using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public static class TM_Server_FileStorage
    {
        [Admin] public static string            tmServer_Location(this TM_FileStorage tmFileStorage)          
        {
            UserRole.Admin.demand();
            return  tmFileStorage.path_XmlDatabase()
                                 .pathCombine(TMConsts.TM_SERVER_FILENAME);                       
        }
        [Admin] public static bool              tmServer_Save(this TM_FileStorage tmFileStorage)
        {
            UserRole.Admin.demand();
            return tmFileStorage.tmServer_Save(tmFileStorage.Server);
        }
        [Admin] public static bool              tmServer_Save(this TM_FileStorage tmFileStorage, TM_Server tmServer)     
        {         
            UserRole.Admin.demand();
            if (tmFileStorage.isNull() || tmServer.isNull())                      
                return false;
            var location = tmFileStorage.tmServer_Location();
            return (location.valid()) && 
                    tmServer.saveAs(location);
        }
        
        /*[Admin] public static TM_FileStorage    load_TMServer(this TM_FileStorage tmFileStorage)
        {        
            return tmFileStorage.tmServer_Load();
        }*/
        [Admin] public static TM_FileStorage    tmServer_Load(this TM_FileStorage tmFileStorage)
        {
            UserRole.Admin.demand();
            if (tmFileStorage.isNull())
                return tmFileStorage;
            var tmServer = new TM_Server();
            tmServer.setDefaultData();
                    
            
            var location = tmFileStorage.tmServer_Location();
            if (location.valid())
            {
                if (location.fileExists().isFalse())
                {
                    "[TM_Xml_Database][load_TMServer_Config] expected TM_Server file didn't exist, so creating it: {0}".info(location);
                    tmServer.saveAs(location);
                }
                var loadedTmServer = location.load<TM_Server>();
                if (loadedTmServer.isNull())                
                    "[TM_Xml_Database][load_TMServer_Config] Failed to load tmServer file: {0}   Default values will be used".error(location);
                else
                    tmServer = loadedTmServer;                                                
            }           
            //tmDatabase.Events.After_TmServer_Load.raise();        
            tmFileStorage.Server = tmServer;
            return  tmFileStorage;
        }
        [Admin] public static TM_FileStorage    tmServer     (this TM_FileStorage tmFileStorage, TM_Server tmServer)
        {
            UserRole.Admin.demand();
            tmFileStorage.Server = tmServer;
            return tmFileStorage;
        }
        [Admin] public static TM_Server         tmServer     (this TM_FileStorage tmFileStorage)
        {
            UserRole.Admin.demand();
            return (tmFileStorage.notNull())
                        ? tmFileStorage.Server
                        : null;
        }
    }
}
