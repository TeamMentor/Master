using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using PostSharp.Aspects;

namespace TeamMentor.CoreLib
{
	[Serializable]
	public class LogTo_GoogleAnalytics : MethodInterceptionAspect
	{
		public override void OnInvoke(MethodInterceptionArgs args)
		{
			if (args.has_Arguments())
			{
				args.argument<UserActivity>().ga_LogEntry();
			}
			else
				"[LogMethod]".ga_LogEntry(args.Method.Name);
			args.Proceed();			
		}		
	}		

	public class GoogleAnalytics
	{
		public static	GoogleAnalytics Current				{ get; set; }
		public static	string			Analytics_Url		{ get; set; }

		public			string			AccountID			{ get; set; }
		public			int				RandomNumber		{ get; set; }   // this shouldn't really be random or we get unique sessions per request (the key is that the userCookie value is unique)
		public			int				UserId				{ get; set; } 
		public			string			UserCookie			{ get; set; }
		public			bool			Enabled				{ get; set; }
		public			bool			LogWebServicesCalls { get; set; }

		public			int				LogCount            { get; set; }

		static GoogleAnalytics()
		{
			Analytics_Url = "http://www.google-analytics.com/__utm.gif";
			Current = new GoogleAnalytics();			
		}

		private GoogleAnalytics()
		{
			Enabled				= false;
			AccountID			= "UA-XXXXXXXX-X";		// invalid value
			LogWebServicesCalls = true;

			RandomNumber		= 1111111111;			
			SetUserCookie(RandomNumber);				// for anonymous user						
		}


		public GoogleAnalytics SetUserCookie(int userId)
		{
			UserId = userId;
			UserCookie = "__utma=.{0}.{1}.{1}.{1}.1;+".format(RandomNumber, UserId);
			return this;
		}

		public GoogleAnalytics LogEntry(string page, string title)
		{
			return LogEntry(page, title, AccountID, UserCookie);
		}

		public GoogleAnalytics LogEntry(string page, string title, string accountId, string userCookie)
		{
			if (Enabled.isFalse())
				return this;
		
			
			var gaRequest = ("{0}?utmdt={1}&utmp={2}&utmac={3}&utmcc={4}").format(
								Analytics_Url, title, page, accountId, userCookie);

			//this should register a new entry in the GoogleAnalytics account, but I'm not sure how we cancheck that it actually worked (the returning gif is the same for success or failure)
		    O2Thread.mtaThread(
                () =>
                    {
                        if (TM_Xml_Database.Current.ServerOnline)
                        {
                            //gaRequest.info();   // to see the actual request
                            "[GA] {0} -> {1}".info(title, page);
                            gaRequest.GET();
                        }
                        else
                        {
                            "[Offline][No GA] {0} -> {1}".info(title, page);
                        }
                    });
			LogCount++;
			return this;
		}
		
	}

	public static class GoogleAnalytics_ExtensionMethods
	{
		public static GoogleAnalytics googleAnalytics;

		static GoogleAnalytics_ExtensionMethods()
		{
			googleAnalytics = GoogleAnalytics.Current;
		}

		/*public static bool ga_LogThisPageType(this string page)
		{
			switch (page)
			{
				case "[WebService]":
					return googleAnalytics.LogWebServicesCalls;
				default:
					return true;
			}
		}*/

		public static GoogleAnalytics ga_LogEntry(this string title, string page)
		{
			return googleAnalytics.LogEntry(page, title);
		}		

		public static UserActivity ga_LogEntry(this UserActivity userActivity)
		{
			if (userActivity.notNull())
			{
				var title = "[UserActivity][{0}]".format(userActivity.Who);
				var page = "{0}: {1}".format(userActivity.Action, userActivity.Detail);
				title.ga_LogEntry(page);
			}
			return userActivity;
		}
	}
}
