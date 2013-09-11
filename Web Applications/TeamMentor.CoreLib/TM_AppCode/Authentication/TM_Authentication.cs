using System;
using System.Threading;
using System.Web;
using FluentSharp;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class TM_Authentication
    {        
        public static bool          Global_Disable_Csrf_Check   { get; set; }    
        public  TM_WebServices      TmWebServices               { get; set; }    
        public  bool                Disable_Csrf_Check          { get; set; }    

        public TM_Authentication    (TM_WebServices tmWebServices)
        {
            TmWebServices = tmWebServices;
            Disable_Csrf_Check = false;	
        }        

        public Guid                 sessionID
        {
            get
            {                
                // first check if there s a session variable already set                
                if (HttpContextFactory.Session.notNull() && HttpContextFactory.Session["sessionID"].notNull() && HttpContextFactory.Session["sessionID"] is Guid)
                    return (Guid)HttpContextFactory.Session["sessionID"];

                // then check the cookie
                var sessionCookie = HttpContextFactory.Request.Cookies["Session"];
                if (sessionCookie.notNull() && sessionCookie.value().isGuid())
                    return sessionCookie.value().guid();

                var sessionHeader = HttpContextFactory.Request.Headers["Session"];
                if (sessionHeader.notNull() && sessionHeader.isGuid())
                    return sessionHeader.guid();

                //if none is set, return an empty Guid	
                return Guid.Empty;                
            }
            set
            {                
                var previousSessionId = sessionID;
             
                if (HttpContextFactory.Session.notNull())                
                    HttpContextFactory.Session["sessionID"] = value;

                HttpContextFactory.Response.set_Cookie("Session", value.str()).httpOnly();
                HttpContextFactory.Request .set_Cookie("Session", value.str()).httpOnly();   
             
                if (value == Guid.Empty)
                {
                    UserGroup.Anonymous.setThreadPrincipalWithRoles();          // ensure that from now on the current user as no more privileges
                    previousSessionId.logout();				                    // and that the previous sessionIS is logged out
                }		
                else    
                    new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(value);
            }
        }
        public TMUser               currentUser
        {
            get
            {
                try
                {
                    var tmUser = sessionID.session_TmUser();
                    if (tmUser.notNull())
                        tmUser.SecretData.CSRF_Token = sessionID.str().hash().str();	
                    return tmUser;
                }
                catch
                {
                    return new TMUser();
                }
            }
        }
        public bool                 check_CSRF_Token()
        {
            if (Global_Disable_Csrf_Check)
            {
                "[TM_Authentication] Global_Disable_Csrf_Check was set".error();
                return true;
            }
            if (Disable_Csrf_Check)
                return true;
            var header_Csrf_Token = TmWebServices.Context.Request.Headers["CSRF-Token"];
            var sessionIdHash = sessionID.str().hash().str();
            if (header_Csrf_Token != null && header_Csrf_Token.valid())
            {
                //"[check_CSRF_Token] {0} == {1} : {2}".debug(header_Csrf_Token, sessionID.str().hash().str(), header_Csrf_Token == sessionID.str().hash().str());
                if (header_Csrf_Token == sessionID.str().hash().str())			// interrestingly session.hash().str() produces a different value
                    return true;
            }
            //"[TM_Authentication] check_CSRF_Token failed, header_Csrf_Token: {0} sessionIDHash: {1}".error(header_Csrf_Token, sessionIdHash);
            return false;
            //throw new SecurityException("Invalid CSRF Token");			
        }
        public TM_Authentication    mapUserRoles()
        {
            return mapUserRoles(false);
        }
        public TM_Authentication    mapUserRoles(bool disable_Csrf_Check)
        {
            Disable_Csrf_Check = disable_Csrf_Check;            
            if (sessionID == Guid.Empty || sessionID.validSession() == false)
                /*if (SingleSignOn.singleSignOn_Enabled)
                {
                    sessionID = new SingleSignOn().authenticateUserBasedOn_SSOToken();
                }
                else*/
                if (WindowsAuthentication.windowsAuthentication_Enabled)
                {                    
                    sessionID = new WindowsAuthentication().authenticateUserBaseOn_ActiveDirectory();
                }            
            
            
            var userGroup = UserGroup.None;
            //"".line().info();
            //">> SessionID: {0} ".info(sessionID);
            //">> URL: {0}".info(HttpContextFactory.Request.Url);
            if (sessionID != Guid.Empty)
            {                
                if (check_CSRF_Token())		// only map the roles if the CSRF check passed
                {
                    //"[TM_Authentication] check_CSRF_Token OK".debug();
                    userGroup = new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(sessionID);					
                }                
            }
            //"[TM_Authentication][1] userGroup for sessionID: {0} : {1}".debug(sessionID, userGroup);
            if (userGroup == UserGroup.None)
            {
                if (TMConfig.Current.TMSecurity.Show_ContentToAnonymousUsers)
                    UserGroup.Reader.setThreadPrincipalWithRoles();
                else
                    UserGroup.Anonymous.setThreadPrincipalWithRoles();
            }
            //"[TM_Authentication][2] userGroup for sessionID: {0} : {1}".debug(sessionID, userGroup);
            var userRoles = Thread.CurrentPrincipal.roles().toList().join(",");
            //"[TM_Authentication][2] Current Principal roles: {0}".debug(userRoles);
            //"[TM_Authentication][3] Thread id: {0}".error(Thread.CurrentThread.ManagedThreadId);
            if (HttpContextFactory.Session.notNull())
            {
                //"[TM_Authentication][4] SessionId: {0}".info(HttpContextFactory.Session["sessionID"]);
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