using System;
using System.Security.Cryptography;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class TM_SecretData
    {        
        public string Rijndael_IV    { get; set; }
        public string Rijndael_Key   { get; set; }
        public string SMTP_Server    { get; set; }
        public string SMTP_UserName  { get; set; }
        public string SMTP_Password  { get; set; }

        public TM_SecretData()
        {
            var rijndael    = Rijndael.Create();
            Rijndael_IV     = rijndael.IV.base64Encode();
            Rijndael_Key    = rijndael.Key.base64Encode();
            SMTP_Server     = "smtp.sendgrid.net";
            SMTP_UserName   = "TeamMentor";            
        }
    }

    public static class TM_Secret_Data_ExtensionMethods
    {
        public static string secretData_FileLocation(this TM_UserData userData)
        {
            return userData.Path_UserData.pathCombine("TMSecretData.config");
        }

        public static string secretDataScript_FileLocation(this TM_UserData userData)
        {
            return userData.Path_UserData.pathCombine("H2Scripts//TMSecretData.h2");
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

        /*public static string secretDataScript_Invoke(this TM_UserData userData)
        {
            var scriptFile = userData.secretDataScript_FileLocation();
            if (scriptFile.fileExists())                
            {
                var assembly = scriptFile.fileContents().compileCodeSnippet();
                if (assembly.isNull())
                    "[secretDataScript_Invoke] couldn't compile script file: {0}".format(scriptFile);
                else
                    return assembly.firstMethod().invoke().str();
            }
            return null;            
        }*/


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
