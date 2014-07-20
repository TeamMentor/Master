using System;
using System.IO;
using FluentSharp.CoreLib;
using FluentSharp.Web;

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
        public Stream     Redirect_Login(string referer)
        {
            var target = "/Login?LoginReferer={0}".format(referer);
            this.redirect_To_Page(target);            
            return "Redirecting to Login Page...\n\n".stream_UFT8();            
        }
       
        [Admin] public Stream      Redirect_PasswordResetPage(string userId)
        {
            UserRole.Admin.demand();
            var tmUser = userId.toInt().tmUser();
            if (tmUser.notNull())
            {
                var token = TmWebServices.NewPasswordResetToken(tmUser.EMail);
                var url = "/passwordReset/{0}/{1}".format(tmUser.UserName, token);
                this.redirect_To_Page(url);                            
                return "Redirecting to Password Rest Page...\n\n".stream_UFT8();            
            }
            
            this.redirect_To_Page("/error");                    
            "[Redirect_PasswordResetPage] could not find user with user ID: {0}".error(userId);                
            return "Redirecting to Error Page...\n\n".stream_UFT8();                        
        }               
    }
}
