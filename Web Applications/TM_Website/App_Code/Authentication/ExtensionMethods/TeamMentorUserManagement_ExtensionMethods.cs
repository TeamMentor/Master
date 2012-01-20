using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Permissions;
using SecurityInnovation.TeamMentor.Authentication.WebServices;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
//O2File:../UserRoles.cs

namespace SecurityInnovation.TeamMentor.Authentication.ExtensionMethods
{
    public static class TeamMentorUserManagement_GUID
    {
        //Note: need to find a way to reuse database connections (the current model of creation a connection per request doesn't make a lot of sense        
        public static string userName(this Guid sessionID)
        {
            "resolving username for sessionID: {0}".info(sessionID);
//            if (sessionID != null && sessionID != Guid.Empty)            
//                return ObjectFactory.AuthenticationManagement().LookupUsernameFromSessionID(sessionID);
            return null;
        }

        public static TMUser tmUser(this Guid sessionID)
        { 
            var userName = sessionID.userName();
//            if (userName != null)
//                return ObjectFactory.AuthenticationManagement().GetUserFromUsername(userName);
            return null;
        }

        public static int groupID(this Guid sessionID)
        { 
            var tmUser = sessionID.tmUser();
            if (tmUser != null)
                return tmUser.GroupID;
            return -1;            
        }
        
        public static UserGroup userGroup(this Guid sessionID)
        {
            return (UserGroup)sessionID.groupID();              
        }

        public static List<UserRole> userRoles(this Guid sessionID)
        {
            return UserRolesMappings.Mappings[sessionID.userGroup()];
        }
        
        public static bool isAdmin(this Guid sessionID)
        {
            return UserGroup.Admin == sessionID.userGroup();
        }        
    }

    public static class TeamMentorUserManagement_TMUser
    {
        public static UserGroup userGroup(this TMUser tmUser)
        {
            return (UserGroup)tmUser.GroupID;
        }

        public static List<UserRole> userRoles(this TMUser tmUser)
        {
            return UserRolesMappings.Mappings[tmUser.userGroup()];
        }
		public static List<String> toStringList(this List<UserRole> userRoles)
		{
			return (from role in userRoles
					select role.str()).toList();
		}
		
    }

  /*  public static class TeamMentorUserManagement_AuthenticationManagement
    {
        public static List<TMUser> users(this AuthenticationManagement authenticationManagement)
        {
            return authenticationManagement.GetAllUsers();
        }
        
        public static List<TMUser> users(this AuthenticationManagement authenticationManagement, UserType userType)            
        {
            return (from user in authenticationManagement.users()
                    where user.userType() == userType
                    select user).ToList();
        }

        public static List<TMUser> admins(this AuthenticationManagement authenticationManagement)
        {
            return authenticationManagement.users(UserType.Admin);
        }

        public static List<TMUser> readers(this AuthenticationManagement authenticationManagement)
        {
            return authenticationManagement.users(UserType.Reader);
        }

        public static List<TMUser> editors(this AuthenticationManagement authenticationManagement)
        {
            return authenticationManagement.users(UserType.Editor);
        }
    }
 */ 
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
