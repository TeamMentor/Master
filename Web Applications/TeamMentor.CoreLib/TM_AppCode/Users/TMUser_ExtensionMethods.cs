using System;
using System.Linq;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using Encoder = Microsoft.Security.Application.Encoder;

namespace TeamMentor.CoreLib
{
    public static class TMUser_ExtensionMethods
    {
        public static bool      account_Expired(this TMUser tmUser)
        {
            return false;
        }   

        public static string    createPasswordHash(this TMUser tmUser, string password)
        {		    		                
            // first we hash the password with the user's name as salt (what used to happen before the )           
            var sha256Hash = tmUser.UserName.hash_SHA256(password);
            
            //then we create a PBKDF2 hash using the user's ID (as GUID) as salt
            var passwordHash = sha256Hash.hash_PBKDF2(tmUser.ID);

            return passwordHash;
        }
    }	
}