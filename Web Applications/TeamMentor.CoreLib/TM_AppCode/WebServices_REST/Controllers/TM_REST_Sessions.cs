using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public partial class TM_REST
    {
        //Login
        public string	SessionId()
        {			
            return Session.SessionID;			
        }
        public Guid		Login(string username, string password)
        {
            return TmWebServices.Login(username, password);
        }
        public Guid		Login_using_Credentials(TM_Credentials credentials)
        {
            return TmWebServices.Login(credentials.UserName, credentials.Password);
        }
        public Guid		Logout()
        {
            return TmWebServices.Logout();
        }
        public bool     User_LoggedIn()
        {
            return TmWebServices.Current_User().notNull();
        }

        [TM_Admin]
        public List<string> ActiveSessions()
        {
            return TmWebServices.GetActiveSessions().toStringList();
        }
        public TMUser ActiveSession(string sessionId)
        {
            return sessionId.isGuid() ? TmWebServices.GetActiveSession(sessionId.guid()) : null;
        }
    }
}
