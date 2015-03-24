using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UserData
{
    public static class Users_Creation
    {
        public static int           createDefaultAdminUser      (this TM_UserData userData)
        {  
            try
            { 
                //var tmServer = userData.tmServer();

                if (userData.isNull())
                    return -1;
            
                //if(userData.tmServer().Users_Create_Default_Admin.isFalse())
                //    return -1;

                var tmConfig = TMConfig.Current;
                lock (tmConfig)
                {                
                    var defaultAdminUser_Name  = tmConfig.TMSecurity.Default_AdminUserName;
                    var defaultAdminUser_Pwd   = tmConfig.TMSecurity.Default_AdminPassword;                
                    var defaultAdminUser_Email = tmConfig.TMSecurity.Default_AdminEmail;                
                    var adminUser              = userData.tmUser(defaultAdminUser_Name);
                
                    if (adminUser.notNull())
                    {
                        if (adminUser.SecretData.PasswordHash.notValid() || tmConfig.OnInstallation.ForceDefaultAdminPassword)
                        {
                            "[createDefaultAdminUser] reseting password since passwordHash was not valid and ForceDefaultAdminPassword was set".error();
                            adminUser.SecretData.PasswordHash = adminUser.createPasswordHash(defaultAdminUser_Pwd);                                                
                            adminUser.event_User_Updated(); //adminUser.saveTmUser();                                              
                        }
                        if (adminUser.GroupID != (int) UserGroup.Admin)
                        {
                            "[createDefaultAdminUser] admin user was not admin (changed to admin)".error();
                            adminUser.GroupID = (int) UserGroup.Admin;
                            adminUser.event_User_Updated(); //adminUser.saveTmUser();
                        }                    
                        return adminUser.UserID;
                    }				
                    "[createDefaultAdminUser] admin user didn't exist (creating it)".debug();
                    var userId = userData.newUser(defaultAdminUser_Name, defaultAdminUser_Pwd,defaultAdminUser_Email,1);
                    adminUser = userId.tmUser();
                    adminUser.AccountStatus.ExpirationDate = DateTime.Now.AddYears(10);        // default to setting the expiry value to 10 years in the future
                    adminUser.event_User_Updated();  //adminUser.saveTmUser();
                    return userId;
                }   
            }
            catch(Exception ex)
            {
                ex.log("[TM_UserData][createDefaultAdminUser]");
                return -1;
            }
        }        
        public static TMUser        createUser                  (this string userName)
        {
            return userName.newUser().tmUser();
        }
        public static int           newUser                     (this string userName)
        {
            return TM_UserData.Current.newUser(userName);
        }
        public static TMUser        createUser                  (this TM_UserData userData)
        {
            return userData.newUser().tmUser();
        }
        public static int           newUser                     (this TM_UserData userData)
        {
            return userData.newUser("test_user_{0}".format(5.randomLetters()));
        }        
        public static int           newUser                     (this TM_UserData userData, string  username)
        {
            return userData.newUser(username, 5.randomLetters());
        }                
        public static int           newUser                     (this TM_UserData userData, string  username, string password)
        {
            var randomEmail = "{0}@{1}.{2}".format(7.randomLetters(), 5.randomLetters(), 2.randomLetters()).lower();
            return userData.newUser(username,password, randomEmail,0);
        }        
        public static int           newUser                     (this TM_UserData userData, string  username, string password, string email)
        {
            return userData.newUser(username,password, email,0);
        }        
        public static int           newUser                     (this TM_UserData userData, string  username, string password, string email, int groupId)
        {
            return userData.newUser(username, password, email,"...","...","...", "...","...","...","...",null,groupId);
        }        
        public static int           newUser                     (this TM_UserData userData, string  username, string password, string email, string firstname, string lastname, string note , string title, string company, string country, string state, List<UserTag> userTags , int groupId)
        {			
            if (userData.isNull())
                return -1;

            var userId = Math.Abs(Guid.NewGuid().hash()); 
            
            "Creating new user: {0} with id {1}".debug(username, userId);
            
            if (groupId <1)						//set default user type						
                groupId = 2;					//by default new users are of type 2 (i.e. Reader)
            else
                UserRole.ManageUsers.demand();	// only users with UserRole.ManageUsers should be able to create non-default users
                
            var tmUser = new TMUser {
                UserID 		 = userId,
                UserName 	 = username,
                FirstName 	 = firstname,
                LastName 	 = lastname,
                Company 	 = (company),
                Country 	 = country,
                State 	     = state,
                GroupID 	 = groupId,
                Title 		 = title, 										
                EMail 		 = email ?? "",    
                UserTags     = userTags 
            };
            
            var tmConfig = TMConfig.Current;
            tmUser.AccountStatus.UserEnabled    = tmConfig.newAccountsEnabled();
            tmUser.AccountStatus.ExpirationDate = tmConfig.currentExpirationDate();

            tmUser.SecretData.PasswordHash = tmUser.createPasswordHash(password);            
            userData.TMUsers.Add(tmUser);            
        
            if (TMConfig.Current.windowsAuthentication_Enabled().isFalse())                
                SendEmails.SendNewUserEmails("New user created: {0}".format(tmUser.UserName), tmUser);
            
            tmUser.logUserActivity("New User",  "");    // this will trigger tmUser.event_User_Updated();
                        
            //tmUser.event_User_Updated(); //tmUser.saveTmUser();
            //userData.triggerGitCommit();
            return userId;    		
        }			
        public static int           create                      (this NewUser newUser)
        {
            return TM_UserData.Current.createTmUser(newUser);
        }
        public static Signup_Result createSigupResponse(this NewUser newUser)
        {
            return TM_UserData.Current.createTmUserResponse(newUser);
        }

        public static int           createTmUser                (this TM_UserData userData, NewUser newUser)
        {
            if (newUser.isNull())
                return 0;
            
            // ensure the email is lowercase (will fail validation otherwise)
            newUser.Email = newUser.Email.lower();              

            //validate user against the DataContract specificed in the NewUser class
            if (newUser.validation_Failed())        
                return 0;            
            if(newUser.UserTags.notEmpty())
                foreach(var userTag in newUser.UserTags)
                    if(userTag.validation_Failed())
                        return 0;
            // if there is a groupId provided we must check if the user has the manageUsers Priviledge							
            if (newUser.GroupId !=0)		 
                UserRole.ManageUsers.demand();		

            // Check if there is already a user with the provided username or email
            if (newUser.Username.tmUser().notNull() || newUser.Email.tmUser_FromEmail().notNull())
            {
                userData.logTBotActivity("User Creation Fail","Username ('{0}') or Email ('{1})already existed".format(newUser.Username, newUser.Email));
                return 0;  
            }

            // Create user      
            
            return userData.newUser(newUser.Username, newUser.Password, newUser.Email, newUser.Firstname, newUser.Lastname, newUser.Note, newUser.Title, newUser.Company, newUser.Country, newUser.State, newUser.UserTags,newUser.GroupId);						
        }

        public static Signup_Result createTmUserResponse(this TM_UserData userData, NewUser newUser)
        {
            var sigupResponse = new Signup_Result();
            var tmConfig = TMConfig.Current;
            if (newUser.isNull())
            {
                userData.logTBotActivity("User Creation Fail", "TEAM Mentor user is null");
                sigupResponse.Signup_Status = Signup_Result.SignupStatus.Signup_Error;
                sigupResponse.UserCreated = 0;
                sigupResponse.Simple_Error_Message = tmConfig.TMErrorMessages.General_SignUp_Error_Message;
                return sigupResponse;
            }
            // ensure the email is lowercase (will fail validation otherwise)
            newUser.Email = newUser.Email.lower();
            //Email check (length, null, valid)
            if (newUser.Email.notNull() && newUser.Email.Length > 256)
            {
                userData.logTBotActivity("User Creation Fail", "Input rejected because email address ('{0}') is larger than 256 characters".format(newUser.Email));
                return ValidateEmailPattern(tmConfig);
            }
            //Check email format
            if (newUser.valid_Email_Address().isFalse())
            {
                return ValidateEmailPattern(tmConfig);
            }

            //Validate Password Length.
            if (newUser.Password.Length < 8 || newUser.Password.Length> 256)
            {
                userData.logTBotActivity("User Creation Fail", "Password must be 8 to 256 character long but was {0}".format(newUser.Password.Length));
                return ValidatePasswordLength(tmConfig);
            }
            //Password complexity
            if (!Regex.IsMatch(newUser.Password, ValidationRegex.PasswordComplexity))
            {
                return ValidatePasswordComplexity(tmConfig);
            }
            //validate user against the DataContract specificed in the NewUser class
            if (newUser.validation_Failed())
            {
                return ValidationFailed(tmConfig, newUser);
            }

            if (newUser.UserTags.notEmpty())
                foreach (var userTag in newUser.UserTags)
                    if (userTag.validation_Failed())
                    {
                        return UserTags_Validation(tmConfig, userTag);
                    }
            // if there is a groupId provided we must check if the user has the manageUsers Priviledge							
            if (newUser.GroupId != 0)
                UserRole.ManageUsers.demand();

            // Check if there is already a user with the provided username or email
            if (newUser.Username.tmUser().notNull())
            {
                userData.logTBotActivity("User Creation Fail", "Username ('{0}') already existed".format(newUser.Username));
                return ValidateUserName(tmConfig);
            }

            if (newUser.Email.tmUser_FromEmail().notNull())
            {
                userData.logTBotActivity("User Creation Fail", "Email ('{0}') already existed".format(newUser.Email));
                return ValidateEmail(tmConfig);
            }
            
            var userCreated = userData.newUser(newUser.Username, newUser.Password, newUser.Email, newUser.Firstname, newUser.Lastname, newUser.Note, newUser.Title, newUser.Company, newUser.Country, newUser.State, newUser.UserTags, newUser.GroupId);
            sigupResponse.UserCreated = userCreated;
            sigupResponse.Signup_Status = Signup_Result.SignupStatus.Signup_Ok;
            return sigupResponse;
        }

        #region Sigup validations, shorter and specialized methods
        private static Signup_Result ValidateEmailPattern(TMConfig config)
        {
            var sigupResponse = new Signup_Result
            {
                Signup_Status = Signup_Result.SignupStatus.Validation_Failed,
                UserCreated = 0
            };
            var errorMessage = TMConfig.Current.TMErrorMessages.Email_Address_Is_Invalid;
            if (config.showDetailedErrorMessages())
            {
                sigupResponse.Validation_Results.Add(new Validation_Results { Field = "Email", Message = errorMessage });
            }
            else
                sigupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;

            return sigupResponse;
        }

        private static Signup_Result ValidatePasswordComplexity(TMConfig config)
        {
            var sigupResponse = new Signup_Result
            {
                Signup_Status = Signup_Result.SignupStatus.Validation_Failed,
                UserCreated = 0
            };
            var errorMessage = TMConfig.Current.TMErrorMessages.PasswordComplexityErroMessage;
            if (config.showDetailedErrorMessages())
            {
                sigupResponse.Validation_Results.Add(new Validation_Results { Field = "Password", Message = errorMessage });
            }
            else
                sigupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;

            return sigupResponse;
        }
        private static Signup_Result ValidatePasswordLength(TMConfig config)
        {
            var sigupResponse = new Signup_Result
            {
                Signup_Status = Signup_Result.SignupStatus.Validation_Failed,
                UserCreated = 0
            };
            var errorMessage = TMConfig.Current.TMErrorMessages.PasswordLengthErrorMessage;
            if (config.showDetailedErrorMessages())
            {
                sigupResponse.Validation_Results.Add(new Validation_Results { Field = "Password", Message = errorMessage });
            }
            else
                sigupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;

            return sigupResponse;
        }
        private static Signup_Result ValidationFailed(TMConfig config, NewUser newUser)
        {
            var sigupResponse = new Signup_Result();
            var validationList = newUser.validate();
            sigupResponse.Signup_Status = Signup_Result.SignupStatus.Validation_Failed;
            sigupResponse.UserCreated = 0;
            if (config.showDetailedErrorMessages())
            {
                foreach (var validation in validationList)
                {
                    var field = validation.MemberNames.FirstOrDefault();

                    sigupResponse.Validation_Results.Add(new Validation_Results
                    {
                        Field = field,
                        Message = validation.ErrorMessage
                    });
                }
            }
            else
                sigupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;
            return  sigupResponse;
        }

        private static Signup_Result UserTags_Validation(TMConfig config, UserTag tag)
        {
            var signupResponse = new Signup_Result();
            var validationList = tag.validate();
            signupResponse.Signup_Status = Signup_Result.SignupStatus.Validation_Failed;
            signupResponse.UserCreated = 0;
            if (!config.showDetailedErrorMessages())
            {
                signupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;
            }
            else
            {
                foreach (var validation in validationList)
                {
                    signupResponse.Validation_Results.Add(new Validation_Results { Field = validation.ToString(), Message = validation.ErrorMessage });
                }
            }
            return signupResponse;
        }

        private static Signup_Result ValidateEmail(TMConfig config)
        {
            var signupResponse = new Signup_Result
            {
                Signup_Status = Signup_Result.SignupStatus.Validation_Failed,
                UserCreated = 0
            };

            if (config.showDetailedErrorMessages())
            {
                signupResponse.Validation_Results.Add(new Validation_Results { Field = "Email", Message = TMConfig.Current.TMErrorMessages.SignUpEmailAlreadyExist });
            }
            else
            {
                signupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;
            }

            return signupResponse;
        }

        private static Signup_Result ValidateUserName(TMConfig config)
        {
            var signupResponse = new Signup_Result
            {
                Signup_Status = Signup_Result.SignupStatus.Validation_Failed,
                UserCreated = 0
            };
            if (config.showDetailedErrorMessages())
            {
                signupResponse.Validation_Results.Add(new Validation_Results { Field = "Username", Message = TMConfig.Current.TMErrorMessages.SignUpUsernameAlreadyExist });
            }
            else
            {
                signupResponse.Simple_Error_Message = config.TMErrorMessages.General_SignUp_Error_Message;
            }

            return signupResponse;
        }
        #endregion
        public static List<int>     createTmUsers               (this TM_UserData userData, string batchUserData) 
        {						
            if (batchUserData.valid().isFalse())
                return new List<int>();
            var newUsers = new List<NewUser>();
            foreach(var line in batchUserData.fix_CRLF().split_onLines())
            {
                var newUser = new NewUser();
                //return _newUser;
                var items = line.split(",");
                
                newUser.Username    = items.size()>0 ? items[0].trim() : "";	 
                newUser.Password    = items.size()>1 ? items[1].trim() : "";	 
                newUser.Firstname   = items.size()>2 ? items[2].trim() : "";	 
                newUser.Lastname    = items.size()>3 ? items[3].trim() : "";	 
                newUser.GroupId     = items.size()>4 ? items[4].trim().toInt() : 0;

                //default values 
                newUser.Company = "...";
                newUser.Country = "...";
                newUser.Email   = "{0}@randomm.xyz".format(10.randomLetters());
                newUser.Note    = "(Batch user created)";
                newUser.State   = "...";
                newUser.Title   = "...";
                if (newUser.validation_Failed())
                {
                    "[createTmUsers] failed validation for user data:{0}".error(newUser.toXml());
                    newUsers.Add(null);
                }
                else
                    newUsers.Add(newUser);
            } 
            return userData.createTmUsers(newUsers);
        }

        [ManageUsers]   
        public static List<int>     createTmUsers               (this TM_UserData userData, List<NewUser> newUsers)
        {
            UserRole.ManageUsers.demand();
            if(newUsers.isNull())
                return new List<int>();
            return newUsers.Select(newUser => userData.createTmUser(newUser)).toList();
        }
    
        public static NewUser       with_Random_Data            (this NewUser newUser)
        {
            foreach(var name in newUser.property_Values_AsStrings().keys())
                newUser.property(name,"!!10Ma".add_5_RandomLetters());
            newUser.Email = "testUser".random_Email();
            newUser.UserTags = new List<UserTag>();
            newUser.UserTags.add(new UserTag {Key = "key".add_5_RandomLetters(), Value = "value".add_5_RandomLetters()});
            return newUser;
        }
        public static bool valid_Email_Address(this NewUser tmUser)
        {
            if (tmUser.Email.isNull())
                return false;
            try
            {
                var address = new System.Net.Mail.MailAddress(tmUser.Email);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
