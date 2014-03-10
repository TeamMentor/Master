using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Secret_Data_ExtensionMethods
    {        
        public static string secretData_FileLocation(this TM_UserData userData)
        {
            return userData.Path_UserData.pathCombine("TMSecretData.config");
        }
        public static TM_SecretData secretData_Load(this TM_UserData userData)
        {
            try
            {
                if (userData.UsingFileStorage)
                {
                    var secretDataFile = userData.secretData_FileLocation();    
                    if (secretDataFile.fileExists())
                        return secretDataFile.load<TM_SecretData>();
                    if (secretDataFile.notNull())
                    {
                        var secretData = new TM_SecretData();
                        secretDataFile.parentFolder().createDir();
                        secretData.saveAs(secretDataFile);
                        userData.triggerGitCommit();
                        return secretData;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.log("in TM_SecretData secretData_Load");
            }
           
            return new TM_SecretData();
        }


        public static bool secretData_Save(this TM_UserData userData)
        {
            var result = true;
            if (userData.UsingFileStorage)
            {
                var secretDataFile = userData.secretData_FileLocation();
                result = userData.SecretData.saveAs(secretDataFile);
                userData.triggerGitCommit();
            }
            return result;
        }
    }
}