using System.Runtime.Serialization;

namespace TeamMentor.CoreLib
{
    [DataContract]
    public class TM_Credentials
    {
        [DataMember] public string UserName  { get; set; }
        [DataMember] public string Password  { get; set; }
    }
}