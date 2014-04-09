using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_FirebaseConfig
    {
        public string Site              { get; set; }
        public string AuthToken         { get; set; }
        public string RootArea          { get; set; }        
        public string RequestUrls_Match { get; set; }
        public bool   Log_RequestUrls   { get; set; }        
        public bool   Log_Activities    { get; set; }        
        public bool   Log_DebugMsgs      { get; set; }
        

        public TM_FirebaseConfig()
        {
            Site                = "";
            AuthToken           = "";
            RootArea            = "";
            RequestUrls_Match   = "";
            Log_RequestUrls     = false;
            Log_Activities      = true; 
            Log_DebugMsgs       = true;
        }
    }

    public static class TM_FirebaseConfig_ExtensionMethods
    {
        public static bool firebase_Log_Activities(this TM_UserData userData)
        {
            return userData.notNull() && userData.SecretData.FirebaseConfig.Log_Activities;
        }
        public static bool firebase_Log_DebugMsg(this TM_UserData userData)
        {
            return userData.notNull() && userData.SecretData.FirebaseConfig.Log_DebugMsgs;
        }
        public static bool firebase_Log_RequestUrls(this TM_UserData userData)
        {
            return userData.notNull() && userData.SecretData.FirebaseConfig.Log_RequestUrls;
        }
    }
}