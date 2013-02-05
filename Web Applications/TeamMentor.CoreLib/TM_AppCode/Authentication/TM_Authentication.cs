using System;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class TM_Authentication
    {        
        public  TM_WebServices      TmWebServices { get; set; }    
        public  bool                disable_Csrf_Check;

        public TM_Authentication    (TM_WebServices tmWebServices)
        {
            TmWebServices = tmWebServices;
            disable_Csrf_Check = false;
            try
            {
                TmWebServices.javascriptProxy.adminSessionID = this.sessionID;
            }
            catch (Exception ex)	// this will happen on the unit tests
            {
                "TM_WebServices.ctor: {0}".error(ex.Message);
            }				
        }        

        public Guid                 sessionID
        {
            get
            {                
                // first check if there s a session variable already set
                //if (TmWebServices.Session.notNull() && TmWebServices.Session["sessionID"].notNull())
                if (HttpContextFactory.Session.notNull() && HttpContextFactory.Session["sessionID"].notNull())
                    return (Guid)TmWebServices.Session["sessionID"];
                // then check the cookie
                var sessionCookie = HttpContextFactory.Request.Cookies["Session"];
                if (sessionCookie.notNull() && sessionCookie.Value.isGuid())
                    return sessionCookie.Value.guid();
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
                {
                    HttpContextFactory.Session["sessionID"] = value;
                }                    
                var sessionCookie = new HttpCookie("Session", value.str());
                sessionCookie.HttpOnly = true;
                HttpContextFactory.Response.Cookies.Add(sessionCookie);                    
             
                if (value != Guid.Empty)
                {					
                    new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(value);
                }
                else
                {
                    previousSessionId.invalidateSession();
                }
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
                        tmUser.CSRF_Token = this.sessionID.str().hash().str();	
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
            if (disable_Csrf_Check)
                return true;
            var header_Csrf_Token = TmWebServices.Context.Request.Headers["CSRF-Token"];
            if (header_Csrf_Token != null && header_Csrf_Token.valid())
            {
                //"[check_CSRF_Token] {0} == {1} : {2}".debug(header_CSRF_Token, sessionID.str().hash().str(), header_CSRF_Token == sessionID.str().hash().str());
                if (header_Csrf_Token == sessionID.str().hash().str())			// interrestingly session.hash().str() produces a different value
                    return true;
            }
            return false;
            //throw new SecurityException("Invalid CSRF Token");			
        }
        public TM_Authentication    mapUserRoles()
        {
            return mapUserRoles(false);
        }
        public TM_Authentication    mapUserRoles(bool _disable_CSRF_Check)
        {
            disable_Csrf_Check = _disable_CSRF_Check;
            //"[TM_Authentication] mapUserRoles".info();
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
            if (sessionID != Guid.Empty)
            {                
                if (check_CSRF_Token())		// only map the roles if the CSRF check passed
                {
                    userGroup = new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(sessionID);					
                }
                //else
                //    "[TM_Authentication] check_CSRF_Token failed".error();
            }
            if (userGroup == UserGroup.None)
            {
                if (TMConfig.Current.ShowContentToAnonymousUsers)
                    UserGroup.Reader.setThreadPrincipalWithRoles();
                else
                    UserGroup.Anonymous.setThreadPrincipalWithRoles();
            }
            return this;
        }
    }
}