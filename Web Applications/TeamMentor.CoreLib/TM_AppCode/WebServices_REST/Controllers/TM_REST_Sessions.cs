using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public partial class TM_REST
    {
        //Login
        public string	SessionId()
        {			
            if (Session.notNull())
                return Session.SessionID;
            return "";
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
        public void     Redirect_Login(string referer)
        {
            HttpContextFactory.Response.Redirect("/Login?LoginReferer={0}".format(referer));
        }
/*        [Admin] public Guid     GetLoginToken(string username)
        {
            return TmWebServices.GetLoginToken(username);
        }        
        [Admin] public bool     SendLoginTokenForUser(string userName)
        {
            var tmUser = userName.tmUser();
            if(tmUser.notNull())
                return SendEmails.SendLoginTokenToUser(tmUser);
            return true;
        }*/
        
        /*public void     Redirect_ToPasswordReset(string email)
        {
            var redirectUrl = "/PasswordReset/{0}".format(SetPasswordResetToken(email));
            HttpContextFactory.Response.Redirect(redirectUrl);
        }*/
/*        [Admin] public void     Redirect_After_Login_Using_Token(string username, string loginToken)
        {
            var sessionId = TmWebServices.Login_Using_LoginToken(username, loginToken.guid());
            if (sessionId != Guid.Empty)
                HttpContextFactory.Response.Redirect("/");
            else            
                HttpContextFactory.Response.Redirect("/error");            
        }*/        
        [Admin] public void      Redirect_PasswordResetPage(string userId)
        {
            var tmUser = userId.toInt().tmUser();
            if (tmUser.notNull())
            {
                var token = TmWebServices.NewPasswordResetToken(tmUser.EMail);
                var url = "/passwordReset/{0}/{1}".format(tmUser.UserName, token);
                HttpContextFactory.Response.Redirect(url);
            }
            else
            {
                "[Redirect_PasswordResetPage] could not find user with user ID: {0}".error(userId);
                HttpContextFactory.Response.Redirect("/error");
            }
        }               
    }
}
