using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;

namespace TeamMentor.CoreLib
{
    //********  Session Management (& Login)]
    public partial class TM_WebServices
    {
        [WebMethod(EnableSession = true)]	public Guid Login(string username, string password)      		    {   return tmAuthentication.sessionID = tmXmlDatabase.login(username,password);  }  			
        [WebMethod(EnableSession = true)]	public Guid Logout()      										    {	return tmAuthentication.logout();                  }	
        [WebMethod(EnableSession = true)]	public Guid Current_SessionID()										{	return tmAuthentication.sessionID;              }
        [WebMethod(EnableSession = true)]	public TMUser Current_User()										{	return tmAuthentication.currentUser;            }
        [WebMethod(EnableSession = true)]	public List<string> GetCurrentUserRoles()							{	return tmAuthentication.sessionID.session_UserRoles().toStringList();  }
        [WebMethod(EnableSession = true)]	public bool SetCurrentUserPassword(string password)			        {	return TM_Xml_Database.Current.setCurrentUserPassword(tmAuthentication , password); }

        //Move these to separate file
        [WebMethod(EnableSession = true)] 	[Admin]	   public List<Guid> GetActiveSessions()				    { 	return TM_Xml_Database.Current.activeSessions().Keys.ToList(); }
        [WebMethod(EnableSession = true)] 	[Admin]	   public TMUser	 GetActiveSession(Guid sessionId)		{
                                                                                                                    var activeSessions = TM_Xml_Database.Current.activeSessions();
                                                                                                                    if (activeSessions.ContainsKey(sessionId))
                                                                                                                        return activeSessions[sessionId];
                                                                                                                    return null;
                                                                                                                }
    }
}
