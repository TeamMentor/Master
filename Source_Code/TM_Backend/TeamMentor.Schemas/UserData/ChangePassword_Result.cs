using System;
using System.Runtime.Serialization;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class ChangePassword_Result : MarshalByRefObject
    {
        [DataMember]
        public bool PasswordChanged { get; set; }
        [DataMember]
        public string Message { get; set; }
        public ChangePassword_Result()
        {
            PasswordChanged  = false;
            Message = String.Empty; 
        }
    }
}
