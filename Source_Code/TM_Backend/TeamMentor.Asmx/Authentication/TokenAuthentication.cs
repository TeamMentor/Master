using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TokenAuthentication
    {
        public TM_UserData userData;
        public TokenAuthentication()
        {
            userData = TM_UserData.Current;
        }
       
    }

    public static class TokenAuthentication_ExtensionMethods
    {
        public static Guid token(this AuthToken authToken)
        {
            if(authToken.notNull())
                return authToken.Token;
            return Guid.Empty;
        }        
        public static bool validToken(this TokenAuthentication tokenAuth , AuthToken authToken)
        {            
            return tokenAuth.validToken(authToken.token());
        }
        public static bool validToken(this TokenAuthentication tokenAuth , Guid token)
        {
            return tokenAuth.tmUser_From_AuthToken(token)
                            .notNull();            
        }
        public static TMUser tmUser_From_AuthToken(this TokenAuthentication tokenAuth ,  AuthToken authToken)
        {            
            return tokenAuth.tmUser_From_AuthToken(authToken.token());
        }
        public static TMUser tmUser_From_AuthToken(this TokenAuthentication tokenAuth , Guid token)
        {
            if(tokenAuth.isNull() || token == Guid.Empty)
                return null;
            return (from tmUser in tokenAuth.userData.tmUsers()
                    from authToken in tmUser.AuthTokens
                    where authToken.Token == token
                    select tmUser).first();
        }
        public static Guid   login_Using_AuthToken(this TokenAuthentication tokenAuth , Guid token, Guid sessionID)
        {
            var tmUser = tokenAuth.tmUser_From_AuthToken(token);
            if (tmUser.notNull())
            {
                if (sessionID.session_TmUser() == tmUser)
                    return sessionID;                       // the auth token matches the sssionId
                return tmUser.login("AuthToken");
            }
            return Guid.Empty;
        }        
        public static Guid login_Using_AuthToken(this Guid token)
        {
            return new TokenAuthentication().login_Using_AuthToken(token, Guid.Empty);
        }
        [Admin] public static  AuthToken add_AuthToken(this TMUser tmUser)
        {
            UserRole.Admin.demand();
            var authToken = new AuthToken
                                    {
                                        Token        = Guid.NewGuid(),
                                        CreationDate = DateTime.Now
                                    };
            tmUser.AuthTokens.add(authToken);
            return authToken;
        }
        [Admin] public static Guid createUserAuthToken(this TM_UserData userData , int userId)
        {
            UserRole.Admin.demand();            
            var tmUser = userId.tmUser();
            if(tmUser.notNull())
                return tmUser.add_AuthToken().Token;
            return Guid.Empty;
        }       
        [Admin] public static List<Guid> getUserAuthTokens(this TM_UserData userData , int userId)
        {
            UserRole.Admin.demand();
            var tmUser = userId.tmUser();
            if(tmUser.notNull())
                return (from authToken in tmUser.AuthTokens
                        select authToken.Token).toList();
            return new List<Guid>();
        }
        
    }
}
