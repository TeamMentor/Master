using System;
using O2.DotNetWrappers.ExtensionMethods;


namespace TeamMentor.CoreLib
{
    public static class TMUser_ExtensionMethods
    {
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
                10.sleep();
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

        public static Guid      current_SingleUseLoginToken(this TMUser tmUser)
        {
            if (tmUser.SecretData.SingleUseLoginToken == Guid.Empty)
                tmUser.SecretData.SingleUseLoginToken = Guid.NewGuid();
            return tmUser.SecretData.SingleUseLoginToken;
        }

        public static Guid      current_PasswordResetToken(this TMUser tmUser)
        {
            if (tmUser.SecretData.PasswordResetToken == Guid.Empty)
                tmUser.SecretData.PasswordResetToken = Guid.NewGuid();
            return tmUser.SecretData.PasswordResetToken;
        }

    }	
}