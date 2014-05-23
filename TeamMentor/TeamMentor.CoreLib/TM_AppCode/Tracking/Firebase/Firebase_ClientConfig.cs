using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class Firebase_ClientConfig
    {
        [DataMember] public string Firebase_Site      { get; set; }
        [DataMember] public string Firebase_AuthToken { get; set; }
        [DataMember] public string Firebase_RootArea  { get; set; }        
        [DataMember] public bool   Enabled            { get; set; }
    }
    
    public static class Firebase_ClientConfig_ExtensionMethods
    {
        public static Firebase_ClientConfig firebase_ClientConfig(this TM_UserData userData)
        {
            var firebaseConfig = userData.SecretData.FirebaseConfig;
            return new Firebase_ClientConfig
                {
                    Firebase_Site      = firebaseConfig.Site,
                    Firebase_AuthToken = firebaseConfig.AuthToken,
                    Firebase_RootArea  = firebaseConfig.RootArea,
                    Enabled            = true
                };
        }
    }
}
