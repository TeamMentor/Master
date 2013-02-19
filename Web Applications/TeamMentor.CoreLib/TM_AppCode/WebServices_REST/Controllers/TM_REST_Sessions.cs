using System;
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
        public Guid     GetLoginToken(string username)
        {
            return TmWebServices.GetLoginToken(username);
        }
        public Guid     Login_Using_LoginToken(string username, string loginToken)
        {
            return TmWebServices.Login_Using_LoginToken(username, loginToken.guid());            
        }
    }
}
