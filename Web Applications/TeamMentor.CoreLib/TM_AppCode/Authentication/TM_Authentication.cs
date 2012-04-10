using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using System.Security;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication;

//O2File:ExtensionMethods/TeamMentorUserManagement_ExtensionMethods.cs
//O2File:UserRoleBaseSecurity.cs
//O2File:WindowsAndLDAP.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public class TM_Authentication
    {

        private Guid _sessionID;	// for unit tests
        public TM_WebServices tmWebServices;
        private HandleUrlRequest handleUrlRequest;
        public bool disable_CSRF_Check;

        public TM_Authentication(TM_WebServices _tmWebServices)
        {
            tmWebServices = _tmWebServices;
            disable_CSRF_Check = false;
			try
			{
				tmWebServices.javascriptProxy.adminSessionID = this.sessionID;
			}
			catch (Exception ex)	// this will happen on the unit tests
			{
				"TM_WebServices.ctor: {0}".error(ex.Message);
			}				
        }

        public TM_Authentication(HandleUrlRequest handleUrlRequest)
        {
            // TODO: Complete member initialization
            this.handleUrlRequest = handleUrlRequest;
        }


        public Guid sessionID
        {
            get
            {
                try
                {
                    // first check if there s a session variable already set
                    if (tmWebServices.Session.notNull() && tmWebServices.Session["sessionID"].notNull())
                        return (Guid)tmWebServices.Session["sessionID"];
                    // then check the cookie
                    var sessionCookie = System.Web.HttpContext.Current.Request.Cookies["Session"];
                    if (sessionCookie.notNull() && sessionCookie.Value.isGuid())
                        return sessionCookie.Value.guid();
                    var sessionHeader = System.Web.HttpContext.Current.Request.Headers["Session"];
                    if (sessionHeader.notNull() && sessionHeader.isGuid())
                        return sessionHeader.guid();
                    //if none is set, return an empty Guid	
                    return Guid.Empty;
                }
                catch//(Exception ex) // this will happen on the unit tests
                {
                    //"sessionID.get: {0}".error(ex.Message);
                    //System.Web.HttpContext.Current.Response.Write("\n\nERROR: {0} ---\n\n".format(ex.Message));
                    return _sessionID;
                }

            }

            set
            {
                //	MyContext.Session["sessionID"] = value;
                var previousSessionId = sessionID;
                try
                {
                    if (tmWebServices.Session.notNull())
                    {
                        tmWebServices.Session["sessionID"] = value;
                    }                    
                    var sessionCookie = new HttpCookie("Session", value.str());
                    sessionCookie.HttpOnly = true;
                    System.Web.HttpContext.Current.Response.Cookies.Add(sessionCookie);                    
                }
                catch//(Exception ex) // this will happen on the unit tests
                {
                    _sessionID = value;
                    //"sessionID.set: {0}".error(ex.Message);
                }
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

        public TMUser currentUser
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


		public bool check_CSRF_Token()
		{
            if (disable_CSRF_Check)
                return true;
			var header_CSRF_Token = tmWebServices.Context.Request.Headers["CSRF_Token"];
			if (header_CSRF_Token.valid())
			{
				if (header_CSRF_Token == sessionID.str().hash().str())			// interrestingly session.hash().str() produces a different value
					return true;
			}
			return false;
			//throw new SecurityException("Invalid CSRF Token");			
		}

		public TM_Authentication mapUserRoles()
		{
            if (WindowsAuthentication.windowsAuthentication_Enabled)
				if (sessionID == Guid.Empty || sessionID.validSession() == false)
					sessionID = new WindowsAuthentication().authenticateUserBaseOnActiveDirectory();

			
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
                if (TMConfig.Current.ShowContentToAnonymousUsers)
                    UserGroup.Reader.setThreadPrincipalWithRoles();
                else
                    UserGroup.Anonymous.setThreadPrincipalWithRoles();
            }
			return this;
		}
	}
}