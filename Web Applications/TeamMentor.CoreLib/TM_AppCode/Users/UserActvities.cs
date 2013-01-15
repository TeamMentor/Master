using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public class UserActivity
	{
		public string	Name		{ get; set; }
		public string	Detail		{ get; set; }
		public DateTime When		{ get; set; }
		public bool		Handled		{ get; set; }
	}

	public class UserActivities
	{
		public static UserActivities Current { get; set; }

		public List<UserActivity> ActivitiesLog { get; set; }



		static UserActivities()
		{
			Current = new UserActivities();
		}

		public UserActivities()
		{
			ActivitiesLog = new List<UserActivity>();
		}

		[LogTo_GoogleAnalytics]
		public UserActivities LogActivity(UserActivity userActivity)
		{
			ActivitiesLog.Add(userActivity);			
			return this;
		}
	}

	public static class UserActivities_ExtensionMethods
	{
		public static UserActivities logActivity(this string name, string detail)
		{			
			return UserActivities.Current.logActivity(name, detail);
		}

		public static UserActivities logActivity(this UserActivities userActivites, string name, string detail)
		{
			if (userActivites.notNull())
			{
				var userActivity = new UserActivity {Name = name, Detail = detail, When = DateTime.Now};
				return userActivites.LogActivity(userActivity);
			}
			return userActivites;
		}

		public static UserActivities reset(this UserActivities userActivites)
		{
			userActivites.ActivitiesLog.clear();
			return userActivites;
		}

	}
}
