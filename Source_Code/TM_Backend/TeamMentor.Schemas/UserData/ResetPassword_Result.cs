using System;
using System.Runtime.Serialization;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class ResetPassword_Result : MarshalByRefObject
    {
        [DataMember]
        public bool PasswordReseted { get; set; }
        [DataMember]
        public string Message { get; set; }
        public ResetPassword_Result()
        {
            PasswordReseted = false;
            Message = String.Empty;
        }
    }
}
