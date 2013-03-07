using System;
using O2.DotNetWrappers.ExtensionMethods;


namespace TeamMentor.CoreLib
{
    public static class TMUser_ExtensionMethods
    {
        public static bool      account_Enabled(this TMUser tmUser)
        {
            return tmUser.notNull() && tmUser.AccountStatus.UserEnabled;
        }

        public static TMUser   disable_Account(this TMUser tmUser)
        {
            if (tmUser.notNull())
            {
                tmUser.AccountStatus.UserEnabled = false;                
                tmUser.logUserActivity("Account was Disabled", "");
            }
            return tmUser;
        }
        public static TMUser   enable_Account(this TMUser tmUser)
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
            if (TMConfig.Current.TMSecurity.EvalAccounts_Enabled)
            return tmUser.AccountStatus.ExpirationDate < DateTime.Now &&
                    tmUser.AccountStatus.ExpirationDate != default(DateTime);
            return false;
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
            {
                tmUser.AccountStatus.ExpirationDate = DateTime.Now;
                10.sleep();
            }
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

        public static Guid      current_SingleUseLoginToken(this TMUser tmUser, bool reset = false)
        {
            if (reset || tmUser.SecretData.SingleUseLoginToken == Guid.Empty)
            {
                tmUser.SecretData.SingleUseLoginToken = Guid.NewGuid();
                tmUser.saveTmUser();
                tmUser.logUserActivity("SingleUseLoginToken Requested", "by asp.netSessionId: {0}".format(HttpContextFactory.Session.SessionID));
            }
            return tmUser.SecretData.SingleUseLoginToken;
        }

        public static bool sendPasswordReminder(this string email)
        {
            var tmUser = email.tmUser_FromEmail();
            if (tmUser.isNull())
                return false;
            var resetToken = email.current_PasswordResetToken();
            if (resetToken != Guid.Empty)
                return SendEmails.SendPasswordReminderToUser(tmUser, resetToken);
            return false;
        }

        public static Guid current_PasswordResetToken(this string email)
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
                if (reset || tmUser.SecretData.PasswordResetToken == Guid.Empty)
                {
                    tmUser.SecretData.PasswordResetToken = Guid.NewGuid();
                    tmUser.saveTmUser();                        
                }
                tmUser.logUserActivity("PasswordReset Requested","by asp.netSessionId: {0}".format(HttpContextFactory.Session.SessionID));
                return tmUser.SecretData.PasswordResetToken;                                    
                //tmUser.logUserActivity("Error:PasswordResetToken","wrong email used, by asp.netSessionId: {0}".format(HttpContextFactory.Session.SessionID));                                    
            }            
            return Guid.Empty;
        }

    }	
}