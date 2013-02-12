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

        public static TM_SecretData secretData_Load(this TM_UserData userData)
        {
            if (userData.UsingFileStorage)
            {
                var secretDataFile = userData.secretData_FileLocation();
                if (secretDataFile.fileExists())
                    return secretDataFile.load<TM_SecretData>();
            }
            return new TM_SecretData();
        }

        public static bool secretData_Save(this TM_UserData userData)
        {
            if (userData.UsingFileStorage)
            {
                var secretDataFile = userData.secretData_FileLocation();
                return userData.saveAs(secretDataFile);
            }
            return true;
        }
    }
}
