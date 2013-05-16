using System.Collections.Generic;
using System.Linq;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public partial class TM_REST
	{        
	    //RBAC
		public string			RBAC_CurrentIdentity_Name()
		{
		    var currentUser = TmWebServices.Current_User();
		    if (currentUser.notNull())
		        return currentUser.UserName;
		    return null;
		}
		public bool				RBAC_CurrentIdentity_IsAuthenticated()
		{
			return TmWebServices.RBAC_CurrentIdentity_IsAuthenticated();
		}
		public List<string>		RBAC_CurrentPrincipal_Roles()
		{
			return TmWebServices.RBAC_CurrentPrincipal_Roles();
		}
		public bool				RBAC_HasRole(string role)
		{
			return TmWebServices.RBAC_HasRole(role);
		}
		public bool				RBAC_IsAdmin()
		{
			return TmWebServices.RBAC_IsAdmin();
		}

        //Admin: User Management
		[Admin] public int              user_New(TM_User user)
		{
			return TmWebServices.CreateUser(user.newUser());
		}
	    [Admin] public bool user_Save(TM_User user)
	    {
	        return TmWebServices.UpdateUser(user.UserId, user.UserName, user.FirstName, 
                                            user.LastName, user.Title, user.Company, user.Email,
                                            user.Country, user.State, user.ExpirationDate, user.PasswordExpired, user.UserEnabled, -1);
	    }
	    [Admin] public TM_User			user(string userNameOrId)
		{
		    var user = TmWebServices.GetUser_byID(userNameOrId.toInt());
			if (user.notNull())
				return user;
		    return TmWebServices.GetUser_byName(userNameOrId);
		}
		[Admin] public List<TM_User>	users(string usersIds)
		{
			var ids = usersIds.split(",").Select((id) => id.toInt()).toList();
		    return TmWebServices.GetUsers_byID(ids);
		}				
		[Admin] public List<TM_User>	users()
		{
		    return TmWebServices.GetUsers();
		}
        [Admin] public bool				DeleteUser(string userId)
		{
			return TmWebServices.DeleteUser(userId.toInt());
		}		
        [Admin] public bool             user_Update(TM_User user)
		{
			var groupId = -1; //not implemented for now
			return TmWebServices.UpdateUser(user.UserId, user.UserName, user.FirstName, user.LastName, user.Title, user.Company,user.Email, user.Country , user.State, user.ExpirationDate, user.PasswordExpired, user.UserEnabled ,groupId);
		}
	}
}
