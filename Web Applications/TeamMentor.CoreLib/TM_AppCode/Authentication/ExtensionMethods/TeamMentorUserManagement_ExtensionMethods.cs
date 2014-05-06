using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Permissions;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{    
    public static class TeamMentorUserManagement_TMUser
    {
        public static UserGroup      userGroup(this TMUser tmUser)
        {
            return (UserGroup)tmUser.GroupID;
        }
        public static List<UserRole> userRoles(this TMUser tmUser)
        {
            return UserRolesMappings.Mappings[tmUser.userGroup()];
        }
		public static List<String>   toStringList(this List<UserRole> userRoles)
		{
			return (from role in userRoles
					select role.str()).toList();
		}		
        public static string         userStatus(this TMUser tmUser)
        {            
            if(tmUser.account_Enabled().isFalse())
                return "Disabled";
            if(tmUser.account_Expired())
                return "Expired";
            if(tmUser.password_Expired())
                return "Pwd Expired";
            return "Enabled";
        }
        
    }

    public static class TeamMentorUserManagement_UserGroup
    {
        public static List<UserRole> userRoles(this UserGroup userGroup)
        {
            return UserRolesMappings.Mappings[userGroup];
        }
    }

    public static class TeamMentorUserManagement_UserRole  
    {
        public static string[] toStringArray(this List<UserRole> userRoles)
        {
            return userRoles.ToArray().toStringArray();
        }
        public static string[] toStringArray(this UserRole[] userRoles)
        {
            return (from userRole in userRoles
                    select userRole.ToString()).ToArray();
        }		
		public static void demand(this UserRole userRole)
        {
			userRole.str().demand();             
        }		
		public static void demand(this string userRole)
        {
             new PrincipalPermission(null, userRole).Demand();
        }		
		public static void demand(this UserGroup userGroup)
		{
			foreach(var userRole in userGroup.userRoles())
				userRole.demand();
			
		}		
		public static bool currentUserHasRole(this UserRole userRole)
		{
			return userRole.str().currentUserHasRole();
		}		
		public static bool currentUserHasRole(this string userRole)
		{
			try
			{
				userRole.demand();
				return true;
			}
			catch//(Exception ex)
			{
				return false;
			}
		}
				
		
    }

    

    
}
