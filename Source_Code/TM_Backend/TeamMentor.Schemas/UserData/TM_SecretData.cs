using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    //[Serializable]                                   // Serializable breaks the TBOT Page 
    public class TM_SMTPConfig // : MarshalByRefObject // MarshalByRef was causing conflict with TBOT Rest API
    {
        public string           Server       { get; set; }
        public string           UserName     { get; set; }
        public string           Password     { get; set; }
        public string           Default_From { get; set; }
        public string           Default_To   { get; set; }       
        public string           Email_Footer { get; set; }

        public TM_SMTPConfig()
        {
            Server          = "smtp.sendgrid.net";
            UserName        = "TeamMentor";
            Password        = "";
            Default_From    = TMConsts.USERDATA_DEFAULT_ADMIN_EMAIL;
            Default_To      = TMConsts.USERDATA_DEFAULT_ADMIN_EMAIL;
            Email_Footer    = TMConsts.EMAIL_DEFAULT_FOOTER;
        }
    }
    
    //[Serializable]                                  // Serializable breaks the TBOT Page 
    public class TM_SecretData //: MarshalByRefObject // MarshalByRef was causing conflict with TBOT Rest API
    {           
        public string               Rijndael_IV     { get; set; }
        public string               Rijndael_Key    { get; set; }
        
        public TM_SMTPConfig        SmtpConfig      { get; set; }                
        public TM_FirebaseConfig    FirebaseConfig  { get; set; }                

        public List<string> Libraries_Git_Repositories { get; set; }

        public TM_SecretData()
        {
            var rijndael    = Rijndael.Create();
            Rijndael_IV     = rijndael.IV.base64Encode();
            Rijndael_Key    = rijndael.Key.base64Encode();

            SmtpConfig                  = new TM_SMTPConfig();            
            FirebaseConfig              = new TM_FirebaseConfig();
            Libraries_Git_Repositories  = new List<string>();
        }        
    }
}
