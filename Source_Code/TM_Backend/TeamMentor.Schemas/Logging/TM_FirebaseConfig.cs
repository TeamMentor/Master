using System;
using System.Runtime.Serialization;

namespace TeamMentor.CoreLib
{
    //[Serializable]                                       // Serializable breaks the TBOT Page 
    public class TM_FirebaseConfig //: MarshalByRefObject  // MarshalByRef was causing conflict with TBOT Rest API
    {
        public string Site                { get; set; }
        public string AuthToken           { get; set; }
        public string RootArea            { get; set; }        
//        public string RequestUrls_Match { get; set; }
        public bool   Log_RequestUrls     { get; set; }        
        public bool   Log_Activities      { get; set; }        
        public bool   Log_DebugMsgs       { get; set; }
        public bool   Force_Offline       { get; set; }
        public bool   DisableSslCertCheck { get; set; }
        

        public TM_FirebaseConfig()
        {
            Site                = "";
            AuthToken           = "";
            RootArea            = "";
//            RequestUrls_Match   = "";
            Log_RequestUrls     = false;
            Log_Activities      = true; 
            Log_DebugMsgs       = true;
            Force_Offline       = false;
        }
    }
}