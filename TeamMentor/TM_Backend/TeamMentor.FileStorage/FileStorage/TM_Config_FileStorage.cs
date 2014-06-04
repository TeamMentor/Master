using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage
{
    public static class TM_Config_FileStorage
    {
        
        public static string tmConfig_Location(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.path_userData().pathCombine(TMConsts.TM_CONFIG_FILENAME);                        
        }         

        public static TM_FileStorage tmConfig_Load(this TM_FileStorage tmFileStorage)
        {            
            if (tmFileStorage.isNull())
                return null;
                           
            var userConfigFile = tmFileStorage.tmConfig_Location();
            if (userConfigFile.fileExists())
            {
                var newConfig = userConfigFile.load<TMConfig>();    // to check that the new TMConfig is not corrupted
                if (newConfig.isNull())
                {
                    "[handleUserDataConfigActions] failed to load config file from: {0}".error(userConfigFile);
                    return null;
                }
                else
                { 
                    TMConfig.Current = newConfig;
                }                    
            }
            else
            { 
            // if userConfigFile doesn't exist, create one and save it 
                TMConfig.Current = new TMConfig();
                tmFileStorage.tmConfig_Save();     
            }    
            return tmFileStorage;            
        }
        public static TMConfig tmConfig_Reload(this TM_FileStorage tmFileStorage)
        {
            TMConfig.Current = tmFileStorage.tmConfig_Location().fileExists()
                                    ? tmFileStorage.tmConfig_Location().load<TMConfig>()
                                    : new TMConfig();
            return TMConfig.Current;
        }
        public static bool tmConfig_Save(this TM_FileStorage tmFileStorage)
        {
            var tmConfig = TMConfig.Current;
            var location = tmFileStorage.tmConfig_Location();
            return  (tmConfig.notNull() && location.valid())
                        ? tmConfig.saveAs(location)
                        : false;
        }
    }
}
