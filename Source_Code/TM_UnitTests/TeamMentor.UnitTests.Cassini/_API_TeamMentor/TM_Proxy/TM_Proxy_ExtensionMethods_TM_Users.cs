using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_TM_Users
    {                        
        public static List<TMUser>              users(this TM_Proxy tmProxy)
        {
            if(tmProxy.TmUserData.notNull())
                return tmProxy.TmUserData.TMUsers;
            return new List<TMUser>();
        }
        
        public static AuthToken                 user_AuthToken_Add(this TM_Proxy tmProxy, TMUser tmUser)
        {
            //var authToken = tmProxy.invoke_Static(typeof(TokenAuthentication_ExtensionMethods), "add_AuthToken", tmUser);

            return tmProxy.invoke_Static<AuthToken>(typeof(TokenAuthentication_ExtensionMethods), "add_AuthToken", tmUser);
        }

        public static Guid                      user_AuthToken_Valid(this TM_Proxy tmProxy, TMUser tmUser)
        {
            if(tmUser.AuthTokens.empty())
                return tmProxy.user_AuthToken_Add(tmUser).Token;
            return tmUser.AuthTokens.first().Token;
        }

        public static TM_Proxy    admin_Assert(this TM_Proxy tmProxy)
        {
            UserGroup.Admin.assert();
            return tmProxy;
        }

        public static TM_Proxy   editor_Assert(this TM_Proxy tmProxy)
        {
            UserGroup.Editor.assert();
            return tmProxy;
        }
    }
}