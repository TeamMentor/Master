using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Secret_Data_ExtensionMethods
    {        
        public static string          secretData_Location(this TM_UserData userData)
        {
            return userData.Path_UserData.pathCombine("TMSecretData.config");
        }        
        public static TM_UserData     secretData_Load(this TM_UserData userData)
        {            
            if (userData.isNull())
                return null;
            userData.SecretData = null;

            if (userData.UsingFileStorage)
            {                    
                var secretDataFile = userData.secretData_Location();
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
            }               
            return userData;
        }
        public static bool            secretData_Save(this TM_UserData userData)
        {
            var result = true;
            if (userData.UsingFileStorage)
            {
                var secretDataFile = userData.secretData_Location();
                result = userData.SecretData.saveAs(secretDataFile);
                //userData.triggerGitCommit();
            }
            userData.logTBotActivity("TM_SecretData saved","");
            return result;
        }
    }
}