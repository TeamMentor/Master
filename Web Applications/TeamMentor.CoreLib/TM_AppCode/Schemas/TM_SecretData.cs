using System.Collections.Generic;
using System.Security.Cryptography;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_SecretData
    {        
        public string Rijndael_IV    { get; set; }
        public string Rijndael_Key   { get; set; }
        public string SMTP_Server    { get; set; }
        public string SMTP_UserName  { get; set; }
        public string SMTP_Password  { get; set; }

        public List<string> Libraries_Git_Repositories { get; set; }

        public TM_SecretData()
        {
            var rijndael    = Rijndael.Create();
            Rijndael_IV     = rijndael.IV.base64Encode();
            Rijndael_Key    = rijndael.Key.base64Encode();
            SMTP_Server     = "smtp.sendgrid.net";
            SMTP_UserName   = "TeamMentor";

            Libraries_Git_Repositories = new List<string>();
        }
    }
}
