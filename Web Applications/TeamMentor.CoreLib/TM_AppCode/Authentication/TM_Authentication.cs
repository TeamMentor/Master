using System;
using System.Security.Principal;
using System.Threading;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_Authentication
    {        
        public static bool          Global_Disable_Csrf_Check   { get; set; }    
        public  TM_WebServices      TmWebServices               { get; set; }    
        public  bool                Disable_Csrf_Check          { get; set; }    
        public WindowsIdentity      Current_WindowsIdentity     { get; set; }

        public TM_Authentication    (TM_WebServices tmWebServices)
        {
            TmWebServices = tmWebServices;
            Disable_Csrf_Check = false;	
            Current_WindowsIdentity = WindowsIdentity.GetCurrent();
        }        
        
        //properties
        public Guid                 sessionID
        {
            get
            {                
                // first check if there s a session variable already set                
/*                if (HttpContextFactory.Session.notNull() && HttpContextFactory.Session["sessionID"].notNull() && HttpContextFactory.Session["sessionID"] is Guid)
                {
                    var sessionGuid = (Guid)HttpContextFactory.Session["sessionID"];
                    if(sessionGuid != Guid.Empty)
                        return sessionGuid;
                }*/

                // then check the cookie
                var sessionCookie = HttpContextFactory.Request.cookie("Session");
                if (sessionCookie.notNull() && sessionCookie.isGuid())
                    return sessionCookie.guid();

                var sessionHeader = HttpContextFactory.Request.header("Session");
                if (sessionHeader.notNull() && sessionHeader.isGuid())
                    return sessionHeader.guid();

                //if none is set, return an empty Guid	
                return Guid.Empty;                
            }
            set
            {                
                var previousSessionId = sessionID;
             
//                if (HttpContextFactory.Session.notNull())                
//                    HttpContextFactory.Session["sessionID"] = value;

                HttpContextFactory.Response.set_Cookie("Session", value.str()).httpOnly();
                HttpContextFactory.Request .set_Cookie("Session", value.str()).httpOnly();   
             
                if (value == Guid.Empty)
                {
                    UserGroup.Anonymous.setThreadPrincipalWithRoles();          // ensure that from now on the current user as no more privileges
                    previousSessionId.logout();				                    // and that the previous session IS is logged out
                }		
                else    
                    new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(value);
            }
        }
        public Guid                 authToken
        {
            get
            {
                var authValue =  HttpContextFactory.Request.QueryString[TMConsts.AUTH_TOKEN_REQUEST_VAR_NAME];
                if (authValue.notNull() && authValue.isGuid())
                    return authValue.guid();
                return Guid.Empty;
            }   
        }
        public TMUser               currentUser
        {
            get
            {

                var tmUser = sessionID.session_TmUser();
                if (tmUser.notNull())
                    tmUser.SecretData.CSRF_Token = sessionID.csrfToken();	
                return tmUser;
            }
        }

        //methods
        public bool                 check_CSRF_Token()
        {
            if (Global_Disable_Csrf_Check)
            {
                "[TM_Authentication] Global_Disable_Csrf_Check was set".error();
                return true;
            }
            if (Disable_Csrf_Check)
                return true;
            var header_Csrf_Token = HttpContextFactory.Context.Request.Headers["CSRF-Token"];
            
            if (header_Csrf_Token != null && header_Csrf_Token.valid())
            {            
                if (header_Csrf_Token == sessionID.csrfToken())		
                    return true;
            }            
            return false;            
        }
        public TM_Authentication    mapUserRoles()
        {
            return mapUserRoles(false);
        }
        public TM_Authentication    mapUserRoles(bool disable_Csrf_Check)           // todo: rename to something like logging request
        {
            Disable_Csrf_Check = disable_Csrf_Check;            

            // check if there is an AuthToken in the current request, then try WindowsAuthentication (if enabled)
            if (authToken != Guid.Empty)
            {
                sessionID = new TokenAuthentication().login_Using_AuthToken(authToken, sessionID);
            }            
            else if (TMConfig.Current.WindowsAuthentication.Enabled)
                if (sessionID == Guid.Empty || sessionID.validSession() == false)
                {                
            
                    sessionID = new WindowsAuthentication().login_Using_WindowsAuthentication(Current_WindowsIdentity);
                }            
            
            //if there is a valid session maps its permissions
            var userGroup = UserGroup.None;
            
            if (sessionID != Guid.Empty)
            {                
                if (check_CSRF_Token())		// only map the roles if the CSRF check passed
                {                    
                    userGroup = new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(sessionID);					
                }                
            }            
            if (userGroup == UserGroup.None)
            {
                if (TMConfig.Current.TMSecurity.Show_ContentToAnonymousUsers)
                    UserGroup.Reader.setThreadPrincipalWithRoles();
                else
                    UserGroup.Anonymous.setThreadPrincipalWithRoles();
            }            
            //var userRoles = Thread.CurrentPrincipal.roles().toList().join(",");            
            if (HttpContextFactory.Session.notNull())
            {                
                HttpContextFactory.Session["principal"] = Thread.CurrentPrincipal;
            }
            return this;
        }
        public Guid                 logout()
        {
            sessionID = Guid.Empty;
            return sessionID;
        }
    }
}