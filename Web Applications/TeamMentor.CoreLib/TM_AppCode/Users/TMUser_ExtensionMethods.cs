using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using O2.DotNetWrappers.ExtensionMethods;


namespace TeamMentor.CoreLib
{
    public static class TMUser_ExtensionMethods
    {
        public static bool      account_Enabled(this TMUser tmUser)
        {
            return tmUser.notNull() && tmUser.AccountStatus.UserEnabled;
        }

        public static TMUser    disable_Account(this TMUser tmUser)
        {
            if (tmUser.notNull())
            {
                tmUser.AccountStatus.UserEnabled = false;                
                tmUser.logUserActivity("Account was Disabled", "");
            }
            return tmUser;
        }
        public static TMUser    enable_Account(this TMUser tmUser)
        {
            if (tmUser.notNull())
            {
                tmUser.AccountStatus.UserEnabled = true;                
                tmUser.logUserActivity("Account was Enabled", "");
            }
            return tmUser;
        }
        public static bool      account_Expired    (this TMUser tmUser)
        {
            if (tmUser.isNull())
                return true;
            if (tmUser.AccountStatus.ExpirationDate == default(DateTime))   // if this ExpirationDate is not set, the user account is NOT expired
                return false;
            return tmUser.AccountStatus.ExpirationDate < DateTime.Now;      // if it is set, check if value is bigger than now
        }
        public static bool      password_Expired   (this TMUser tmUser)
        {
            if (tmUser.isNull())
                return true;
            return tmUser.AccountStatus.PasswordExpired;            
        }
        public static TMUser    expire_Account     (this TMUser tmUser)
        {
            if (tmUser.notNull())            
                tmUser.AccountStatus.ExpirationDate = DateTime.Now.AddMilliseconds(-10); // 10 miliseconds so that the value is in the past                            
            return tmUser;
        }
        public static TMUser    expire_Password    (this TMUser tmUser)
        {
            if (tmUser.notNull())
            {
                tmUser.AccountStatus.PasswordExpired = true;
                //10.sleep();
                tmUser.logUserActivity("Account was Expired", "");
            }
            return tmUser;
        }
        public static string    createPasswordHash (this TMUser tmUser, string password)
        {		    		                
            // first we hash the password with the user's name as salt (what used to happen before the )           
            var sha256Hash = tmUser.UserName.hash_SHA256(password);
            
            //then we create a PBKDF2 hash using the user's ID (as GUID) as salt
            var passwordHash = sha256Hash.hash_PBKDF2(tmUser.ID);

            return passwordHash;
        }

        /*public static Guid      current_SingleUseLoginToken(this TMUser tmUser, bool reset = false)
        {
            if (reset || tmUser.SecretData.SingleUseLoginToken == Guid.Empty)
            {
                tmUser.SecretData.SingleUseLoginToken = Guid.NewGuid();
                tmUser.saveTmUser();
                tmUser.logUserActivity("SingleUseLoginToken Requested", "by asp.netSessionId: {0}".format(HttpContextFactory.Session.SessionID));
            }
            return tmUser.SecretData.SingleUseLoginToken;
        }*/
        public static bool      sendPasswordReminder(this string email)
        {
            var tmUser = email.tmUser_FromEmail();
            if (tmUser.isNull())
                return false;
            var resetToken = email.passwordResetToken_getHash();
            if (resetToken != Guid.Empty)
                return SendEmails.SendPasswordReminderToUser(tmUser, resetToken);
            return false;
        } 
        
