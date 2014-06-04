using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
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