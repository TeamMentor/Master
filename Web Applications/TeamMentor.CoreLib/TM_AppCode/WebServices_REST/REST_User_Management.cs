using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.CoreLib
{
	public partial class REST_Admin
	{
		//REST_User_Management
		public bool user_Update(TM_User user)
		{
			var groupId = -1; //not implemented for now
			return TmWebServices.UpdateUser(user.UserId, user.UserName, user.FirstName, user.LastName, user.Title, user.Company,user.Email, groupId);
		}

		public Stream users_html()
		{
			//var xml = users().toXml();
			//var html = xml.xsl_Transform();
			var html = "test";
			this.response_ContentType_Html();
			return html.stream_UFT8();
		}

		public static int loopCount = 0;

		public Stream users_Activities()
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
		}
	}
}
