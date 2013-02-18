using System.Collections.Generic;
using System.Linq;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public partial class TM_REST
	{        
	    //RBAC
		public string			RBAC_CurrentIdentity_Name()
		{
		    var currentUser = TmWebServices.Current_User();
		    if (currentUser.notNull())
		        return currentUser.UserName;
		    return null;
		}
		public bool				RBAC_CurrentIdentity_IsAuthenticated()
		{
			return TmWebServices.RBAC_CurrentIdentity_IsAuthenticated();
		}
		public List<string>		RBAC_CurrentPrincipal_Roles()
		{
			return TmWebServices.RBAC_CurrentPrincipal_Roles();
		}
		public bool				RBAC_HasRole(string role)
		{
			return TmWebServices.RBAC_HasRole(role);
		}
		public bool				RBAC_IsAdmin()
		{
			return TmWebServices.RBAC_IsAdmin();
		}
		//Admin: User Management

		/*public TM_User				users_New()
		{
			return TmWebServices.CreateUser_Random().user();
		}*/
		public int              user_New(TM_User user)
		{
			return TmWebServices.CreateUser(user.newUser());
		}
		public TM_User			user(string userNameOrId)
		{
		    var user = TmWebServices.GetUser_byID(userNameOrId.toInt());
			if (user.notNull())
				return user;
		    return TmWebServices.GetUser_byName(userNameOrId);
		}
		public List<TM_User>	users(string usersIds)
		{
			var ids = usersIds.split(",").Select((id) => id.toInt()).toList();
		    return TmWebServices.GetUsers_byID(ids);
		}				
		public List<TM_User>	users()
		{
		    return TmWebServices.GetUsers();
		}

		public bool				    DeleteUser(string userId)
		{
			return TmWebServices.DeleteUser(userId.toInt());
		}

		//REST_User_Management
		public bool         user_Update(TM_User user)
		{
			var groupId = -1; //not implemented for now
			return TmWebServices.UpdateUser(user.UserId, user.UserName, user.FirstName, user.LastName, user.Title, user.Company,user.Email, groupId);
		}
/*		public Stream       users_html()
		{
			//var xml = users().toXml();
			//var html = xml.xsl_Transform();
			var html = "test";
			this.response_ContentType_Html();
			return html.stream_UFT8();
		}*/
		//public static int   loopCount = 0;

/*		public Stream       users_Activities()
		{
			var loops = 10;

			var userActivities = UserActivities.Current;
			Context.Response.Write("<h1>UserActivites</h1>");
			Context.Response.Flush();

			Context.Response.Write("Waiting {0} times for user activity events<hr>".format(loops));
			Context.Response.Flush();
			//200.wait();
			loops.loop(500, () =>
				{
					
					loopCount++;
					if (userActivities.ActivitiesLog.notEmpty())
					{
						foreach(var activity in userActivities.ActivitiesLog)
							Context.Response.Write("[{0}] {1} : {2}<br>".format(loopCount,activity.Name, activity.Detail));
						userActivities.ActivitiesLog.clear();
						Context.Response.Flush();
					}																			
				});

			Context.Response.Write("<hr>Loop Ends");
			Context.Response.Flush();
			return new MemoryStream();
		}*/
	}
}
