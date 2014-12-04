using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class Login_Response : MarshalByRefObject
    {
        public Login_Response()
        {
            Validation_Results = new List<Validation_Results>();
            Token = Guid.Empty;
            Post_Login_Url = string.Empty;
            Simple_Error_Message= String.Empty;
        }
        public enum LoginStatus
        {
            Login_Ok =0,
            Login_Fail=1,
            Login_Error=2,
            Validation_Failed=3
        };
        [DataMember]
        public LoginStatus Login_Status { get; set; }
        [DataMember]
        public String Post_Login_Url { get; set; } //This should be an URI but Uri are not serializable
        [DataMember]
        public string Simple_Error_Message { get; set; }
        [DataMember]
        public List<Validation_Results> Validation_Results { get; set; }
        [DataMember]
        public Guid Token { get; set; }
    }
    [DataContract]
    public class Validation_Results
    {
        [DataMember]
        public string Field { get; set; }
        [DataMember]
        public string Message { get; set; }
        public enum Validation_Status { };
    }
}
