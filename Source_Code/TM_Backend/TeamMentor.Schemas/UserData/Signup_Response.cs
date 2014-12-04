using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class Signup_Response : MarshalByRefObject
    {
        #region class constructor
        public Signup_Response()
        {
            Post_Login_Url = string.Empty;
            Simple_Error_Message = string.Empty;
            Validation_Results = new List<Validation_Results>();
            UserCreated = 0;
        }
        #endregion  
        public enum SignupStatus
        {
            Login_Ok = 0,
            Login_Fail = 1,
            Login_Error = 2,
            Validation_Failed = 3
        };
        [DataMember]
        public int UserCreated { get; set; }
        [DataMember]
        public SignupStatus Signup_Status { get; set; }
        [DataMember]
        public string Post_Login_Url { get; set; } //This should be an URI but Uri are not serializable
        [DataMember]
        public string Simple_Error_Message { get; set; }
        [DataMember]
        public List<Validation_Results> Validation_Results { get; set; }
    }
}
