using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.Kernel.ExtensionMethods;
using O2.XRules.Database.Utils;
using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using System.Security;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public class TM_Authentication
    {

        private Guid _sessionID;	// for unit tests
        public TM_WebServices tmWebServices;

        public TM_Authentication(TM_WebServices _tmWebServices)
        {
            tmWebServices = _tmWebServices;

			try
			{
				tmWebServices.javascriptProxy.adminSessionID = this.sessionID;
			}
			catch (Exception ex)	// this will happen on the unit tests
			{
				"TM_WebServices.ctor: {0}".error(ex.Message);
			}				
        }


        public Guid sessionID
        {
            get
            {
                try
                {
                    // first check if there s a session variable already set
                    if (tmWebServices.Session["sessionID"].notNull())
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

                try
                {
                    //if (Session.notNull())
                    tmWebServices.Session["sessionID"] = value;
                    //var sessionCookie = System.Web.HttpContext.Current.Request.Cookies["Session"];
                    //if (sessionCookie.isNull())
                    //{
                    var sessionCookie = new HttpCookie("Session", value.str());
                    sessionCookie.HttpOnly = true;
                    System.Web.HttpContext.Current.Response.Cookies.Add(sessionCookie);
                    //}
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
						tmUser.CSRF_Token = this.sessionID.hash().str();
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
			var header_CSRF_Token = tmWebServices.Context.Request.Headers["CSRF_Token"];
			if (header_CSRF_Token.valid())
			{
				if (header_CSRF_Token == sessionID.hash().str())
					return true;
			}
			return false;
			//throw new SecurityException("Invalid CSRF Token");			
		}

		public TM_Authentication mapUserRoles()
		{
			if (sessionID != Guid.Empty)
			{
				if(check_CSRF_Token())		// only map the roles if the CSRF check passed
					new UserRoleBaseSecurity().MapRolesBasedOnSessionGuid(sessionID);
			}

			if (tmWebServices.GetCurrentUserRoles().size() == 0)
				if (TMConfig.Current.ShowContentToAnonymousUsers)
					UserGroup.Reader.setThreadPrincipalWithRoles();
				else
					UserGroup.Anonymous.setThreadPrincipalWithRoles();

			return this;
		}
	}
}