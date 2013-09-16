using System;
using System.Collections.Generic;
using System.Web.Services;

namespace TeamMentor.CoreLib
{
    //********  Session Management (& Login)]
    public partial class TM_WebServices
    {
        [WebMethod(EnableSession = true)]	public Guid Login(string username, string password)      		                {   return tmAuthentication.sessionID = userData.login(username,password);                    }  			
        //[WebMethod(EnableSession = true)]	public Guid Login_Using_LoginToken(string username, Guid loginToken)            {   return tmAuthentication.sessionID = userData.login_Using_LoginToken(username,loginToken); }  			      
        [WebMethod(EnableSession = true)]	public Guid Logout()      										                {	return tmAuthentication.logout();                                                         }	 // note: logout will set tmAuthentication.sessionID 
        [WebMethod(EnableSession = true)]	public Guid Current_SessionID()										            {	return tmAuthentication.sessionID;                                                        }
        [WebMethod(EnableSession = true)]	public TM_User Current_User()										            {	return tmAuthentication.currentUser.user();                                               }
        [WebMethod(EnableSession = true)]	public List<string> GetCurrentUserRoles()							            {	return tmAuthentication.sessionID.session_UserRoles().toStringList();                     }
        [WebMethod(EnableSession = true)]	public bool SetCurrentUserPassword(string currentPassword, string newPassword)  {	return userData.setCurrentUserPassword(tmAuthentication , currentPassword, newPassword);  }        
    }
}
