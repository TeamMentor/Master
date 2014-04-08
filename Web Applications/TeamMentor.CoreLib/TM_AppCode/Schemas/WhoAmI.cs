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

        public WhoAmI() : this (null)
        {}

        public WhoAmI(TMUser tmUser)
        {
            if (tmUser.isNull())
            {
                UserName  = "";
                UserId    = -1;
                GroupId   = 0;
                GroupName = "Anonymous";
                UserRoles = "";
            }
            else
            {
                UserName  = tmUser.UserName;
                UserId    = tmUser.UserID;
                GroupId   = tmUser.GroupID;
            GroupName = tmUser.userGroup().str();
            UserRoles = tmUser.userRoles().toStringArray().toList().join(",");
            }
        }
    }

    public static class WhoAmI_ExtensionMethods
    {
        public static WhoAmI whoAmI(this TMUser tmUser)
        {            
            return new WhoAmI(tmUser);
        }
    }
}
