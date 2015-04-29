using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentSharp.CoreLib;
using TeamMentor.UserData;

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
		[Admin] public int              user_New   (TM_User user)
		{
            UserRole.Admin.demand();
			return TmWebServices.CreateUser(user.newUser());
		}
	    [Admin] public bool             user_Save  (TM_User user)
	    {
            UserRole.Admin.demand();
            return TmWebServices.UpdateTmUser(user);	        
	    }
        [Admin] public bool             user_Delete(string userId)
	    {
            UserRole.Admin.demand();
            if (userId.isInt())
                return TmWebServices.DeleteUser(userId.toInt());
            return false;
	    }
        [Admin] public List<string>             user_Verify(NewUser newUser)
	    {       
            UserRole.Admin.demand();
            return TmWebServices.CreateUser_Validate(newUser);
	    }
        [Admin] public int                      user_Create(NewUser newUser)
	    {      
            UserRole.Admin.demand();
            return TmWebServices.CreateUser(newUser);                                
	    }
        
	    [Admin] public TM_User			user(string userNameOrId    )
		{     
            UserRole.Admin.demand();
		    var user = TmWebServices.GetUser_byName(userNameOrId);      // need to this one first in case the username is an int
			if (user.notNull())
				return user;
		    return TmWebServices.GetUser_byID(userNameOrId.toInt());
		}
        [Admin] public TM_User          user_inDomain(string domain, string user)
        {
            UserRole.Admin.demand();
            var username = "{0}\\{1}".format(domain,user);
            return TmWebServices.GetUser_byName(username);
        }
		[Admin] public List<TM_User>	users(string usersIds)
		{
            UserGroup.Admin.demand();
			var ids = usersIds.split(",").Select((id) => id.toInt()).toList();
		    return TmWebServices.GetUsers_byID(ids);
		}				
		[Admin] public List<TM_User>	users()
		{
            UserRole.Admin.demand();
		    return TmWebServices.GetUsers();
		}
        [Admin] public string CreateCSVUsers(string payload)
        {
            UserRole.Admin.demand();
            //Since the verification is performed in another call, an authenticated user can execute this method and eventually bypass any validation
            //By performing the validationa again on user creation, we prevent this.
            var verification = VerifyUserData(payload);
            if (String.Compare(verification, "Success", System.StringComparison.OrdinalIgnoreCase) != 0)
            {
                return verification;
            }
            var users = payload.split("\n");
            
            var tmFileStorage = TmWebServices.tmFileStorage;            
            var userData      = tmFileStorage.UserData;           
           // var userData = xmlDatabase.UserData;
            var errorMessage = string.Empty;

            var emailAdmin_On_NewUsers_originalValue = TMConfig.Current.TMSecurity.EmailAdmin_On_NewUsers;
            TMConfig.Current.TMSecurity.EmailAdmin_On_NewUsers = false;

            foreach (var user in users)
            {
                var rawData = user.split(",");
                var userName = rawData[0] ?? "";
                var password = rawData[1] ?? "";
                var email = rawData[2] ?? "";
                var firstName = rawData[3] ?? "";
                var lastName = rawData[4] ?? "";
                var company = rawData[5] ?? "";
                var title = rawData[6] ?? "";
                var country = rawData[7] ?? "";
                var state = rawData[8] ?? "";
                var expiryDate = rawData[9] ?? "";
                var role = rawData[10] ?? "";
                var passwordExpire = rawData[11] ?? "";
                var accountNeverExpires = TMConfig.Current.TMSecurity.NewAccounts_DontExpire;
                var userEnabled = rawData[12] ?? "";
                //Safe check in case the user clicked several times.
                if (userName.tmUser().notNull())
                {
                    errorMessage = string.Format("Username {0} already exist.", userName);
                    break;
                }
                var userId = userData.newUser(userName, password,email);
                DateTime outputDate;
                DateTime.TryParse(expiryDate, out outputDate);
                if (userId > 0)
                {
                    var doesPwdExpire = passwordExpire.trim().ToLower() == "y";
                    var enabled = userEnabled.trim().ToLower() == "y";
                    userData.updateTmUser(userId, userName, firstName, lastName, title, company, email, country,
                                          state, outputDate, doesPwdExpire,
                                          enabled, accountNeverExpires, 
                                          int.Parse(role));
                }
                else
                {
                    TMConfig.Current.TMSecurity.EmailAdmin_On_NewUsers = emailAdmin_On_NewUsers_originalValue;
                    throw new Exception(String.Format("Failed to create user {0}", userName));
                }
            }
            TMConfig.Current.TMSecurity.EmailAdmin_On_NewUsers = emailAdmin_On_NewUsers_originalValue;
            return (String.IsNullOrEmpty(errorMessage) ? "Success" : errorMessage);
        }

        [Admin]public string VerifyUserData(string payload)
        {
            UserRole.Admin.demand();
            var users = payload.split("\n");            
            var userData      = TM_UserData.Current;   
            var errorMessage = string.Empty;
            var emails = new HashSet<string>();
            var usernames = new HashSet<string>();
            foreach (var user in users)
            {
                var rawData = user.split(",");
                //Safe check for
                if (rawData.count() < 13)
                {
                    errorMessage = string.Format("There is a missing field for user {0}.Please verify.", rawData[0] ?? "");
                    break;
                }
                var userName = rawData[0] ?? "";
                var password = rawData[1] ?? "";
                var email = rawData[2] ?? "";
                var firstName = rawData[3] ?? "";
                var lastName = rawData[4] ?? "";
                var company = rawData[5] ?? "";
                var title = rawData[6] ?? "";
                var country = rawData[7] ?? "";
                var state = rawData[8] ?? "";
                var expiryDate = rawData[9] ?? "";
                var role = rawData[10] ?? "";
                var passwordExpire = rawData[11] ?? "";
                var userEnabled = rawData[12] ?? "";
                var tmUser = new NewUser { Username = userName, Password = password, Company = company, Country = country, Email = email, Firstname = firstName, Lastname = lastName, GroupId = int.Parse(role), Note = "CSV user creation", State = state, Title = title };
                if (!usernames.Contains(userName))
                    usernames.Add(userName);
                else
                {
                    errorMessage = string.Format("Username {0} is already being used in this import.Please verify.", userName);
                    break; 
                }
                if (!emails.Contains(email.ToString()))
                    emails.Add(email);
                else
                {
                    errorMessage = string.Format("Email address {0} is already being used for another user in this import.Please verify.", email);
                    break;
                }
                //Check wether or not the user does exist.
                if (userName.tmUser().notNull())
                {
                    errorMessage = string.Format("Username {0} already exist.", userName);
                    break;
                }
                if (tmUser.valid_Email_Address().isFalse())
                {
                    errorMessage = TMConsts.DEFAULT_EMAIL_ADDRESS_IS_INVALID;
                    break;
                }
                if (tmUser.validate().Count > 0)
                {
                    errorMessage = string.Format("Please verify data for user {0}  :", userName);
                    errorMessage = tmUser.validate().Aggregate(errorMessage, (current, message) => current + " {0}".format(message.ErrorMessage));
                    break;
                }
                if (userData.TMUsers.Exists(x => x.EMail == email))
                {
                    errorMessage = string.Format("Email {0} already exist", email);
                    break;
                }
                if (firstName=="")
                {
                    errorMessage = string.Format("FirstName is a required field for user {0}", userName);
                    break;
                }
                if (lastName=="")
                {
                    errorMessage = string.Format("Last Name is a required field for user {0}", userName);
                    break;
                }
                DateTime outputDate;
                if (String.IsNullOrEmpty(expiryDate)||!DateTime.TryParse(expiryDate, out outputDate))
                {
                    errorMessage = string.Format("Please enter a valid Expiration date for user {0}. Format must be {1}.", userName, "yyyy/mm/dd");
                    break;
                }
                if (outputDate <= DateTime.Now)
                {
                    errorMessage = string.Format("Expiry date cannot be prior  or equal than today. User {0}", userName);
                    break;
                }
                if (passwordExpire.trim().ToLower() != "y" && passwordExpire.trim().ToLower() != "n")
                {
                    errorMessage = string.Format("Please verify data for user {0}, Password expire value must be Y (for yes) or N (for No)", userName);
                    break;
                }

                if (userEnabled.trim().ToLower() != "y" && userEnabled.trim().ToLower() != "n")
                {
                    errorMessage = string.Format("Please verify data for user {0}, User Enabled value must be Y (for yes) or N (for No)", userName);
                    break;
                }
                if (!Enum.IsDefined(typeof(UserGroup), int.Parse(role)))
                {
                    errorMessage =
                        string.Format("The group value set for user {0} is invalid. Valid groups are {1} {2} and {3}",
                                      userName, UserGroup.Admin, UserGroup.Editor, UserGroup.Reader);
                    break;
                }
            }
            return(String.IsNullOrEmpty(errorMessage) ? "Success" : errorMessage);
        }

        [Admin] public bool				DeleteUser(string userId)
		{
            UserRole.Admin.demand();
			return TmWebServices.DeleteUser(userId.toInt());
		}		
        [Admin] public bool             user_Update(TM_User user)
		{
            UserRole.Admin.demand();
			var groupId = -1; //not implemented for now
			return TmWebServices.UpdateUser(user.UserId, user.UserName, user.FirstName, user.LastName, user.Title, user.Company,user.Email, user.Country , user.State, user.ExpirationDate, user.PasswordExpired, user.UserEnabled , user.AccountNeverExpires, groupId);
		}
	}
}
