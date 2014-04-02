using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class Firebase_ClientConfig
    {
        public string Firebase_Site      { get; set; }
        public string Firebase_AuthToken { get; set; }
        public string Firebase_RootArea  { get; set; }        
        public bool   Enabled            { get; set; }
    }
    
    public static class Firebase_ClientConfig_ExtensionMethods
    {
        public static Firebase_ClientConfig firebase_ClientConfig(this TM_UserData userData)
        {
            return new Firebase_ClientConfig
                {
                    Firebase_Site      = userData.SecretData.Firebase_Site,
                    Firebase_AuthToken = userData.SecretData.Firebase_AuthToken,
                    Firebase_RootArea  = userData.SecretData.Firebase_RootArea,
                    Enabled            = true
                };
        }
    }
}
