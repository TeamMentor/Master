using System;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;


namespace TeamMentor.FileStorage
{
    public static class TM_Secret_Data_ExtensionMethods
    {        
        public static string          secretData_Location(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.path_userData()
                                .pathCombine("TMSecretData.config");
        }        
        public static TM_FileStorage     secretData_Load(this TM_FileStorage tmFileStorage)
        {            
            var userData = tmFileStorage.userData();
            if (userData.isNull())
                return tmFileStorage;
            userData.SecretData = null;

            //if (userData.usingFileStorage().isFalse())
            //     userData.SecretData = new TM_SecretData();
            //else
            //{                    
            var secretDataFile = tmFileStorage.secretData_Location();
            if (secretDataFile.notValid())
            {
                userData.SecretData = null;
                "[TM_UserData][TM_UserData] UsingFileStorage was set, but could not map secretData_Location".error();
                return userData;
            }

            if (secretDataFile.fileExists())
            {
                var secretData = secretDataFile.load<TM_SecretData>();
                if (secretData.isNull())
                    "[TM_UserData][TM_UserData] Failed to load SecretData file: {0}".error(secretDataFile);
                else
                {
                    userData.SecretData = secretData;
                    "[TM_UserData][TM_UserData] SecretData file loaded from: {0}".debug(secretDataFile); 
                }
            }
            else
            {       
                userData.SecretData = new TM_SecretData();
                userData.SecretData.saveAs(secretDataFile);
                "[TM_UserData][TM_UserData] SecretData file didn't exist, so creating one at: {0}".debug(secretDataFile);                     
            }            
     
            userData.Events.After_TM_SecretData_Load.raise();
            return tmFileStorage;
        }
        public static bool            secretData_Save(this TM_FileStorage tmFileStorage)
        {
            var secretData = tmFileStorage.userData().SecretData;
            var location   = tmFileStorage.secretData_Location();
            return location.valid()     && 
                   secretData.notNull() && 
                   secretData.saveAs(location);
            //userData.triggerGitCommit();            
            //userData.logTBotActivity("TM_SecretData saved","");            
        }
    }
}