        /*public static Guid      current_PasswordResetToken(this string email)
        {
            var tmUser = email.tmUser_FromEmail();
            if (tmUser.notNull())
                return tmUser.current_PasswordResetToken();
            "[current_PasswordResetToken] failed for email= {0}".error(email);
            return Guid.Empty;
        }
        public static Guid      current_PasswordResetToken(this TMUser tmUser, bool reset = false)
        {
            if (tmUser.notNull())
            {
                if (reset || tmUser.SecretData.PasswordResetToken.isNull())
                {
                    var passwordResetToken = tmUser.new_PasswordResetToken();                    
                }
                tmUser.logUserActivity("PasswordReset Requested","by asp.netSessionId: {0}".format(HttpContextFactory.Session.SessionID));
                return tmUser.SecretData.PasswordResetToken;                                    
                //tmUser.logUserActivity("Error:PasswordResetToken","wrong email used, by asp.netSessionId: {0}".format(HttpContextFactory.Session.SessionID));                                    
            }            
            return Guid.Empty;
        }*/
        public static bool passwordResetToken_isValid(this TMUser tmUser, Guid resetToken)
        {
            if(tmUser.notNull())
                if (tmUser.SecretData.PasswordResetToken == tmUser.passwordResetToken_getHash(resetToken))
                    return true;
            return false;
        }
        
        public static string passwordResetToken_getHash(this TMUser tmUser, Guid resetToken)
        {
            if(tmUser.notNull())                
                return tmUser.UserName.hash_PBKDF2(resetToken);
            return null;
        }
        public static Guid passwordResetToken_getHash(this string email)
        {
            var tmUser = email.tmUser_FromEmail();
            if (tmUser.notNull())
                return tmUser.passwordResetToken_getTokenAndSetHash();
            return Guid.NewGuid();
        }
        public static Guid passwordResetToken_getTokenAndSetHash(this TMUser tmUser)
        {
            if (tmUser.notNull())
            {
                var newToken = Guid.NewGuid();
                tmUser.SecretData.PasswordResetToken = tmUser.passwordResetToken_getHash(newToken);
                tmUser.saveTmUser();
                return newToken;
            }
            return Guid.Empty;
        }
        public static string    passwordExpiredUrl(this TM_User user)
        {
            if (user.notNull())
            {
                var tmUser = user.UserName.tmUser();
                if (tmUser.notNull())
                    if (tmUser.password_Expired())
                    {
                        tmUser.logUserActivity("Password Expired", "Created password Reset Url for user");
                        return "/passwordReset/{0}/{1}".format(user.UserName, tmUser.passwordResetToken_getTokenAndSetHash());
                    }                                    
            }
            return "/error";
        }

        public static TMUser    set_UserGroup       (this TMUser tmUser, UserGroup userGroup)
        {
            if (tmUser.notNull())
                tmUser.GroupID = (int)userGroup;
            return tmUser;
        }
        public static TMUser    make_Admin          (this TMUser tmUser)
        {
            return tmUser.set_UserGroup(UserGroup.Admin);            
        }
        public static TMUser    make_Editor          (this TMUser tmUser)
        {
            return tmUser.set_UserGroup(UserGroup.Editor);            
        }        
        public static TMUser    make_Reader          (this TMUser tmUser)
        {
            return tmUser.set_UserGroup(UserGroup.Reader);            
        }
        public static TMUser    make_Anonymous          (this TMUser tmUser)
        {
            return tmUser.set_UserGroup(UserGroup.Anonymous);            
        }
    }

    public static class TM_User_ExtensionMethod_Validation
    {
        
    }

    public static class DataContracts_ExtensionMethods
    {
        public static List<ValidationResult>           validate             (this object objectTovalidate)
        {
            var results = new List<ValidationResult>();
            if (objectTovalidate.notNull())
            {
                var context = new ValidationContext(objectTovalidate, null, null);
                Validator.TryValidateObject(objectTovalidate, context, results, true);
            }
            return results;
        }
        public static bool                             validation_Ok        (this object objectTovalidate)
        {
            return objectTovalidate.validate().empty();
        }
        public static bool                             validation_Failed    (this object objectTovalidate)
        {
            return objectTovalidate.validate().notEmpty();
        }
        public static Dictionary<string, List<string>> indexed_By_MemberName(this List<ValidationResult> validationResults)
        {
            var mappedData = new Dictionary<string, List<string>>();
            foreach(var validationResult in validationResults)
                foreach (var memberName in validationResult.MemberNames)
                    mappedData.add(memberName, validationResult.ErrorMessage);  
            return mappedData;
        }
    }
}