using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{        
    [DataContract]
    public class WhoAmI
    {
        [DataMember] public string UserName  { get; set; }
        [DataMember] public int    UserId    { get; set; }
        [DataMember] public int    GroupId   { get; set; }
        [DataMember] public string GroupName { get; set; }
        [DataMember] public string UserRoles { get; set; }

        public WhoAmI()
        {
            UserName = "";
            UserId = -1;
            GroupId = 0;
            GroupName = "Anonymous";
            UserRoles = "";
        }  
    }
}
